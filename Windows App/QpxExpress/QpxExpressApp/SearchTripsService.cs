using Google.Apis.QPXExpress.v1;
using Google.Apis.QPXExpress.v1.Data;
using Google.Apis.Services;
using QpxExpress.Data;
using QpxExpress.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace QpxExpressApp
{
    class SearchTripsService
    {
        private const int RequestsLimit = 5000;

        private readonly string apiKey;
        private readonly string origin;
        private readonly List<int> skippedMonths = new List<int>();
        private readonly List<string> ticketTypes = new List<string>
        {
            "adult",
            "child",
            "infant"
        };

        public SearchTripsService()
        {
            apiKey = Environment.GetEnvironmentVariable(ConfigurationProvider.ApiKeyVariableName);
            origin = ConfigurationManager.AppSettings["origin"];

            var skipMonthsConfig = ConfigurationManager.AppSettings["skipMonths"];
            if (!string.IsNullOrWhiteSpace(skipMonthsConfig))
            {
                skippedMonths.AddRange(skipMonthsConfig.Split(',').Select(x => int.TryParse(x, out int result) ? result : 0));
            }
        }

        public async Task FetchTripFares()
        {
            var service = new QPXExpressService(new BaseClientService.Initializer
            {
                ApplicationName = string.Empty,
                ApiKey = apiKey,
            });

            using (var context = new TicketsContext())
            {
                var trips = context.Trips.ToList();

                var adultTrips = trips.Where(x => x.TicketType == "adult").ToList();

                var requestsCount = 0;
                foreach (var destination in context.TripDestinations.ToList())
                {
                    for (var i = 0; i < 12; i++)
                    {
                        var departureDate = DateTime.Today.AddMonths(i);

                        if (skippedMonths.Contains(departureDate.Month))
                        {
                            continue;
                        }

                        departureDate = new DateTime(departureDate.Year, departureDate.Month, 1);
                        var arrivalDate = new DateTime(departureDate.Year, departureDate.Month, 1).AddMonths(1).AddDays(-1);

                        if (destination.DepartureDay.HasValue)
                        {
                            for (var day = 0; day < 7; day++)
                            {
                                if ((int)departureDate.DayOfWeek == destination.DepartureDay.Value)
                                {
                                    break;
                                }

                                departureDate = departureDate.AddDays(1);
                            }
                        }

                        if (destination.ArrivalDay.HasValue)
                        {
                            for (var day = 0; day < 7; day++)
                            {
                                if ((int)arrivalDate.DayOfWeek == destination.ArrivalDay.Value)
                                {
                                    break;
                                }

                                departureDate = departureDate.AddDays(-1);
                            }
                        }

                        foreach (var ticketType in ticketTypes)
                        {
                            if (destination.Airline == "000")
                            {
                                continue;
                            }

                            var tripExists = trips.Any(x => x.Destination == destination.Name
                                                   && x.AirlineCode == destination.Airline
                                                   && x.Period == departureDate
                                                   && x.TicketType == ticketType);

                            if (!tripExists)
                            {
                                requestsCount++;

                                if (requestsCount > RequestsLimit)
                                {
                                    break;
                                }

                                var searchRequest = service.Trips.Search(CreateRequest(destination.Code, departureDate, arrivalDate, destination.Airline, ticketType));
                                var searchResponse = await searchRequest.ExecuteAsync();

                                if (searchResponse.Trips.TripOption == null)
                                {
                                    Console.WriteLine("No flights found for destination {0} with {1} airline for period {2:MMM yyyy}.", destination.Name, destination.Airline, departureDate);

                                    SaveTripWithZeroFare(context, destination, departureDate);

                                    break;
                                }

                                var trip = new Trip
                                {
                                    Destination = destination.Name,
                                    Country = destination.Country,
                                    AirlineCode = destination.Airline,
                                    Period = departureDate,
                                    TicketType = ticketType,
                                    Fare = (int)searchResponse.Trips.TripOption.Average(x => int.Parse(x.SaleTotal.Replace("AED", string.Empty)))
                                };

                                if (ticketType == "adult")
                                {
                                    adultTrips.Add(trip);
                                }
                                else
                                {
                                    var adultTrip = adultTrips.First(x => x.Country == trip.Country
                                                                       && x.AirlineCode == trip.AirlineCode
                                                                       && x.Destination == trip.Destination
                                                                       && x.Period == trip.Period);
                                    trip.Fare -= adultTrip.Fare;
                                }

                                Console.WriteLine("{0}: Fetched for {1} / {2} for period {3:MMM yyyy}: {4}", requestsCount, destination.Code, ticketType, departureDate, trip.Fare);

                                context.Trips.Add(trip);

                                if (requestsCount % 10 == 0)
                                {
                                    context.SaveChanges();
                                }
                            }
                        }

                        if (requestsCount > RequestsLimit)
                        {
                            break;
                        }
                    }

                    if (requestsCount > RequestsLimit)
                    {
                        break;
                    }
                }

                context.SaveChanges();
            }
        }

        private TripsSearchRequest CreateRequest(string destination, DateTime departureDate, DateTime arrivalDate, string airlineCode, string ticketType)
        {
            return new TripsSearchRequest
            {
                Request = new TripOptionsRequest
                {
                    Slice = new List<SliceInput>
                    {
                        new SliceInput
                        {
                            Origin = origin,
                            Destination = destination,
                            Date = departureDate.ToString("yyyy-MM-dd"),
                            MaxStops = 0,
                            PreferredCabin = "COACH",
                            PermittedCarrier = new string[] { airlineCode }
                        },
                        new SliceInput
                        {
                            Origin = destination,
                            Destination = origin,
                            Date = arrivalDate.ToString("yyyy-MM-dd"),
                            MaxStops = 0,
                            PreferredCabin = "COACH",
                            PermittedCarrier = new string[] { airlineCode }
                        }
                    },
                    Passengers = new PassengerCounts
                    {
                        AdultCount = 1,
                        InfantInLapCount = ticketType == "infant" ? 1 : 0,
                        InfantInSeatCount = 0,
                        ChildCount = ticketType == "child" ? 1 : 0,
                        SeniorCount = 0
                    },
                    Solutions = 20,
                    Refundable = false
                }
            };
        }

        private void SaveTripWithZeroFare(ITicketsContext context, TripDestination destination, DateTime period)
        {
            foreach (var ticketType in ticketTypes)
            {
                var trip = new Trip
                {
                    Destination = destination.Name,
                    Country = destination.Country,
                    AirlineCode = destination.Airline,
                    Period = period,
                    TicketType = ticketType,
                    Fare = 0
                };

                context.Trips.Add(trip);
            }
        }
    }
}

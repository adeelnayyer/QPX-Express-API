using Google.Apis.QPXExpress.v1;
using Google.Apis.QPXExpress.v1.Data;
using Google.Apis.Services;
using QpxExpress.Data;
using QpxExpress.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace QpxExpressApp
{
    class TripFareService
    {
        private readonly string apiKey;
        private readonly string origin;
        private readonly int resultsCount;
        private readonly int requestsLimit;
        private readonly List<int> skippedMonths = new List<int>();
        private readonly List<string> ticketTypes = new List<string>
        {
            "adult",
            "child",
            "infant"
        };

        public TripFareService()
        {
            apiKey = Environment.GetEnvironmentVariable(ConfigurationProvider.ApiKeyVariableName);
            origin = ConfigurationManager.AppSettings["origin"];
            resultsCount = int.Parse(ConfigurationManager.AppSettings["resultsCount"]);
            requestsLimit = int.Parse(ConfigurationManager.AppSettings["requestsLimit"]);

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
                var firstDayOfMonth = DateTime.Today;
                firstDayOfMonth = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, 1);
                foreach (var destination in context.TripDestinations.Where(x => x.UpdatedOn == null || x.UpdatedOn < firstDayOfMonth).ToList())
                {
                    requestsCount = await FetchTripFares(service, context, trips, adultTrips, requestsCount, destination, false);

                    if (destination.BusinessClass)
                    {
                        requestsCount = await FetchTripFares(service, context, trips, adultTrips, requestsCount, destination, true);
                    }
                    
                    destination.UpdatedOn = DateTime.Now;
                    context.Entry(destination).State = EntityState.Modified;
                }

                context.SaveChanges();
            }
        }

        private TripsSearchRequest CreateRequest(string destination, DateTime departureDate, DateTime arrivalDate, string airlineCode, string ticketType, bool businessClass, int maxStops)
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
                            MaxStops = maxStops,
                            PreferredCabin = !businessClass ? "COACH" : "BUSINESS",
                            PermittedCarrier = airlineCode.Split(',')
                        },
                        new SliceInput
                        {
                            Origin = destination,
                            Destination = origin,
                            Date = arrivalDate.ToString("yyyy-MM-dd"),
                            MaxStops = maxStops,
                            PreferredCabin = !businessClass ? "COACH" : "BUSINESS",
                            PermittedCarrier = airlineCode.Split(',')
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
                    Solutions = resultsCount,
                    Refundable = false
                }
            };
        }

        private Trip CreateTrip(int destinationId, DateTime period, string ticketType, bool businessClass, double fare)
        {
            return new Trip
            {
                DestinationId = destinationId,
                Period = period,
                TicketType = ticketType,
                BusinessClass = businessClass,
                Fare = fare
            };
        }

        private async Task<int> FetchTripFares(QPXExpressService service, TicketsContext context, List<Trip> trips, List<Trip> adultTrips, int requestsCount, TripDestination destination, bool businessClass)
        {
            for (var i = 0; i < 12; i++)
            {
                var period = DateTime.Today.AddMonths(i);
                period = new DateTime(period.Year, period.Month, 1);

                if (skippedMonths.Contains(period.Month))
                {
                    continue;
                }

                var departureDate = GetDepartureDate(destination.DepartureDay, period);
                var arrivalDate = GetArrivalDate(destination.ArrivalDay, period);

                var tripExists = trips.Any(x => x.DestinationId == destination.Id
                                             && x.Period == period
                                             && x.BusinessClass == businessClass);

                if (!tripExists)
                {
                    foreach (var ticketType in ticketTypes)
                    {
                        var payload = CreateRequest(
                            destination.Code,
                            departureDate,
                            arrivalDate,
                            destination.Airline,
                            ticketType,
                            businessClass,
                            destination.Connections);

                        var searchRequest = service.Trips.Search(payload);
                        var searchResponse = await searchRequest.ExecuteAsync();

                        requestsCount++;

                        if (searchResponse.Trips.TripOption == null)
                        {
                            Console.WriteLine($"{requestsCount}: No flights found for destination: {destination.Name}, airline: {destination.Airline}, " +
                                                  $"period: {period:MMM yyyy}, class: {(!businessClass ? "Economy" : "Business")}.");

                            break;
                        }

                        var fare = (int)searchResponse.Trips.TripOption.Average(x => int.Parse(x.SaleTotal.Replace("AED", string.Empty)));
                        var trip = CreateTrip(destination.Id, period, ticketType, businessClass, fare);

                        if (ticketType == "adult")
                        {
                            adultTrips.Add(trip);
                        }
                        else
                        {
                            var adultTrip = adultTrips.First(x => x.DestinationId == trip.DestinationId
                                                               && x.Period == trip.Period
                                                               && x.BusinessClass == businessClass);
                            trip.Fare -= adultTrip.Fare;
                        }

                        Console.WriteLine($"{requestsCount}: Fetched for destination: {destination.Code}, ticket type:{ticketType}, " +
                                          $"period: {period:MMM yyyy}, departure: {departureDate:dd MMM yyyy}, arrival: {arrivalDate:dd MMM yyyy}, " +
                                          $"class: {(!businessClass ? "Economy" : "Business")} - Fare {trip.Fare}");

                        context.Trips.Add(trip);

                        if (requestsCount % 10 == 0)
                        {
                            context.SaveChanges();
                        }

                        if (requestsCount > requestsLimit)
                        {
                            break;
                        }
                    }
                }

                if (requestsCount > requestsLimit)
                {
                    break;
                }
            }

            return requestsCount;
        }

        private DateTime GetArrivalDate(byte? arrivalDay, DateTime period)
        {
            var arrivalDate = new DateTime(period.Year, period.Month, 1).AddMonths(1).AddDays(-1);

            if (arrivalDay.HasValue)
            {
                for (var day = 0; day < 7; day++)
                {
                    if ((int)arrivalDate.DayOfWeek == arrivalDay.Value)
                    {
                        break;
                    }

                    arrivalDate = arrivalDate.AddDays(-1);
                }
            }

            return arrivalDate;
        }

        private DateTime GetDepartureDate(byte? departureDay, DateTime period)
        {
            var departureDate = new DateTime(period.Year, period.Month, 1);
            if (departureDay.HasValue)
            {
                for (var day = 0; day < 7; day++)
                {
                    if ((int)departureDate.DayOfWeek == departureDay.Value)
                    {
                        break;
                    }

                    departureDate = departureDate.AddDays(1);
                }
            }

            return departureDate;
        }
    }
}

using BingMapsRESTToolkit;
using EPiServer.Commerce.Order;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.IdentityTracker;
using EPiServer.Reference.Commerce.Site.Features.Warehouses.Models;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Warehouses.Services
{
    [ServiceConfiguration(ServiceType = typeof(IFulfillmentWarehouseProcessor), Lifecycle = ServiceInstanceScope.Singleton)]
    public class LocationFulfillmentWarehouseProcessor : IFulfillmentWarehouseProcessor
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(LocationFulfillmentWarehouseProcessor));

        public LocationFulfillmentWarehouseProcessor(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public IWarehouse GetFulfillmentWarehouse(IShipment shipment)
        {
            if (!string.IsNullOrEmpty(shipment.WarehouseCode))
            {
                return _warehouseRepository.Get(shipment.WarehouseCode);
            }

            var fulfillmentCenters = _warehouseRepository.List().Where(w => w.IsActive && w.IsFulfillmentCenter).ToList();
            if (fulfillmentCenters.Count == 1)
            {
                return fulfillmentCenters.FirstOrDefault();
            }

            var userPosition = GetUserPosition();

            if (string.IsNullOrEmpty(userPosition))
            {
                return fulfillmentCenters.FirstOrDefault();
            }

            var warehouseAddressList = GetAllWarehouseAddresses(fulfillmentCenters);
            var distanceResult = AsyncHelpers.RunSync(() => GetDistanceMatrix(userPosition, warehouseAddressList));
            var data = new List<DistanceData>();
            var elements = distanceResult.Results;

            for (int i = 0; i < warehouseAddressList.Count; i++)
            {
                if (!elements[i].HasError)
                {
                    data.Add(new DistanceData
                    {
                        DistanceValue = elements[i].TravelDistance,
                        WarehouseCode = warehouseAddressList.Keys.ElementAt(i)
                    });
                }
            }

            if (data.Count == 0)
            {
                return fulfillmentCenters.FirstOrDefault();
            }

            return _warehouseRepository.Get(data.OrderBy(x => x.DistanceValue).First().WarehouseCode);
        }

        private string GetUserPosition()
        {
            try
            {
                var userPosition = GeoPosition.GetUsersLocation().ToFindLocation();
                return userPosition.Latitude + "," + userPosition.Longitude;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return "";
            }
        }

        /// <summary>
        /// Gets all warehouse addresses.
        /// </summary>
        /// <returns>A dictionary contains "Warehouse code" and full line of "Warehouse address".</returns>
        public Dictionary<string, string> GetAllWarehouseAddresses(IList<IWarehouse> fulfillmentCenters)
        {
            var warehouseMapper = new Dictionary<string, string>();
            var warehouseList = fulfillmentCenters
                .Where(x => (!string.IsNullOrEmpty(x.ContactInformation.Line1)
                        && !string.IsNullOrEmpty(x.ContactInformation.City)
                        && !string.IsNullOrEmpty(x.ContactInformation.CountryName)))
                .ToList();

            foreach (var warehouse in warehouseList)
            {
                warehouseMapper.Add(warehouse.Code,
                    warehouse.ContactInformation.Line1 + " " + warehouse.ContactInformation.City + " " + warehouse.ContactInformation.CountryName);
            }

            return warehouseMapper;
        }

        private async Task<DistanceMatrix> GetDistanceMatrix(string userPosition, Dictionary<string, string> warehouseAddressList)
        {
            var request = new DistanceMatrixRequest
            {
                BingMapsKey = "Agf8opFWW3n3881904l3l0MtQNID1EaBrr7WppVZ4v38Blx9l8A8x86aLVZNRv2I",
                Origins = new List<SimpleWaypoint>()
                {
                    new SimpleWaypoint(userPosition)
                },
                Destinations = warehouseAddressList.Values.Select(x => new SimpleWaypoint(x)).ToList()
            };
            return await request.GetEuclideanDistanceMatrix();
        }
    }
}
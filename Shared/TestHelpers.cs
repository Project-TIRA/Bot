using EntityModel;
using EntityModel.Helpers;
using Shared.ApiInterface;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared
{
    public static class TestHelpers
    {
        public const int DefaultTotal = 10;
        public const int DefaultOpen = 5;
        public const bool DefaultWaitlistIsOpen = true;

        public static string DefaultLocation = "Seattle";
        public static LocationPosition DefaultLocationPosition = new LocationPosition { Lat = 47.6038321, Lon = -122.3300624 };

        // ~11mi straight line
        public static string DefaultLocationDistanceShort = "Redmond, WA";
        public static LocationPosition DefaultLocationPositionShort = new LocationPosition { Lat = 47.6694141, Lon = -122.1238767 };

        // ~26mi straight line
        public static string DefaultLocationDistanceMid = "Everett, WA";
        public static LocationPosition DefaultLocationPositionMid = new LocationPosition { Lat = 47.9673056, Lon = -122.2013998 };

        // ~96mi straight line
        public static string DefaultLocationDistanceLong = "Wenatchee, WA";
        public static LocationPosition DefaultLocationPositionLong = new LocationPosition { Lat = 47.4234599, Lon = -120.3103494 };

        // ~145mi straight line
        public static string DefaultLocationDistanceTooFar = "Portland, OR";
        public static LocationPosition DefaultLocationPositionTooFar = new LocationPosition { Lat = 45.5202471, Lon = -122.6741949 };

        public static async Task<Organization> CreateOrganization(IApiInterface api, bool isVerified)
        {
            var organization = new Organization()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Organization",
                IsVerified = isVerified,
                Address = DefaultLocation,
                Latitude = DefaultLocationPosition.Lat.ToString(),
                Longitude = DefaultLocationPosition.Lon.ToString()
            };

            await api.Create(organization);
            return organization;
        }
        public static async Task<Organization> CreateOrganization(IApiInterface api, string name, string location, LocationPosition position, bool isVerified)
        {
            var organization = new Organization()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                IsVerified = isVerified,
                Address = location,
                Latitude = position.Lat.ToString(),
                Longitude = position.Lon.ToString()
            };

            await api.Create(organization);
            return organization;
        }


        public static async Task<User> CreateUser(IApiInterface api, string organizationId)
        {
            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                OrganizationId = organizationId,
                Name = "Test User",
                PhoneNumber = Guid.NewGuid().ToString()
            };

            await api.Create(user);
            return user;
        }

        public static async Task<Service> CreateService(IApiInterface api, string organizationId, ServiceType serviceType)
        {
            if (serviceType == ServiceType.Invalid)
            {
                return null;
            }

            var service = new Service()
            {
                Id = Guid.NewGuid().ToString(),
                OrganizationId = organizationId,
                Name = $"Test Service ({serviceType.ToString()})",
                Type = serviceType
            };

            await api.Create(service);
            return service;
        }

        public static async Task<ServiceData> CreateServiceData(IApiInterface api, string createdById, string serviceId, ServiceData dataType, bool hasWaitlist = false)
        {
            var data = Helpers.CreateSubType(dataType);
            data.CreatedById = createdById;
            data.ServiceId = serviceId;
            data.IsComplete = true;

            foreach (var serviceCategory in data.ServiceCategories())
            {
                foreach (var subService in serviceCategory.Services)
                {
                    data.SetProperty(subService.TotalPropertyName, DefaultTotal);
                    data.SetProperty(subService.OpenPropertyName, DefaultOpen);
                    data.SetProperty(subService.HasWaitlistPropertyName, hasWaitlist);
                }
            }

            await api.Create(data);
            return data;
        }

        public static async Task CreateServicesAndData(IApiInterface api, string organizationId, string createdById, bool hasWaitlist = false, ServiceFlags serviceFlags = ServiceFlags.None)
        {
            if (serviceFlags == ServiceFlags.None)
            {
                return;
            }

            var datas = new Dictionary<ServiceType, ServiceData>();

            // Go through each flag individually.
            foreach (var flag in ServiceFlagsHelpers.SplitFlags(serviceFlags))
            {
                // Get the data type that handles the flag.
                var dataType = Helpers.ServiceFlagToDataType(flag);

                if (!datas.TryGetValue(dataType.ServiceType(), out ServiceData data))
                {
                    // Create a service and data of the type.
                    var service = await CreateService(api, organizationId, dataType.ServiceType());
                    data = Helpers.CreateSubType(dataType);
                    data.CreatedById = createdById;
                    data.ServiceId = service.Id;
                    data.IsComplete = true;

                    datas[data.ServiceType()] = data;
                }

                // Add the fields for any sub-service that handles the flag.
                foreach (var serviceCategory in dataType.ServiceCategories())
                {
                    foreach (var subService in serviceCategory.Services)
                    {
                        if (subService.ServiceFlags.HasFlag(flag))
                        {
                            data.SetProperty(subService.TotalPropertyName, DefaultTotal);
                            data.SetProperty(subService.OpenPropertyName, DefaultOpen);
                            data.SetProperty(subService.HasWaitlistPropertyName, hasWaitlist);
                        }
                    }
                }
            }

            datas.Values.ToList().ForEach(async d => await api.Create(d));
        }
    }
}

using EntityModel;
using Shared.ApiInterface;
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

        public static async Task<Organization> CreateOrganization(IApiInterface api, bool isVerified)
        {
            var organization = new Organization()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Organization",
                IsVerified = isVerified,
                Address = "Seattle"
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
            foreach (var flag in Helpers.SplitServiceFlags(serviceFlags))
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

using EntityModel;
using Shared.ApiInterface;
using System;
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

        public static async Task<ServiceData> CreateServiceData(IApiInterface api, string createdById, string serviceId, ServiceData type, bool hasWaitlist = false)
        {
            var data = Helpers.CreateSubType(type);
            data.CreatedById = createdById;
            data.ServiceId = serviceId;

            foreach (var step in data.UpdateSteps())
            {
                data.SetProperty(step.TotalPropertyName, DefaultTotal);
                data.SetProperty(step.OpenPropertyName, DefaultOpen);
                data.SetProperty(step.HasWaitlistPropertyName, hasWaitlist);
            }

            await api.Create(data);
            return data;
        }
    }
}

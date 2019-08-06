using EntityModel;
using Shared.ApiInterface;
using System;
using System.Threading.Tasks;

namespace Shared
{
    public static class TestHelpers
    {
        public static async Task<Organization> CreateOrganization(IApiInterface api, bool isVerified)
        {
            var organization = new Organization()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Organization",
                IsVerified = isVerified
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
            };

            await api.Create(user);
            return user;
        }

        public static async Task<Service> CreateService(IApiInterface api, string organizationId, ServiceType type)
        {
            if (type == ServiceType.Invalid)
            {
                return null;
            }

            var service = new Service()
            {
                Id = Guid.NewGuid().ToString(),
                OrganizationId = organizationId,
                Name = $"Test Service ({type.ToString()})",
                Type = (int)type
            };

            await api.Create(service);
            return service;
        }

        public static async Task<HousingData> CreateHousingData(IApiInterface api, string serviceId, bool hasWaitlist,
            int emergencyPrivateBedsTotal, int emergencySharedBedsTotal, int longtermPrivateBedsTotal, int longtermSharedBedsTotal)
        {
            var data = new HousingData()
            {
                Id = Guid.NewGuid().ToString(),
                ServiceId = serviceId,
                Name = "Test Data",
                CreatedOn = DateTime.UtcNow,
                HasWaitlist = hasWaitlist,
                EmergencyPrivateBedsTotal = emergencyPrivateBedsTotal,
                EmergencySharedBedsTotal = emergencySharedBedsTotal,
                LongTermPrivateBedsTotal = longtermPrivateBedsTotal,
                LongTermSharedBedsTotal = longtermSharedBedsTotal
            };

            await api.Create(data);
            return data;
        }
    }
}

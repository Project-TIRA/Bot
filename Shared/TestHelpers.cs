using EntityModel;
using Shared.ApiInterface;
using System;
using System.Threading.Tasks;

namespace Shared
{
    public static class TestHelpers
    {
        public static int DefaultTotal = 10;
        public static int DefaultOpen = 5;
        public static bool DefaultWaitlistIsOpen = true;

        public static async Task<Organization> CreateOrganization(IApiInterface api, bool isVerified)
        {
            var organization = new Organization()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Organization",
                IsVerified = isVerified,
                Location = "Seattle"
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

        public static async Task<Service> CreateService<T>(IApiInterface api, string organizationId) where T : ServiceDataBase
        {
            var type = Helpers.GetServiceType<T>();
            if (type == ServiceType.Invalid)
            {
                return null;
            }

            var service = new Service()
            {
                Id = Guid.NewGuid().ToString(),
                OrganizationId = organizationId,
                Name = $"Test Service ({type.ToString()})",
                Type = type
            };

            await api.Create(service);
            return service;
        }

        public static async Task<CaseManagementData> CreateCaseManagementData(IApiInterface api, string createdById, string serviceId,
            bool isComplete, bool hasWaitlist, int total)
        {
            var data = new CaseManagementData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = isComplete,
                HasWaitlist = hasWaitlist,
                Total = total
            };

            await api.Create(data);
            return data;
        }

        public static async Task<HousingData> CreateHousingData(IApiInterface api, string createdById, string serviceId, bool isComplete,
            bool hasWaitlist, int emergencyPrivateBedsTotal, int emergencySharedBedsTotal, int longtermPrivateBedsTotal, int longtermSharedBedsTotal)
        {
            var data = new HousingData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,
                EmergencyPrivateBedsTotal = emergencyPrivateBedsTotal,
                EmergencySharedBedsTotal = emergencySharedBedsTotal,
                LongTermPrivateBedsTotal = longtermPrivateBedsTotal,
                LongTermSharedBedsTotal = longtermSharedBedsTotal,
                EmergencyPrivateBedsHasWaitlist = hasWaitlist,
                EmergencySharedBedsHasWaitlist = hasWaitlist,
                LongTermPrivateBedsHasWaitlist = hasWaitlist,
                LongTermSharedBedsHasWaitlist = hasWaitlist
            };

            await api.Create(data);
            return data;
        }

        public static async Task<EmploymentData> CreatEmploymentData(IApiInterface api, string createdById, string serviceId, bool isComplete,
            bool hasWaitlist, int jobReadinessTotal, int paidInternshipTotal, int vocationalTrainingTotal, int employmentPlacementTotal)
        {
            var data = new EmploymentData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,
                JobReadinessTrainingTotal = jobReadinessTotal,
                PaidInternshipTotal = paidInternshipTotal,
                VocationalTrainingTotal = vocationalTrainingTotal,
                EmploymentPlacementTotal = employmentPlacementTotal,
                JobReadinessTrainingHasWaitlist = hasWaitlist,
                PaidInternshipHasWaitlist = hasWaitlist,
                VocationalTrainingHasWaitlist = hasWaitlist,
                EmploymentPlacementHasWaitlist = hasWaitlist
            };

            await api.Create(data);
            return data;
        }

        public static async Task<MentalHealthData> CreateMentalHealthData(IApiInterface api, string createdById, string serviceId,
            bool isComplete, bool hasWaitlist, int inPatientTotal, int outPatientTotal)
        {
            var data = new MentalHealthData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,
                InPatientTotal = inPatientTotal,
                OutPatientTotal = outPatientTotal,
                InPatientHasWaitlist = hasWaitlist,
                OutPatientHasWaitlist = hasWaitlist
            };

            await api.Create(data);
            return data;
        }

        public static async Task<SubstanceUseData> CreateSubstanceUseData(IApiInterface api, string createdById, string serviceId,
            bool isComplete, bool hasWaitlist, int detoxTotal, int inPatientTotal, int outPatientTotal, int groupTotal)
        {
            var data = new SubstanceUseData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,
                DetoxTotal = detoxTotal,
                InPatientTotal = inPatientTotal,
                OutPatientTotal = outPatientTotal,
                GroupTotal = groupTotal,
                DetoxHasWaitlist = hasWaitlist,
                InPatientHasWaitlist = hasWaitlist,
                OutPatientHasWaitlist = hasWaitlist,
                GroupHasWaitlist = hasWaitlist
            };

            await api.Create(data);
            return data;
        }
    }
}

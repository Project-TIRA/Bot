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

        public static async Task<Service> CreateService<T>(IApiInterface api, string organizationId) where T : ServiceDataBase
        {
            var serviceType = Helpers.GetServiceType<T>();
            return await CreateService(api, organizationId, serviceType);
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

        public static async Task<ServiceDataBase> CreateServiceData(IApiInterface api, Service service)
        {
            switch (service.Type)
            {
                case ServiceType.CaseManagement: return await CreateCaseManagementData(api, string.Empty, service.Id);
                case ServiceType.Employment: return await CreatEmploymentData(api, string.Empty, service.Id);
                case ServiceType.Housing: return await CreateHousingData(api, string.Empty, service.Id);
                case ServiceType.MentalHealth: return await CreateMentalHealthData(api, string.Empty, service.Id);
                case ServiceType.SubstanceUse: return await CreateSubstanceUseData(api, string.Empty, service.Id);
                default: return null;
            }
        }

        public static async Task<CaseManagementData> CreateCaseManagementData(
            IApiInterface api, string createdById, string serviceId, bool hasWaitlist = true,
            int total = DefaultTotal, int open = DefaultOpen)
        {
            var data = new CaseManagementData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,
                HasWaitlist = hasWaitlist,
                Total = total,
                Open = open
            };

            await api.Create(data);
            return data;
        }

        public static async Task<EmploymentData> CreatEmploymentData(
            IApiInterface api, string createdById, string serviceId, bool hasWaitlist = true,
            int jobReadinessTotal = DefaultTotal, int jobReadinessOpen = DefaultOpen,
            int paidInternshipTotal = DefaultTotal, int paidInternshipOpen = DefaultOpen,
            int vocationalTrainingTotal = DefaultTotal, int vocationalTrainingOpen = DefaultOpen,
            int employmentPlacementTotal = DefaultTotal, int employmentPlacementOpen = DefaultOpen)
        {
            var data = new EmploymentData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,

                JobReadinessTrainingHasWaitlist = hasWaitlist,
                JobReadinessTrainingTotal = jobReadinessTotal,
                JobReadinessTrainingOpen = jobReadinessOpen,

                PaidInternshipHasWaitlist = hasWaitlist,
                PaidInternshipTotal = paidInternshipTotal,
                PaidInternshipOpen = paidInternshipOpen,

                VocationalTrainingHasWaitlist = hasWaitlist,
                VocationalTrainingTotal = vocationalTrainingTotal,
                VocationalTrainingOpen = vocationalTrainingOpen,

                EmploymentPlacementHasWaitlist = hasWaitlist,
                EmploymentPlacementTotal = employmentPlacementTotal,
                EmploymentPlacementOpen = employmentPlacementOpen
            };

            await api.Create(data);
            return data;
        }

        public static async Task<HousingData> CreateHousingData(
            IApiInterface api, string createdById, string serviceId, bool hasWaitlist = true,
            int emergencyPrivateBedsTotal = DefaultTotal, int emergencyPrivateBedsOpen = DefaultOpen,
            int emergencySharedBedsTotal = DefaultTotal, int emergencySharedBedsOpen = DefaultOpen,
            int longtermPrivateBedsTotal = DefaultTotal, int longtermPrivateBedsOpen = DefaultOpen,
            int longtermSharedBedsTotal = DefaultTotal, int longtermSharedBedsOpen = DefaultOpen)
        {
            var data = new HousingData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,

                EmergencyPrivateBedsHasWaitlist = hasWaitlist,
                EmergencyPrivateBedsTotal = emergencyPrivateBedsTotal,
                EmergencyPrivateBedsOpen = emergencyPrivateBedsOpen,

                EmergencySharedBedsHasWaitlist = hasWaitlist,
                EmergencySharedBedsTotal = emergencySharedBedsTotal,
                EmergencySharedBedsOpen = emergencySharedBedsOpen,

                LongTermPrivateBedsHasWaitlist = hasWaitlist,
                LongTermPrivateBedsTotal = longtermPrivateBedsTotal,
                LongTermPrivateBedsOpen = longtermPrivateBedsOpen,

                LongTermSharedBedsHasWaitlist = hasWaitlist,
                LongTermSharedBedsTotal = longtermSharedBedsTotal,
                LongTermSharedBedsOpen = longtermSharedBedsOpen
            };

            await api.Create(data);
            return data;
        }

        public static async Task<MentalHealthData> CreateMentalHealthData(
            IApiInterface api, string createdById, string serviceId, bool hasWaitlist = true,
            int inPatientTotal = DefaultTotal, int inPatientOpen = DefaultOpen,
            int outPatientTotal = DefaultTotal, int outPatientOpen = DefaultOpen)
        {
            var data = new MentalHealthData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,

                InPatientHasWaitlist = hasWaitlist,
                InPatientTotal = inPatientTotal,
                InPatientOpen = inPatientOpen,

                OutPatientHasWaitlist = hasWaitlist,
                OutPatientTotal = outPatientTotal,
                OutPatientOpen = outPatientOpen
            };

            await api.Create(data);
            return data;
        }

        public static async Task<SubstanceUseData> CreateSubstanceUseData(
            IApiInterface api, string createdById, string serviceId, bool hasWaitlist = true,
            int detoxTotal = DefaultTotal, int detoxOpen = DefaultOpen,
            int inPatientTotal = DefaultTotal, int inPatientOpen = DefaultOpen,
            int outPatientTotal = DefaultTotal, int outPatientOpen = DefaultOpen,
            int groupTotal = DefaultTotal, int groupOpen = DefaultOpen)
        {
            var data = new SubstanceUseData()
            {
                CreatedById = createdById,
                ServiceId = serviceId,
                IsComplete = true,

                DetoxHasWaitlist = hasWaitlist,
                DetoxTotal = detoxTotal,
                DetoxOpen = detoxOpen,

                InPatientHasWaitlist = hasWaitlist,
                InPatientTotal = inPatientTotal,
                InPatientOpen = inPatientOpen,

                OutPatientHasWaitlist = hasWaitlist,
                OutPatientTotal = outPatientTotal,
                OutPatientOpen = outPatientOpen,

                GroupHasWaitlist = hasWaitlist,
                GroupTotal = groupTotal,
                GroupOpen = groupOpen
            };

            await api.Create(data);
            return data;
        }
    }
}

﻿using EntityModel;
using Shared.ApiInterface;
using System;
using System.Threading.Tasks;

namespace Shared
{
    public static class TestHelpers
    {
        public static int DefaultTotal = 10;
        public static int DefaultOpen = 5;
        public static int DefaultWaitlistLength = 1;

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

        public static async Task<Service> CreateService<T>(IApiInterface api, string organizationId) where T : ServiceModelBase
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
                Type = (int)type
            };

            await api.Create(service);
            return service;
        }

        public static async Task<CaseManagementData> CreateCaseManagementData(IApiInterface api, string serviceId, bool isComplete, bool hasWaitlist, int total)
        {
            var data = new CaseManagementData()
            {
                ServiceId = serviceId,
                IsComplete = true,
                HasWaitlist = hasWaitlist,
                Total = total
            };

            await api.Create(data);
            return data;
        }

        public static async Task<HousingData> CreateHousingData(IApiInterface api, string serviceId, bool isComplete, bool hasWaitlist,
            int emergencyPrivateBedsTotal, int emergencySharedBedsTotal, int longtermPrivateBedsTotal, int longtermSharedBedsTotal)
        {
            var data = new HousingData()
            {
                ServiceId = serviceId,
                IsComplete = true,
                HasWaitlist = hasWaitlist,
                EmergencyPrivateBedsTotal = emergencyPrivateBedsTotal,
                EmergencySharedBedsTotal = emergencySharedBedsTotal,
                LongTermPrivateBedsTotal = longtermPrivateBedsTotal,
                LongTermSharedBedsTotal = longtermSharedBedsTotal
            };

            await api.Create(data);
            return data;
        }

        public static async Task<JobTrainingData> CreateJobTrainingData(IApiInterface api, string serviceId, bool isComplete, bool hasWaitlist, int total)
        {
            var data = new JobTrainingData()
            {
                ServiceId = serviceId,
                IsComplete = true,
                HasWaitlist = hasWaitlist,
                Total = total
            };

            await api.Create(data);
            return data;
        }

        public static async Task<MentalHealthData> CreateMentalHealthData(IApiInterface api, string serviceId, bool isComplete, bool hasWaitlist,
            int inPatientTotal, int outPatientTotal)
        {
            var data = new MentalHealthData()
            {
                ServiceId = serviceId,
                IsComplete = true,
                HasWaitlist = hasWaitlist,
                InPatientTotal = inPatientTotal,
                OutPatientTotal = outPatientTotal
            };

            await api.Create(data);
            return data;
        }

        public static async Task<SubstanceUseData> CreateSubstanceUseData(IApiInterface api, string serviceId, bool isComplete, bool hasWaitlist,
            int detoxTotal, int inPatientTotal, int outPatientTotal, int groupTotal)
        {
            var data = new SubstanceUseData()
            {
                ServiceId = serviceId,
                IsComplete = true,
                HasWaitlist = hasWaitlist,
                DetoxTotal = detoxTotal,
                InPatientTotal = inPatientTotal,
                OutPatientTotal = outPatientTotal,
                GroupTotal = groupTotal
            };

            await api.Create(data);
            return data;
        }
    }
}
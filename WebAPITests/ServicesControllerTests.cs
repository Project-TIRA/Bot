using EntityModel;
using EntityModel.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Controllers;
using Xunit;

namespace WebAPITests.Controllers
{
    public class ServicesControllerTests
    {
        protected readonly IApiInterface api;

        protected readonly ServicesController servicesController;
        private readonly List<Organization> organizations = new List<Organization>();

        private List<Service> services = new List<Service>();

        public ServicesControllerTests()
        {
            api = new EfInterface(DbModelFactory.CreateInMemory());
            // create organizations and services to use for tests
            setupTest();
            servicesController = new ServicesController((EfInterface)api);
        }

        private async void setupTest()
        {
            string[] locations = { TestHelpers.DefaultLocation, TestHelpers.DefaultLocationDistanceShort, TestHelpers.DefaultLocationDistanceMid, TestHelpers.DefaultLocationDistanceLong, TestHelpers.DefaultLocationDistanceTooFar };
            LocationPosition[] positions = { TestHelpers.DefaultLocationPosition, TestHelpers.DefaultLocationPositionShort, TestHelpers.DefaultLocationPositionMid, TestHelpers.DefaultLocationPositionLong, TestHelpers.DefaultLocationPositionTooFar };
            bool waitlist = false;
            var flags = ServiceFlagsHelpers.AllFlags().ToArray();

            for (int i = 0; i < 10; i++)
            {
                var organization = await TestHelpers.CreateOrganization(api, $"{locations[i % locations.Length]} Organization", locations[i % locations.Length], positions[i % positions.Length], true);
                await TestHelpers.CreateServicesAndData(api, organization.Id, string.Empty, waitlist ^= true, flags[i % flags.Length]);
                var tempServices = await api.GetServices(organization.Id);
                organizations.Add(organization);
                services.AddRange(tempServices);
            }
            services = services.Distinct(new Helpers.KeyEqualityComparer<Service>(a => a.Name)).ToList();
        }

        [Fact]
        public async void Get_Ok_Test()
        {
            //Given
            //When
            var result = await servicesController.Get();
            // Then
            Assert.IsNotType<NotFoundResult>(result.Value);
        }

        [Fact]
        public async void Get_All()
        {
            //Given
            //When
            var result = await servicesController.Get();
            //Then
            Assert.Equal(result.Value.Count, services.Count);
        }

        [Fact]
        public async void Get_Distance()
        {
            //Given
            LocationPosition position = TestHelpers.DefaultLocationPosition;
            var maxDistance = 25;

            services.Clear();

            foreach (var organization in organizations)
            {
                if (!Helpers.organiztionWithinDistance(organization, position.Lat.ToString(), position.Lon.ToString(), maxDistance))
                {
                    continue;
                }
                var tempServices = await api.GetServices(organization.Id);
                services.AddRange(tempServices);
            }

            //When
            var result = await servicesController.Get(lat: position.Lat.ToString(), lon: position.Lon.ToString(), maxDistance: maxDistance);
            //Then
            Assert.Equal(services.Count, result.Value.Count);
        }
    }
}
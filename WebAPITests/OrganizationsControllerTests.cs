using EntityModel;
using EntityModel.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Controllers;
using Xunit;

namespace WebAPITests.Controllers
{
    public class OrganizationsControllerTests
    {
        protected readonly IApiInterface api;

        protected readonly OrganizationsController organizationsController;

        private readonly List<Organization> organizations = new List<Organization>();

        private readonly List<Service> services = new List<Service>();

        public OrganizationsControllerTests()
        {
            api = new EfInterface(DbModelFactory.CreateInMemory());
            // create organizations and services to use for tests
            setupTest();
            organizationsController = new OrganizationsController((EfInterface)api);
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
        }

        [Fact]
        public async void Get_Ok_Test()
        {
            //Given
            var result = await organizationsController.Get();
            //When
            // Then
            Assert.IsNotType<NotFoundResult>(result.Value);
        }

        [Fact]
        public async void Get_All()
        {
            //Given
            var numOrg = await api.GetVerifiedOrganizations();
            //When
            var result = await organizationsController.Get();
            //Then
            Assert.Equal(result.Value.Count, numOrg.Count);
        }

        [Fact]
        public async void Get_Name()
        {
            //Given
            var name = $"{TestHelpers.DefaultLocation} Organization";
            //When
            var result = await organizationsController.Get(name: name);
            //Then
            Assert.All(result.Value, x => { Assert.Equal(x.Name, name); });
        }

        [Fact]
        public async void Get_Services()
        {
            //Given
            string[] services = { "Test Service (Housing)" };
            var servicesString = String.Join(",", services);
            //When
            var result = await organizationsController.Get(services: servicesString);
            //Then
            Assert.All(result.Value,
            x =>
            {
                Assert.All(services, s => { Assert.Contains(s, x.ServicesProvided); });
            });
        }

        [Fact]
        public async void Get_Distance()
        {
            //Given
            LocationPosition position = TestHelpers.DefaultLocationPosition;
            var maxDistance = 25;
            Coordinates searchCoordinates = new Coordinates(position.Lat, position.Lon);
            //When
            var result = await organizationsController.Get(lat: position.Lat.ToString(), lon: position.Lon.ToString(), maxDistance: maxDistance);
            //Then
            Assert.All(result.Value, o =>
            {
                Coordinates organizationCoordinates = new Coordinates(Convert.ToDouble(o.Latitude), Convert.ToDouble(o.Longitude));
                var distance = searchCoordinates.DistanceTo(organizationCoordinates, UnitOfLength.Miles);
                Assert.True(distance < maxDistance);
            });
        }

        [Fact]
        public async void Get_Unknown_ID()
        {
            //Given

            //When
            var result = await organizationsController.GetID(Guid.NewGuid().ToString());
            //Then
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async void Get_Ok_Known_Id()
        {
            //Given
            var organization = organizations[2];

            //When
            var result = await organizationsController.GetID(organization.Id);
            //Then
            Assert.Equal(organization.Id, result.Value.Id);
        }
    }
}
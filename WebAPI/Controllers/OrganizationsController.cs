using EntityModel;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.ApiInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrganizationsController : ControllerBase
    {
        protected readonly IApiInterface api;

        public OrganizationsController(EfInterface api) => this.api = api ?? throw new ArgumentNullException(nameof(api));

        // GET: api/organizations?name={}&lat={}&lon={}&services={}&distance={}
        [HttpGet]
        public async Task<ActionResult<List<OrganizationDTO>>> Get(string name = "", string lat = "", string lon = "", string services = "", double maxDistance = 25)
        {
            var organizations = await api.GetVerifiedOrganizations();


            if (organizations != null)
            {
                var orgsDTO = new List<OrganizationDTO>();

                if (!String.IsNullOrEmpty(name))
                {
                    organizations = organizations.Where(o => o.Name == name).ToList();
                }
                if (!(String.IsNullOrEmpty(lat) && String.IsNullOrEmpty(lon)))
                {
                    Coordinates searchCoordinates = new Coordinates(Convert.ToDouble(lat), Convert.ToDouble(lon));

                    List<Organization> tempList = new List<Organization>();

                    foreach (Organization org in organizations)
                    {
                        Coordinates organizationCoordinates = new Coordinates(Convert.ToDouble(org.Latitude), Convert.ToDouble(org.Longitude));
                        var distanceTo = searchCoordinates.DistanceTo(organizationCoordinates, UnitOfLength.Miles);
                        if (distanceTo < maxDistance) { tempList.Add(org); }
                    }
                    organizations = tempList;
                }
                if (!String.IsNullOrEmpty(services))
                {
                    List<Organization> tempList = new List<Organization>();
                    foreach (Organization org in organizations)
                    {
                        var ServicesProvided = await api.GetServices(org.Id);
                        var ServicesName = ServicesProvided.Select(s => s.Name).ToList();
                        var retServices = ServicesName.Intersect(services.Split(",")).Any();

                        if (retServices)
                        {
                            tempList.Add(org);
                        }
                    }
                    organizations = tempList;
                }

                foreach (var org in organizations)
                {
                    var ServicesProvided = await api.GetServices(org.Id);
                    var tempOrg = new OrganizationDTO()
                    {
                        Id = org.Id,
                        Name = org.Name,
                        PhoneNumber = org.PhoneNumber,
                        Address = org.Address,
                        Latitude = org.Latitude,
                        Longitude = org.Longitude,
                        ServicesProvided = ServicesProvided.Select(service => service.Name).ToList()
                    };
                    orgsDTO.Add(tempOrg);
                }
                return orgsDTO;
            }
            return NotFound();
        }

        //GET: api/organizations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDTO>> GetID(string id)
        {

            var organization = await api.GetOrganization(id);


            if (organization != null)
            {
                var OrgServices = await api.GetLatestServicesData(organization.Id);

                var services = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();


                foreach (var service in OrgServices)
                {
                    services[service.Key.ToString()] = new Dictionary<string, Dictionary<string, object>>();
                    foreach (var serviceCategory in service.Value.ServiceCategories())
                    {

                        services[service.Key.ToString()][serviceCategory.Name] = new Dictionary<string, object>();
                        foreach (var subService in serviceCategory.Services)
                        {

                            services[service.Key.ToString()][serviceCategory.Name][subService.Name] = new Dictionary<string, object>(){
                                        {subService.TotalPropertyName,service.Value.GetProperty(subService.TotalPropertyName)},
                                        {subService.OpenPropertyName,service.Value.GetProperty(subService.OpenPropertyName)},
                                        {subService.HasWaitlistPropertyName,service.Value.GetProperty(subService.HasWaitlistPropertyName)},
                                        {subService.WaitlistIsOpenPropertyName,service.Value.GetProperty(subService.WaitlistIsOpenPropertyName)}
                                    };
                        }

                    }
                }

                var orgDTO = new OrganizationDTO()
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    PhoneNumber = organization.PhoneNumber,
                    Address = organization.Address,
                    Latitude = organization.Latitude,
                    Longitude = organization.Longitude,
                    Services = services

                };
                return orgDTO;
            }
            return NotFound();
        }
    }
}

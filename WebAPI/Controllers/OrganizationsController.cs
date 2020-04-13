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

                foreach (var org in organizations)
                {
                    if (!String.IsNullOrEmpty(name))
                    {
                        if (org.Name != name) { continue; }
                    }

                    if (!(String.IsNullOrEmpty(lat) || String.IsNullOrEmpty(lon)))
                    {
                        if (!Helpers.organiztionWithinDistance(org, lat, lon, maxDistance))
                        {
                            continue;
                        }
                    }

                    var ServicesProvided = await api.GetServices(org.Id);

                    if (!String.IsNullOrEmpty(services))
                    {
                        var ServicesName = ServicesProvided.Select(s => s.Type.ToString()).ToList();
                        var retServices = ServicesName.Intersect(services.Split(",")).Any();

                        if (!retServices) { continue; }
                    }

                    orgsDTO.Add(new OrganizationDTO()
                    {
                        Id = org.Id,
                        Name = org.Name,
                        PhoneNumber = org.PhoneNumber,
                        Address = org.Address,
                        Latitude = org.Latitude,
                        Longitude = org.Longitude,
                        ServicesProvided = ServicesProvided.Select(service => service.Type.ToString()).ToList()
                    });
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
                    services[service.Item1.ToString()] = new Dictionary<string, Dictionary<string, object>>();
                    foreach (var serviceCategory in service.Item2.ServiceCategories())
                    {
                        services[service.Item1.ToString()][serviceCategory.Name] = new Dictionary<string, object>();
                        foreach (var subService in serviceCategory.Services)
                        {
                            services[service.Item1.ToString()][serviceCategory.Name][subService.Name] = new Dictionary<string, object>(){
                                        {subService.TotalPropertyName,service.Item2.GetProperty(subService.TotalPropertyName)},
                                        {subService.OpenPropertyName,service.Item2.GetProperty(subService.OpenPropertyName)},
                                        {subService.HasWaitlistPropertyName,service.Item2.GetProperty(subService.HasWaitlistPropertyName)},
                                        {subService.WaitlistIsOpenPropertyName,service.Item2.GetProperty(subService.WaitlistIsOpenPropertyName)}
                                    };
                        }
                    }
                }

                return new OrganizationDTO()
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    PhoneNumber = organization.PhoneNumber,
                    Address = organization.Address,
                    Latitude = organization.Latitude,
                    Longitude = organization.Longitude,
                    Services = services
                }; ;
            }
            return NotFound();
        }
    }
}
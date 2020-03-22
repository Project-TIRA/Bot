using EntityModel;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Shared.ApiInterface;
using System.Threading.Tasks;
using WebAPI.Models;
using System.Collections.Generic;
using Shared;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrganizationsController : ControllerBase
    {
        protected readonly IApiInterface api;

        public OrganizationsController(EfInterface api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
        }

        // GET: api/organizations/
        [HttpGet]
        public async Task<ActionResult<List<OrganizationDTO>>> Get(string name, string Lat, string Lon, string services, double distance = 25)
        {
            var organizations = await this.api.GetVerifiedOrganizations();


            if (organizations != null)
            {
                var orgsDTO = new List<OrganizationDTO>();

                if (name != null)
                {
                    organizations = organizations.Where(o => o.Name == name).ToList();
                }
                if ((Lat != null) && (Lon != null))
                {
                    Coordinates searchCoordinates = new Coordinates(Convert.ToDouble(Lat), Convert.ToDouble(Lon));
                    organizations = organizations.Where(o =>
                    {
                        Coordinates organizationCoordinates = new Coordinates(Convert.ToDouble(o.Latitude), Convert.ToDouble(o.Longitude));
                        var distanceTo = searchCoordinates.DistanceTo(organizationCoordinates, UnitOfLength.Miles);
                        return distanceTo < distance;
                    }).ToList();
                }
                if (services != null)
                {
                    List<Organization> tempList = new List<Organization>();
                    foreach (Organization org in organizations)
                    {
                        var ServicesProvided = await this.api.GetServices(org.Id);
                        var ServicesName = ServicesProvided.Select(s => s.Name).ToList();
                        var retServices = ServicesName.Intersect(services.Split(","));

                        if (retServices != null)
                        {
                            tempList.Add(org);
                        }
                    }
                    organizations = tempList;
                }

                foreach (var org in organizations)
                {
                    var ServicesProvided = await this.api.GetServices(org.Id);
                    var tempOrg = new OrganizationDTO()
                    {
                        Id = org.Id,
                        Name = org.Name,
                        PhoneNumber = org.PhoneNumber,
                        Address = org.Address,
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
        public async Task<ActionResult<OrganizationDTO>> Get(string id)
        {

            var organization = await this.api.GetOrganization(id);


            if (organization != null)
            {
                var Services = await this.api.GetLatestServicesData(organization.Id);

                var ServicesCategories = new Dictionary<string, ServiceCategoryDTO>();


                foreach (var service in Services)
                {
                    foreach (var serviceCategory in service.Value.ServiceCategories())
                    {
                        if (serviceCategory != null)
                        {
                            var tempCategory = new ServiceCategoryDTO();
                            var ServicesData = new Dictionary<string, ServiceDataDTO>();

                            foreach (var subService in serviceCategory.Services)
                            {
                                var tempServicedata = new ServiceDataDTO();
                                tempServicedata.TotalPropertyName = service.Value.GetProperty(subService.TotalPropertyName);
                                tempServicedata.OpenPropertyName = service.Value.GetProperty(subService.OpenPropertyName);
                                tempServicedata.HasWaitlistPropertyName = service.Value.GetProperty(subService.HasWaitlistPropertyName);
                                tempServicedata.WaitlistIsOpenPropertyName = service.Value.GetProperty(subService.WaitlistIsOpenPropertyName);

                                ServicesData[subService.Name] = tempServicedata;
                            }
                            tempCategory.ServicesData = ServicesData;
                            ServicesCategories[serviceCategory.Name] = tempCategory;
                        }
                    }
                }

                var orgDTO = new OrganizationDTO()
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    PhoneNumber = organization.PhoneNumber,
                    Address = organization.Address,
                    Services = ServicesCategories

                };
                return orgDTO;
            }
            return NotFound();



        }

    }
}

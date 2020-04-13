using EntityModel;
using EntityModel.Helpers;
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
    public class ServicesController : ControllerBase
    {
        protected readonly IApiInterface api;

        public ServicesController(EfInterface api) => this.api = api ?? throw new ArgumentNullException(nameof(api));

        // GET: api/services/lat={}&lon={}&services={}&distance={}
        [HttpGet]
        public async Task<ActionResult<List<ServicesDTO>>> Get(string lat = "", string lon = "", double maxDistance = 25)
        {
            var organizations = await api.GetVerifiedOrganizations();

            if (organizations != null)
            {
                List<(ServiceType, ServiceData)> services = new List<(ServiceType, ServiceData)>();

                foreach (var organization in organizations)
                {
                    if (!(String.IsNullOrEmpty(lat) && String.IsNullOrEmpty(lon)))
                    {
                        if (!Helpers.organiztionWithinDistance(organization, lat, lon, maxDistance))
                        {
                            continue;
                        }
                    }

                    var serviceData = await api.GetLatestServicesData(organization.Id);
                    services.AddRange(serviceData);
                }

                var servicesDTO = new Dictionary<string, ServicesDTO>();

                foreach (var service in services)
                {
                    ServicesDTO current;
                    if (!servicesDTO.ContainsKey(service.Item1.ToString()))
                    {
                        current = servicesDTO[service.Item1.ToString()] = new ServicesDTO() { Name = service.Item1.ToString(), ServicesCategories = new Dictionary<string, HashSet<string>>() };
                    }

                    current = servicesDTO[service.Item1.ToString()];

                    foreach (var serviceCategory in service.Item2.ServiceCategories())

                    {
                        if (!current.ServicesCategories.ContainsKey(serviceCategory.Name)) { current.ServicesCategories[serviceCategory.Name] = new HashSet<string>(); }

                        foreach (var subService in serviceCategory.Services)
                        {
                            current.ServicesCategories[serviceCategory.Name].Add(subService.Name);
                        }
                    }
                }
                return servicesDTO.Values.ToList();
            }
            return NotFound();
        }
    }
}
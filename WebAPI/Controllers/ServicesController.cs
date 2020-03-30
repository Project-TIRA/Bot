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
    public class ServicesController : ControllerBase
    {
        protected readonly IApiInterface api;

        public ServicesController(EfInterface api) => this.api = api ?? throw new ArgumentNullException(nameof(api));

        // GET: api/services/lat={}&lon={}&services={}&distance={}
        [HttpGet]
        public async Task<ActionResult<List<ServicesDTO>>> Get(string lat = "", string lon = "", double distance = 25)
        {


            var organizations = await api.GetVerifiedOrganizations();



            if (organizations != null)
            {

                if (!(String.IsNullOrEmpty(lat) && String.IsNullOrEmpty(lon)))
                {
                    Coordinates searchCoordinates = new Coordinates(Convert.ToDouble(lat), Convert.ToDouble(lon));
                    organizations = organizations.Where(o =>
                    {
                        Coordinates organizationCoordinates = new Coordinates(Convert.ToDouble(o.Latitude), Convert.ToDouble(o.Longitude));
                        var distanceTo = searchCoordinates.DistanceTo(organizationCoordinates, UnitOfLength.Miles);
                        return distanceTo < distance;
                    }).ToList();
                }
                var services = new List<Service>();

                foreach (var organization in organizations)
                {
                    var tempServices = await api.GetServices(organization.Id);
                    services.AddRange(tempServices);

                }

                services = services.Distinct(new Helpers.KeyEqualityComparer<Service>(a => a.Name)).ToList();

                var servicesDTO = new List<ServicesDTO>();

                foreach (var service in services)
                {
                    var tempService = new ServicesDTO()
                    {
                        Id = service.Id,
                        Name = service.Name,


                    };
                    servicesDTO.Add(tempService);
                }

                return servicesDTO;
            }
            return NotFound();
        }
    }
}

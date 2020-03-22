using EntityModel;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Shared.ApiInterface;
using System.Threading.Tasks;
using WebAPI.Models;
using System.Collections.Generic;
using Shared;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServicesController : ControllerBase
    {
        protected readonly IApiInterface api;

        public ServicesController(EfInterface api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
        }

        // GET: api/services/
        [HttpGet]
        public async Task<ActionResult<List<ServicesDTO>>> Get(string name, string Lat, string Lon, double distance = 25)
        {


            var organizations = await this.api.GetVerifiedOrganizations();



            if (organizations != null)
            {
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
                var services = new List<Service>();

                foreach (var organization in organizations)
                {
                    var tempServices = await this.api.GetServices(organization.Id);
                    services.AddRange(tempServices);

                }


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

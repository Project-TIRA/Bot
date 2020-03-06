using EntityModel;
using System;
using Microsoft.AspNetCore.Mvc;
using Shared.ApiInterface;
using System.Threading.Tasks;
using WebAPI.Models;
using System.Collections.Generic;

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
        public async Task<ActionResult<List<ServiceDTO>>> Get()
        {


            var organizations = await this.api.GetVerifiedOrganizations();



            if (organizations != null)
            {
                var services = new List<Service>();

                foreach (var organization in organizations)
                {
                    var tempServices = await this.api.GetServices(organization.Id);
                    services.AddRange(tempServices);

                }


                var servicesDTO = new List<ServiceDTO>();

                foreach (var service in services)
                {
                    var tempService = new ServiceDTO()
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

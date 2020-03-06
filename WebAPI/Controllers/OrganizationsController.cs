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
    public class OrganizationsController : ControllerBase
    {
        protected readonly IApiInterface api;

        public OrganizationsController(EfInterface api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
        }

        // GET: api/organizations/
        [HttpGet]
        public async Task<ActionResult<List<OrganizationDTO>>> Get()
        {

            var organizations = await this.api.GetVerifiedOrganizations();


            if (organizations != null)
            {
                var orgsDTO = new List<OrganizationDTO>();

                foreach (var org in organizations)
                {
                    var tempOrg = new OrganizationDTO()
                    {
                        Id = org.Id,
                        Name = org.Name,
                        PhoneNumber = org.PhoneNumber,
                        Address = org.Address

                    };
                    orgsDTO.Add(tempOrg);
                }

                return orgsDTO;
            }
            return NotFound();
        }

        //GET: api/organizations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDTO>> GetOrganization(string id)
        {
            // Todo: Use OrganizationDTO to limit what gets returned;

            var organization = await this.api.GetOrganization(id);


            if (organization != null)
            {
                var orgDTO = new OrganizationDTO()
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    PhoneNumber = organization.PhoneNumber,
                    Address = organization.Address
                };
                return orgDTO;
            }
            return NotFound();



        }

    }
}

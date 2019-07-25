using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using Shared.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Bot.Builder;

namespace Shared
{
    public class ApiInterface
    {
        const string CLIENT_ID = "4cd0f1ea-6f83-419a-a4fa-24878d30dd09";
        const string CLIENT_SECRET = "L-7r?rQY/5xo+QQ1yBJ02IEk3z9V297f";
        const string TENANT_ID = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        const string RESOURCE_URL = "https://obsdev.api.crm.dynamics.com";
        const string AUTH_URL = "https://login.microsoftonline.com/{0}/oauth2/token";
        const string API_URL = "https://obsdev.api.crm.dynamics.com/api/data/v9.1/";

        HttpClient client;
        string authToken;
        bool isInitialized;

        public ApiInterface()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(API_URL);
        }

        ~ApiInterface()
        {
            this.client.Dispose();
        }

        /// <summary>
        /// Gets a user from a user ID.
        /// </summary>
        public async Task<User> GetUser(string userId)
        {
            JObject response = await GetJsonData(User.TABLE_NAME, "$filter=contactid eq " + userId);
            if (response == null)
            {
                return null;
            }

            return response["value"].HasValues ? response["value"][0].ToObject<User>() : null;
        }

        /// <summary>
        /// Gets an organization from a user ID.
        /// </summary>
        public async Task<Organization> GetUserOrganization(string userId)
        {
            User user = await GetUser(userId);
            return user != null ? await GetOrganization(user) : null;
        }

        /// <summary>
        /// Gets an organization's services from a user ID.
        /// </summary>
        public async Task<List<Service>> GetUserOrganizationServices(string userId)
        {
            User user = await GetUser(userId);
            if (user != null)
            {
                Organization organization = await GetOrganization(user);
                if (organization != null)
                {
                    return await GetOrganizationServices(organization);
                }
            }

            return new List<Service>();
        }

        /// <summary>
        /// Gets the latest shapshot for a housing service.
        /// </summary>
        public async Task<HousingData> GetLatestHousingServiceData(string userId)
        {
            var organization = await GetUserOrganization(userId);
            if (organization != null)
            {
                var service = await GetOrganizationHousingService(organization);
                if (service != null)
                {
                    JObject response = await GetJsonData(HousingData.TABLE_NAME, $"$filter=_tira_housingserviceid_value eq {service.Id} &$orderby=createdon desc &$top=1");
                    if (response == null)
                    {
                        return null;
                    }

                    return response["value"].HasValues ? response["value"][0].ToObject<HousingData>() : null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the latest shapshot for a case management service.
        /// </summary>
        public async Task<CaseManagementData> GetLatestCaseManagementServiceData(string userId)
        {
            var organization = await GetUserOrganization(userId);
            if (organization != null)
            {
                var service = await GetOrganizationCaseManagementService(organization);
                if (service != null)
                {
                    JObject response = await GetJsonData(CaseManagementData.TABLE_NAME, $"$filter=_tira_casemanagementserviceid_value eq {service.Id} &$orderby=createdon desc &$top=1");
                    if (response == null)
                    {
                        return null;
                    }

                    return response["value"].HasValues ? response["value"][0].ToObject<CaseManagementData>() : null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the latest shapshot for a housing service.
        /// </summary>
        public async Task<SubstanceUseData> GetLatestSubstanceUseServiceData(string userId)
        {
            var organization = await GetUserOrganization(userId);
            if (organization != null)
            {
                var service = await GetOrganizationSubstanceUseService(organization);
                if (service != null)
                {
                    JObject response = await GetJsonData(SubstanceUseData.TABLE_NAME, $"$filter=_tira_substanceuseserviceid_value eq {service.Id} &$orderby=createdon desc &$top=1");
                    if (response == null)
                    {
                        return null;
                    }

                    return response["value"].HasValues ? response["value"][0].ToObject<SubstanceUseData>() : null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a user's organization.
        /// </summary>
        public async Task<Organization> GetOrganization(User user)
        {
            JObject response = await GetJsonData(Organization.TABLE_NAME, "$filter=accountid eq " + user.OrganizationId);
            if (response == null)
            {
                return null;
            }

            return response["value"].HasValues ? response["value"][0].ToObject<Organization>() : null;
        }

        /// <summary>
        /// Gets an organization's services.
        /// </summary>
        public async Task<List<Service>> GetOrganizationServices(Organization organization)
        {
            JObject response = await GetJsonData(Service.TABLE_NAME, "$filter=_tira_organizationservicesid_value eq " + organization.Id);
            return response != null ? response["value"].ToObject<List<Service>>() : new List<Service>();
        }

        /// <summary>
        /// Gets an organization's housing service.
        /// </summary>
        public async Task<Service> GetOrganizationHousingService(Organization organization)
        {
            // TODO: use enum for service type
            JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organization.Id} and tira_servicetype eq 1");
            return response["value"].HasValues ? response["value"][0].ToObject<Service>() : null;
        }

        /// <summary>
        /// Gets an organization's case management service.
        /// </summary>
        public async Task<Service> GetOrganizationCaseManagementService(Organization organization)
        {
            // TODO: use enum for service type
            JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organization.Id} and tira_servicetype eq 2");
            return response["value"].HasValues ? response["value"][0].ToObject<Service>() : null;
        }

        /// <summary>
        /// Gets an organization's substance use service.
        /// </summary>
        public async Task<Service> GetOrganizationSubstanceUseService(Organization organization)
        {
            // TODO: use enum for service type
            JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organization.Id} and tira_servicetype eq 4");
            return response["value"].HasValues ? response["value"][0].ToObject<Service>() : null;
        }

        public async Task<JObject> GetJsonData(string tableName, string paramString)
        {
            await EnsureAuthHeader();
            string url = Path.Combine(API_URL, tableName) + "?" + paramString;
            HttpResponseMessage response = await this.client.GetAsync(url, HttpCompletionOption.ResponseContentRead);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseContent);
        }

        public async Task<string> PostJsonData(string tableName, string json)
        {
            await EnsureAuthHeader();
            string url = Path.Combine(API_URL, tableName);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            if (response.Headers.TryGetValues("OData-EntityId", out var values) && values.Count() > 0)
            {
                return values.First();
            }

            return string.Empty;
        }

        public async Task<bool> PatchJsonData(string tableName, string resourceId, string json)
        {
            await EnsureAuthHeader();
            string url = Path.Combine(API_URL, tableName) + "(" + resourceId + ")";
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.client.PatchAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Ensures that an auth token is added to the request headers.
        /// Requests an auth token from AAD if one is not already available.
        /// </summary>
        private async Task EnsureAuthHeader()
        {
            var authToken = await GetAuthToken();
            var authHeader = new AuthenticationHeaderValue("Bearer", authToken);
            this.client.DefaultRequestHeaders.Authorization = authHeader;
        }

        /// <summary>
        /// Retrieve an authentication token from AAD.
        /// </summary>
        private async Task<string> GetAuthToken()
        {
            if (!string.IsNullOrEmpty(this.authToken))
            {
                return this.authToken;
            }

            string authorityUrl = string.Format(AUTH_URL, TENANT_ID);
            //var authParameters = await AuthenticationParameters.CreateFromUrlAsync(new Uri(authorityUrl));
            var authContext = new AuthenticationContext(authorityUrl, false);

            ClientCredential clientCred = new ClientCredential(CLIENT_ID, CLIENT_SECRET);
            AuthenticationResult authResult = await authContext.AcquireTokenAsync(RESOURCE_URL, clientCred);

            this.authToken = authResult.AccessToken;
            return authResult.AccessToken;
        }
    }
}

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

namespace ServiceProviderBot.Bot.Utils
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

        public async Task awesomeness()
        {
            // Get user
            await GetUser("17be44bf-d9ac-e911-a981-000d3a30d7c8");
        }

        /// <summary>
        /// Gets a user from a user ID.
        /// </summary>
        public async Task<User> GetUser(string userId)
        {
            JObject response = await GetJsonData(User.TableName, "$filter=contactid eq " + userId);
            return response["value"][0].ToObject<User>();
        }

        /// <summary>
        /// Gets an organization from an organization ID.
        /// </summary>
        public async Task<Organization> GetOrganization(string orgId)
        {
            JObject response = await GetJsonData(Organization.TableName, "$filter=accountid eq " + orgId);
            return response["value"][0].ToObject<Organization>();
        }

        /// <summary>
        /// Gets a list of services from an organization.
        /// </summary>
        public async Task<List<Service>> GetServicesForOrganization(Organization org)
        {
            JObject response = await GetJsonData(Service.TableName, "$filter=_tira_organizationservicesid_value eq " + org.Id);
            return response["value"].ToObject<List<Service>>();
        }

        public async Task UpdateHousingService(Service service)
        {
            JObject response = await GetJsonData(Service.TableName, "$filter=_tira_organizationservicesid_value eq " + org.Id);
            
        }

        private async Task<JObject> GetJsonData(string tableName, string paramString)
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

        private async Task<bool> PostJsonData(string tableName, string json)
        {
            await EnsureAuthHeader();
            string url = Path.Combine(API_URL, tableName);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.client.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> PatchJsonData(string tableName, string resourceId, string json)
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

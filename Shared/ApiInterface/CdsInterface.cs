using EntityModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EntityModel.Helpers;

namespace Shared.ApiInterface
{
    /// <summary>
    /// API interface for Common Data Service (Dynamics)
    /// </summary>
    public class CdsInterface : IApiInterface
    {
        // TODO: Put these in app settings and get from configuration.
        const string APP_ID = "";
        const string APP_SECRET = "";
        const string TENANT_ID = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        const string RESOURCE_URL = "https://obsdev.api.crm.dynamics.com";
        const string AUTH_URL = "https://login.microsoftonline.com/{0}/oauth2/token";
        const string API_URL = "https://obsdev.api.crm.dynamics.com/api/data/v9.1/";

        IConfiguration configuration;
        HttpClient client;
        string authToken;

        public CdsInterface(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(API_URL);
        }

        ~CdsInterface()
        {
            this.client.Dispose();
        }

        /// <summary>
        /// Creates a new record of a model.
        /// </summary>
        public async Task<string> Create(Model model)
        {
            if (!string.IsNullOrEmpty(model.Id))
            {
                return string.Empty;
            }

            return await PostJsonData(model.TableName(), JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        public async Task<bool> Update(Model model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return false;
            }

            return await PatchJsonData(model.TableName(), model.Id, JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        public async Task<User> GetUser(string userId)
        {
            JObject response = await GetJsonData(User.TABLE_NAME, $"$filter=contains(TODO!,'{userId}')");
            if (response != null && response["value"].HasValues)
            {
                return JsonConvert.DeserializeObject<User>(response["value"].ToString(), GetJsonSettings(User.Resolver.Instance));
            }

            return null;
        }

        /// <summary>
        /// Gets a user from the turn context.
        /// </summary>
        public async Task<User> GetUser(ITurnContext turnContext)
        {
            var userToken = Helpers.GetUserToken(turnContext);
            var field = string.Empty;

            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator: field = "firstname"; break;
                case Channels.Sms: field = "mobilephone"; break;
                default: Debug.Fail("Missing channel type"); return null;
            }

            JObject response = await GetJsonData(User.TABLE_NAME, $"$filter=contains({field},'{userToken}')");
            if (response != null && response["value"].HasValues)
            {
                return JsonConvert.DeserializeObject<User>(response["value"].ToString(), GetJsonSettings(User.Resolver.Instance));
            }

            return null;
        }

        /// <summary>
        /// Gets an organization by ID.
        /// </summary>
        public async Task<Organization> GetOrganization(string organizationId)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                JObject response = await GetJsonData(Organization.TABLE_NAME, "$filter=accountid eq " + organizationId);
                if (response != null && response["value"].HasValues)
                {
                    return JsonConvert.DeserializeObject<Organization>(response["value"][0].ToString(), GetJsonSettings(Organization.Resolver.Instance));
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the count of an organization's services.
        /// </summary>
        public async Task<int> GetServiceCount(string organizationId)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organizationId} &$count=true");
                if (response != null)
                {
                    return (int)response["@odata.count"];
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets an organization's service by type.
        /// </summary>
        public async Task<Service> GetService(string organizationId, ServiceType serviceType)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                if (serviceType != ServiceType.Invalid)
                {
                    JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organizationId} and tira_servicetype eq {serviceType}");
                    if (response != null && response["value"].HasValues)
                    {
                        return JsonConvert.DeserializeObject<Service>(response["value"][0].ToString(), GetJsonSettings(Service.Resolver.Instance));
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all of an organization's services.
        /// </summary>
        public async Task<List<Service>> GetServices(string organizationId)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organizationId}");
                if (response != null && response["value"].HasValues)
                {
                    return JsonConvert.DeserializeObject<List<Service>>(response["value"].ToString(), GetJsonSettings(User.Resolver.Instance));
                }
            }

            return new List<Service>();
        }

        /// <summary>
        /// Gets the latest shapshot for a service.
        /// </summary>
        /// <param name="createdByUser">Optionally pass a turn context to get the latest data created by the user</param>
        public async Task<ServiceData> GetLatestServiceData(string organizationId, ServiceData dataType, ITurnContext createdByUserTurnContext = null)
        {
            var service = await GetService(organizationId, dataType.ServiceType());
            if (service != null)
            {
                if (dataType.ServiceType() != ServiceType.Invalid)
                {
                    string userFilter = string.Empty;

                    if (createdByUserTurnContext != null)
                    {
                        var user = await GetUser(createdByUserTurnContext);
                        userFilter = $" and tira_createdby eq {user.Id}";
                    }

                    JObject response = await GetJsonData(dataType.TableName(), $"$filter={dataType.PrimaryKey()} eq {service.Id}{userFilter} &$orderby=createdon desc &$top=1");
                    if (response != null && response["value"].HasValues)
                    {
                        return JsonConvert.DeserializeObject<ServiceData>(response["value"][0].ToString(), GetJsonSettings(dataType.ContractResolver()));
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all verified organizations.
        /// </summary>
        public async Task<List<Organization>> GetVerifiedOrganizations()
        {
            JObject response = await GetJsonData(Organization.TABLE_NAME, "$filter=tira_isverified eq true");
            if (response != null && response["value"].HasValues)
            {
                return JsonConvert.DeserializeObject<List<Organization>>(response["value"].ToString(), GetJsonSettings(Organization.Resolver.Instance));
            }

            return new List<Organization>();
        }

        /// <summary>
        /// Gets all users for an organization.
        /// </summary>
        public async Task<List<User>> GetUsersForOrganization(Organization organization)
        {
            JObject response = await GetJsonData(User.TABLE_NAME, $"$filter=_parentcustomerid_value eq {organization.Id}");
            if (response != null && response["value"].HasValues)
            {
                return JsonConvert.DeserializeObject<List<User>>(response["value"].ToString(), GetJsonSettings(User.Resolver.Instance));
            }

            return new List<User>();
        }

        /// <summary>
        /// Gets JSON data from the API.
        /// </summary>
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

        /// <summary>
        /// Posts JSON data to the API.
        /// </summary>
        private async Task<string> PostJsonData(string tableName, string json)
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

        /// <summary>
        /// Patches JSON data to the API.
        /// </summary>
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
        /// Gets an authentication token from AAD.
        /// </summary>
        private async Task<string> GetAuthToken()
        {
            if (!string.IsNullOrEmpty(this.authToken))
            {
                return this.authToken;
            }

            string authorityUrl = string.Format(AUTH_URL, TENANT_ID);
            var authContext = new AuthenticationContext(authorityUrl, false);

            ClientCredential clientCred = new ClientCredential(APP_ID, APP_SECRET);
            AuthenticationResult authResult = await authContext.AcquireTokenAsync(RESOURCE_URL, clientCred);

            this.authToken = authResult.AccessToken;
            return authResult.AccessToken;
        }

        private JsonSerializerSettings GetJsonSettings(IContractResolver contractResolver)
        {
            return new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
        }
    }
}

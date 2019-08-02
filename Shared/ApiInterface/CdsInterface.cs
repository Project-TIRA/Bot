using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using EntityModel;

namespace Shared.ApiInterface
{
    /// <summary>
    /// API interface for Common Data Service (Dynamics)
    /// </summary>
    public class CdsInterface : IApiInterface
    {
        // TODO: Put these in app settings.
        const string CLIENT_ID = "4cd0f1ea-6f83-419a-a4fa-24878d30dd09";
        const string CLIENT_SECRET = "L-7r?rQY/5xo+QQ1yBJ02IEk3z9V297f";
        const string TENANT_ID = "72f988bf-86f1-41af-91ab-2d7cd011db47";
        const string RESOURCE_URL = "https://obsdev.api.crm.dynamics.com";
        const string AUTH_URL = "https://login.microsoftonline.com/{0}/oauth2/token";
        const string API_URL = "https://obsdev.api.crm.dynamics.com/api/data/v9.1/";

        HttpClient client;
        string authToken;

        public CdsInterface()
        {
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
        public async Task<string> Create<T>(T model) where T : ModelBase
        {
            if (!string.IsNullOrEmpty(model.ResourceId))
            {
                return string.Empty;
            }

            return await PostJsonData(model.TableName, JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        public async Task<bool> Update<T>(T model) where T : ModelBase
        {
            if (string.IsNullOrEmpty(model.ResourceId))
            {
                return false;
            }

            return await PatchJsonData(model.TableName, model.ResourceId, JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// Gets a user from a user ID.
        /// </summary>
        public async Task<User> GetUser(string userId)
        {
            string phoneNumber = PhoneNumber.Standardize(userId);

            JObject response = await GetJsonData(User.TABLE_NAME, $"$filter=contains(mobilephone,'{phoneNumber}')");
            if (response == null)
            {
                return null;
            }

            return response["value"].HasValues ? response["value"][0].ToObject<User>() : null;
        }

        /// <summary>
        /// Gets an organization from a user ID.
        /// </summary>
        public async Task<Organization> GetOrganization(string userId)
        {
            string phoneNumber = PhoneNumber.Standardize(userId);

            var user = await GetUser(phoneNumber);
            if (user != null)
            {
                JObject response = await GetJsonData(Organization.TABLE_NAME, "$filter=accountid eq " + user.OrganizationId);
                if (response != null)
                {
                    return response["value"].HasValues ? response["value"][0].ToObject<Organization>() : null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the count of an organization's services from a user ID.
        /// </summary>
        public async Task<int> GetServiceCount(string userId)
        {
            string phoneNumber = PhoneNumber.Standardize(userId);

            Organization organization = await GetOrganization(phoneNumber);
            if (organization != null)
            {
                JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organization.Id} &$count=true");
                if (response != null)
                {
                    return (int)response["@odata.count"];
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets an organization's service by type from a user ID.
        /// </summary>
        public async Task<Service> GetService<T>(string userId) where T : ServiceModelBase
        {
            string phoneNumber = PhoneNumber.Standardize(userId);

            Organization organization = await GetOrganization(phoneNumber);
            if (organization != null)
            {
                var type = Helpers.GetServiceType<T>();
                if (type != ServiceType.Invalid)
                {
                    JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organization.Id} and tira_servicetype eq {(int)type}");
                    if (response != null)
                    {
                        return response["value"].HasValues ? response["value"][0].ToObject<Service>() : null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the latest shapshot for a service from a user ID.
        /// </summary>
        public async Task<T> GetLatestServiceData<T>(string userId) where T : ServiceModelBase
        {
            string phoneNumber = PhoneNumber.Standardize(userId);

            var service = await GetService<T>(phoneNumber);
            if (service != null)
            {
                var type = Helpers.GetServiceType<T>();
                if (type != ServiceType.Invalid)
                {
                    var tableName = Helpers.GetServiceTableName(type);
                    var primaryKey = Helpers.GetServicePrimaryKey(type);

                    JObject response = await GetJsonData(tableName, $"$filter={primaryKey} eq {service.Id} &$orderby=createdon desc &$top=1");
                    if (response != null)
                    {
                        return response["value"].HasValues ? response["value"][0].ToObject<T>() : null;
                    }
                }
            }

            return null;
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

            ClientCredential clientCred = new ClientCredential(CLIENT_ID, CLIENT_SECRET);
            AuthenticationResult authResult = await authContext.AcquireTokenAsync(RESOURCE_URL, clientCred);

            this.authToken = authResult.AccessToken;
            return authResult.AccessToken;
        }
    }
}

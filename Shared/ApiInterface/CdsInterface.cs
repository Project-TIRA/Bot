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
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

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
            if (!string.IsNullOrEmpty(model.Id))
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
            if (string.IsNullOrEmpty(model.Id))
            {
                return false;
            }

            return await PatchJsonData(model.TableName, model.Id, JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// Deletes a model.
        /// </summary>
        public async Task<bool> Delete<T>(T model) where T : ModelBase
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return false;
            }

            return await DeleteJsonData(model.TableName, model.Id);
        }

        /// <summary>
        /// Gets a user from a user token.
        /// </summary>
        public async Task<User> GetUser(string userToken)
        {
            JObject response = await GetJsonData(User.TABLE_NAME, $"$filter=contains(mobilephone,'{userToken}')");
            if (response != null && response["value"].HasValues)
            {
                return JsonConvert.DeserializeObject<User>(response["value"].ToString(), GetJsonSettings(User.Resolver.Instance));
            }

            return null;
        }

        /// <summary>
        /// Gets an organization from a user token.
        /// </summary>
        public async Task<Organization> GetOrganization(string userToken)
        {
            var user = await GetUser(userToken);
            if (user != null)
            {
                JObject response = await GetJsonData(Organization.TABLE_NAME, "$filter=accountid eq " + user.OrganizationId);
                if (response != null && response["value"].HasValues)
                {
                    return JsonConvert.DeserializeObject<Organization>(response["value"][0].ToString(), GetJsonSettings(Organization.Resolver.Instance));
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the count of an organization's services from a user token.
        /// </summary>
        public async Task<int> GetServiceCount(string userToken)
        {
            Organization organization = await GetOrganization(userToken);
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
        /// Gets an organization's service by type from a user token.
        /// </summary>
        public async Task<Service> GetService<T>(string userToken) where T : ServiceModelBase
        {
            Organization organization = await GetOrganization(userToken);
            if (organization != null)
            {
                var type = Helpers.GetServiceType<T>();
                if (type != ServiceType.Invalid)
                {
                    JObject response = await GetJsonData(Service.TABLE_NAME, $"$filter=_tira_organizationservicesid_value eq {organization.Id} and tira_servicetype eq {(int)type}");
                    if (response != null && response["value"].HasValues)
                    {
                        return JsonConvert.DeserializeObject<Service>(response["value"][0].ToString(), GetJsonSettings(Service.Resolver.Instance));
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the latest shapshot for a service from a user token.
        /// </summary>
        /// <param name="createdByUser">Whether or not to get the latest token that was created by the given user</param>
        public async Task<T> GetLatestServiceData<T>(string userToken, bool createdByUser) where T : ServiceModelBase, new()
        {
            var service = await GetService<T>(userToken);
            if (service != null)
            {
                var type = Helpers.GetServiceType<T>();
                if (type != ServiceType.Invalid)
                {
                    var tableName = Helpers.GetServiceTableName(type);
                    var primaryKey = Helpers.GetServicePrimaryKey(type);

                    string userFilter = string.Empty;

                    if (createdByUser)
                    {
                        var user = await GetUser(userToken);
                        userFilter = $" and tira_createdby eq {user.Id}";
                    }

                    JObject response = await GetJsonData(tableName, $"$filter={primaryKey} eq {service.Id}{userFilter} &$orderby=createdon desc &$top=1");
                    if (response != null && response["value"].HasValues)
                    {
                        return JsonConvert.DeserializeObject<T>(response["value"][0].ToString(), GetJsonSettings(new T().ContractResolver));
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
        /// Clears incomplete snapshots and returns whether or not the conversation was expired.
        /// </summary>
        public async Task<bool> ClearIncompleteSnapshots(string userToken, bool forceExpire)
        {
            // Expires after 12 hours.
            var expiration = DateTime.UtcNow.AddHours(-12);
            bool didRemove = false;

            didRemove |= await ClearIncompleteSnapshot<CaseManagementData>(userToken, forceExpire);
            didRemove |= await ClearIncompleteSnapshot<HousingData>(userToken, forceExpire);
            didRemove |= await ClearIncompleteSnapshot<JobTrainingData>(userToken, forceExpire);
            didRemove |= await ClearIncompleteSnapshot<MentalHealthData>(userToken, forceExpire);
            didRemove |= await ClearIncompleteSnapshot<SubstanceUseData>(userToken, forceExpire);

            return didRemove || forceExpire;
        }

        private async Task<bool> ClearIncompleteSnapshot<T>(string userToken, bool forceExpire) where T : ServiceModelBase, new()
        {
            var expiration = DateTime.UtcNow.AddHours(-Phrases.Reset.TimeoutHours);

            var data = await GetLatestServiceData<T>(userToken, createdByUser: true);
            if (data != null && !data.IsComplete && (forceExpire || data.CreatedOn < expiration))
            {
                return await this.Delete(data);
            }

            return false;
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
        /// Deletes JSON data from the API.
        /// </summary>
        public async Task<bool> DeleteJsonData(string tableName, string resourceId)
        {
            await EnsureAuthHeader();
            string url = Path.Combine(API_URL, tableName) + "(" + resourceId + ")";
            HttpResponseMessage response = await this.client.DeleteAsync(url);
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

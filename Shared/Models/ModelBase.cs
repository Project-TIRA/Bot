using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ModelBase
    {
        public virtual string TableName { get { return string.Empty; } }
        public virtual string ResourceId { get { return string.Empty; } }

        public async Task<string> Create(ApiInterface apiInterface)
        {
            if (!string.IsNullOrEmpty(this.ResourceId))
            {
                return string.Empty;
            }

            return await apiInterface.PostJsonData(this.TableName, JsonConvert.SerializeObject(this));
        }

        public async Task<bool> Update(ApiInterface apiInterface)
        {
            if (string.IsNullOrEmpty(this.ResourceId))
            {
                return false;
            }

            return await apiInterface.PatchJsonData(this.TableName, this.ResourceId, JsonConvert.SerializeObject(this));
        }
    }
}

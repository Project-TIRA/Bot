using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EntityModel
{
    public abstract class ModelBase
    {
        public virtual string TableName { get { return string.Empty; } }
        public virtual string ResourceId { get { return string.Empty; } }
    }
}

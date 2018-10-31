using System.Runtime.Serialization;

namespace Catalogo.API.Model
{
    [DataContract]
    public abstract class BaseModel
    {
        [DataMember]
        public int Id { get; protected set; }
    }
}

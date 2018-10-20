using System.Runtime.Serialization;

namespace CasaDoCodigo.Catalogo.Model
{
    [DataContract]
    public abstract class BaseModel
    {
        [DataMember]
        public int Id { get; protected set; }
    }
}

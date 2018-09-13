using System.Runtime.Serialization;

namespace CasaDoCodigo.OrdemDeCompra.Models
{
    [DataContract]
    public abstract class BaseModel
    {
        [DataMember]
        public int Id { get; protected set; }
    }
}

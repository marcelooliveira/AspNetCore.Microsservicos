using CasaDoCodigo.Mensagens.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CasaDoCodigo.Mensagens.IntegrationEvents.Events
{
    public class CadastroEvent : IntegrationEvent
    {
        public CadastroEvent()
        {

        }

        public CadastroEvent(string usuarioId, string nome, string email, string telefone, string endereco, string complemento, string bairro, string municipio, string uF, string cEP)
        {
            UsuarioId = usuarioId;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Endereco = endereco;
            Complemento = complemento;
            Bairro = bairro;
            Municipio = municipio;
            UF = uF;
            CEP = cEP;
        }

        public string UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
    }
}

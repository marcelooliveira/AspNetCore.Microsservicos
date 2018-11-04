using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using Identity.API.Managers;
using IdentityModel;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.API.Commands
{
    public class CadastroCommandHandler
        : IRequestHandler<IdentifiedCommand<CadastroCommand, bool>, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CadastroCommandHandler> _logger;
        private readonly IClaimsManager _claimsManager;

        public CadastroCommandHandler(IMediator mediator, ILogger<CadastroCommandHandler> logger, IClaimsManager claimsManager)
        {
            this._mediator = mediator;
            this._logger = logger;
            this._claimsManager = claimsManager;
        }

        public async Task<bool> Handle(IdentifiedCommand<CadastroCommand, bool> request, CancellationToken cancellationToken)
        {
            try
            {
                var command = request.Command;
                IDictionary<string, string> claims = new Dictionary<string, string>
                {
                    ["name"] = command.Nome,
                    [JwtClaimTypes.Email] = command.Email,
                    ["address"] = command.Endereco,
                    ["address_details"] = command.Complemento,
                    ["phone"] = command.Telefone,
                    ["neighborhood"] = command.Bairro,
                    ["city"] = command.Municipio,
                    ["state"] = command.UF,
                    ["zip_code"] = command.CEP
                };
            
                await _claimsManager.AddUpdateClaim(command.UsuarioId, claims);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}

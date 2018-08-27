using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Status.ViewModels
{
    public class NamedCheckResult
    {
        public NamedCheckResult(string name, IHealthCheckResult result)
        {
            Name = name;
            Result = result;
        }

        public string Name { get; }
        public IHealthCheckResult Result { get; }
    }
}

using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Status.ViewModels
{
    public class HealthStatusViewModel
    {
        public CheckStatus OverallStatus { get; set; }

        private readonly IDictionary<string, IHealthCheckResult> _results;

        public IEnumerable<NamedCheckResult> Results
        {
            get
            {
                return _results.Select(kvp => new NamedCheckResult(kvp.Key, kvp.Value));
            }
        }

        private HealthStatusViewModel()
        {
            _results = new Dictionary<string, IHealthCheckResult>();
        }

        public HealthStatusViewModel(CheckStatus overall) : this()
        {
            OverallStatus = overall;
        }

        public void AddResult(string name, IHealthCheckResult result)
        {
            _results.Add(name, result);
        }
    }
}

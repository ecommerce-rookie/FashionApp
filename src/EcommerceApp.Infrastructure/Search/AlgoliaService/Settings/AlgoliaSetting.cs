using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Search.AlgoliaService.Settings
{
    public class AlgoliaSetting
    {
        public string ApplicationId { get; set; } = string.Empty;
        public string WriteApiKey { get; set; } = string.Empty;
        public string SearchApiKey { get; set; } = string.Empty;
    }
}

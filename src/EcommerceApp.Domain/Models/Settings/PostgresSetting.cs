using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Settings
{
    public class PostgresSetting
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } // Calculate timeout in seconds
        public int RetryCount { get; set; } // Number of retry attempts
        public int RetryDelay { get; set; } // Delay between retries in seconds
    }
}

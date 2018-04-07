using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class SwapResponse
    {
        public bool Success { get; set; }
        public string ProductionDeployId { get; set; }
        public string StagingDeployId { get; set; }
    }
}

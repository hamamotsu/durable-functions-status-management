using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class DeployToStagingRequest
    {
        public string DeployId { get; set; }
        public bool? IsTest { get; set; }
    }
}

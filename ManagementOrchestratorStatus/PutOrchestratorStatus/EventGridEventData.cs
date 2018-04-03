using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class EventGridEventData
    {
        public string HubName { get; set; }
        public string FunctionName { get; set; }
        public string Version { get; set; }
        public string InstanceId { get; set; }
        public string Reason { get; set; }
        public int EventType { get; set; }
        public DateTime EventTime { get; set; }
    }
}

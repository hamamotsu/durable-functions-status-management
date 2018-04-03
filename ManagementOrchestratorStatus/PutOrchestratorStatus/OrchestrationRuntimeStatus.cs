using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public enum OrchestrationRuntimeStatus
    {
        /// <summary>
        /// The orchestration is running (it may be actively running or waiting for input).
        /// </summary>
        Running = 0,

        /// <summary>
        /// The orchestration ran to completion.
        /// </summary>
        Completed = 1,

        /// <summary>
        /// The orchestration completed with ContinueAsNew as is in the process of restarting.
        /// </summary>
        ContinuedAsNew = 2,

        /// <summary>
        /// The orchestration failed with an error.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// The orchestration was gracefully canceled.
        /// </summary>
        Canceled = 4,

        /// <summary>
        /// The orchestration was abruptly terminated via an API call.
        /// </summary>
        Terminated = 5,

        /// <summary>
        /// The orchestration was scheduled but is not yet active.
        /// </summary>
        Pending = 6,
    }
}

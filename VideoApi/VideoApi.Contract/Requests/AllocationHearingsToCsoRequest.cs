using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class AllocationHearingsToCsoRequest
    {
        public string AllocatedCsoUserName { get; set; }
        public IList<string> Messages { get; set; } = new List<string>();
    }
}

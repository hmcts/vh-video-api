using System.Collections.Generic;

namespace VideoApi.Contract.Responses
{
    public class JudgeNameListResponse
    {
        /// <summary>
        /// List of Judge's first name
        /// </summary>
        public IList<string> FirstNames { get; set; }
    }
}

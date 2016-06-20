using System;

namespace Hafnium.WebServices.Json
{
    public class JsonRequest
    {
        public Guid ExecutionId { get; set; }

        public string Rule { get; set; }

        public string Message { get; set; }

        public DateTime MomentStart { get; set; }
    }
}

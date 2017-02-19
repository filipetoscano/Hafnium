using System;

namespace Hafnium.WebServices.Json
{
    /// <summary />
    public class JsonRequest
    {
        /// <summary />
        public Guid ExecutionId { get; set; }

        /// <summary />
        public string Rule { get; set; }

        /// <summary />
        public string Message { get; set; }

        /// <summary />
        public DateTime MomentStart { get; set; }
    }
}

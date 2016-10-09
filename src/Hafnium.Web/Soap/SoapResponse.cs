namespace Hafnium.WebServices.Soap
{
    /// <summary />
    public class SoapResponse
    {
        /// <summary />
        public SoapVersion Version { get; set; }

        /// <summary />
        public bool IsFault { get; set; }

        /// <summary />
        public string Message { get; set; }
    }
}

namespace Hafnium.WebServices.Soap
{
    /// <summary />
    public class SoapRequest
    {
        /// <summary />
        public SoapVersion Version { get; set; }

        /// <summary />
        public string SoapAction { get; set; }

        /// <summary />
        public string Message { get; set; }
    }
}

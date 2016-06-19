namespace Hafnium.Web.Soap
{
    public class SoapRequest
    {
        public SoapVersion Version { get; set; }

        public string SoapAction { get; set; }

        public string Message { get; set; }
    }
}

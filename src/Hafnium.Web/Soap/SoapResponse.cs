namespace Hafnium.WebServices.Soap
{
    public class SoapResponse
    {
        public SoapVersion Version { get; set; }

        public bool IsFault { get; set; }

        public string Message { get; set; }
    }
}

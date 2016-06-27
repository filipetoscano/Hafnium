using System;
using System.Text;

namespace Hafnium
{
    public class RuleContent
    {
        public string Version
        {
            get;
            set;
        }


        public DateTime LastModified
        {
            get;
            set;
        }


        public byte[] Bytes
        {
            get;
            set;
        }


        public string AsString()
        {
            return Encoding.UTF8.GetString( this.Bytes );
        }
    }
}

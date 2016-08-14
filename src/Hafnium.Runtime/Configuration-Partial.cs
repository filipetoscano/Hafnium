using Platinum.Configuration;
using System.Collections.Specialized;
using System.Configuration;
using KV = Platinum.Configuration.KeyValueConfigurationElement;

namespace Hafnium.Runtime
{
    public partial class RuntimeLoaderConfiguration
    {
        [ConfigurationProperty( "", IsRequired = true, IsDefaultCollection = true )]
        public ConfigurationElementCollection<KV> Settings
        {
            get { return (ConfigurationElementCollection<KV>) this[ "" ]; }
            set { this[ "" ] = value; }
        }


        public NameValueCollection ToNvcSettings()
        {
            NameValueCollection nvc = new NameValueCollection();

            foreach ( var kv in this.Settings )
                nvc.Set( kv.Key, kv.Value );

            return nvc;
        }
    }
}

using System.Collections.Generic;

namespace Hafnium.Runtime
{
    public partial class RuntimeLoaderConfiguration
    {
        public Dictionary<string,string> AsDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach ( var kv in this.Settings )
                dict[ kv.Key ] = kv.Value;

            return dict;
        }
    }
}

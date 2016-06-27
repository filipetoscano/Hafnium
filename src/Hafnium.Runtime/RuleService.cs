using System.Collections.Generic;

namespace Hafnium.Runtime
{
    public class RuleService
    {
        public string Name
        {
            get;
            internal set;
        }


        public string Namespace
        {
            get;
            internal set;
        }


        public Dictionary<string, IRule> Rules
        {
            get;
            internal set;
        }
    }
}

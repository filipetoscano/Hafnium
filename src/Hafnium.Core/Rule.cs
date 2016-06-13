using System;

namespace Hafnium
{
    public class Rule
    {
        public string Name
        {
            get;
            set;
        }


        public Type RequestType
        {
            get;
            set;
        }


        public Type ResponseType
        {
            get;
            set;
        }
    }
}

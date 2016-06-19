using System;

namespace Hafnium
{
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class MapAttribute : Attribute
    {
        public MapAttribute()
        {
        }


        public string Expression
        {
            get;
            set;
        }
    }
}

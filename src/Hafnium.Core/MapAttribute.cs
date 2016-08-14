using System;

namespace Hafnium
{
    /// <summary>
    /// Rule-engine specific mapping information.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class MapAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapAttribute" />.
        /// </summary>
        public MapAttribute()
        {
        }


        /// <summary>
        /// Gets or sets the rule-engine specific mapping expression.
        /// </summary>
        public string Expression
        {
            get;
            set;
        }
    }
}

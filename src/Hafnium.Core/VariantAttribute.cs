using System;

namespace Hafnium
{
    /// <summary>
    /// Used to mark that a property is to be used to set the
    /// variant name.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false, Inherited = false )]
    public class VariantAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="VariantAttribute" />.
        /// </summary>
        /// <param name="position">
        /// Position of the part in the variant name.
        /// </param>
        public VariantAttribute( int position = 0 )
        {
            this.Position = position;
        }


        /// <summary>
        /// Gets the position of the part in the variant name.
        /// </summary>
        public int Position
        {
            get;
            private set;
        }
    }
}

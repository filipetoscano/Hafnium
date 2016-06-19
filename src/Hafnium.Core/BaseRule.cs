using System;
using System.Linq;

namespace Hafnium
{
    public class BaseRule
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BaseRule" />. This base
        /// class is only usable if the derived class also implements
        /// <see cref="IRule{Rq, Rp}"/>.
        /// </summary>
        public BaseRule()
        {
            Type tr = this.GetType()
                .GetInterfaces()
                .Where( x => x != typeof( IRule ) )
                .Where( x => typeof( IRule ).IsAssignableFrom( x ) )
                .First();

            Type[] gta = tr.GenericTypeArguments;

            this.RequestType = gta[ 0 ];
            this.ResponseType = gta[ 1 ];
        }


        /// <summary>
        /// Gets documentation pertinent to the present rule.
        /// </summary>
        public RuleMetadata Metadata
        {
            get
            {
                // TODO
                return null;
            }
        }


        /// <summary>
        /// Gets the .NET type of the request class to the present rule.
        /// </summary>
        public Type RequestType
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the .NET type of the response class to the present rule.
        /// </summary>
        public Type ResponseType
        {
            get;
            private set;
        }
    }
}

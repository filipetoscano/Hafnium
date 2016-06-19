using System;

namespace Hafnium
{
    public interface IRule
    {
        /// <summary>
        /// Gets documentation pertinent to the present rule.
        /// </summary>
        RuleMetadata Metadata
        {
            get;
        }


        /// <summary>
        /// Gets the .NET type of the request class to the present rule.
        /// </summary>
        Type RequestType
        {
            get;
        }


        /// <summary>
        /// Gets the .NET type of the response class to the present rule.
        /// </summary>
        Type ResponseType
        {
            get;
        }
    }


    public interface IRule<Rq,Rp> : IRule
    {
    }
}

using System;

namespace Hafnium
{
    /// <summary>
    /// Describes a rule.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        string Name
        {
            get;
        }

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


    /// <summary>
    /// Describes a rule.
    /// </summary>
    /// <typeparam name="Rq">Type of request message.</typeparam>
    /// <typeparam name="Rp">Type of response message.</typeparam>
    public interface IRule<Rq,Rp> : IRule
    {
    }
}

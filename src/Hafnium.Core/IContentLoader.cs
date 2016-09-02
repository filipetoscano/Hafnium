using System.Collections.Generic;

namespace Hafnium
{
    /// <summary>
    /// Describes a component which can load the content of a given
    /// rule.
    /// </summary>
    public interface IContentLoader
    {
        /// <summary>
        /// Initializes the content loader instance.
        /// </summary>
        /// <param name="settings">Configuration settings.</param>
        void Initialize( Dictionary<string,string> settings );


        /// <summary>
        /// Loads the rule content.
        /// </summary>
        /// <param name="context">Rule execution context.</param>
        /// <returns>Content, as a byte array.</returns>
        RuleContent Load( RuleContext context );
    }
}

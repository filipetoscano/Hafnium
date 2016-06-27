using System.Collections.Specialized;

namespace Hafnium
{
    public interface IContentLoader
    {
        /// <summary>
        /// Initializes the content loader instance.
        /// </summary>
        /// <param name="settings">Configuration settings.</param>
        void Initialize( NameValueCollection settings );


        /// <summary>
        /// Loads the rule content.
        /// </summary>
        /// <param name="context">Rule execution context.</param>
        /// <returns>Content, as a byte array.</returns>
        byte[] Load( RuleContext context );
    }
}

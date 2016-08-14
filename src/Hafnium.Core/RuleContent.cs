using System;
using System.Text;

namespace Hafnium
{
    public class RuleContent
    {
        /// <summary>
        /// Gets or sets the version of the rule content.
        /// </summary>
        public string Version
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the moment in which the file was last modified.
        /// </summary>
        public DateTime LastModified
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the rule content.
        /// </summary>
        public byte[] Bytes
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the rule content as a character sequence.
        /// </summary>
        /// <returns>String content.</returns>
        public string AsString()
        {
            return Encoding.UTF8.GetString( this.Bytes );
        }
    }
}

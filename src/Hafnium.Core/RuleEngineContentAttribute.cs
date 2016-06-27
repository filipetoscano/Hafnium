using System;

namespace Hafnium
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class RuleEngineContentAttribute : Attribute
    {
        public RuleEngineContentAttribute()
        {
        }


        /// <summary>
        /// Gets the filename extension for files targetting that language.
        /// </summary>
        public string Extension
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the filename extension for files targetting that language.
        /// </summary>
        public string Syntax
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the filename extension for files targetting that language.
        /// </summary>
        public string MimeType
        {
            get;
            set;
        }
    }
}

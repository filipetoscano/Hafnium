using System;

namespace Hafnium
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class RuleEngineAttribute : Attribute
    {
        public RuleEngineAttribute( string name )
        {
            this.Name = name;
        }


        /// <summary>
        /// Gets the name of the rule engine which executes the current rule.
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }
    }
}

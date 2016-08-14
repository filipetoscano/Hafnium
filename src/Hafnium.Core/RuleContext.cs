using System;

namespace Hafnium
{
    public class RuleContext
    {
        public Guid ExecutionId
        {
            get;
            set;
        }


        public IRule Rule
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the name of the rule variant.
        /// </summary>
        /// <remarks>
        /// These are rules that have the exact same interface (in terms of
        /// contract), but the rule itself is different.
        /// </remarks>
        public string RuleVariant
        {
            get;
            set;
        }


        public IRuleEngine Engine
        {
            get;
            set;
        }


        public RuleContent Content
        {
            get;
            set;
        }
    }
}

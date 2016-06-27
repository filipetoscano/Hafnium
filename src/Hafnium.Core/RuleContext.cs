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


        public IRuleEngine Engine
        {
            get;
            set;
        }


        public byte[] Content
        {
            get;
            set;
        }
    }
}

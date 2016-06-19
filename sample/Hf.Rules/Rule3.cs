using Hafnium;
using System;

namespace Hf.Rules
{
    public class Rule3Request
    {
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        public string String { get; set; }
    }


    public class Rule3Response
    {
        public bool Boolean { get; set; }
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        public DateTime DateTime { get; set; }
        public string String { get; set; }
    }


    [RuleEngine( "Python" )]
    public class Rule3 : BaseRule, IRule<Rule3Request, Rule3Response>
    {
    }
}

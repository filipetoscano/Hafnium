using Hafnium;
using System;

namespace Hf.Rules
{
    public class Rule2Request
    {
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        public string String { get; set; }
    }


    public class Rule2Response
    {
        public bool Boolean { get; set; }
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        public DateTime DateTime { get; set; }
        public string String { get; set; }
    }


    [RuleEngine( "Javascript" )]
    public class Rule2 : BaseRule, IRule<Rule2Request, Rule2Response>
    {
    }
}

using Hafnium;
using System;

namespace Hf.Rules
{
    public class Rule4Request
    {
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        public string String { get; set; }
    }


    public class Rule4Response
    {
        public bool Boolean { get; set; }
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        public DateTime DateTime { get; set; }
        public string String { get; set; }
    }


    [RuleEngine( "CSharp" )]
    public class Rule4 : BaseRule, IRule<Rule4Request, Rule4Response>
    {
    }
}

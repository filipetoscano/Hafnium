using Hafnium;
using System;

namespace Hf.Rules
{
    public class Rule1Request
    {
        [Map( Expression = "API!B3" )]
        public int Integer { get; set; }

        [Map( Expression = "API!B4" )]
        public decimal Decimal { get; set; }

        [Map( Expression = "API!B5" )]
        public string String { get; set; }
    }


    public class Rule1Response
    {
        [Map( Expression = "API!E3" )]
        public bool Boolean { get; set; }

        [Map( Expression = "API!E4" )]
        public int Integer { get; set; }

        [Map( Expression = "API!E5" )]
        public decimal Decimal { get; set; }

        [Map( Expression = "API!E6" )]
        public DateTime DateTime { get; set; }

        [Map( Expression = "API!E7" )]
        public string String { get; set; }
    }


    [RuleEngine( "Excel" )]
    public class Rule1 : BaseRule, IRule<Rule1Request, Rule1Response>
    {
    }
}

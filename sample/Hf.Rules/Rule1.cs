// autogenerate: do NOT manually edit
using Hafnium;
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Hf.Rules
{
    /// <summary>
    /// Request message for flow.
    /// </summary>
    public class Rule1Request
    {
        /// <summary />
        [Map( Expression = "API!B3" )]
        public int Integer { get; set; }

        /// <summary />
        [Map( Expression = "API!B4" )]
        public decimal Decimal { get; set; }

        /// <summary />
        [Map( Expression = "API!B5" )]
        public string String { get; set; }
    }


    /// <summary>
    /// Response message for flow.
    /// </summary>
    public class Rule1Response
    {
        /// <summary />
        [Map( Expression = "API!E3" )]
        public bool Boolean { get; set; }

        /// <summary />
        [Map( Expression = "API!E4" )]
        public int Integer { get; set; }

        /// <summary />
        [Map( Expression = "API!E5" )]
        public decimal Decimal { get; set; }

        /// <summary />
        [JsonConverter( typeof( Platinum.Json.DateConverter ) )]
        [XmlElement( "xsd:date" )]
        [Map( Expression = "API!E6" )]
        public DateTime DateTime { get; set; }

        /// <summary />
        [Map( Expression = "API!E7" )]
        public string String { get; set; }
    }


    /// <summary>
    /// ?
    /// </summary>
    [RuleEngine( "Excel" )]
    public partial class Rule1 : BaseRule, IRule<Rule1Request, Rule1Response>
    {
    }

}
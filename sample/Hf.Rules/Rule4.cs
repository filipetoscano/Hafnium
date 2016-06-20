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
    public class Rule4Request
    {
        /// <summary />
        public int Integer { get; set; }

        /// <summary />
        public decimal Decimal { get; set; }

        /// <summary />
        public string String { get; set; }
    }


    /// <summary>
    /// Response message for flow.
    /// </summary>
    public class Rule4Response
    {
        /// <summary />
        public bool Boolean { get; set; }

        /// <summary />
        public int Integer { get; set; }

        /// <summary />
        public decimal Decimal { get; set; }

        /// <summary />
        [JsonConverter( typeof( Platinum.Json.DateConverter ) )]
        [XmlElement( "xsd:date" )]
        public DateTime DateTime { get; set; }

        /// <summary />
        public string String { get; set; }
    }


    /// <summary>
    /// ?
    /// </summary>
    [RuleEngine( "CSharp" )]
    public partial class Rule4 : BaseRule, IRule<Rule4Request, Rule4Response>
    {
    }

}
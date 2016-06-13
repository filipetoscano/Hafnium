using Hafnium;

namespace Hf.Rules
{
    public class Rule1Request
    {
        public int Age { get; set; }
        public double Amount { get; set; }
    }


    public class Rule1Response
    {
        public bool IsAllowed { get; set; }
    }


    public class Rule1 : IRule<Rule1Request, Rule1Response>
    {
    }
}

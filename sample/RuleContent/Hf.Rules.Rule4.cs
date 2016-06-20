using System;
using System.Linq;

namespace Hf.Rules
{
    public class Rule
    {
        public static Rule4Response Run( Rule4Request request )
        {
            var response = new Rule4Response();
            response.String = "from c#, orig=" + request.String;

            return response;
        }
    }
}
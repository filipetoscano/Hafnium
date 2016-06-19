using Hafnium.Runtime;
using Hf.Rules;
using System;

namespace Hf.Client
{
    class Program
    {
        static void Main( string[] args )
        {
            int i = 10;
            decimal d = 1500.0m;
            string s = "hello";

            if ( args.Length > 0 )
                i = int.Parse( args[ 0 ] );

            if ( args.Length > 1 )
                d = decimal.Parse( args[ 1 ] );

            if ( args.Length > 2 )
                s = args[ 2 ];

            //Rule1( i, d, s );
            Rule2( i, d, s );
            Rule3( i, d, s );
            Rule4( i, d, s );
        }


        /// <summary>
        /// Excel rule.
        /// </summary>
        public static void Rule1( int i, decimal d, string s )
        {
            Rule1Request request = new Rule1Request();
            request.Integer = i;
            request.Decimal = d;
            request.String = s;

            Console.WriteLine( "--- Rule1Request ---" );
            Console.WriteLine( request.Integer );
            Console.WriteLine( request.Decimal );
            Console.WriteLine( request.String );

            RuleRunner rr = new RuleRunner();
            Rule1Response response = (Rule1Response) rr.Run( "Hf.Rules.Rule1", request );

            Console.WriteLine( "--- Rule1Response ---" );
            Console.WriteLine( response.Boolean );
            Console.WriteLine( response.DateTime );
            Console.WriteLine( response.Decimal );
            Console.WriteLine( response.Integer );
            Console.WriteLine( response.String );
        }


        /// <summary>
        /// Javascript rule.
        /// </summary>
        public static void Rule2( int i, decimal d, string s )
        {
            Rule2Request request = new Rule2Request();
            request.Integer = i;
            request.Decimal = d;
            request.String = s;

            Console.WriteLine( "--- Rule2Request ---" );
            Console.WriteLine( request.Integer );
            Console.WriteLine( request.Decimal );
            Console.WriteLine( request.String );

            RuleRunner rr = new RuleRunner();
            Rule2Response response = (Rule2Response) rr.Run( "Hf.Rules.Rule2", request );

            Console.WriteLine( "--- Rule2Response ---" );
            Console.WriteLine( response.Boolean );
            Console.WriteLine( response.DateTime );
            Console.WriteLine( response.Decimal );
            Console.WriteLine( response.Integer );
            Console.WriteLine( response.String );
        }


        /// <summary>
        /// Python rule.
        /// </summary>
        public static void Rule3( int i, decimal d, string s )
        {
            Rule3Request request = new Rule3Request();
            request.Integer = i;
            request.Decimal = d;
            request.String = s;

            Console.WriteLine( "--- Rule3Request ---" );
            Console.WriteLine( request.Integer );
            Console.WriteLine( request.Decimal );
            Console.WriteLine( request.String );

            RuleRunner rr = new RuleRunner();
            Rule3Response response = (Rule3Response) rr.Run( "Hf.Rules.Rule3", request );

            Console.WriteLine( "--- Rule3Response ---" );
            Console.WriteLine( response.Boolean );
            Console.WriteLine( response.DateTime );
            Console.WriteLine( response.Decimal );
            Console.WriteLine( response.Integer );
            Console.WriteLine( response.String );
        }


        /// <summary>
        /// C# rule.
        /// </summary>
        public static void Rule4( int i, decimal d, string s )
        {
            Rule4Request request = new Rule4Request();
            request.Integer = i;
            request.Decimal = d;
            request.String = s;

            Console.WriteLine( "--- Rule4Request ---" );
            Console.WriteLine( request.Integer );
            Console.WriteLine( request.Decimal );
            Console.WriteLine( request.String );

            RuleRunner rr = new RuleRunner();
            Rule4Response response = (Rule4Response) rr.Run( "Hf.Rules.Rule4", request );

            Console.WriteLine( "--- Rule4Response ---" );
            Console.WriteLine( response.Boolean );
            Console.WriteLine( response.DateTime );
            Console.WriteLine( response.Decimal );
            Console.WriteLine( response.Integer );
            Console.WriteLine( response.String );
        }
    }
}

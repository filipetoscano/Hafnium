using System;
using System.Collections.Generic;
using System.Linq;

namespace Hafnium.Runtime
{
    public class RuleCatalogue
    {
        private static object _lock = new object();
        private static bool _init = false;
        private static List<RuleService> _services;
        private static List<IRule> _rules;


        public static IReadOnlyCollection<RuleService> Services
        {
            get
            {
                if ( _init == false )
                    Ensure();

                return _services.AsReadOnly();
            }
        }


        public static IRule Get( string ruleName )
        {
            #region Validations

            if ( ruleName == null )
                throw new ArgumentNullException( nameof( ruleName ) );

            #endregion

            if ( _init == false )
                Ensure();


            /*
             * 
             */
            return _rules.FirstOrDefault( r => r.Name == ruleName );
        }


        private static void Ensure()
        {
            lock ( _lock )
            {
                if ( _init == false )
                {
                    EnsureDo();

                    _init = true;
                }
            }
        }


        private static void EnsureDo()
        {
            _services = new List<RuleService>();
            _rules = new List<IRule>();


            /*
             * How many years did it take, to finally be able to write 'irule' in code? :P
             */
            Type irule = typeof( IRule );

            var rules = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where( a => !a.IsDynamic )
                .SelectMany( a => a.GetExportedTypes() )
                .Where( t => t.IsAbstract == false )
                .Where( t => t.IsInterface == false )
                .Where( t => irule.IsAssignableFrom( t ) );

            foreach ( var ruleType in rules )
            {
                /*
                 * Flat structure of rules.
                 */
                IRule rule = Platinum.Activator.Create<IRule>( ruleType );

                _rules.Add( rule );


                /*
                 * Grouped into services.
                 *   1..N-1 = service namespace
                 *   N-1    = service name
                 *   Nth    = rule name
                 */
                string[] parts = ruleType.FullName.Split( '.' );

                int n = parts.Length;
                string ruleName = parts[ n - 1 ];
                string serviceName = parts[ n - 2 ];
                string serviceNs = string.Join( "/", parts.Take( n - 2 ).ToArray() ) + "/";

                RuleService rs = _services.FirstOrDefault( s => s.Name == serviceName && s.Namespace == serviceNs );

                if ( rs == null )
                {
                    rs = new RuleService()
                    {
                        Name = serviceName,
                        Namespace = serviceNs,
                        Rules = new Dictionary<string, IRule>()
                    };

                    _services.Add( rs );
                }

                rs.Rules.Add( ruleName, rule );
            }
        }
    }
}

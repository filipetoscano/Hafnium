using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hafnium.Runtime
{
    public static class EngineFactory
    {
        private static object _lock = new object();
        private static bool _init = false;
        private static Dictionary<string, Type> _engines;


        public static IRuleEngine For( string name )
        {
            #region Validations

            if ( name == null )
                throw new ArgumentNullException( nameof( name ) );

            #endregion

            if ( _init == false )
                Ensure();


            /*
             * 
             */
            Type type;

            if ( _engines.TryGetValue( name, out type ) == false )
                return null;

            return Platinum.Activator.Create<IRuleEngine>( type );
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
            Dictionary<string, Type> engines = new Dictionary<string, Type>();

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where( a => !a.IsDynamic )
                .SelectMany( a => a.GetExportedTypes() )
                .Where( t => t.IsAbstract == false )
                .Where( t => t.IsInterface == false )
                .Where( t => typeof( IRuleEngine ).IsAssignableFrom( t ) );

            foreach ( var type in types )
            {
                RuleEngineAttribute rea = type.GetCustomAttribute<RuleEngineAttribute>();

                if ( rea == null )
                    engines.Add( type.Name.Replace( "Engine", "" ), type );
                else
                    engines.Add( rea.Name, type );
            }

            _engines = engines;
        }
    }
}

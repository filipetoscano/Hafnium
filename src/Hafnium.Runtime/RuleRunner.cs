using System;
using System.Reflection;

namespace Hafnium.Runtime
{
    public class RuleRunner
    {
        public IRule Get( string name )
        {
            Type todo = Type.GetType( "Hf.Rules.Rule1Request" );

            IRule r = Platinum.Activator.Create<IRule>( todo );

            return r;
        }


        public object Run( string name, object request )
        {
            /*
             * 
             */
            IRule r = Get( name );


            /*
             * 
             */
            IRuleEngine re = null;
            RuleEngineAttribute  rea = r.GetType().GetCustomAttribute<RuleEngineAttribute>();

            if ( rea.Name == "Excel" )
                re = Platinum.Activator.Create<IRuleEngine>( "Hafnium.Engine.Excel.ExcelEngine,Hafnium.Engine.Excel" );

            if ( rea.Name == "Javascript" )
                re = Platinum.Activator.Create<IRuleEngine>( "Hafnium.Engine.Javascript.JavascriptEngine,Hafnium.Engine.Javascript" );

            if ( re == null )
                throw new NotSupportedException( rea.Name );


            /*
             * 
             */
            return re.Run( r, request );
        }
    }
}

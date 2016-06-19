using Jurassic;
using Jurassic.Library;
using System;
using System.IO;
using System.Reflection;

namespace Hafnium.Engine.Javascript
{
    [RuleEngine( "Javascript" )]
    public class JavascriptEngine : IRuleEngine
    {
        public object Run( IRule rule, object request )
        {
            /*
             * 
             */
            string script = File.ReadAllText( @"C:\Work\Transition\Hafnium\sample\RuleContent\Hf.Rules.Rule2.js" );


            var engine = new ScriptEngine();
            engine.EnableExposedClrTypes = true;
            engine.SetGlobalValue( "request", request );

            engine.Execute( script );


            /*
             * 
             */
            ObjectInstance rx = (ObjectInstance) engine.GetGlobalValue( "response" );
            object response = Activator.CreateInstance( rule.ResponseType );

            foreach ( var prop in rx.Properties )
            {
                PropertyInfo pi = rule.ResponseType.GetProperty( prop.Name );

                if ( pi == null )
                    continue;

                object value = JsType.ToValue( pi.PropertyType, prop.Value );
                pi.SetValue( response, value );
            }

            return response;
        }
    }
}

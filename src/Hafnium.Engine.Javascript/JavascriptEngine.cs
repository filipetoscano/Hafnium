using Jurassic;
using Jurassic.Library;
using System;
using System.Reflection;

namespace Hafnium.Engine.Javascript
{
    [RuleEngine( "Javascript" )]
    [RuleEngineContent( Extension = ".js", Syntax = "javascript", MimeType = "application/javascript" )]
    public class JavascriptEngine : IRuleEngine
    {
        public object Run( RuleContext context, object request )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion

            /*
             * 
             */
            IRule rule = context.Rule;
            string script = context.Content.AsString();


            /*
             * Engine
             */
            var engine = new ScriptEngine();
            engine.EnableExposedClrTypes = true;


            /*
             * Input
             */
            engine.SetGlobalValue( "request", request );


            /*
             * Table lookups
             */
            engine.SetGlobalFunction( "lookup", new Func<ObjectInstance, ObjectInstance>( ( ObjectInstance keys ) =>
            {
                int i = (int) keys.GetPropertyValue( "KeyInt" );

                ObjectInstance oi = engine.Object;
                oi.SetPropertyValue( "Value1", i * 2, false );
                oi.SetPropertyValue( "Value2", i * 3, false );

                return oi;
            } ) );


            /*
             * Execute
             */
            try
            {
                engine.Execute( script );
            }
            catch ( JavaScriptException ex )
            {
                throw new JavascriptEngineException( ER.ExecuteFailed, ex, rule.Name );
            }


            /*
             * Output
             */
            ObjectInstance rx = (ObjectInstance) engine.GetGlobalValue( "response" );

            if ( rx == null )
                throw new JavascriptEngineException( ER.ResponseNull, rule.Name );


            /*
             * 
             */
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

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Text;

namespace Hafnium.Engine.Python
{
    [RuleEngine( "Python" )]
    [RuleEngineContent( Extension = ".py", Syntax = "python", MimeType = "text/plain" )]
    public class PythonEngine : IRuleEngine
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
            string script = Encoding.UTF8.GetString( context.Content );


            /*
             * Actual engine :) We're just the very lazy wrapper that does parameter
             * mapping.
             */
            ScriptEngine py = IronPython.Hosting.Python.CreateEngine();
            ScriptScope pys = py.CreateScope();

            ScriptSource src = py.CreateScriptSourceFromString( script );
            CompiledCode compiled;

            try
            {
                compiled = src.Compile();
            }
            catch ( SyntaxErrorException ex )
            {
                throw new PythonEngineException( ER.CompileError, ex, rule.Name );
            }


            /*
             * Input
             */
            pys.SetVariable( "request", request );


            /*
             * Run!
             */
            compiled.Execute( pys );


            /*
             * Output
             */
            object response = pys.GetVariable( "response" );

            if ( response == null )
                throw new PythonEngineException( ER.ResponseNull, rule.Name, rule.ResponseType.FullName );

            if ( response.GetType() != rule.ResponseType )
                throw new PythonEngineException( ER.ResponseWrongType, rule.Name, rule.ResponseType.FullName, response.GetType() );

            return response;
        }
    }
}

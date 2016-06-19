using Microsoft.Scripting.Hosting;
using System;
using System.IO;

namespace Hafnium.Engine.Python
{
    [RuleEngine( "Python" )]
    public class PythonEngine : IRuleEngine
    {
        public object Run( IRule rule, object request )
        {
            /*
             * 
             */
            string script = File.ReadAllText( @"C:\Work\Transition\Hafnium\sample\RuleContent\Hf.Rules.Rule3.py" );


            /*
             * Actual engine :) We're just the very lazy wrapper that does parameter
             * mapping.
             */
            ScriptEngine py = IronPython.Hosting.Python.CreateEngine();
            ScriptScope pys = py.CreateScope();

            ScriptSource src = py.CreateScriptSourceFromString( script );
            CompiledCode compiled = src.Compile();


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

            if ( response.GetType() != rule.ResponseType )
                throw new NotSupportedException();

            return response;
        }
    }
}

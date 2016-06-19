using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace Hafnium.Engine.CSharp
{
    [RuleEngine( "CSharp" )]
    public class CSharpEngine : IRuleEngine
    {
        public object Run( IRule rule, object request )
        {
            /*
             * 
             */
            string script = File.ReadAllText( @"C:\Work\Transition\Hafnium\sample\RuleContent\Hf.Rules.Rule4.cs" );


            /*
             * 
             */
            var csc = new CSharpCodeProvider();

            var parameters = new CompilerParameters();
            parameters.CompilerOptions = "/optimize";
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            parameters.TreatWarningsAsErrors = false;

            // TODO: How to figure out?
            parameters.ReferencedAssemblies.Add( "System.dll" );
            parameters.ReferencedAssemblies.Add( "Hf.Rules.dll" );

            CompilerResults compile = csc.CompileAssemblyFromSource( parameters, script );

            if ( compile.Errors.Count > 0 )
                throw new ArgumentException( "compilation" );


            /*
             * 
             */
            Module module = compile.CompiledAssembly.GetModules()[ 0 ];
            Type type = module.GetType( rule.GetType().Namespace + ".Rule" );
            MethodInfo run = type.GetMethod( "Run" );


            /*
             * 
             */
            object response = run.Invoke( null, new object[] { request } );

            return response;
        }
    }
}

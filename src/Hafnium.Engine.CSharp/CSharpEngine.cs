using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hafnium.Engine.CSharp
{
    [RuleEngine( "CSharp" )]
    public class CSharpEngine : IRuleEngine
    {
        public object Run( IRule rule, object request )
        {
            #region Validations

            if ( rule == null )
                throw new ArgumentNullException( nameof( rule ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion


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
            var assemblies = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where( a => !a.IsDynamic )
                            .Select( a => a.Location );

            parameters.ReferencedAssemblies.AddRange( assemblies.ToArray() );

            //parameters.ReferencedAssemblies.Add( "System.dll" );
            //parameters.ReferencedAssemblies.Add( "System.Core.dll" );
            //parameters.ReferencedAssemblies.Add( "Hf.Rules.dll" );

            CompilerResults compile = csc.CompileAssemblyFromSource( parameters, script );

            if ( compile.Errors.Count > 0 )
            {
                // TODO: Collect errors.

                throw new CSharpEngineException( ER.CompilationError, rule.Name );
            }


            /*
             * 
             */
            Module module = compile.CompiledAssembly.GetModules()[ 0 ];

            string typeName = rule.GetType().Namespace + ".Rule";
            Type type = module.GetType( typeName );

            if ( type == null )
                throw new CSharpEngineException( ER.TypeNotFound, rule.Name, typeName );

            MethodInfo run = type.GetMethod( "Run" );

            if ( run == null )
                throw new CSharpEngineException( ER.MethodNotFound, rule.Name, "Run" );


            /*
             * 
             */
            object response;

            try
            {
                response = run.Invoke( null, new object[] { request } );
            }
            catch ( TargetInvocationException ex )
            {
                throw new CSharpEngineException( ER.InvokeException, ex.InnerException, rule.Name );
            }


            /*
             * 
             */
            if ( response == null )
                throw new CSharpEngineException( ER.ResponseNull, rule.Name, rule.RequestType.FullName );

            if ( response.GetType() != rule.ResponseType )
                throw new CSharpEngineException( ER.ResponseWrongType, rule.Name, rule.RequestType.FullName, response.GetType().FullName );

            return response;
        }
    }
}

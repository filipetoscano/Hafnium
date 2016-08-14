using Platinum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hafnium.Runtime
{
    public class RuleRunner
    {
        public IRule Get( string ruleName )
        {
            #region Validations

            if ( ruleName == null )
                throw new ArgumentNullException( nameof( ruleName ) );

            #endregion

            return RuleCatalogue.Get( ruleName );
        }


        /// <summary>
        /// Runs the designated rule, with the given input parameters.
        /// </summary>
        /// <param name="ruleName">Name of the rule.</param>
        /// <param name="request">Request object for the corresponding rule.</param>
        /// <returns>Response object, as a result of executing the rule.</returns>
        public object Run( string ruleName, object request )
        {
            IRule r = Get( ruleName );
            return Run( r, request );
        }


        /// <summary>
        /// Runs the designated rule, with the given input parameters.
        /// </summary>
        /// <param name="rule">Rule.</param>
        /// <param name="request">Request object for the corresponding rule.</param>
        /// <returns>Response object, as a result of executing the rule.</returns>
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
            if ( request.GetType() != rule.RequestType )
                throw new RuleRuntimeException( ER.RequestWrongType, rule.Name, rule.RequestType.FullName, request.GetType().FullName );

            RuleContext ctx = new RuleContext();
            ctx.ExecutionId = Guid.NewGuid();
            ctx.Rule = rule;


            /*
             * 
             */
            RuleEngineAttribute rea = rule.GetType().GetCustomAttribute<RuleEngineAttribute>();

            if ( rea == null )
                throw new RuleRuntimeException( ER.RuleNoEngine, rule.Name, rule.GetType().FullName );

            IRuleEngine engine = EngineFactory.For( rea.Name );

            if ( engine == null )
                throw new RuleRuntimeException( ER.EngineNotFound, rea.Name );

            ctx.Engine = engine;


            /*
             * 
             */
            List<Tuple<int, string>> variantTuples = new List<Tuple<int, string>>();

            foreach ( PropertyInfo prop in request.GetType().GetProperties() )
            {
                VariantAttribute variantAttr = prop.GetCustomAttribute<VariantAttribute>();

                if ( variantAttr == null )
                    continue;

                object v = prop.GetValue( request );

                if ( v == null )
                    continue;

                string sv = v.ToString();

                variantTuples.Add( new Tuple<int, string>( variantAttr.Position, sv ) );
            }

            if ( variantTuples.Count == 1 )
            {
                ctx.RuleVariant = variantTuples[ 0 ].Item2;
            }
            else if ( variantTuples.Count > 0 )
            {
                ctx.RuleVariant = string.Join( "-", variantTuples.OrderBy( x => x.Item1 )
                                                                 .Select( x => x.Item2 )
                                                                 .ToArray() );
            }


            /*
             *
             */
            IContentLoader loader = Platinum.Activator.Create<IContentLoader>( RuntimeConfiguration.Current.ContentLoader.LoaderMoniker );
            loader.Initialize( RuntimeConfiguration.Current.ContentLoader.ToNvcSettings() );

            RuleContent content;

            try
            {
                content = loader.Load( ctx );
            }
            catch ( ActorException ex )
            {
                throw new RuleRuntimeException( ER.ContentFail, ex, rule.Name, rea.Name, loader.GetType().FullName );
            }

            if ( content == null )
                throw new RuleRuntimeException( ER.ContentNull, rule.Name, rea.Name, loader.GetType().FullName );

            ctx.Content = content;


            /*
             * 
             */
            object response;

            try
            {
                response = engine.Run( ctx, request );
            }
            catch ( ActorException )
            {
                throw;
            }
            catch ( Exception ex )
            {
                throw new RuleRuntimeException( ER.EngineFail, ex, rule.Name, rea.Name );
            }

            if ( response == null )
                throw new RuleRuntimeException( ER.ResponseNull, rule.Name, rea.Name );

            if ( response.GetType() != rule.ResponseType )
                throw new RuleRuntimeException( ER.ResponseWrongType, rule.Name, rule.ResponseType.FullName, response.GetType().FullName );

            return response;
        }
    }
}

using System;
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

            Type type = Type.GetType( ruleName + ",Hf.Rules" );

            IRule rule = Platinum.Activator.Create<IRule>( type );

            return rule;
        }



        public object Run( string ruleName, object request )
        {
            IRule r = Get( ruleName );
            return Run( r, request );
        }


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
            RuleEngineAttribute rea = rule.GetType().GetCustomAttribute<RuleEngineAttribute>();
            IRuleEngine engine = EngineFactory.For( rea.Name );

            if ( engine == null )
                throw new NotSupportedException( rea.Name );


            /*
             * 
             */
            object response = engine.Run( rule, request );

            return response;
        }
    }
}

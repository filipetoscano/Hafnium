using System;

namespace Hafnium.Runtime
{
    public class RuleCatalogue
    {
        public static IRule Get( string ruleName )
        {
            #region Validations

            if ( ruleName == null )
                throw new ArgumentNullException( nameof( ruleName ) );

            #endregion

            Type type = Type.GetType( ruleName + ",Hf.Rules" );

            IRule rule = Platinum.Activator.Create<IRule>( type );

            return rule;
        }
    }
}

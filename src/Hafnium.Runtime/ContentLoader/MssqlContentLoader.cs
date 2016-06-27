using System;
using System.Collections.Specialized;

namespace Hafnium.Runtime.ContentLoader
{
    public class MssqlContentLoader : IContentLoader
    {
        private NameValueCollection Settings
        {
            get;
            set;
        }


        public void Initialize( NameValueCollection settings )
        {
            this.Settings = new NameValueCollection();

            if ( settings != null )
            {
                foreach ( string k in settings )
                {
                    this.Settings.Set( k, settings.Get( k ) );
                }
            }
        }


        public RuleContent Load( RuleContext context )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( context.Engine == null )
                throw new ArgumentNullException( nameof( context ) + ".Engine" );

            if ( context.Rule == null )
                throw new ArgumentNullException( nameof( context ) + ".Rule" );

            #endregion

            throw new NotImplementedException();
        }
    }
}

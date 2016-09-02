using Dapper;
using Platinum.Configuration;
using Platinum.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hafnium.Runtime.ContentLoader
{
    public class MssqlContentLoader : IContentLoader
    {
        private string _connectionName;


        public void Initialize( Dictionary<string,string> settings )
        {
            #region Validations

            if ( settings == null )
                throw new ArgumentNullException( nameof( settings ) );

            #endregion


            /*
             * ConnectionName
             */
            _connectionName = settings[ "ConnectionName" ];

            if ( string.IsNullOrEmpty( _connectionName ) == true )
                throw new RuleRuntimeConfigurationException( ER.MssqlContentLoader_ConnectionName_Missing );

            if ( AppConfiguration.ConnectionStrings[ _connectionName ] == null )
                throw new RuleRuntimeConfigurationException( ER.MssqlContentLoader_ConnectionName_NotDefined, _connectionName );
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

            DataConnection db = Db.Connection( _connectionName );
            RuleContent rc;

            if ( context.RuleVariant == null )
            {
                rc = db.Query<RuleContent>( Db.Command( "ContentLoader/MssqlLoadRule" ), new
                {
                    RuleName = context.Rule.Name,
                } ).FirstOrDefault();
            }
            else
            {
                rc = db.Query<RuleContent>( Db.Command( "ContentLoader/MssqlLoadVariant" ), new
                {
                    RuleName = context.Rule.Name,
                    RuleVariant = context.RuleVariant
                } ).FirstOrDefault();
            }

            return rc;
        }
    }
}

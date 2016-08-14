using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace Hafnium.Runtime.ContentLoader
{
    public class FilesystemContentLoader : IContentLoader
    {
        private NameValueCollection Settings
        {
            get;
            set;
        }


        public void Initialize( NameValueCollection settings )
        {
            this.Settings = new NameValueCollection();
            this.Settings[ "base" ] = "~/../Rules";

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


            /*
             * 
             */
            string extension = ".txt";

            RuleEngineContentAttribute eca = context.Engine.GetType().GetCustomAttribute<RuleEngineContentAttribute>();

            if ( eca != null && eca.Extension != null )
                extension = eca.Extension;


            /*
             * 
             */
            string baseDir = this.Settings[ "base" ];

            if ( baseDir.StartsWith( "~/", StringComparison.Ordinal ) == true )
            {
                string root = AppDomain.CurrentDomain.BaseDirectory;

                baseDir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    baseDir.Substring( 2 ) );
            }


            /*
             * 
             */
            string fileName;

            if ( context.RuleVariant == null )
                fileName = context.Rule.Name + extension;
            else
                fileName = context.Rule.Name + "-" + context.RuleVariant + extension;


            /*
             * 
             */
            string path = Path.Combine( baseDir, fileName );

            if ( File.Exists( path ) == false )
                return null;


            /*
             * 
             */
            RuleContent rc = new RuleContent();
            rc.Bytes = File.ReadAllBytes( path );
            rc.LastModified = File.GetLastWriteTime( path );
            rc.Version = "TODO:md5sum";

            return rc;
        }
    }
}

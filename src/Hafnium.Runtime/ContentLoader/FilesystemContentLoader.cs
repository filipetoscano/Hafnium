using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace Hafnium.Runtime.ContentLoader
{
    public class FilesystemContentLoader : IContentLoader
    {
        private string _baseDir;


        public void Initialize( NameValueCollection settings )
        {
            #region Validations

            if ( settings == null )
                throw new ArgumentNullException( nameof( settings ) );

            #endregion


            /*
             * RuleDirectory
             */
            string baseDir = settings[ "RuleDirectory" ];

            if ( string.IsNullOrEmpty( baseDir ) == true )
                throw new RuleRuntimeConfigurationException( ER.FilesystemContentLoader_RuleDirectory_Missing ); 

            if ( baseDir.StartsWith( "~/", StringComparison.Ordinal ) == true )
            {
                string root = AppDomain.CurrentDomain.BaseDirectory;

                baseDir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    baseDir.Substring( 2 ) );
            }

            if ( Directory.Exists( baseDir ) == false )
                throw new RuleRuntimeConfigurationException( ER.FilesystemContentLoader_RuleDirectory_NotExists, baseDir );

            _baseDir = baseDir;
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
            string fileName;

            if ( context.RuleVariant == null )
                fileName = context.Rule.Name + extension;
            else
                fileName = context.Rule.Name + "-" + context.RuleVariant + extension;


            /*
             * 
             */
            string path = Path.Combine( _baseDir, fileName );

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hafnium.Runtime
{
    public static class AppDomainExtensions
    {
        public static void PreLoadDeployedAssemblies( this AppDomain appDomain )
        {
            foreach ( string path in GetBinFolders() )
            {
                foreach ( FileInfo fi in new DirectoryInfo( path ).GetFiles( "*.dll", SearchOption.TopDirectoryOnly ) )
                {
                    var s = fi.FullName;
                    var a = AssemblyName.GetAssemblyName( s );

                    if ( appDomain.GetAssemblies().Any( assembly => AssemblyName.ReferenceMatchesDefinition( a, assembly.GetName() ) ) == false )
                    {
                        Assembly.Load( a );
                    }
                }
            }
        }


        private static IEnumerable<string> GetBinFolders()
        {
            List<string> paths = new List<string>();


            /*
             * Main directory
             */
            if ( HasHttpContext() == true )
            {
                var binFolder = HttpRuntimeBin();
                paths.Add( binFolder );
            }
            else
            {
                paths.Add( AppDomain.CurrentDomain.BaseDirectory );
            }


            /*
             * TODO: PrivateBinPath
             */

            return paths;
        }


        private static bool HasHttpContext()
        {
            Type type = Type.GetType( "System.Web.HttpContext, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true, true );
            PropertyInfo prop = type.GetProperty( "Current", BindingFlags.Public | BindingFlags.Static );

            object context = prop.GetValue( null, null );

            return context != null;
        }


        private static string HttpRuntimeBin()
        {
            Type type = Type.GetType( "System.Web.HttpRuntime, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true, true );
            PropertyInfo prop = type.GetProperty( "BinDirectory", BindingFlags.Public | BindingFlags.Static );

            string binDirectory = (string) prop.GetValue( null, null );

            return binDirectory;
        }
    }
}

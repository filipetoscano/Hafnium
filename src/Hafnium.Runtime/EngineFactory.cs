using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hafnium.Runtime
{
    public static class EngineFactory
    {
        public static IRuleEngine For( string name )
        {
            IRuleEngine re = null;

            if ( name == "Excel" )
                re = Platinum.Activator.Create<IRuleEngine>( "Hafnium.Engine.Excel.ExcelEngine,Hafnium.Engine.Excel" );

            if ( name == "Javascript" )
                re = Platinum.Activator.Create<IRuleEngine>( "Hafnium.Engine.Javascript.JavascriptEngine,Hafnium.Engine.Javascript" );

            if ( name == "CSharp" )
                re = Platinum.Activator.Create<IRuleEngine>( "Hafnium.Engine.CSharp.CSharpEngine,Hafnium.Engine.CSharp" );

            if ( name == "Python" )
                re = Platinum.Activator.Create<IRuleEngine>( "Hafnium.Engine.Python.PythonEngine,Hafnium.Engine.Python" );

            return re;
        }
    }
}

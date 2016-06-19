using System;
using System.Reflection;

namespace Hafnium.Engine.Excel
{
    [RuleEngine( "Excel" )]
    public class ExcelEngine : IRuleEngine
    {
        public object Run( IRule rule, object request )
        {
            /*
             * 
             */
            object response = Activator.CreateInstance( rule.ResponseType );

            using ( var xls = new Spreadsheet() )
            {
                xls.Load( @"C:\Work\Transition\Hafnium\sample\RuleContent\Hf.Rules.Rule1.xlsx" );
                xls.AutoCalculate = false;


                /*
                 * Input
                 */
                foreach ( PropertyInfo prop in request.GetType().GetProperties() )
                {
                    MapAttribute map = prop.GetCustomAttribute<MapAttribute>();

                    if ( map == null )
                        continue;

                    if ( map.Expression == null )
                        continue;

                    object value = prop.GetValue( request );
                    xls.SetValue( map.Expression, value );
                }


                /*
                 * 
                 */
                xls.Calculate();


                /*
                 * Output
                 */
                foreach ( PropertyInfo prop in rule.ResponseType.GetProperties() )
                {
                    MapAttribute map = prop.GetCustomAttribute<MapAttribute>();

                    if ( map == null )
                        continue;

                    if ( map.Expression == null )
                        continue;

                    object value = xls.GetValue( prop.PropertyType, map.Expression );
                    prop.SetValue( response, value );
                }
            }

            return response;
        }
    }
}

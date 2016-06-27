using System;
using System.IO;
using System.Reflection;

namespace Hafnium.Engine.Excel
{
    [RuleEngine( "Excel" )]
    [RuleEngineContent( Extension = ".xlsx", MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" )]
    public class ExcelEngine : IRuleEngine
    {
        public object Run( RuleContext context, object request )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( request == null )
                throw new ArgumentNullException( nameof( request ) );

            #endregion


            /*
             * 
             */
            MemoryStream ms = new MemoryStream();
            ms.Write( context.Content, 0, context.Content.Length );


            /*
             * 
             */
            IRule rule = context.Rule;
            object response = Activator.CreateInstance( rule.ResponseType );

            using ( var xls = new Spreadsheet() )
            {
                /*
                 * One would think that .Load would throw an error if the file isn't a
                 * valid Excel file -- but it seems to be apparently very resilient! :P
                 */
                xls.Load( ms );
                xls.AutoCalculate = false;


                /*
                 * Input
                 */
                foreach ( PropertyInfo prop in request.GetType().GetProperties() )
                {
                    MapAttribute map = prop.GetCustomAttribute<MapAttribute>();

                    if ( map == null )
                        throw new ExcelEngineException( ER.InputMapMissing, rule.Name, prop.Name );

                    if ( map.Expression == null )
                        throw new ExcelEngineException( ER.InputExpressionNull, rule.Name, prop.Name );

                    object value = prop.GetValue( request );

                    if ( xls.HasCell( map.Expression ) == false )
                        throw new ExcelEngineException( ER.InputCellMissing, rule.Name, prop.Name, map.Expression );

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
                        throw new ExcelEngineException( ER.OutputMapMissing, rule.Name, prop.Name );

                    if ( map.Expression == null )
                        throw new ExcelEngineException( ER.OutputExpressionNull, rule.Name, prop.Name );

                    if ( xls.HasCell( map.Expression ) == false )
                        throw new ExcelEngineException( ER.OutputCellMissing, rule.Name, prop.Name, map.Expression );

                    object value = xls.GetValue( prop.PropertyType, map.Expression );
                    prop.SetValue( response, value );
                }
            }

            return response;
        }
    }
}

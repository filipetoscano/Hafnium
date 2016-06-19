﻿using System;
using System.Reflection;

namespace Hafnium.Engine.Excel
{
    [RuleEngine( "Excel" )]
    public class ExcelEngine : IRuleEngine
    {
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
            string ruleName = rule.GetType().FullName;
            object response = Activator.CreateInstance( rule.ResponseType );

            using ( var xls = new Spreadsheet() )
            {
                /*
                 * One would think that .Load would throw an error if the file isn't a
                 * valid Excel file -- but it seems to be apparently very resilient! :P
                 */
                xls.Load( @"C:\Work\Transition\Hafnium\sample\RuleContent\Hf.Rules.Rule1.xlsx" );
                xls.AutoCalculate = false;


                /*
                 * Input
                 */
                foreach ( PropertyInfo prop in request.GetType().GetProperties() )
                {
                    MapAttribute map = prop.GetCustomAttribute<MapAttribute>();

                    if ( map == null )
                        throw new ExcelEngineException( ER.InputMapMissing, ruleName, prop.Name );

                    if ( map.Expression == null )
                        throw new ExcelEngineException( ER.InputExpressionNull, ruleName, prop.Name );

                    object value = prop.GetValue( request );

                    if ( xls.HasCell( map.Expression ) == false )
                        throw new ExcelEngineException( ER.InputCellMissing, ruleName, prop.Name, map.Expression );

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
                        throw new ExcelEngineException( ER.OutputMapMissing, ruleName, prop.Name );

                    if ( map.Expression == null )
                        throw new ExcelEngineException( ER.OutputExpressionNull, ruleName, prop.Name );

                    if ( xls.HasCell( map.Expression ) == false )
                        throw new ExcelEngineException( ER.OutputCellMissing, ruleName, prop.Name, map.Expression );

                    object value = xls.GetValue( prop.PropertyType, map.Expression );
                    prop.SetValue( response, value );
                }
            }

            return response;
        }
    }
}

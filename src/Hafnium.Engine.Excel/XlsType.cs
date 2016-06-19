using System;

namespace Hafnium.Engine.Excel
{
    /// <summary>
    /// Helper class, converting XLS values to/from CLR values.
    /// </summary>
    public static class XlsType
    {
        public static object FromValue( object clrValue )
        {
            if ( clrValue is DateTime )
            {
                DateTime dt = (DateTime) clrValue;
                return dt.ToOADate();
            }

            return clrValue;
        }


        public static object ToValue( Type propertyType, object excelValue )
        {
            if ( propertyType == typeof( int ) )
            {
                double d = (double) excelValue;
                return Convert.ToInt32( d );
            }

            if ( propertyType == typeof( Decimal ) )
            {
                double d = (double) excelValue;
                return Convert.ToDecimal( d );
            }

            if ( propertyType == typeof( DateTime ) )
            {
                double d = (double) excelValue;
                return DateTime.FromOADate( d );
            }

            return excelValue;
        }
    }
}

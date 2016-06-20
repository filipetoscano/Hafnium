using Jurassic;
using System;

namespace Hafnium.Engine.Javascript
{
    /// <summary>
    /// Helper class, converting Javascript values to/from CLR values.
    /// </summary>
    /// <remarks>
    /// Not all mappings will be possible or even exact, given the type system
    /// in Javascript.
    /// </remarks>
    public static class JsType
    {
        public static object ToValue( Type propertyType, object value )
        {
            if ( propertyType == typeof( bool ) )
                return ToBoolean( value );

            if ( propertyType == typeof( int ) )
                return ToInt32( value );

            if ( propertyType == typeof( decimal ) )
                return ToDecimal( value );

            if ( propertyType == typeof( string ) )
                return ToString( value );

            throw new NotSupportedException( "Unsupported property type:" + propertyType.FullName );
        }


        private static bool ToBoolean( object value )
        {
            if ( value == null )
                throw new ArgumentNullException( "value" );

            return (bool) value;
        }


        private static int ToInt32( object value )
        {
            if ( value == null )
                throw new ArgumentNullException( "value" );

            return Convert.ToInt32( value );
        }


        private static decimal ToDecimal( object value )
        {
            if ( value == null )
                throw new ArgumentNullException( "value" );

            if ( value is double )
            {
                double d = (double) value;
                return new Decimal( d );
            }

            throw new NotSupportedException( "Cannot convert." );
        }


        private static string ToString( object value )
        {
            if ( value == null )
                throw new ArgumentNullException( "value" );

            if ( value is ConcatenatedString )
            {
                ConcatenatedString cs = (ConcatenatedString) value;
                return cs.ToString();
            }

            return value.ToString();
        }
    }
}

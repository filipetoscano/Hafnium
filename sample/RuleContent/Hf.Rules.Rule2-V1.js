'use strict';

var row = lookup({ KeyInt: request.Integer });

var bool = false;
var int = row.Value1 + row.Value2;
var dec = 5.55;

if ( request.Integer > 5 ) {
    bool = true;
}

if ( request.Integer > 10 && request.Decimal > 10 ) {
    int = -int;
    dec = -dec;
}

var response = {};
response.Boolean = bool;
response.Integer = int;
response.Decimal = dec;
response.String = 'from javascript [V1], orig=' + request.String;

/* eof */
'use strict';

var bool = false;
var int = 5;
var dec = 5.55;

if ( request.Integer > 0 ) {
    bool = true;
}

if ( request.Integer > 10 && request.Decimal > 10 ) {
    int = -30;
    dec = -30;
}

var response = {};
response.Boolean = bool;
response.Integer = int;
response.Decimal = dec;
response.String = 'from javascript [V2], orig=' + request.String;

/* eof */
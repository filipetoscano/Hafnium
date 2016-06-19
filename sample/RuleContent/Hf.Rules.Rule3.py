import clr
clr.AddReference( 'Hf.Rules' )
from Hf.Rules import Rule3Response

# Logic


# Response
response = Rule3Response();
response.Boolean = True
response.Integer = 10
response.Decimal = 12.55
response.String = "from python, orig=" + request.String

# eof
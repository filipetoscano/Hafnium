using System;
using System.Reflection;

namespace Hafnium.Runtime
{
    public class RuleRunner
    {
        public Rule Get( string name )
        {
            Type rq = Type.GetType( "Hf.Rules.Rule1Request" );
            Type rp = Type.GetType( "Hf.Rules.Rule1Request" );

            Rule r = new Rule();
            r.Name = "Hf.Rules.Rule1";
            r.RequestType = rq;
            r.ResponseType = rp;

            return r;
        }


        public object Run( string name, object request )
        {
            Rule r = Get( name );

            MethodInfo template = this.GetType().GetMethod( "Execute" );
            MethodInfo execute = template.MakeGenericMethod( r.RequestType, r.ResponseType );

            object response = execute.Invoke( this, new object[] { request } );

            return response;
        }
    }
}

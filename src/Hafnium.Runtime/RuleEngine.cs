namespace Hafnium.Runtime
{
    public abstract class RuleEngine
    {
        protected Rp Execute<Rq, Rp>( string name, Rq request )
            where Rp : class
        {
            return Platinum.Activator.Create<Rp>( typeof( Rp ) );
        }
    }
}

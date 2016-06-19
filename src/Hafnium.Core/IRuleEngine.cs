namespace Hafnium
{
    public interface IRuleEngine
    {
        object Run( IRule rule, object request );
    }
}

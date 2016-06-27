namespace Hafnium
{
    public interface IRuleEngine
    {
        object Run( RuleContext context, object request );
    }
}

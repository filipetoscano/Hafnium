namespace Hafnium
{
    /// <summary>
    /// Describes a rule engine: a component which will run the
    /// dynamically loaded rule (content) against the values in
    /// the request structure.
    /// </summary>
    public interface IRuleEngine
    {
        object Run( RuleContext context, object request );
    }
}

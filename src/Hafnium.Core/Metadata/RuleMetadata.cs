namespace Hafnium
{
    public class RuleMetadata
    {
        /// <summary>
        /// Gets the (public) name of the rule.
        /// </summary>
        public string Name
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the description of the rule.
        /// </summary>
        public string Description
        {
            get;
            set;
        }


        /// <summary>
        /// Gets list of (technical) authors.
        /// </summary>
        public User[] Authors
        {
            get;
            set;
        }


        /// <summary>
        /// Gets list of (business) owners.
        /// </summary>
        public User[] Owners
        {
            get;
            set;
        }
    }
}

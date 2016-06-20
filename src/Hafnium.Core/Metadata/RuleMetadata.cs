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
        /// Gets list of authors.
        /// </summary>
        public Author[] Authors
        {
            get;
            set;
        }
    }
}

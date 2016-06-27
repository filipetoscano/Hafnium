namespace Hafnium
{
    /// <summary>
    /// Description of a user, in case they need to be contacted.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets the organization specific Id of the user.
        /// </summary>
        /// <remarks>
        /// Could be the Windows account name?
        /// </remarks>
        public string Id
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the name of the person.
        /// </summary>
        public string Name
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the e-mail address of the person.
        /// </summary>
        public string Email
        {
            get;
            set;
        }
    }
}

using Platinum.Resolver;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Hafnium
{
    /// <summary>
    /// Base class for rules where we load the metadata from the same XML
    /// where the API is defined.
    /// </summary>
    public abstract class ResxBaseRule : BaseRule, IRule
    {
        private RuleMetadata _metadata;


        /// <summary>
        /// Initializes a new instance of <see cref="ResxBaseRule" />.
        /// </summary>
        public ResxBaseRule()
            : base()
        {
        }


        /// <summary>
        /// Gets documentation pertinent to the present rule.
        /// </summary>
        public RuleMetadata Metadata
        {
            get
            {
                if ( _metadata == null )
                {
                    lock ( this )
                    {
                        if ( _metadata == null )
                        {
                            _metadata = MetadataLoad();
                        }
                    }
                }

                return _metadata;
            }
        }


        /// <summary>
        /// Load the metadata from the embedded XML resource file.
        /// </summary>
        /// <returns>Metadata for the current rule.</returns>
        private RuleMetadata MetadataLoad()
        {
            /*
             * 
             */
            string uri = string.Format( CultureInfo.InvariantCulture, "assembly:///{0}/{1}.xml",
                this.GetType().Assembly.FullName.Split( ',' ).First(),
                this.GetType().FullName.Replace( ".", "/" ) );


            /*
             * Load
             */
            UrlResolver r = new UrlResolver( false );

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.XmlResolver = r;

            XDocument doc;

            using ( XmlReader xr = XmlReader.Create( uri, xrs ) )
            {
                doc = XDocument.Load( xr );
            }


            /*
             * 
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "hf", "urn:hafnium" );


            /*
             * 
             */
            RuleMetadata md = new RuleMetadata();


            /*
             * 
             */
            XElement nameElem = doc.XPathSelectElement( " /hf:rule/hf:name ", manager );

            if ( nameElem != null )
                md.Name = nameElem.Value;


            /*
             * 
             */
            XElement descElem = doc.XPathSelectElement( " /hf:rule/hf:description ", manager );

            if ( descElem != null )
                md.Description = descElem.Value;


            /*
             * 
             */
            List<User> authors = new List<User>();

            foreach ( XElement authorElem in (IEnumerable) doc.XPathEvaluate( " /hf:rule/hf:author ", manager ) )
            {
                User u = new User();
                u.Id = authorElem.Attribute( "id" )?.Value;
                u.Name = authorElem.Attribute( "name" )?.Value;
                u.Email = authorElem.Attribute( "email" )?.Value;

                authors.Add( u );
            }

            md.Authors = authors.ToArray();


            /*
             * 
             */
            List<User> owners = new List<User>();

            foreach ( XElement ownerElem in (IEnumerable) doc.XPathEvaluate( " /hf:rule/hf:owner ", manager ) )
            {
                User u = new User();
                u.Id = ownerElem.Attribute( "id" )?.Value;
                u.Name = ownerElem.Attribute( "name" )?.Value;
                u.Email = ownerElem.Attribute( "email" )?.Value;

                owners.Add( u );
            }

            md.Owners = owners.ToArray();


            return md;
        }
    }
}

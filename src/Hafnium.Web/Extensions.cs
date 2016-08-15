using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hafnium.WebServices
{
    internal static class Extensions
    {
        public static XElement SerializeAsXElement( this XmlSerializer xs, object o )
        {
            // XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            // ns.Add( "", "" );

            XDocument d = new XDocument();

            using ( XmlWriter w = d.CreateWriter() )
            { 
                xs.Serialize( w, o );
            }

            XElement e = d.Root;
            e.Remove();

            return e;
        }
    }
}

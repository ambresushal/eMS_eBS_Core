
using System.IO;
using System.Xml.Serialization;

namespace tmg.equinox.identitymanagement.Models
{
    public class UserProfileInfo
    {
        public string DisplayName { get; set; }
        public string ClaimsIdentifier { get; set; }
        public int UserId { get; set; }

        public override string ToString()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserProfileInfo));
            using (var stream = new StringWriter())
            {
                serializer.Serialize(stream, this);
                return stream.ToString();
            }
        }

        public static UserProfileInfo FromString(string userContextData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserProfileInfo));
            using (var stream = new StringReader(userContextData))
            {
                return serializer.Deserialize(stream) as UserProfileInfo;
            }
        }
    }
}
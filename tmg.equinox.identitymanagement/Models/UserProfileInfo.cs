
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;

namespace tmg.equinox.identitymanagement.Models
{
    public class UserProfileInfo
    {
        public string DisplayName { get; set; }
        /*public string ClaimsIdentifier { get; set; }*/
        public int Id { get; set; }
        public int TenantID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

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
            /*TODO: Remove commented code and check if it is working instead of Json Deserializer as written below.*/
            //XmlSerializer serializer = new XmlSerializer(typeof(UserProfileInfo));
            //using (var stream = new StringReader(userContextData))
            //{
            //    return serializer.Deserialize(stream) as UserProfileInfo;
            //}
            return JsonConvert.DeserializeObject<UserProfileInfo>(userContextData);
        }
    }
}
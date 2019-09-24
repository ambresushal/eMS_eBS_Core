using System;
using System.IO;
using System.Web;
using System.Xml.Linq;
using System.Web;
using tmg.equinox.infrastructure.exceptionhandling;
namespace tmg.equinox.infrastructure.util
{
    public class XmlDocumentManager
    {

        #region Public Properties
        private string xmlfileName = "App_Data/Claims_";
        private string fileExt = ".xml";
        #endregion

        public XDocument SetXmlDocumentClaims(ClaimsSchema claims, string userId, out string xmlFilepathName)
        {
            XDocument xdocument= null;
            xmlFilepathName = string.Empty;
            try
            {
                xmlfileName = xmlfileName + userId + fileExt;
                 xmlFilepathName = HttpContext.Current.Server.MapPath(xmlfileName);               
                claims.Serialize();
                claims.SaveToFile(xmlFilepathName);
                xdocument = XDocument.Load(xmlFilepathName);                
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return xdocument;
        }

        /// <summary>
        /// Delete Xml claims file if exists.
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteClaimsXmlFile(string userId)
        {
            try
            {
                string xmlFilepathName = string.Empty;
                xmlfileName = xmlfileName + userId + fileExt;
                xmlFilepathName = HttpContext.Current.Server.MapPath("..\\" + xmlfileName);
                if (File.Exists(xmlFilepathName))
                {
                    File.Delete(xmlFilepathName);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
        }

    }
}

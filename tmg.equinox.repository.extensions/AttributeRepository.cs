using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class AttributeRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool IsAttributeExists(this IRepositoryAsync<domain.entities.Models.Attribute> attributeRepository, string name, string nameCamelCase, string attributeType, string cardinality, int? length, int? precision, string regex, string formatter, bool? synthetic, string defaultValue,int versionID)
        {
            return attributeRepository
                             .Query()
                             .Include(inc => inc.ObjectVersionAttribXrefs)
                             .Filter(c => c.Name == name && c.NameCamelcase == nameCamelCase && 
                                    c.AttrType == attributeType && c.Cardinality == cardinality && 
                                    c.Length == length && c.Precision == precision && c.EditRegex == regex && 
                                    c.Formatter == formatter && c.Synthetic == synthetic && c.DefaultValue == defaultValue &&
                                    c.ObjectVersionAttribXrefs.FirstOrDefault().VersionID == versionID)
                             .Get()
                             .Any();
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class MasterLists
    {
        int tenantId;
        IUnitOfWorkAsync _unitOfWork;
        int _formDesignVersionId;
        private List<LayoutType> _layoutList;
        private List<UIElementType> _uiElementTypeList;
        private List<ApplicationDataType> _uiElementDataTypeList;
        private List<RegexLibrary> _regexLibraryList;
        private List<FormDesignUIElementMap> _elementMap;
        private List<FormDesignIgnoreUIElementMap> _ignoreUIElementList;
        private List<AlternateUIElementLabel> _alternateLabels;
        internal MasterLists(int tenantId, IUnitOfWorkAsync unitOfWork, int formDesignVersionId)
        {
            this.tenantId = tenantId;
            this._unitOfWork = unitOfWork;
            this._formDesignVersionId = formDesignVersionId;
        }

        internal List<LayoutType> LayoutTypes
        {
            get
            {
                if (_layoutList == null)
                {
                    _layoutList = this._unitOfWork.RepositoryAsync<LayoutType>().Get().ToList();
                }
                return _layoutList;
            }
        }

        internal List<UIElementType> ElementTypes
        {
            get
            {
                if (_uiElementTypeList == null)
                {
                    _uiElementTypeList = this._unitOfWork.RepositoryAsync<UIElementType>().Get().ToList();

                }
                return _uiElementTypeList;
            }
        }

        internal List<ApplicationDataType> ElementDataTypes
        {
            get
            {
                if (_uiElementDataTypeList == null)
                {
                    _uiElementDataTypeList = this._unitOfWork.RepositoryAsync<ApplicationDataType>().Get().ToList();
                }
                return _uiElementDataTypeList;
            }
        }
        internal List<RegexLibrary> LibraryRegexes
        {
            get
            {
                if (_regexLibraryList == null)
                {
                    _regexLibraryList = this._unitOfWork.RepositoryAsync<RegexLibrary>().Get().ToList();
                }
                return _regexLibraryList;
            }
        }

        internal List<FormDesignUIElementMap> ElementMap
        {
            get
            {
                if (_elementMap == null)
                {
                    _elementMap = (from map in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                   where map.FormDesignVersionID == _formDesignVersionId
                                   select new FormDesignUIElementMap()
                                   {
                                       UIElementID = map.UIElementID,
                                       EffectiveDate = map.EffectiveDate
                                   }).ToList();
                }
                return _elementMap;
            }
        }

        internal List<FormDesignIgnoreUIElementMap> IgnoreElementList
        {
            get
            {
                if (_ignoreUIElementList == null)
                {
                    _ignoreUIElementList = (from map in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                            where map.FormDesignVersionID == _formDesignVersionId && map.Operation == "I"
                                            select new FormDesignIgnoreUIElementMap()
                                            {
                                                UIElementID = map.UIElementID,
                                                Operation = map.Operation
                                            }).ToList();
                }
                return _ignoreUIElementList;
            }
        }
      
        internal List<AlternateUIElementLabel> AlternateLabelList
        {
            get
            {
                if (_alternateLabels == null)
                {
                    _alternateLabels = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Get().Where(s => s.FormDesignVersionID == _formDesignVersionId).ToList();
                }
                return _alternateLabels;
            }
        }
    }
}

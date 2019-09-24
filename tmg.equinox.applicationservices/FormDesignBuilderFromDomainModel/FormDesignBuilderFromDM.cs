using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignBuilderFromDomainModel
{
    public class FormDesignBuilderFromDM
    {
        #region Private Variables

        private int _formDesignVersionId;
        private int _tenantId;
        private IUnitOfWorkAsync _unitOfWork;

        #endregion

        #region Constructor

        public FormDesignBuilderFromDM(int tenantId, int formDesignVersionId, IUnitOfWorkAsync unitOfWork)
        {
            this._formDesignVersionId = formDesignVersionId;
            this._tenantId = tenantId;
            this._unitOfWork = unitOfWork;
        }

        #endregion

        #region Public Methods

        public FormDesignVersionDetailFromDM BuildFormDesignFromDataModel()
        {
            FormDesignVersionDetailFromDM detail = null;

            FormDesignVersion formVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                       .Query()
                                                                       .Filter(c => c.FormDesignVersionID == this._formDesignVersionId)
                                                                       .Include(c => c.FormDesign)
                                                                       .Get()
                                                                       .SingleOrDefault();
            if (formVersion != null)
            {
                detail = new FormDesignVersionDetailFromDM();
                detail.FormDesignId = formVersion.FormDesignID.Value;
                detail.FormDesignVersionId = _formDesignVersionId;
                detail.TenantID = _tenantId;
                detail.FormVersion = formVersion.VersionNumber;

                var formVersionObjectVersionMap = _unitOfWork.RepositoryAsync<FormVersionObjectVersionMap>()
                                                    .Query()
                                                    .Filter(fil =>
                                                                fil.FormDesignVersionID == _formDesignVersionId)
                                                    .Get()
                                                    .SingleOrDefault();

                var objectTrees = _unitOfWork.RepositoryAsync<ObjectTree>()
                                                .Query()
                                                .Include(include =>
                                                    include.ObjectVersion.ObjectVersionAttribXrefs)
                                                .Filter(fil => fil.VersionID == formVersionObjectVersionMap.ObjectVersionID)
                                                .Get()
                                                .ToList();

                var objectDefintions = _unitOfWork.RepositoryAsync<ObjectDefinition>()
                                                .Query()
                                                .Get()
                                                .ToList();

                var topLevelTabElement = objectTrees.SingleOrDefault(e => e.ParentOID == null);

                if (topLevelTabElement != null)
                {
                    detail.FormName = objectDefintions.Where(e => e.OID == topLevelTabElement.RootOID)
                                        .Select(sel => sel.ObjectName)
                                        .SingleOrDefault();

                    var topLevelSectionElements = objectTrees.Where(e => e.ParentOID == e.RootOID).ToList();

                    detail.Sections = new List<SectionDesignFromDM>();

                    foreach (var section in topLevelSectionElements)
                    {
                        var builder = new SectionDesignBuilderFromDM(section, objectDefintions, objectTrees, _unitOfWork);
                        SectionDesignFromDM design = builder.BuildSectionFromDomainModel(detail.FormName);
                        detail.Sections.Add(design);
                    }
                }
            }
            return detail;
        }

        #endregion
    }
}

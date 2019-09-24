using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignBuilderFromDomainModel
{
    public class SectionDesignBuilderFromDM
    {
        #region Private Variables

        IUnitOfWorkAsync _unitOfWork;
        ObjectTree _topLevelSectionElement;
        private List<ObjectDefinition> _objectDefintions;
        private List<ObjectTree> _objectTrees;
        private List<ObjectRelation> ObjectRelations { get; set; }

        #endregion

        #region Constructor

        public SectionDesignBuilderFromDM(ObjectTree topLevelSectionElement, List<ObjectDefinition> objectDefintions, List<ObjectTree> objectTrees, IUnitOfWorkAsync unitOfWork)
        {
            _topLevelSectionElement = topLevelSectionElement;
            _objectDefintions = objectDefintions;
            _objectTrees = objectTrees;
            this._unitOfWork = unitOfWork;
        }

        #endregion

        #region Public Methods

        public SectionDesignFromDM BuildSectionFromDomainModel(string parentName)
        {
            SectionDesignFromDM sectionDesignElement = null;

           ObjectRelations = _unitOfWork.RepositoryAsync<ObjectRelation>()
                                           .Query()
                                           .Get().ToList();



           var objectRelation = ObjectRelations.SingleOrDefault(fil => fil.RelationID == _topLevelSectionElement.RelationID);

            if (_topLevelSectionElement != null && objectRelation != null)
            {
                sectionDesignElement = new SectionDesignFromDM();

                sectionDesignElement.Elements = new List<ElementDesignFromDM>();
               
                var objectDefintion = _objectDefintions.SingleOrDefault(e => e.OID == objectRelation.RelatedObjectID);

                if (objectDefintion != null)
                {
                    sectionDesignElement.GeneratedName = objectDefintion.ObjectName;

                    sectionDesignElement.FullName = parentName + "." + sectionDesignElement.GeneratedName;

                    sectionDesignElement.ID = objectDefintion.OID;

                    GetChildElementDesignsFromDomainModel(sectionDesignElement, "parent");
                }
            }
            return sectionDesignElement;
        }

        #endregion

        #region Private Methods

        private void GetChildElementDesignsFromDomainModel(SectionDesignFromDM sectionDesignElement, string parent)
        {
            var objectVersionAttribXrefs = _unitOfWork.RepositoryAsync<ObjectVersionAttribXref>()
                                               .Query()
                                               .Include(include => include.Attribute)
                                               .Get()
                                               .ToList();

            if (_objectTrees.Any(e => e.ParentOID == sectionDesignElement.ID))
            {
                foreach (var objectTree in _objectTrees.Where(e => e.ParentOID == sectionDesignElement.ID))
                {
                    var builder = new UIELementDesignBuilderFromDM(objectTree, _objectTrees, _objectDefintions,
                                                                   ObjectRelations, _unitOfWork);
                    sectionDesignElement.Elements.Add(builder.BuildElement(sectionDesignElement.GeneratedName));
                }
            }
            GetSectionElements(sectionDesignElement, objectVersionAttribXrefs);
        }

        private void GetSectionElements(SectionDesignFromDM sectionDesignElement, List<ObjectVersionAttribXref> objectVersionAttribXrefs)
        {
            if (objectVersionAttribXrefs.Any(e => e.OID == sectionDesignElement.ID))
            {
                foreach (var objectAttribute in objectVersionAttribXrefs.Where(e => e.OID == sectionDesignElement.ID))
                {
                    var builder = new UIELementDesignBuilderFromDM(objectAttribute, _objectTrees, _objectDefintions,
                                                                   ObjectRelations, _unitOfWork);
                    sectionDesignElement.Elements.Add(builder.BuildElementAttributes(sectionDesignElement.GeneratedName));
                }
            }
        }

        #endregion
    }
}

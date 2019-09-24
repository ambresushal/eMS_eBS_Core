using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignBuilderFromDomainModel
{
    public class RepeaterDesignBuilderFromDM
    {
        #region Private Variables

        IUnitOfWorkAsync _unitOfWork;
        ObjectTree _repeaterElement;
        private List<ObjectDefinition> _objectDefintions;
        private List<ObjectTree> _objectTrees;
        private List<ObjectRelation> _objectRelations;

        #endregion

        #region Constructor

        public RepeaterDesignBuilderFromDM(ObjectTree objectTree, List<ObjectDefinition> objectDefintions, 
                    List<ObjectTree> objectTrees,List<ObjectRelation> objectRelations, IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repeaterElement = objectTree;
            _objectDefintions = objectDefintions;
            _objectRelations = objectRelations;
            _objectTrees = objectTrees;
        }

        #endregion

        #region Public Methods

        public RepeaterDesignFromDM BuildRepeaterFromDomainModel(string fullParentName)
        {
            RepeaterDesignFromDM design = null;

            design = new RepeaterDesignFromDM();
            design.Elements = new List<ElementDesignFromDM>();
            var objectRelation = _objectRelations.SingleOrDefault(fil => fil.RelationID == _repeaterElement.RelationID);
            var objectDefintion = _objectDefintions.SingleOrDefault(e => objectRelation != null && e.OID == objectRelation.RelatedObjectID);
            if (objectDefintion != null)
            {
                design.ID = objectDefintion.OID;
                design.GeneratedName = objectDefintion.ObjectName;
                design.FullName = fullParentName + design.Name;
            }
            GetChildElementDesignsFromDomainModel(design, "parent");
            return design;
        }

        #endregion

        #region Private Methods

        private void GetChildElementDesignsFromDomainModel(RepeaterDesignFromDM sectionDesignElement, string parent)
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
                    var builder = new UIELementDesignBuilderFromDM(objectTree, _objectTrees, _objectDefintions, _objectRelations, _unitOfWork);
                    sectionDesignElement.Elements.Add(builder.BuildElement(sectionDesignElement.GeneratedName));
                }
            }
            GetRepeaterElements(sectionDesignElement, objectVersionAttribXrefs);
        }

        private void GetRepeaterElements(RepeaterDesignFromDM sectionDesignElement, List<ObjectVersionAttribXref> objectVersionAttribXrefs)
        {
            if (objectVersionAttribXrefs.Any(e => e.OID == sectionDesignElement.ID))
            {
                foreach (var objectAttribute in objectVersionAttribXrefs.Where(e => e.OID == sectionDesignElement.ID))
                {
                    var builder = new UIELementDesignBuilderFromDM(objectAttribute, _objectTrees, _objectDefintions,
                                                                   _objectRelations, _unitOfWork);
                    sectionDesignElement.Elements.Add(
                        builder.BuildElementAttributes(sectionDesignElement.GeneratedName));
                }
            }
        }

        #endregion
    }
}

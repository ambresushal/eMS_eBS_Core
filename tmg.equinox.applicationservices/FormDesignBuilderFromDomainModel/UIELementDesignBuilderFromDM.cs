using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignBuilderFromDomainModel
{
    public class UIELementDesignBuilderFromDM
    {
        #region Private Variables

        private ObjectTree _objectTree;
        private List<ObjectTree> _objectTrees;
        private List<ObjectDefinition> _objectDefintions;
        private List<ObjectRelation> _objectRelations;
        private IUnitOfWorkAsync _unitOfWork;
        private ObjectVersionAttribXref _objectAttribute;

        #endregion

        #region Constructor

        public UIELementDesignBuilderFromDM(ObjectTree objectTree, List<ObjectTree> _objectTrees,
            List<ObjectDefinition> _objectDefintions, List<ObjectRelation> objectRelations, IUnitOfWorkAsync _unitOfWork)
        {
            this._objectTree = objectTree;
            this._objectTrees = _objectTrees;
            this._objectDefintions = _objectDefintions;
            this._objectRelations = objectRelations;
            this._unitOfWork = _unitOfWork;
        }

        #endregion

        #region Public Methods

        public UIELementDesignBuilderFromDM(ObjectVersionAttribXref objectAttribute, List<ObjectTree> objectTrees, 
            List<ObjectDefinition> objectDefintions, List<ObjectRelation> objectRelations, IUnitOfWorkAsync unitOfWork)
        {
            this._objectAttribute = objectAttribute;
            this._objectTrees = objectTrees;
            this._objectDefintions = objectDefintions;
            this._objectRelations = objectRelations;
            this._unitOfWork = unitOfWork;
        }

        public ElementDesignFromDM BuildElement(string fullName)
        {
            ElementDesignFromDM design = new ElementDesignFromDM();

            BuildElementContainers(fullName, design);
         
            return design;
        }

        public ElementDesignFromDM BuildElementAttributes(string fullName)
        {
            ElementDesignFromDM design = new ElementDesignFromDM();

            var objectDefintion = _objectDefintions.SingleOrDefault(e => e.OID == _objectAttribute.OID);
            if (objectDefintion != null)
            {
                design.ElementID = _objectAttribute.AttrID;
                design.GeneratedName = _objectAttribute.Attribute.Name;
                design.FullName = fullName + "." + design.GeneratedName;
                design.DefaultValue = _objectAttribute.Attribute.DefaultValue;
                design.Type = _objectAttribute.Attribute.AttrType;
                if (_objectAttribute.Attribute.Length != null)
                    design.MaxLength = _objectAttribute.Attribute.Length.Value;
            }
            return design;
        }

        #endregion

        #region Private Methods

        private void BuildElementContainers(string fullName, ElementDesignFromDM design)
        {
            if (_objectTree.ObjectRelation != null && _objectTree.ObjectRelation.ObjectDefinition != null)
                design.GeneratedName = _objectTree.ObjectRelation.ObjectDefinition.ObjectName;
            else
                return;
           

            design.FullName = fullName + "." + design.GeneratedName;

            var objCardinality = _objectTree.ObjectRelation.Cardinality;

            if (objCardinality != null)
            {
                if (objCardinality.Equals("O"))
                {
                    design.Section = GetSectionDesign(design.GeneratedName);
                }
                else if (objCardinality.Equals("M"))
                {
                    design.Repeater = GetRepeaterDesign(design.GeneratedName);
                }
            }
        }

        private RepeaterDesignFromDM GetRepeaterDesign(string fullParentName)
        {
            RepeaterDesignFromDM design = null;

            var builder = new RepeaterDesignBuilderFromDM(_objectTree, _objectDefintions, _objectTrees,_objectRelations, _unitOfWork);
            design = builder.BuildRepeaterFromDomainModel(fullParentName);

            return design;
        }

        private SectionDesignFromDM GetSectionDesign(string fullParentName)
        {
            var builder = new SectionDesignBuilderFromDM(_objectTree, _objectDefintions, _objectTrees,_unitOfWork);
            SectionDesignFromDM design = builder.BuildSectionFromDomainModel(fullParentName);
            return design;
        }

        #endregion
    }
}

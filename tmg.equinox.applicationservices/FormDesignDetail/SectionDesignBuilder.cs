using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.identitymanagement;
using tmg.equinox.mapper;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class SectionDesignBuilder
    {
        SectionUIElement section;
        IEnumerable<UIElement> formElementList;
        List<DataSourceDesign> dataSourceList;
        IUnitOfWorkAsync _unitOfWork;
        List<ValidationDesign> ValidationList;
        List<DuplicationDesign> DuplicationList;
        List<Validator> ValidatorList;
        List<UIElement> DuplicatorList;
        List<RuleDesign> rulesList;
        List<ElementRuleMap> ruleMapList;
        MasterLists msLists;
        StringBuilder customRulesJSON;
        IdentityAccessMapper _IdentityAccessMapper;
        List<FormDesignVersionUIElement> _frmDesignVersionElementList;
        int _formDesignVersionId;
        internal SectionDesignBuilder(SectionUIElement section, IEnumerable<UIElement> formElementList, List<DataSourceDesign> dataSourceList, List<RuleDesign> rulesList,
            List<Validator> validatorList, List<UIElement> duplicatorList, MasterLists msLists, IUnitOfWorkAsync unitOfWork, ref List<ValidationDesign> validationList, ref List<DuplicationDesign> duplicationList, ref List<ElementRuleMap> ruleMapList, ref StringBuilder customRulesJSON, int formDesignVersionId,List<FormDesignVersionUIElement> frmDesignVersionElementList)
        {
            this.section = section;
            this.formElementList = formElementList;
            this.dataSourceList = dataSourceList;
            this.rulesList = rulesList;
            this.ruleMapList = ruleMapList;
            this._unitOfWork = unitOfWork;
            this.ValidationList = validationList;
            this.DuplicationList = duplicationList;
            this.ValidatorList = validatorList;
            this.DuplicatorList = duplicatorList;
            this.msLists = msLists;
            this.customRulesJSON = customRulesJSON;
            _IdentityAccessMapper = new IdentityAccessMapper();
            this._frmDesignVersionElementList = frmDesignVersionElementList;
            this._formDesignVersionId = formDesignVersionId;
        }

        internal SectionDesign BuildSection(string fullParentName)
        {
            SectionDesign design = null;
            try
            {
                if (section != null)
                {

                    design = new SectionDesign();
                    design.Elements = new List<ElementDesign>();
                    design.AccessPermissions = _IdentityAccessMapper.MapToElementAcessViewModel(IdentityManager.GetClaims(section.UIElementID), IdentityManager.GetApplicationRoles());
                    design.ID = section.UIElementID;
                    design.Name = section.UIElementName;
                    design.GeneratedName = section.GeneratedName;
                    if (string.IsNullOrEmpty(fullParentName))
                    {
                        design.FullName = design.GeneratedName;
                    }
                    else
                    {
                        design.FullName = fullParentName + "." + design.GeneratedName;
                    }
                    design.Label = this.GetAlternateLabel() ?? section.Label;
                    design.Enabled = section.Enabled.HasValue == true ? section.Enabled.Value : false;
                    design.Visible = section.Visible.HasValue == true ? section.Visible.Value : false;
                    design.HelpText = section.HelpText;

                    //set DataSource

                    var sectionLayout = msLists.LayoutTypes
                                    .SingleOrDefault(s => s.LayoutTypeID == ((SectionUIElement)section).LayoutTypeID);
                    //default layout to 3-column if none is set for the Section
                    design.LayoutColumn = sectionLayout != null ? sectionLayout.ColumnCount : 3;
                    design.LayoutClass = sectionLayout != null ? sectionLayout.ClassName : "threeColumn";

                    design.LayoutClass = sectionLayout != null ? sectionLayout.ClassName : "threeColumn";
                    design.CustomHtml = section.CustomHtml;
                    design.SectionNameTemplate = "#" + section.GeneratedName + "Custom";
                    design.MDMName = section.MDMName;
                    GetChildElementDesigns(design, "parent");
                    design.ChildCount = design.Elements.Count;
                    design.EffDt = GetEffectiveDate(section.UIElementID);
                    design.Op = GetOperation(section.UIElementID);

                    customRulesJSON.AppendLine(section.CustomRule);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return design;
        }

        private string GetEffectiveDate(int UIElementID)
        {
            DateTime? effectiveDt = msLists.ElementMap
                                    .Where(c => c.UIElementID == UIElementID)
                                    .Select(c => c.EffectiveDate)
                                    .SingleOrDefault();
            return ((DateTime)effectiveDt).ToString("MM/dd/yyyy");
        }

        private string GetOperation(int UIElementID)
        {
            string operation = msLists.IgnoreElementList
                                    .Where(c => c.UIElementID == UIElementID)
                                    .Select(c => c.Operation)
                                    .SingleOrDefault();
            return operation;
        }

        private void GetChildElementDesigns(SectionDesign design, string isParent)
        {
            var elements = from t in section.UIElement1
                           join u in formElementList
                           on t.UIElementID equals u.UIElementID
                           orderby t.Sequence
                           select t;
            if (elements != null && elements.Count() > 0)
            {
                foreach (var elem in elements)
                {
                    UIElementDesignBuilder builder = new UIElementDesignBuilder(elem, formElementList, dataSourceList, rulesList, ValidatorList, DuplicatorList, msLists, _unitOfWork, ref ValidationList, ref DuplicationList, ref ruleMapList, ref customRulesJSON, this._formDesignVersionId, this._frmDesignVersionElementList);
                    design.Elements.Add(builder.BuildElement(design.FullName));
                }
            }
        }

        private string GetAlternateLabel()
        {
            string altLabel = null;
            try
            {
                var altLabelRow = msLists.AlternateLabelList
                                    .Where(c => c.UIElementID == section.UIElementID).FirstOrDefault();

                if (altLabelRow != null)
                {
                    altLabel = altLabelRow.AlternateLabel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return altLabel;

        }
    }
}

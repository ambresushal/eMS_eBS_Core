using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class RepeaterDesignBuilder
    {
        RepeaterUIElement repeater;
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

        List<FormDesignVersionUIElement> _frmDesignVersionElementList;
        int _formDesignVersionId;

        internal RepeaterDesignBuilder(RepeaterUIElement repeater, IEnumerable<UIElement> formElementList, List<DataSourceDesign> dataSourceList, List<RuleDesign> rulesList,
            List<Validator> validatorList, List<UIElement> duplicatorList, MasterLists msLists, IUnitOfWorkAsync unitOfWork, ref List<ValidationDesign> validationList, ref List<DuplicationDesign> duplicationList, ref List<ElementRuleMap> ruleMapList, ref StringBuilder customRulesJSON, int formDesignVersionId,List<FormDesignVersionUIElement> frmDesignVersionElementList)
        {
            this.repeater = repeater;
            this.formElementList = formElementList;
            this.dataSourceList = dataSourceList;
            this._unitOfWork = unitOfWork;
            this.ValidationList = validationList;
            this.DuplicationList = duplicationList;
            this.ValidatorList = validatorList;
            this.DuplicatorList = duplicatorList;
            this.rulesList = rulesList;
            this.ruleMapList = ruleMapList;
            this.msLists = msLists;
            this.customRulesJSON = customRulesJSON;
            this._frmDesignVersionElementList = frmDesignVersionElementList;
            this._formDesignVersionId = formDesignVersionId;
        }

        internal RepeaterDesign BuildRepeater(string fullParentName)
        {
            RepeaterDesign design = null;
            if (repeater != null)
            {
                design = new RepeaterDesign();
                design.Elements = new List<ElementDesign>();
                design.ID = repeater.UIElementID;
                design.Name = repeater.UIElementName;
                design.GeneratedName = repeater.GeneratedName;
                design.FullName = fullParentName + "." + repeater.GeneratedName;
                design.Label = this.GetAlternateLabel() ?? repeater.Label;
                design.HelpText = repeater.HelpText;
                design.RepeaterType = "child";
                design.RowCount = 1;
                var repeaterLayout = msLists.LayoutTypes
                                .SingleOrDefault(s => s.LayoutTypeID == repeater.LayoutTypeID);
                //default layout to 3-column if none is set for the Section
                design.LayoutColumn = repeaterLayout != null ? repeaterLayout.ColumnCount : 3;
                design.LayoutClass = repeaterLayout != null ? repeaterLayout.ClassName : "threeColumn";
                design.ChildCount = design.Elements.Count;
                design.LoadFromServer = repeater.LoadFromServer;
                design.IsDataRequired = repeater.IsDataRequired;
                design.AllowBulkUpdate = repeater.AllowBulkUpdate;
                design.IsEnabled = repeater.Enabled;

                // Properties for Configuring Param Query Features 
                design.DisplayTopHeader = repeater.DisplayTopHeader;
                design.DisplayTitle = repeater.DisplayTitle;
                design.FrozenColCount = repeater.FrozenColCount;
                design.FrozenRowCount = repeater.FrozenRowCount;
                design.AllowPaging = repeater.AllowPaging;
                design.RowsPerPage = repeater.RowsPerPage;
                design.AllowExportToExcel = repeater.AllowExportToExcel;
                design.AllowExportToCSV = repeater.AllowExportToCSV;
                design.FilterMode = repeater.FilterMode;
                design.RepeaterUIElementProperties = GetRepeaterUIElementProperties(repeater.UIElementID);
                design.Op = GetOperation(repeater.UIElementID);
                design.IsSameSectionRuleSource = repeater.IsSameSectionRuleSource;
                design.MDMName = repeater.MDMName;
                if (this.dataSourceList != null && this.dataSourceList.Count > 0)
                {
                    string compareName = fullParentName + "." + repeater.GeneratedName;
                    var pri = (from des in this.dataSourceList
                               where des.TargetParent == compareName &&
                                   des.DisplayMode == "Primary"
                               select des).FirstOrDefault();
                    if (pri != null)
                    {
                        design.PrimaryDataSource = pri;
                    }
                    var children = from des in this.dataSourceList
                                   where des.TargetParent == compareName &&
                                        (des.DisplayMode == "In Line" || des.DisplayMode == "Child")
                                   select des;
                    if (children != null && children.Count() > 0)
                    {
                        design.ChildDataSources = children.ToList();
                    }
                }
                GetChildElementDesigns(design, "child", fullParentName + "." + repeater.GeneratedName);

                customRulesJSON.AppendLine(repeater.CustomRule);

            }
            return design;
        }

        private string GetOperation(int UIElementID)
        {
            string operation = msLists.IgnoreElementList
                                    .Where(c => c.UIElementID == UIElementID)
                                    .Select(c => c.Operation)
                                    .SingleOrDefault();
            return operation;
        }

        private void GetChildElementDesigns(RepeaterDesign design, string isParent, string fullParentName)
        {
            var elements = from t in repeater.UIElement1
                           join u in formElementList
                           on t.UIElementID equals u.UIElementID
                           orderby t.Sequence
                           select t;
            if (elements != null && elements.Count() > 0)
            {
                foreach (var elem in elements)
                {
                    UIElementDesignBuilder builder = new UIElementDesignBuilder(elem, formElementList, dataSourceList, rulesList, ValidatorList, DuplicatorList, msLists, _unitOfWork, ref ValidationList, ref DuplicationList, ref ruleMapList, ref customRulesJSON, this._formDesignVersionId,this._frmDesignVersionElementList);
                    ElementDesign elementDesign = builder.BuildElement(fullParentName);
                    elementDesign.IsKey = this.IsKeyElement(elementDesign.ElementID);
                    elementDesign.IsPrimary = true;
                    if (design.ChildDataSources != null && design.ChildDataSources.Count() > 0)
                    {
                        foreach (var dataSource in design.ChildDataSources)
                        {
                            var match = (from el in dataSource.Mappings
                                         where
                                             el.TargetElement == elementDesign.GeneratedName
                                         select el.TargetElement).FirstOrDefault();
                            if (!string.IsNullOrEmpty(match))
                            {
                                elementDesign.IsPrimary = false;
                                break;
                            }
                        }
                    }

                    design.Elements.Add(elementDesign);
                }
            }
        }

        private string GetAlternateLabel()
        {
            string altLabel = null;
            try
            {
                var altLabelRow = msLists.AlternateLabelList
                             .Where(c => c.UIElementID == repeater.UIElementID).FirstOrDefault();

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
        private bool IsKeyElement(int uiElementID)
        {
            return this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Query().Get().Any(c => c.UIElementID == uiElementID);
        }

        private RepeaterUIElementPropertyModel GetRepeaterUIElementProperties(int UIElementID)
        {
            RepeaterUIElementPropertyModel repeaterUIElementPropertyModel = null;
            RepeaterUIElementProperties repeaterElementProperties = this._unitOfWork.Repository<RepeaterUIElementProperties>()
                                                              .Query()
                                                              .Filter(c => c.RepeaterUIElementID == UIElementID)
                                                              .Get().FirstOrDefault();
            if (repeaterElementProperties != null)
            {
                repeaterUIElementPropertyModel = new RepeaterUIElementPropertyModel();
                repeaterUIElementPropertyModel.RowTemplate = repeaterElementProperties.RowTemplate;
                repeaterUIElementPropertyModel.HeaderTemplate = repeaterElementProperties.HeaderTemplate;
                repeaterUIElementPropertyModel.FooterTemplate = repeaterElementProperties.FooterTemplate;
            }
            return repeaterUIElementPropertyModel;
        }
    }
}

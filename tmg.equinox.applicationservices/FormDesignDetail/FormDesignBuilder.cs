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
using System.Data.SqlClient;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class FormDesignBuilder
    {
        int formDesignVersionId;
        int formDesignId;
        int tenantId;
        IUnitOfWorkAsync _unitOfWork;

        internal FormDesignBuilder(int tenantId, int formDesignVersionId, IUnitOfWorkAsync unitOfWork)
        {
            this.formDesignVersionId = formDesignVersionId;
            this.tenantId = tenantId;
            this._unitOfWork = unitOfWork;
        }

        internal FormDesignVersionDetail BuildFormDesign()
        {
            FormDesignVersionDetail detail = null;
            SqlParameter paramFormDesignVersionID = new SqlParameter("@FormDesignVersionID", this.formDesignVersionId);
            FormDesignVersion formVersion = this._unitOfWork.Repository<FormDesignVersion>().ExecuteSql("exec [dbo].[uspGetFormDesignVersion] @FormDesignVersionID", paramFormDesignVersionID).FirstOrDefault();

            if (formVersion != null)
            {
                if (formVersion.FormDesignID.HasValue == true)
                {
                    this.formDesignId = formVersion.FormDesignID.Value;
                }
                FormDesign formDesign = GetFormDesign();

                detail = new FormDesignVersionDetail();
                detail.TenantID = tenantId;
                detail.FormDesignId = this.formDesignId;
                detail.FormDesignVersionId = formVersion.FormDesignVersionID;
                detail.FormVersion = formVersion.VersionNumber;
                detail.FormName = formDesign.FormName;
                detail.IsMasterList = formDesign.IsMasterList;
                detail.IsAliasDesignMasterList = formDesign.IsAliasDesignMasterList;
                detail.UsesAliasDesignMasterList = formDesign.UsesAliasDesignMasterList;
                StringBuilder customRulesJSON = new StringBuilder();

                //get Master Lists /metadata required
                MasterLists msLists = new MasterLists(tenantId, _unitOfWork, formDesignVersionId);

                //get all elements for the Form Design Version
                List<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Get()
                                                   join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                           .Query()
                                                                           .Get()
                                                   on u.UIElementID equals fd.UIElementID
                                                   where fd.FormDesignVersionID == formDesignVersionId
                                                   select u).ToList();
                //list of all validations
                List<Validator> validatorList = (from v in this._unitOfWork.RepositoryAsync<Validator>()
                                                           .Query()
                                                           .Get()
                                                 join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                         .Query()
                                                                         .Get()
                                                 on v.UIElementID equals fd.UIElementID
                                                 where fd.FormDesignVersionID == formDesignVersionId && v.IsActive == true
                                                 select v).ToList();

                //list of all duplications
                List<UIElement> duplicatorList = formElementList.Where(x => x.CheckDuplicate == true).ToList();

                //get top-level Section
                var topLevelTabElement = formElementList
                                .Where(c => c.ParentUIElementID == null)
                                .OrderBy(c => c.Sequence)
                                .FirstOrDefault(); //returns null if matching record is not found in table

                if (topLevelTabElement != null)
                {
                    SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", this.formDesignVersionId);
                    List<FormDesignVersionUIElement> frmDesignVersionElementList = this._unitOfWork.Repository<FormDesignVersionUIElement>().ExecuteSql("exec [dbo].[uspGetFormDesignVersionUIElement] @FormDesignVersionID", paramFrmDesignVersionID).ToList();

                    //get data sources
                    DataSourceDesignBuilder dsBuilder = new DataSourceDesignBuilder(this.tenantId, this.formDesignId, this.formDesignVersionId,
                        formElementList, this._unitOfWork, msLists);
                    detail.DataSources = dsBuilder.GetDataSources();

                    //get Rules
                    RuleDesignBuilder ruleBuilder = new RuleDesignBuilder(this.tenantId, this.formDesignVersionId, formElementList, detail.DataSources, frmDesignVersionElementList, this._unitOfWork);
                    detail.Rules = ruleBuilder.GetRules();


                    List<SectionDesign> topLevelSectionDesignList = new List<SectionDesign>();
                    //get top level sections
                    var topLevelSectionList = (from p in topLevelTabElement.UIElement1
                                               join u in formElementList
                                               on p.UIElementID equals u.UIElementID
                                               orderby p.Sequence
                                               select p
                                                ).ToList();
                    detail.Sections = new List<SectionDesign>();

                    List<ValidationDesign> validationList = new List<ValidationDesign>();
                    List<DuplicationDesign> duplicationList = new List<DuplicationDesign>();
                    List<ElementRuleMap> ruleMapList = new List<ElementRuleMap>();


                    foreach (var section in topLevelSectionList)
                    {

                        SectionDesignBuilder builder = new SectionDesignBuilder((SectionUIElement)section, formElementList, detail.DataSources, detail.Rules, validatorList, duplicatorList, msLists, _unitOfWork, ref validationList, ref duplicationList, ref ruleMapList, ref customRulesJSON, this.formDesignVersionId, frmDesignVersionElementList);
                        SectionDesign design = builder.BuildSection("");
                        detail.Sections.Add(design);
                    }

                    detail.Duplications = duplicationList;
                    detail.Validations = validationList;
                    detail.ElementRuleMaps = ruleMapList;
                    //detail.CustomRules = "try { " + (!string.IsNullOrEmpty(topLevelTabElement.CustomRule) ? topLevelTabElement.CustomRule : "") + customRulesJSON.ToString() + " } catch(e) { }";
                    detail.CustomRules = string.Empty;
                    // Re-Initialize rule.Expressions  
                    try
                    {
                        foreach (var rule in detail.Rules)
                        {
                            rule.Expressions = new List<ExpressionDesign>();
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
            return detail;
        }
        private FormDesign GetFormDesign()
        {
            FormDesign frmDesign = null;
            try
            {
                frmDesign = (from design in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                             where design.FormID == this.formDesignId
                             select design).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return frmDesign;
        }
    }
}
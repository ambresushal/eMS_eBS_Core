using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Enums;

namespace tmg.equinox.repository.extensions
{
    public static class UIElementRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static IQueryable<UIElement> GetUIElementListForFormDesignVersionWithInclude(this IRepositoryAsync<UIElement> uiElementRepository, int tenantId, int formDesignVersionId)
        {
            IList<UIElement> elementList = null;
            try
            {
                var uiElementList = (from u in uiElementRepository
                                                            .Query()
                                                            .Include(c => c.UIElement1)
                                                            .Include(c => c.UIElement2)
                                                            .Include(c => c.Validators)
                                                            .Include(c => c.ApplicationDataType)
                                                            .Include(c => c.FormDesignVersionUIElementMaps)
                                                            .AsNoTracking(true)
                                                            .Get()
                                     where u.FormDesignVersionUIElementMaps.Any(c => c.FormDesignVersionID == formDesignVersionId)
                                     select u).ToList();

                var rootNode = uiElementList.FirstOrDefault(c => c.ParentUIElementID == null);

                elementList = uiElementList;

                if (elementList.Count > 1)
                {
                    //get elements in hierarchical order 
                    elementList = (from t in rootNode.DepthFirst(n => uiElementList
                                                                                .Where(c => c.ParentUIElementID == n.UIElementID)
                                                                                .OrderBy(c => c.Sequence))
                                   select t).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return elementList.AsQueryable();
        }

        public static IQueryable<UIElement> GetUIElementListForFormDesignVersion(this IRepositoryAsync<UIElement> uiElementRepository, int tenantId, int formDesignVersionId)
        {
            IList<UIElement> elementList = null;
            try
            {
                var uiElementList = (from u in uiElementRepository
                                                            .Query()
                                                            .Include(c => c.ApplicationDataType)
                                                            .Include(c => c.DataSourceMappings1)
                                                            .AsNoTracking(true)
                                                            .Get()
                                     where u.FormDesignVersionUIElementMaps.Where(c => c.FormDesignVersionID == formDesignVersionId && c.Operation != "I").Any()
                                     select u).ToList();

                var rootNode = uiElementList.Where(c => c.ParentUIElementID == null).FirstOrDefault();

                elementList = uiElementList;

                if (elementList.Count > 1)
                {
                    //get elements in hierarchical order 
                    elementList = (from t in rootNode.DepthFirst(n => uiElementList
                                                                                .Where(c => c.ParentUIElementID == n.UIElementID)
                                                                                .OrderBy(c => c.Sequence))
                                   select t).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return elementList.AsQueryable();
        }

        public static IQueryable<UIElement> GetParentSectionForFormDesignVersion(this IRepositoryAsync<UIElement> uiElementRepository, int tenantId, int formDesignVersionId)
        {
            IList<UIElement> elementList = null;
            try
            {
                var uiElementList = (from u in uiElementRepository
                                                            .Query()
                                                            .AsNoTracking(true)
                                                            .Get()
                                     where u.FormDesignVersionUIElementMaps.Where(c => c.FormDesignVersionID == formDesignVersionId).Any()
                                     select u).ToList();

                var rootNode = uiElementList.Where(c => c.ParentUIElementID == null).FirstOrDefault();

                elementList = uiElementList;

                if (elementList.Count > 1)
                {
                    elementList = (from t in uiElementList.Where(c => c.ParentUIElementID == rootNode.UIElementID && c.Visible == true)
                                                                                 .OrderBy(c => c.Sequence)
                                   select t).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return elementList.AsQueryable();
        }

        public static int GetMaxUIElementSequence(this IRepositoryAsync<UIElement> uiElementRepository, int elementId, int formID)
        {
            int maxSequence = 0;
            try
            {
                List<int> sequenceList = uiElementRepository
                                .Query()
                                .Filter(c => c.UIElementID == elementId & c.FormID == formID)
                                .Get()
                                .OrderByDescending(d => d.Sequence)
                                .Select(c => c.Sequence)
                                .ToList();
                maxSequence = sequenceList.Count() > 0 ? sequenceList.Max() + 1 : 1;
            }
            catch (Exception es)
            {
                throw;
            }
            return maxSequence;
        }

        public static IQueryable<UIElement> GetAllContainerElements(this IRepositoryAsync<UIElement> uiElementRepository, int tenandId, int formDesignVersionId)
        {
            return uiElementRepository
                        .GetUIElementListForFormDesignVersionWithInclude(tenandId, formDesignVersionId)
                        .Where(c => c is TabUIElement || c is SectionUIElement || c is RepeaterUIElement);
        }

        //TODO: This Methods needs to be optimized from performance point of view.
        public static bool Delete(this IRepositoryAsync<UIElement> uiElementRepository, IUnitOfWorkAsync unitOfWork, int tenantId, int uiElementID, int formDesignId, int formDesignVersionID)
        {
            bool isDeleted = false;
            try
            {
                UIElement uiElement = uiElementRepository.Query()
                                                          .Filter(c => c.UIElementID == uiElementID)
                                                          //Include FormDesignVersionUIElementMaps
                                                          .Include(inc => inc.FormDesignVersionUIElementMaps)
                                                          .Get()
                                                          .FirstOrDefault();
                if (uiElement != null)
                {
                    //Delete all FormDesignVersionUIElementMap associated to respective formDesignVersionID
                    foreach (var item in unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                   .Query()
                                                   .Filter(c => c.UIElementID == uiElementID && c.FormDesignVersionID == formDesignVersionID)
                                                   .Get()
                                                   .ToList())
                    {
                        unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Delete(item);
                    }

                    // After delete All MappedUIElement form collection Delete all dependent table records
                    if (uiElement.FormDesignVersionUIElementMaps.Count <= 0)
                    {
                        //Delete all data source mappings
                        foreach (var dataSourceMap in unitOfWork.RepositoryAsync<DataSourceMapping>()
                                            .Query()
                                            .Filter(c => (c.UIElementID == uiElementID
                                                         || c.MappedUIElementID == uiElementID)
                                                         && c.FormDesignID == formDesignId
                                                         && c.FormDesignVersionID == formDesignVersionID)
                                            .Get().ToList())
                        {
                            unitOfWork.RepositoryAsync<DataSourceMapping>().Delete(dataSourceMap);
                        }
                        //Delete all RepeaterKey
                        DeleteRepeaterKey(unitOfWork, uiElementID);
                        //Delete alternate Label
                        DeleteAlternateLabel(unitOfWork, uiElementID, formDesignId, formDesignVersionID);
                        //Delete Document Rule
                        DeleteDocumentRule(unitOfWork, uiElementID, formDesignId, formDesignVersionID);
                        //Delete all PropertyRuleMaps & Rules & Expression
                        foreach (var propertyRuleMap in unitOfWork.RepositoryAsync<PropertyRuleMap>().Query().Filter(c => c.UIElementID == uiElementID).Get().ToList())
                        {
                            //delete property rule map
                            unitOfWork.RepositoryAsync<PropertyRuleMap>().Delete(propertyRuleMap);

                            //get all rules to delete for the given property rule map
                            List<Rule> rulesToDelete = unitOfWork.Repository<Rule>()
                                                                    .Query()
                                                                    .Filter(c => c.RuleID == propertyRuleMap.RuleID)
                                                                    .Include(c => c.Expressions)
                                                                    .Get()
                                                                    .ToList();
                            //Deletes all rules
                            foreach (var rule in rulesToDelete)
                            {
                                //delete all expressions in rule first
                                var expressions = (from exp in unitOfWork.RepositoryAsync<Expression>().Get() where exp.RuleID == rule.RuleID select exp).ToList();

                                foreach (var exp in expressions)
                                {
                                    //Delete Complex Operator
                                    var complexOp = (from op in unitOfWork.RepositoryAsync<ComplexOperator>().Get() where op.ExpressionID == exp.ExpressionID select op).ToList();
                                    foreach (var comOp in complexOp)
                                    {
                                        unitOfWork.RepositoryAsync<ComplexOperator>().Delete(comOp.ComplexOperatorID);
                                    }

                                    //Delete Repeater Key filters
                                    var rptFilters = (from flt in unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get() where flt.ExpressionID == exp.ExpressionID select flt).ToList();
                                    foreach (var filter in rptFilters)
                                    {
                                        unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Delete(filter.RepeaterKeyID);
                                    }
                                }

                                foreach (var exp in expressions)
                                {
                                    unitOfWork.RepositoryAsync<Expression>().Delete(exp.ExpressionID);
                                }

                                //delete Target repeater key filter
                                var tgtFilter = (from flt in unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get() where flt.RuleID == rule.RuleID select flt).ToList();
                                foreach (var tfilter in tgtFilter)
                                {
                                    unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Delete(tfilter.TargetRepeaterKeyID);
                                }

                                //delete rule
                                unitOfWork.RepositoryAsync<Rule>().Delete(rule);
                            }
                        }
                        //Delete all Validators
                        foreach (var validator in unitOfWork.RepositoryAsync<Validator>().Query().Filter(c => c.UIElementID == uiElementID).Get().ToList())
                        {
                            unitOfWork.RepositoryAsync<Validator>().Delete(validator);
                        }

                        //Dlete all DropdownUIELement
                        if (uiElement is DropDownUIElement)
                        {
                            foreach (var dropDownItem in unitOfWork.RepositoryAsync<DropDownElementItem>().Query().Filter(c => c.UIElementID == uiElementID).Get().ToList())
                            {
                                unitOfWork.RepositoryAsync<DropDownElementItem>().Delete(dropDownItem);
                            }
                        }

                        DeleteDependentExpression(unitOfWork, uiElement);
                        //Delete UIElement
                        unitOfWork.RepositoryAsync<UIElement>().Delete(uiElement);
                        isDeleted = true;
                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return isDeleted;
        }

        public static void DeleteElement(this IRepositoryAsync<UIElement> uiElementRepository, IUnitOfWorkAsync unitOfWork, int tenantId, int uiElementId, int formDesignVersionId, out ExceptionMessages exceptionMessage)
        {

            exceptionMessage = ExceptionMessages.NULL;
            UIElement uiElement = unitOfWork.RepositoryAsync<UIElement>().Query().Filter(c => c.UIElementID == uiElementId).Get().FirstOrDefault();

            int? formDesignId = unitOfWork.RepositoryAsync<FormDesignVersion>()
                                .Query()
                                .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                .Get()
                                .Select(c => c.FormDesignID)
                                .SingleOrDefault();

            //if UIElement is a Container item 

            if (uiElement != null)
            {
                if (uiElement.IsContainer)
                {
                    int dataSourceID;

                    //Delete all SectionUIElement and also delete records form data source if that section is a data source
                    if (uiElement is SectionUIElement)
                    {
                        dataSourceID = ((SectionUIElement)(uiElement)).DataSourceID ?? 0;
                        if (dataSourceID > 0)
                        {
                            DeleteDataSource(unitOfWork, dataSourceID, formDesignId, formDesignVersionId, out exceptionMessage);
                            if (exceptionMessage.ToDescriptionString() != "")
                            {
                                return;
                            }
                        }
                    }
                    //Delete all RepeaterUIElement and also delete records form data source if that repeater is a data source
                    else if (uiElement is RepeaterUIElement)
                    {
                        dataSourceID = ((RepeaterUIElement)(uiElement)).DataSourceID ?? 0;
                        if (dataSourceID > 0)
                        {
                            DeleteDataSource(unitOfWork, dataSourceID, formDesignId, formDesignVersionId, out exceptionMessage);
                            if (exceptionMessage.ToDescriptionString() != "")
                            {
                                return;
                            }
                        }

                    }

                    //get the children's of the element & call the function recursively.
                    foreach (var item in unitOfWork.RepositoryAsync<UIElement>().Query().Filter(c => c.ParentUIElementID == uiElementId).Get().ToList())
                    {
                        DeleteElement(uiElementRepository, unitOfWork, tenantId, item.UIElementID, formDesignVersionId, out exceptionMessage);
                    }
                }

                bool isFormDesignVersionFinzalized = unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                if (isFormDesignVersionFinzalized)
                {
                    //throw new NotSupportedException("Unable to Delete finalized Form Design Version");
                    exceptionMessage = ExceptionMessages.FINALIZEDVERSION;
                }
                else
                {
                    //FormDesignVersionUIElementMap list for element if used in some other form design version
                    List<FormDesignVersionUIElementMap> list = unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
                                                                                .Include(c => c.FormDesignVersion.Status)
                                                                                .Filter(c => c.UIElementID == uiElementId && c.FormDesignVersionID != formDesignVersionId)
                                                                                .Get()
                                                                                .ToList();
                    FormDesignVersionUIElementMap currentMap = unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
                                                                                .Filter(c => c.FormDesignVersionID == formDesignVersionId && c.UIElementID == uiElementId)
                                                                                .Get()
                                                                                .FirstOrDefault();
                    if (currentMap != null)
                    {
                        //Assuming Finalized would be the text for finalization status.
                        if (list.Any(c => c.FormDesignVersion.StatusID == Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized)))
                        {
                            //update effective date of removal 
                            foreach (var item in list)
                            {
                                item.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);
                                unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(item);
                            }

                            //Delete existing element map from FormDesignVersionUIElementMap to have new UIElementID
                            //unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Delete(currentMap);
                            currentMap.Operation = "I";
                            unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(currentMap);

                        }
                        else
                        {
                            //call recursively to delete
                            Delete(uiElementRepository, unitOfWork, tenantId, uiElementId, formDesignId.Value, formDesignVersionId);
                        }

                        using (var scope = new TransactionScope())
                        {
                            unitOfWork.Save();
                            scope.Complete();
                        }
                    }
                }
            }


        }

        public static int GetElementID(this IRepositoryAsync<UIElement> uiElementRepository, int elementId, string id)
        {
            if (id == "ParentID")
            {
                return uiElementRepository
                                .Query()
                                .Filter(c => c.UIElementID == elementId)
                                .Get()
                                .Select(c => c.ParentUIElementID.Value)
                                .FirstOrDefault();
            }
            else
            {
                return uiElementRepository
                                .Query()
                                .Filter(c => c.UIElementID == elementId)
                                .Get()
                                .Select(c => c.FormID)
                                .FirstOrDefault();
            }
        }
        #endregion Public Methods

        #region Private Methods

        private static void DeleteDataSource(IUnitOfWorkAsync unitOfWork, int dataSourceID, int? formDesignId, int formDesignVersionId, out ExceptionMessages exceptionMessage)
        {

            exceptionMessage = ExceptionMessages.NULL;
            //get the data source details
            var dataSource = unitOfWork.RepositoryAsync<DataSource>()
                                                         .Query()
                                                         .Include(inc => inc.DataSourceMappings)
                                                         .Filter(fil =>
                                                             fil.DataSourceID == dataSourceID
                                                             && fil.FormDesignID == formDesignId
                                                             && fil.FormDesignVersionID == formDesignVersionId)
                                                         .Get()
                                                         .SingleOrDefault();


            if (dataSource != null)
            {

                if (dataSource.DataSourceMappings != null && dataSource.DataSourceMappings.Any())
                {
                    //if DataSourceMapping is present then Throw exception
                    //throw new NotSupportedException("Data source is already in use. You can not delete it.");
                    exceptionMessage = ExceptionMessages.USEDDATASOURCE;
                }
                else
                {
                    var isDataSourceFromFinzalizedFormDesignVersion = unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(dataSource.FormDesignVersionID);
                    if (!isDataSourceFromFinzalizedFormDesignVersion)
                    {
                        unitOfWork.RepositoryAsync<DataSource>().Delete(dataSource);
                    }
                    else
                    {
                        //if DataSourceMapping is present then Throw exception
                        //throw new NotSupportedException("Data source is associated with Finalized FormDesignVersion. You can not delete it.");
                        exceptionMessage = ExceptionMessages.USEDDATASOURCEINFINALIZEDFORM;
                    }
                }
            }

        }

        private static void DeleteRepeaterKey(IUnitOfWorkAsync unitOfWork, int uielementID)
        {
            RepeaterKeyUIElement repKey = unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Query().Filter(c => c.UIElementID == uielementID).Get().FirstOrDefault();
            if (repKey != null)
            {
                unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Delete(repKey);
            }
        }

        private static void DeleteDocumentRule(IUnitOfWorkAsync unitOfWork, int uielementID, int formdesignId, int formdesignversionID)
        {
            var docRule = unitOfWork.RepositoryAsync<DocumentRule>().Query().Get()
                                    .Where(c => c.TargetUIElementID == uielementID && c.FormDesignID == formdesignId
                                                 && c.FormDesignVersionID == formdesignversionID).ToList();
            if (docRule != null)
            {
                foreach (var d in docRule)
                {
                    var getEvents = unitOfWork.RepositoryAsync<DocumentRuleEventMap>()
                                                              .Query()
                                                              .Get()
                                                              .Where(f => f.DocumentRuleID == d.DocumentRuleID)
                                                              .ToList();
                    if (getEvents.Count > 0)
                    {
                        foreach (var i in getEvents)
                        {
                            unitOfWork.RepositoryAsync<DocumentRuleEventMap>().Delete(i.DocumentRuleEventMapID);
                            unitOfWork.Save();
                        }

                    }

                    unitOfWork.RepositoryAsync<DocumentRule>().Delete(d.DocumentRuleID);
                }
            }
        }

        private static void DeleteAlternateLabel(IUnitOfWorkAsync unitOfWork, int uielementID, int formdesignId, int formdesignversionID)
        {
            AlternateUIElementLabel altlabel = unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Query()
                                                .Filter(c => c.UIElementID == uielementID && c.FormDesignID == formdesignId
                                                            && c.FormDesignVersionID == formdesignversionID)
                                                .Get().FirstOrDefault();
            if (altlabel != null)
            {
                unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Delete(altlabel);
            }
        }


        private static void GetRepeaterKey(IUnitOfWorkAsync unitOfWork, int parentId)
        {
            List<string> key = new List<string>();
            key = (from u in unitOfWork.RepositoryAsync<UIElement>().Get()
                   join r in unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Get() on u.UIElementID equals r.UIElementID
                   where (u.ParentUIElementID == parentId
                         && u.IsActive == true
                         )
                   select
                    u.GeneratedName.ToString()
                   ).ToList<string>();
        }

        private static void DeleteDependentExpression(IUnitOfWorkAsync unitOfWork, UIElement uiElement)
        {
            try
            {
                List<Expression> DependentExpressionList = unitOfWork.RepositoryAsync<Expression>().Query()
                                                      .Filter(c => c.LeftOperand == uiElement.UIElementName ||
                                                      c.RightOperand == uiElement.UIElementName
                                                      )
                                                      .Get().
                                                      ToList();
                if (DependentExpressionList != null)
                {
                    foreach (var exp in DependentExpressionList)
                    {
                        if (exp != null)
                        {
                            var complexOp = (from op in unitOfWork.RepositoryAsync<ComplexOperator>().Get()
                                             where op.ExpressionID == exp.ExpressionID
                                             select op)
                                .ToList();
                            if (complexOp != null)
                            {
                                foreach (var comOp in complexOp)
                                {
                                    unitOfWork.RepositoryAsync<ComplexOperator>().Delete(comOp.ComplexOperatorID);
                                }
                            }
                            var rptFilters = (from flt in unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get() where flt.ExpressionID == exp.ExpressionID select flt).ToList();
                            if (rptFilters != null)
                            {
                                foreach (var filter in rptFilters)
                                {
                                    unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Delete(filter.RepeaterKeyID);
                                }
                            }
                            unitOfWork.RepositoryAsync<Expression>().Delete(exp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion Private Methods
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.applicationservices.ServiceDesignDetail
{
    internal class ServiceDesignBuilder
    {
        private int formDesignVersionID;
        private int formDesignID;
        private int tenantID;
        private int serviceDesignVersionID;
        private int serviceDesignID;
        IUnitOfWorkAsync _unitOfWork;

        internal ServiceDesignBuilder(int tenantID, int formDesignID, int formDesignVersionID, int serviceDesignID, int serviceDesignVersionID, IUnitOfWorkAsync unitOfWork)
        {
            this.formDesignVersionID = formDesignVersionID;
            this.tenantID = tenantID;
            this.formDesignID = formDesignID;
            this._unitOfWork = unitOfWork;
            this.serviceDesignID = serviceDesignID;
            this.serviceDesignVersionID = serviceDesignVersionID;
        }

        internal ServiceDesignVersionDetail BuildServiceDesign()
        {
            ServiceDesignVersionDetail detail = null;
            ServiceDesignVersion serviceVersion = this._unitOfWork.RepositoryAsync<ServiceDesignVersion>()
                                                                        .Query()
                                                                        .Filter(c => c.ServiceDesignVersionID == this.serviceDesignVersionID)
                                                                        .Include(c => c.ServiceDetail)
                                                                        .Include(c => c.FormDesign)
                                                                        .Get()
                                                                        .SingleOrDefault();

            if (serviceVersion != null)
            {
                detail = new ServiceDesignVersionDetail();
                detail.TenantID = tenantID;
                detail.FormDesignID = this.formDesignID;
                detail.FormDesignVersionID = serviceVersion.FormDesignVersionID;
                detail.ServiceDesignID = this.serviceDesignID;
                detail.ServiceDesignVersionID = this.serviceDesignVersionID;
                detail.ServiceMethodName = serviceVersion.ServiceDetail.ServiceMethodName;
                detail.FormDesignName = serviceVersion.FormDesign.FormName;
                detail.VersionNumber = serviceVersion.VersionNumber;
                detail.ResponseType = serviceVersion.ServiceDetail.IsReturnJSON ? ResponseType.Json : ResponseType.Xml;

                //get all elements for the Form Design Version
                List<ServiceDefinition> serviceElementList = (from u in this._unitOfWork.RepositoryAsync<ServiceDefinition>()
                                                            .Query()
                                                            .Get()
                                                              join fd in this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                                              .Query()
                                                                              .Get()
                                                      on u.ServiceDefinitionID equals fd.ServiceDefinitionID
                                                              where fd.ServiceDesignVersionID == serviceDesignVersionID
                                                              select u).ToList();

                //get top-level Section
                ServiceDefinition topLevelTabElement = serviceElementList
                                .Where(c => c.ParentServiceDefinitionID == null)
                                .OrderBy(c => c.Sequence)
                                .FirstOrDefault(); //returns null if matching record is not found in table

                if (topLevelTabElement != null)
                {
                    //get all elements for the Form Design Version
                    List<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Get()
                                                       join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                               .Query()
                                                                               .Get()
                                                       on u.UIElementID equals fd.UIElementID
                                                       where fd.FormDesignVersionID == formDesignVersionID
                                                       select u).ToList();

                    //get data sources
                    DataSourceDesignBuilder dsBuilder = new DataSourceDesignBuilder(this.tenantID, this.formDesignID, this.formDesignVersionID,
                        formElementList, this._unitOfWork);
                    detail.DataSources = dsBuilder.GetDataSources();

                    List<SectionDesign> topLevelSectionDesignList = new List<SectionDesign>();
                    //get top level sections
                    var topLevelSectionList = (from u in serviceElementList
                                               where u.ParentServiceDefinitionID == topLevelTabElement.ServiceDefinitionID
                                               orderby u.Sequence
                                               select u
                                                ).ToList();
                    detail.Sections = new List<SectionDesign>();
                    detail.Repeaters = new List<RepeaterDesign>();

                    //get Master Lists /metadata required
                    tmg.equinox.applicationservices.FormDesignDetail.MasterLists msLists = new tmg.equinox.applicationservices.FormDesignDetail.MasterLists(tenantID, _unitOfWork, formDesignVersionID);

                    foreach (var section in topLevelSectionList)
                    {
                        UIElement element = formElementList.Where(c => c.UIElementID == section.UIElementID).FirstOrDefault();
                        if (element is SectionUIElement)
                        {
                            SectionDesignBuilder builder = new SectionDesignBuilder(section, serviceElementList, element as SectionUIElement, formElementList, detail.DataSources, msLists, _unitOfWork);
                            SectionDesign design = builder.BuildSection(ServiceDesignBuilder.GetUIElementFullPath(element, formElementList));
                            detail.Sections.Add(design);
                        }
                        else if (element is RepeaterUIElement)
                        {
                            RepeaterDesignBuilder builder = new RepeaterDesignBuilder(section, serviceElementList, element as RepeaterUIElement, formElementList, detail.DataSources, msLists, _unitOfWork);
                            RepeaterDesign design = builder.BuildRepeater(ServiceDesignBuilder.GetUIElementFullPath(element, formElementList));
                            detail.Repeaters.Add(design);
                        }
                    }
                }
            }
            return detail;
        }

        public static string GetUIElementFullPath(UIElement element, List<UIElement> formElementList)
        {
            string fullName = "";
            try
            {
                if (element != null)
                {
                    int currentElementID = element.UIElementID;
                    int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                    fullName = element.GeneratedName;
                    while (parentUIElementID > 0)
                    {
                        element = (from elem in formElementList
                                   where elem.UIElementID == parentUIElementID
                                   select elem).FirstOrDefault();
                        parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                        if (parentUIElementID > 0)
                        {
                            fullName = element.GeneratedName + "." + fullName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return fullName;
        }
    }
}
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
    public static class ServiceDefinitionRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static IQueryable<ServiceDefinition> GetServiceDefinitionListForServiceDesignVersion(this IRepositoryAsync<ServiceDefinition> serviceDefinitionRepository, int tenantId, int serviceDesignVersionID)
        {
            IList<ServiceDefinition> serviceDefinitionList = null;
            try
            {
                var allServiceDefinitionElementList = (from u in serviceDefinitionRepository
                                                            .Query()
                                                            .Include(c => c.ApplicationDataType)
                                                            .Include(c => c.UIElement)
                                                            .Include(c => c.UIElementType)
                                                            .AsNoTracking(true)
                                                            .Get()
                                                       where u.ServiceDesignVersionServiceDefinitionMaps.Where(c => c.ServiceDesignVersionID == serviceDesignVersionID).Any()
                                                       select u).ToList();

                var rootNode = allServiceDefinitionElementList.Where(c => c.ParentServiceDefinitionID == null).FirstOrDefault();

                serviceDefinitionList = allServiceDefinitionElementList;

                if (serviceDefinitionList.Count > 1)
                {
                    //get elements in hierarchical order 
                    serviceDefinitionList = (from t in rootNode.DepthFirst(n => allServiceDefinitionElementList
                                                                                .Where(c => c.ParentServiceDefinitionID == n.ServiceDefinitionID)
                                                                                .OrderBy(c => c.Sequence))
                                             select t).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
            return serviceDefinitionList.AsQueryable();
        }

        public static void DeleteServiceDefinition(this IRepositoryAsync<ServiceDefinition> repository, IUnitOfWorkAsync unitOfWork, int tenantID, int serviceDefinitionID, int serviceDesignVersionID, out string message)
        {
            message = string.Empty;

            ServiceDefinition servicedef = repository.FindById(serviceDefinitionID);

            if (servicedef != null)
            {
                Delete(repository, unitOfWork, servicedef.TenantID, servicedef, serviceDesignVersionID, out message);

                using (var scope = new TransactionScope())
                {
                    unitOfWork.Save();
                    scope.Complete();
                }
            }
        }

        private static void Delete(this IRepositoryAsync<ServiceDefinition> repository, IUnitOfWorkAsync unitOfWork, int tenantID, ServiceDefinition serviceDef, int serviceDesignVersionID, out string message)
        {
            message = string.Empty;

            //delete all the child elements
            foreach (var item in unitOfWork.Repository<ServiceDefinition>().Query().AsNoTracking(false).Filter(c => c.ParentServiceDefinitionID == serviceDef.ServiceDefinitionID).Get())
            {
                Delete(repository, unitOfWork, item.TenantID, item, serviceDesignVersionID, out message);
            }

            bool isFinalized = unitOfWork.RepositoryAsync<ServiceDesignVersion>().IsFinalized(serviceDesignVersionID);

            if (isFinalized)
                message = "Element in Finalized Service Design Version can not be deleted";
            else
            {
                List<ServiceDesignVersionServiceDefinitionMap> list = unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                                                .Query()
                                                                                .AsNoTracking(false)
                                                                                .Include(c => c.ServiceDesignVersion)
                                                                                .Filter(c => c.ServiceDefinitionID == serviceDef.ServiceDefinitionID && c.ServiceDesignVersionID != serviceDesignVersionID)
                                                                                .Get()
                                                                                .ToList();
                ServiceDesignVersionServiceDefinitionMap currentMap = unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                                            .Query()
                                                                            .AsNoTracking(false)
                                                                            .Include(c => c.ServiceDesignVersion)
                                                                            .Filter(c => c.ServiceDesignVersionID == serviceDesignVersionID && c.ServiceDefinitionID == serviceDef.ServiceDefinitionID)
                                                                            .Get()
                                                                            .FirstOrDefault();
                if (currentMap != null)
                {
                    if (list.Any(c => c.ServiceDesignVersion.IsFinalized == true))
                    {
                        //update effective date of removal 
                        //to current map effective date - 1
                        foreach (var item in list.ToList())
                        {
                            unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>().Update(item);
                        }

                        unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Delete(currentMap);
                    }
                    else
                    {
                        foreach (var item in unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                            .Query()
                                                            .AsNoTracking(false)
                                                            .Filter(c => c.ServiceDefinitionID == serviceDef.ServiceDefinitionID)
                                                            .Get())
                        {
                            unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Delete(item);
                        }
                    }

                    unitOfWork.Repository<ServiceDefinition>().Delete(serviceDef);
                }
            }
        }
        #endregion Public Methods
    }
}

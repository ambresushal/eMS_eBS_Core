using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.domain.Models;

namespace tmg.equinox.integration.domain.services
{
    public interface ITranslatorService
    {
        TranslatorRowModel GetAllProductsInTranslationQueue();
        TranslatorRowModel GetCompletedProductsInTranslationQueue();
        TranslatorRowModel GetAllRecordsFromProcessQueue(int productId);
        Facet481Model GetFacetTableEntries(string tableName, int productId, string batchId, string product);
        dynamic ReturnList(string tableName, Facet481Model facet481Model);
        IList<PluginRowModel> GetPluginList();
        IList<VersionRowModel> GetVersionList();
        IList<TranslatorProductRowModel> GetProductList(TranslatorProductRowModel prodreqmodel);
        string AddProducttoTranslate(IList<TranslatorProductRowModel> producttoTranslate);
        string UpdateProducttoTranslate(TranslatorProductRowModel producttoTranslate);
        bool DeleteProcessQueue(int Id);
        bool ExecuteTranslator(string plugin, string pluginVersion);
    }
}

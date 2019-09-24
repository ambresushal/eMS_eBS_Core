using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.domain.Models;

namespace tmg.equinox.integration.domain.services
{
    public interface ITransmitterService
    {
        TranslatorRowModel GetTransmissionQueueProducts();
        IList<TranslatorProductRowModel> GetPluginList();
        IList<TranslatorProductRowModel> GetVersionList();
        IList<TranslatorProductRowModel> GetProductList();
        string AddProductstoTransmissionQueue(IList<TranslatorProductRowModel> producttoTransmit);
        bool DeleteProcessQueue(int Id);
        bool Transmitfiles(string plugin, string pluginVersion);
    }
}

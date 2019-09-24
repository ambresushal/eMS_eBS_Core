using Force.DeepCloner;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching;

namespace tmg.equinox.ruleprocessor.formdesignmanager
{
    public class FormDesignVersionManager
    {
        private int _tenantId ;
        private int _formDesignVersionId;
        private IFormDesignService _formDesignService;


        public FormDesignVersionManager(int tenantId, int formDesignVersionId, IFormDesignService formDesignService)
        {
            this._tenantId = tenantId;
            this._formDesignVersionId = formDesignVersionId;
            this._formDesignService = formDesignService;
        }

        public FormDesignVersionDetail GetFormDesignVersion(bool readOnly)
        {
            FormDesignVersionDetail result = null;
            FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(_tenantId, _formDesignVersionId, _formDesignService);
            if (readOnly == true)
            {
                result = detail;
                //detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(_tenantId, _formDesignVersionId, _formDesignService);
            }
            else
            {
                //FormDesignDataCacheHandler formDesignCacheHandler = new FormDesignDataCacheHandler();
                //string formDesign = formDesignCacheHandler.Get(_tenantId, _formDesignVersionId, _formDesignService);
                //detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesign);
                result = detail.DeepClone();
            }
            return result;
        }
    }
}
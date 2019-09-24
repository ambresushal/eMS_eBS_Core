
namespace tmg.equinox.integration.masterlisttranslator
{
    /// <summary>
    /// Interface for Translator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITranslator 
    {   
        /// <summary>
        /// ExecuteMasterListTranslator method will get the service response in form of viewmodel and insert in facet stagging tables.
        /// </summary>
        /// <returns></returns>
        bool ExecuteMasterListTranslator();        
    }
}

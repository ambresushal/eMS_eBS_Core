
namespace tmg.equinox.integration.translator.translators
{
    /// <summary>
    /// Interface for Translator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITranslator 
    {   
        /// <summary>
        /// ExecuteTranslator method will get the service response in form of viewmodel and insert in facet stagging tables.
        /// </summary>
        /// <returns></returns>
        bool ExecuteTranslator();        
    }
}

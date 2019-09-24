
namespace tmg.equinox.integration.translator.translators
{
    public interface ITranslatorFacet481<T> : ITranslator where T : class 
    {
        /// <summary>
        /// Get the service response, will call all the respective methods like PDPD, PDDS PDBC etc.
        /// </summary>
        /// <returns></returns>
        T GetServiceResponse();

        /// <summary>
        /// Get the PDPDDetails response
        /// </summary>
        /// <returns></returns>
        T GetPDPDDetails(T model);

        /// <summary>
        /// Get the PDDSDetails response
        /// </summary>
        /// <returns></returns>
        T GetPDDSDetails(T model);

        /// <summary>
        /// Get the PDBCDetails response
        /// </summary>
        /// <returns></returns>
        T GetPDBCDetails(T model);

        /// <summary>
        /// Get the PDVCDetails response
        /// </summary>
        /// <returns></returns>
        T GetPDVCDetails(T model);

        /// <summary>
        /// Get the SEPYDetails response
        /// </summary>
        /// <returns></returns>
        T GetSEPYDetails(T model);

        /// <summary>
        /// Get the LTLTDetails response
        /// </summary>
        /// <returns></returns>
        T GetLTLTDetails(T model);

        /// <summary>
        /// Insert in respective tables
        /// </summary>
        /// <param name="insertObject"></param>
        /// <returns></returns>
        string InsertServiceResponse(T insertObject);

       
    }
}

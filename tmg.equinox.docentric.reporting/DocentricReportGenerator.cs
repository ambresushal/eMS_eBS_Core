using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docentric.Documents.Reporting;
using System.IO;
using System.Xml.Linq;
using tmg.equinox.applicationservices.viewmodels.WindwardReport;
using System.Xml;


namespace tmg.equinox.docentric.reporting
{
    public class DocentricReportGenerator
    {
        // GenerateReport
        public static string GenerateReport(ReportDataSource datasource, string reportDocumentFileName)
		{			
			using (Stream reportDocumentStream = File.Create(reportDocumentFileName))
			{
				// Open the report template file.
                using (Stream reportTemplateStream = File.OpenRead(datasource.Location))
				{
					// Generate the report document using 'DocumentGenerator'.
                    DocumentGenerator dg = null;
                    //Default data source for the rpeort 
                    foreach (var data in datasource.DataSources)
                    {
                        dg = new DocumentGenerator(data.Xml);
                        break;
                    }

                    //Setting the datasources which have been used in the report template. 
                    foreach (var data in datasource.DataSources)
                        dg.SetNamedDataSourceValue(data.DataSourceName, data.Xml);

                    DocumentGenerationResult result = dg.GenerateDocument(reportTemplateStream, reportDocumentStream, Docentric.Documents.ObjectModel.SaveOptions.Pdf);
                    
					if (result.HasErrors)
					{
						foreach (Error error in result.Errors) Console.Out.WriteLine(error.Message);
					}
				}
			}

			return reportDocumentFileName;
		}
	}

    
}

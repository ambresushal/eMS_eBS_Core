using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.integration.domain.Models
{
    public class TranslatorRowModel
    {
        public List<TranslatorProductRowModel> translatorProducts { get; set; }
        public List<VersionRowModel> versions { get; set; }
        public List<PluginRowModel> plugins { get; set; }
    }

    public class TranslatorProductRowModel
    {
        public int Id { get; set; }
        public bool IsIncluded { get; set; }
        public string Batch { get; set; }
        public int PluginId { get; set; }
        public int PluginVersionProcessorId { get; set; }
        public string Plugin { get; set; }
        public string Version { get; set; }
        public string VersionId { get; set; }
        public string Product { get; set; }
        public string ProductId { get; set; }
        public string Status { get; set; }
        public string OutPutFormat { get; set; }

        public string FolderVersionNumber { get; set; }
        public string FormInstanceName { get; set; }
        public string FolderName { get; set; }
        public string TrasmittedFilePath { get; set; }
    }

    public class PluginRowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class VersionRowModel
    {
        public int Id { get; set; }
        public int PluginId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
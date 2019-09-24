using System.Configuration;

namespace tmg.equinox.web.Salesforce
{
    public class SFTemplateSettings : ConfigurationSection
    {
        private static SFTemplateSettings settings = ConfigurationManager.GetSection("SFTemplateSettings") as SFTemplateSettings;

        public static SFTemplateSettings Settings()
        {
            return settings;
        }

        [ConfigurationProperty("AccountId", IsRequired = true)]
        public int AccountId
        {
            get { return (int)this["AccountId"]; }
            set { this["AccountId"] = value; }
        }

        [ConfigurationProperty("FolderId", IsRequired = true)]
        public int FolderId
        {
            get { return (int)this["FolderId"]; }
            set { this["FolderId"] = value; }
        }

        [ConfigurationProperty("FolderVersionId", IsRequired = true)]
        public int FolderVersionId
        {
            get { return (int)this["FolderVersionId"]; }
            set { this["FolderVersionId"] = value; }
        }

        [ConfigurationProperty("UserId", IsRequired = true)]
        public int UserId
        {
            get { return (int)this["UserId"]; }
            set { this["UserId"] = value; }
        }

        [ConfigurationProperty("UserName", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["UserName"]; }
            set { this["UserName"] = value; }
        }

        [ConfigurationProperty("MarketSegment", IsRequired = true)]
        public int MarketSegment
        {
            get { return (int)this["MarketSegment"]; }
            set { this["MarketSegment"] = value; }
        }

        [ConfigurationProperty("CategoryId", IsRequired = true)]
        public int CategoryId
        {
            get { return (int)this["CategoryId"]; }
            set { this["CategoryId"] = value; }
        }
    }
}

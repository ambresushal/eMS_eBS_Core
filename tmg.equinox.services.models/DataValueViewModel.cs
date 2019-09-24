namespace tmg.equinox.applicationservices.viewmodels
{
    public class DataValueViewModel
    {
        public long ElementID { get; set; }
        public string ElementName { get; set; }
        public long? ParentElementID { get; set; }
        public bool IsContainer { get; set; }
        public bool IsRoot { get; set; }
        public Attribute Attribute { get; set; }
        public bool IsRepeater { get; set; }
        public int RowIDInfo { get; set; }
        public string Value { get; set; }

    }

    public class Attribute
    {
        public int AttrID { get; set; }
        public string Name { get; set; }
        public string AttrType { get; set; }
        public string Cardinality { get; set; }
        public int ObjVerID { get; set; }
        public string DefaultValue { get; set; }
    }
}

namespace GMAH.Models.Models
{
    public class DatatableParam
    {
        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public SearchRequestItem search { get; set; }
    }

    public class SearchRequestItem
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }
}

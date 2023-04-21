namespace GMAH.Models.Models
{
    public class DataTableColumn
    {
        public DataTableColumn(string name, string key)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; set; }
        public string Key { get; set; }
    }
}

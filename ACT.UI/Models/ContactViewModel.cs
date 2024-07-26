namespace ACT.UI.Models
{
    public class ContactViewModel
    {
        public int? Id { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactCell { get; set; }
        public string ContactEmail { get; set; }
        public int? JobTitle { get; set; } = 0;
    }
}
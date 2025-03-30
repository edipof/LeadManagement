namespace LeadManagement.API.Models
{
    public class LeadDTO
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Suburb { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public decimal Price { get; set; }
    }
}

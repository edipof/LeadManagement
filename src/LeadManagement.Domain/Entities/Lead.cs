namespace LeadManagement.Domain.Entities
{
    public class Lead
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Suburb { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public string? JobTitle { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int? JobId { get; set; }
        private Lead() { }

        public Lead(string firstName, string lastName, string suburb, string category, string description, decimal price, string email, string phoneNumber, int? jobId)
        {
            FirstName = firstName;
            LastName = lastName;
            Suburb = suburb;
            Category = category;
            Description = description;
            Price = price;
            Email = email;
            PhoneNumber = phoneNumber;
            DateCreated = DateTime.UtcNow;
            Status = "Invited";
            JobId = jobId;
            UpdateTimestamps();
        }

        public void AcceptLead()
        {
            if (Status != "Invited")
                throw new InvalidOperationException("Only invited leads can be accepted.");

            if (Price > 500)
            {
                Price *= 0.9m; // Aplica 10% de desconto
            }

            Status = "Accepted";

            UpdateTimestamps();
        }

        public void DeclineLead()
        {
            if (Status != "Invited")
                throw new InvalidOperationException("Only invited leads can be declined.");

            Status = "Declined";
            UpdateTimestamps();
        }

        private void UpdateTimestamps()
        {
            DateUpdated = DateTime.UtcNow;
        }
    }
}

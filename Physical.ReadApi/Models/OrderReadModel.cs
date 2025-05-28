namespace Physical.ReadApi.Models
{
    public class OrderReadModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

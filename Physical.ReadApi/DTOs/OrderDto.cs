﻿namespace Physical.ReadApi.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

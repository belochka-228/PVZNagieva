using System;

namespace WPF_Nagieva.DTO
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int? PackQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public double AverageRating { get; set; }
    }
}
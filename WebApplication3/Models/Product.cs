using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication3.ValidationAttributes;

namespace WebApplication3.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }

        [NoForbiddenWords]
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100)]
        public string ProductName { get; set; }

        [PricePositive]
        public decimal Price { get; set; }

        [CategoryExists]
        public int CategoryId { get; set; }

        [EvenNumber]
        [NotMapped]
        public int? Quantity { get; set; }

        [FutureDate]
        [NotMapped]
        public DateTime? ReleaseDate { get; set; }

        
        [NotMapped]
        public string? Description { get; set; }

        public string? Image { get; set; }

        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
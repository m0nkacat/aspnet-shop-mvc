using System;
using System.Collections.Generic;

namespace WebApplication3.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

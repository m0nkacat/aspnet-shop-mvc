using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ShopDbContext _context;

        public CartController(ShopDbContext context)
        {
            _context = context;
        }

        private async Task<Cart> GetOrCreateCartAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return null;

            var userIdInt = int.Parse(userId);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userIdInt);
            if (cart == null)
            {
                cart = new Cart { UserId = userIdInt };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetOrCreateCartAsync();
            if (cart == null)
                return RedirectToAction("Login", "Account");

            var items = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.CartId)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Add(int id)
        {
            var cart = await GetOrCreateCartAsync();
            if (cart == null)
                return RedirectToAction("Login", "Account");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == id);
            if (existingItem == null)
            {
                var newItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = id,
                    Quantity = 1
                };
                _context.CartItems.Add(newItem);
            }
            else
            {
                existingItem.Quantity++;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int id)
        {
            var cart = await GetOrCreateCartAsync();
            if (cart == null)
                return RedirectToAction("Login", "Account");

            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.CartItemId == id);
            if (item != null)
            {
                item.Quantity++;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Decrease(int id)
        {
            var cart = await GetOrCreateCartAsync();
            if (cart == null)
                return RedirectToAction("Login", "Account");

            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.CartItemId == id);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            var cart = await GetOrCreateCartAsync();
            if (cart == null)
                return RedirectToAction("Login", "Account");

            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.CartItemId == id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order()
        {
            var cart = await GetOrCreateCartAsync();
            if (cart == null)
                return RedirectToAction("Login", "Account");

            var items = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.CartId)
                .ToListAsync();

            if (items.Count == 0)
            {
                TempData["Error"] = "Корзина пуста";
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                UserId = cart.UserId,
                    OrderDate = System.DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                _context.OrderItems.Add(orderItem);
            }
            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Заказ успешно оформлен!";
            return RedirectToAction("Index", "Home");
        }
    }
}
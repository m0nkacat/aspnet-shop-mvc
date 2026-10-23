using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ShopDbContext _context;
        private const int PageSize = 6;
        private readonly IMemoryCache _cache;

        public CatalogController(ShopDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, string sortOrder, int page = 1)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                products = products.Where(p => p.ProductName.Contains(searchString));


            if (categoryId.HasValue && categoryId > 0)
                products = products.Where(p => p.CategoryId == categoryId);


            switch (sortOrder)
            {
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price_asc": products = products.OrderBy(p => p.Price); break;
                case "price_desc": products = products.OrderByDescending(p => p.Price); break;
                default: products = products.OrderBy(p => p.ProductName); break;
            }

            var totalItems = await products.CountAsync();
            var items = await products.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;
            ViewBag.SortOrder = sortOrder;


            var categories = await _cache.GetOrCreateAsync("Categories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _context.Categories.ToListAsync();
            });
            ViewBag.Categories = categories;

            return View(items);
        }
    }
}
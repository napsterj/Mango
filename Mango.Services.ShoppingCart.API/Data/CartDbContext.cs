using Mango.Services.ShoppingCart.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCart.API.Data
{
	public class CartDbContext: DbContext
	{
		private readonly DbContextOptions<CartDbContext> _cartContext;

		public CartDbContext(DbContextOptions<CartDbContext> cartContext): base(cartContext)
		{
			_cartContext = cartContext;
		}		

		public DbSet<CartHeader> CartHeaders { get; set; }
		public DbSet<CartDetails> CartDetails { get; set; }
	}
}

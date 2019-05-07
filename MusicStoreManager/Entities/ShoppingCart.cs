using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicStoreManager.Models;
using MusicStoreManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Entities
{
    public class ShoppingCart
    {
        private readonly mvcMusicStoreContext _mvcMusicStoreContext;

        public ShoppingCart(mvcMusicStoreContext mvcMusicStoreContext)
        {
            _mvcMusicStoreContext = mvcMusicStoreContext;
        }

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<mvcMusicStoreContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Album album, int amount)
        {
            var shoppingCartItem =
                _mvcMusicStoreContext.ShoppingCartItems.SingleOrDefault(s => s.Album.AlbumId == album.AlbumId && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Album = album,
                    Amount = 1
                };

                _mvcMusicStoreContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }
            _mvcMusicStoreContext.SaveChanges();
        }

        public int RemoveFromCart(Album album)
        {
            var shoppingCartItem =
                _mvcMusicStoreContext.ShoppingCartItems.SingleOrDefault(s => s.Album.AlbumId == album.AlbumId && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount >1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _mvcMusicStoreContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            _mvcMusicStoreContext.SaveChanges();
            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??
                (ShoppingCartItems =
                    _mvcMusicStoreContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                    .Include(s => s.Album)
                    .ToList());                         
        }

        public void ClearCart()
        {
            var cartItems = _mvcMusicStoreContext.ShoppingCartItems.Where(cart => cart.ShoppingCartId == ShoppingCartId);
            _mvcMusicStoreContext.ShoppingCartItems.RemoveRange(cartItems);
            _mvcMusicStoreContext.SaveChanges();
        }

        public decimal GetShoppingCartTotal()
        {
            var total = _mvcMusicStoreContext.ShoppingCartItems.Where(cart => cart.ShoppingCartId == ShoppingCartId)
                .Select(s => s.Album.Price * s.Amount).Sum();
            return total;
        }
    }
}

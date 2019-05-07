using MusicStoreManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly mvcMusicStoreContext _mvcMusicStoreContext;
        private readonly ShoppingCart _shoppingCart;

        public OrderRepository(IAlbumRepository albumRepository, mvcMusicStoreContext mvcMusicStoreContext, ShoppingCart shoppingCart)
        {
            _mvcMusicStoreContext = mvcMusicStoreContext;
            _albumRepository = albumRepository;
            _shoppingCart = shoppingCart;
        }

        public void CreateOrder(Order order)
        {
            order.OrderDate = DateTime.Now;
            _mvcMusicStoreContext.Order.Add(order);

            var shoppingCartItems = _shoppingCart.ShoppingCartItems;

            foreach (var shoppingCartItem in shoppingCartItems)
            {
                var orderDetail = new OrderDetail()
                {
                    Quantity = shoppingCartItem.Amount,
                    AlbumId = shoppingCartItem.Album.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = shoppingCartItem.Album.Price
                };

                _mvcMusicStoreContext.OrderDetail.Add(orderDetail);
            }

            _mvcMusicStoreContext.SaveChanges();
        }
    }
}

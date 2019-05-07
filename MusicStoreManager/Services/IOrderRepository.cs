using MusicStoreManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Services
{
    public interface IOrderRepository 
    {
        void CreateOrder(Order order);
    }
}

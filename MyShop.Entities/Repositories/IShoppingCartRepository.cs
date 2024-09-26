using MyShop.Entities.Models;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Entities.Repositories
{


    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        int IncresseCount(ShoppingCart shoppingCart, int count);
        int DecresseCount(ShoppingCart shoppingCart, int count);
    }
}

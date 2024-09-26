using MyShop.Entities.Models;


namespace MyShop.Entities.Repositories
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateOrderStatus(int id, string OrderStatus, string PaymentStarus);
    }


}

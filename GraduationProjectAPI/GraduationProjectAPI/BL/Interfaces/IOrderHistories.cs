using GraduationProjectAPI.BL.VM;
using GraduationProjectAPI.DAL.Models;

namespace GraduationProjectAPI.BL.Interfaces
{
    public interface IOrderHistories
    {
        void Add(OrderHistory orderHistory);
        void Delete(int id);

        void Update(OrderHistory orderHistory);

        IEnumerable<OrderHistory> GetAll();
        OrderHistory GetById(int id);
        public void AddRange(IEnumerable<OrderHistory> orderHistory);
    }
}

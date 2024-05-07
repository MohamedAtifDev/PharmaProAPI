using GraduationProjectAPI.BL.Interfaces;
using GraduationProjectAPI.DAL.Database;
using GraduationProjectAPI.DAL.Models;

namespace GraduationProjectAPI.BL.Repos
{
    public class OrderHistoryRepo : IOrderHistories
    {
        private readonly DataContext db;

        public OrderHistoryRepo(DataContext db)
        {
            this.db = db;
        }
        public void Add(OrderHistory orderHistory)
        {
           this.db.orderHistories.Add(orderHistory);
            this.db.SaveChanges();
        }
        public void AddRange(IEnumerable<OrderHistory> orderHistory)
        {
            this.db.orderHistories.AddRange(orderHistory);
            this.db.SaveChanges();
        }

        public void Delete(int id)
        {
            var data = db.orderHistories.Where(a=>a.Id==id).FirstOrDefault();
            this.db.orderHistories.Remove(data);
            db.SaveChanges();
        }

        public IEnumerable<OrderHistory> GetAll()
        {
            return db.orderHistories.ToList();
        }

        public OrderHistory GetById(int id)
        {
            return this.db.orderHistories.Where(a=>a.Id==id).FirstOrDefault();
        }

        public void Update(OrderHistory orderHistory)
        {
            db.Entry(orderHistory).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
        }
    }
}

using GraduationProjectAPI.BL.Interfaces;
using GraduationProjectAPI.DAL.Database;
using GraduationProjectAPI.DAL.Models;

namespace GraduationProjectAPI.BL.Repos
{
    public class ShelfStatusRepo : IShelfStatus
    {
        private readonly DataContext db;

        public ShelfStatusRepo(DataContext db)
        {
            this.db = db;
        }
        public void AddRange(IEnumerable<ShelfNumberStatus> shelfNumbers)
        {
          this.db.AddRange(shelfNumbers);
            db.SaveChanges();
        }

        public IEnumerable<ShelfNumberStatus> GetAll()
        {
            return db.shelfNumberStatus.ToList();
        }
        public void Create(ShelfNumberStatus shelfNumberStatus)
        {
db.shelfNumberStatus.Add(shelfNumberStatus);
            db.SaveChanges();
        }

        public void RemoveRange(IEnumerable<ShelfNumberStatus> shelfNumbers)
        {
            this.db.RemoveRange(shelfNumbers);
            db.SaveChanges();
        }
    }
}

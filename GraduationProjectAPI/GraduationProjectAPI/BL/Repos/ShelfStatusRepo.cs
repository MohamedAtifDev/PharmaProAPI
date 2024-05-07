using GraduationProjectAPI.BL.Interfaces;
using GraduationProjectAPI.BL.VM;
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

        public void InsertForESP(IEnumerable<MedicineVM> medicines, string status)
        {
            db.shelfNumberStatus.RemoveRange(db.shelfNumberStatus);
            foreach (var item in medicines)
            {
                var sh = new ShelfNumberStatus
                {
                    shelfNumber = (int)item.ShelFNumber,
                    status = status
                };
                db.shelfNumberStatus.Add(sh);
            }
            db.SaveChanges();
        }
    }
}

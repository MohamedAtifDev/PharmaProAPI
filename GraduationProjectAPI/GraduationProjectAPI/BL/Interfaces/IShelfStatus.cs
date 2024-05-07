using GraduationProjectAPI.DAL.Models;

namespace GraduationProjectAPI.BL.Interfaces
{
    public interface IShelfStatus
    {
        IEnumerable<ShelfNumberStatus> GetAll();
        void AddRange(IEnumerable<ShelfNumberStatus> shelfNumbers);
        void Create(ShelfNumberStatus shelfNumberStatus);
        void RemoveRange(IEnumerable<ShelfNumberStatus> shelfNumbers);

    }
}

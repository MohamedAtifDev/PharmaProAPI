using GraduationProjectAPI.BL;
using GraduationProjectAPI.BL.Interfaces;
using GraduationProjectAPI.BL.VM;
using GraduationProjectAPI.DAL.Database;
using GraduationProjectAPI.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfStatusController : ControllerBase
    {
    
        private readonly IShelfStatus shelfStatus;

        public ShelfStatusController(IShelfStatus shelfStatus)
        {
            this.shelfStatus = shelfStatus;
        }

        [HttpGet]
        [Route("GetAll")]
        public CustomResponse<IEnumerable<ShelfNumberStatus>> GetAll()
        {

            return new CustomResponse<IEnumerable<ShelfNumberStatus>> { StatusCode = 200, Data = shelfStatus.GetAll(), Message = "data retreived successfully" };
        }


        [HttpPost]
        [Route("Create")]
        public void insert(ShelfNumberStatus shelfNumber)
        {
            shelfStatus.Create(shelfNumber);
   
        }
        [HttpPost]

        [Route("insertForESP/{status}")]
        public void insertForESP([FromBody]IEnumerable<MedicineVM> medicines,[FromRoute]string status){
            shelfStatus.RemoveRange(shelfStatus.GetAll());
        
            var data =new List<ShelfNumberStatus>();
            foreach (var item in medicines)
            {
                var sh = new ShelfNumberStatus
                {

                    shelfNumber = (int)item.ShelFNumber,
                    status = status
                };
                data.Add(sh);
            }
            shelfStatus.AddRange(data);

            }

    }
}

using AutoMapper;
using GraduationProjectAPI.BL;
using GraduationProjectAPI.BL.Interfaces;
using GraduationProjectAPI.BL.VM;
using GraduationProjectAPI.DAL.Database;
using GraduationProjectAPI.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHistoryController : ControllerBase
    {
        private readonly IShelfStatus shelfStatus;
        private readonly IOrderHistories orderHistories;
        private readonly IMapper mapper;
        private readonly IMedicine medicine;
        private readonly IPrescription prescription;
     
        public OrderHistoryController(IShelfStatus shelfStatus,IOrderHistories orderHistories,IMapper mapper,IMedicine medicine,IPrescription prescription)
        {
            this.shelfStatus = shelfStatus;
            this.orderHistories = orderHistories;
            this.mapper = mapper;
            this.medicine = medicine;
            this.prescription = prescription;
      
        }

        [HttpGet]
        [Route("GetAll")]
        public CustomResponse<IEnumerable<OrderHistoryVM>> GetAll()
        {
            var data = orderHistories.GetAll();
            var result = mapper.Map<IEnumerable<OrderHistoryVM>>(data);
            return new CustomResponse<IEnumerable<OrderHistoryVM>> { StatusCode = 200, Data = result, Message = "Data Retreived Successfully" };
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public CustomResponse<OrderHistoryVM> GetById(int id)
        {
            var data = orderHistories.GetById(id);
            if (data is not null)
            {
                var result = mapper.Map<OrderHistoryVM>(data);
                return new CustomResponse<OrderHistoryVM> { StatusCode = 200, Data = result, Message = "Data Retreived Successfully" };

            }
            else
            {
                return new CustomResponse<OrderHistoryVM> { StatusCode = 400, Data = null, Message = "data Not Found" };

            }
        }

        [HttpPost]
        [Route("create")]
        public CustomResponse<OrderHistoryVM> Create(OrderHistoryVM orderHistory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = mapper.Map<OrderHistory>(orderHistory);
                    orderHistories.Add(data);
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 200, Data = orderHistory, Message = "OrderHistory Added Successfully" };

                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                    var message = "";
               
                    message = string.Join(",", errors);
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 400, Data = null, Message = message };
                }
            }
            catch (Exception ex)
            {
                return new CustomResponse<OrderHistoryVM> { StatusCode = 500, Data = null, Message = ex.Message };

            }
        }


        [HttpPut]
        [Route("Update")]
        public CustomResponse<OrderHistoryVM> Update(OrderHistoryVM orderHistory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = mapper.Map<OrderHistory>(orderHistory);
                    orderHistories.Update(data);
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 200, Data = orderHistory, Message = "OrderHistory Updated Successfully" };

                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                    var message = string.Join(",", errors);
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 400, Data = null, Message = message };
                }
            }
            catch (Exception ex)
            {
                return new CustomResponse<OrderHistoryVM> { StatusCode = 500, Data = null, Message = ex.Message };

            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public CustomResponse<OrderHistoryVM> Delete(int id)
        {
            var data = orderHistories.GetById(id);
            var result = mapper.Map<OrderHistoryVM>(data);
            if (data is not null)
            {
                orderHistories.Delete(id);
                return new CustomResponse<OrderHistoryVM> { StatusCode = 200, Data = result, Message = "OrderHistory deleted successfully" };

            }
            else
            {
                return new CustomResponse<OrderHistoryVM> { StatusCode = 404, Data = null, Message = "OrderHistory Not Found" };

            }
        }
   
    
    [HttpPost]
    [Route("Submit/{Prescriptionid}/{pharmacistid}")]
    public CustomResponse<OrderHistoryVM> Submit(int pharmacistid,int Prescriptionid)
    {
            var pres = prescription.GetByIDWithSPecificRelatedData(Prescriptionid);
             
        
            var record = new OrderHistoryVM
            {
                PatientId = pres.PatientID,
                PharmacistId = pharmacistid,
                PrescriptionId = pres.Id
            };

            foreach (var item in pres.medicineOfPrescriptions)
            {
                var differenceInDays = 0;
                var date = DateTime.Now.Date;
                var medicineExpDate = item.Medicine.ExpirationDate.Date;
                TimeSpan difference = medicineExpDate - date;
                differenceInDays = (int)difference.TotalDays ;
                if (item.Medicine.NumberInStock == 0)
                {
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 400, Data = null, Message = $"{item.Medicine.Name} is out of Stock  " };

                }
                if(differenceInDays <= 0)
                {
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 400, Data = null, Message = $"{item.Medicine.Name} is expired " };

                }

            }

            var data = mapper.Map<OrderHistory>(record);
                orderHistories.Add(data);

            shelfStatus.RemoveRange(shelfStatus.GetAll());
            var shelfs = new List<ShelfNumberStatus>();
            foreach (var item in pres.medicineOfPrescriptions)
            {
                var shelfstatus = new ShelfNumberStatus
                {
                    shelfNumber = item.Medicine.ShelFNumber,
                    status = "Green"
                };
                shelfs.Add(shelfstatus);
              
              
            }
            shelfStatus.AddRange(shelfs);
    
            foreach (var item in pres.medicineOfPrescriptions)
            {
                medicine.decrementQuanity(item.Medicine.Id, 1);

            }
            return new CustomResponse<OrderHistoryVM> { StatusCode = 200, Data = null, Message = "Prescription  submitted successfully" };


        }

    }
}


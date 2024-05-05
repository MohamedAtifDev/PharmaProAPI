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
        private readonly IOrderHistories orderHistories;
        private readonly IMapper mapper;
        private readonly IMedicine medicine;
        private readonly IPrescription prescription;
        private readonly DataContext db;
        public OrderHistoryController(DataContext db,IOrderHistories orderHistories,IMapper mapper,IMedicine medicine,IPrescription prescription)
        {
            this.orderHistories = orderHistories;
            this.mapper = mapper;
            this.medicine = medicine;
            this.prescription = prescription;
            this.db = db;
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
        [Route("GetById/{PrescriptionId}/{PharmacistId}")]
        public CustomResponse<OrderHistoryVM> GetById(int PrescriptionId, int PharmacistId)
        {
            var data = orderHistories.GetById(PrescriptionId,PharmacistId);
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
        [Route("Delete/{PrescriptionId}/{PharmacistId}")]
        public CustomResponse<OrderHistoryVM> Delete(int PrescriptionId, int PharmacistId)
        {
            var data = orderHistories.GetById(PrescriptionId, PharmacistId);
            var result = mapper.Map<OrderHistoryVM>(data);
            if (data is not null)
            {
                orderHistories.Delete(PrescriptionId, PharmacistId);
                return new CustomResponse<OrderHistoryVM> { StatusCode = 200, Data = result, Message = "OrderHistory deleted successfully" };

            }
            else
            {
                return new CustomResponse<OrderHistoryVM> { StatusCode = 404, Data = null, Message = "OrderHistory Not Found" };

            }
        }
   
    
    [HttpPost]
    [Route("Submit")]
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
                var date = DateTime.Now;
                var medicineExpDate = item.Medicine.ExpirationDate;
                TimeSpan difference = medicineExpDate - date;
                differenceInDays = (int)difference.TotalDays ;
                if (item.Medicine.NumberInStock == 0)
                {
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 400, Data = null, Message = $"{item.Medicine.Name} is Expired " };

                }
                if(differenceInDays <= 0)
                {
                    return new CustomResponse<OrderHistoryVM> { StatusCode = 400, Data = null, Message = $"{item.Medicine.Name} is out of Stock " };

                }

            }

            var data = mapper.Map<OrderHistory>(record);
                orderHistories.Add(data);
            foreach (var item in pres.medicineOfPrescriptions)
            {
                medicine.decrementQuanity(item.Medicine.Id, 1);

            }
            db.shelfNumberStatus.RemoveRange(db.shelfNumberStatus.Select(a => a));
            db.SaveChanges();
            foreach (var item in pres.medicineOfPrescriptions)
            {
                var shelfstatus = new ShelfNumberStatus
                {
                    shelfNumber = item.Medicine.ShelFNumber,
                    status = "Green"
                };
                db.shelfNumberStatus.Add(shelfstatus);
                db.SaveChanges();
            }

            return new CustomResponse<OrderHistoryVM> { StatusCode = 200, Data = null, Message = "Prescription  submitted successfully" };


        }

    }
}


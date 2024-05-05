using AutoMapper;
using GraduationProjectAPI.BL;
using GraduationProjectAPI.BL.Interfaces;
using GraduationProjectAPI.BL.VM;
using GraduationProjectAPI.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMedicine imedicine;

        public MedicinesController(IMapper mapper,IMedicine Imedicine)
        {
            this.mapper = mapper;
            imedicine = Imedicine;
        }
        [HttpGet]
        [Route("GetAll")]
        public CustomResponse<IEnumerable<MedicineVM>> GetAll()
        {
            var data=imedicine.GetAll();
            var result=mapper.Map<IEnumerable<MedicineVM>>(data);
            return new CustomResponse<IEnumerable<MedicineVM>> { StatusCode=200, Data = result,Message="Data Retreived Successfully" };
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public CustomResponse<MedicineVM> GetById(int id)
        {
            var data = imedicine.GetByID(id);
            if(data is not null)
            {
                var result = mapper.Map<MedicineVM>(data);
                return new CustomResponse<MedicineVM> { StatusCode = 200, Data = result, Message = "Data Retreived Successfully" };

            }
            else
            {
                return new CustomResponse<MedicineVM> { StatusCode = 404, Data = null, Message = "Medicine Not Found" };

            }
        }

        [HttpPost]
        [Route("create")]
        public CustomResponse<MedicineVM> Create(MedicineVM medicine)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = mapper.Map<Medicine>(medicine);
                    imedicine.Add(data);
                    return new CustomResponse<MedicineVM> { StatusCode = 200, Data = medicine, Message = "Medicine Added Successfully" };

                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                    var message = "";
                   
                    message=string.Join(",", errors);
                    return new CustomResponse<MedicineVM> { StatusCode = 400, Data = null, Message = message };
                }
            }catch(Exception ex)
            {
                return new CustomResponse<MedicineVM> { StatusCode = 500, Data = null, Message = ex.Message };

            }
        }


        [HttpPut]
        [Route("Update")]
        public CustomResponse<MedicineVM> Update(MedicineVM medicine)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = mapper.Map<Medicine>(medicine);
                    imedicine.Update(data);
                    return new CustomResponse<MedicineVM> { StatusCode = 200, Data = medicine, Message = "Medicine Updated Successfully" };

                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                    var message = string.Join(",", errors);
                    return new CustomResponse<MedicineVM> { StatusCode = 400, Data = null, Message = message };
                }
            }
            catch (Exception ex)
            {
                return new CustomResponse<MedicineVM> { StatusCode = 500, Data = null, Message = ex.Message };

            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public CustomResponse<MedicineVM> Delete(int id)
        {
            var data = imedicine.GetByID(id);
            var result=mapper.Map<MedicineVM>(data);
            if (data is not null)
            {
                imedicine.Delete(id);
                return new CustomResponse<MedicineVM> { StatusCode = 200, Data = result, Message = "Medicine deleted successfully" };

            }
            else
            {
                return new CustomResponse<MedicineVM> { StatusCode = 404, Data = null, Message = "Medicine Not Found" };

            }
        }
        [HttpGet("GetShelfNumbers")]
        public CustomResponse<IEnumerable<int>> GetShelfNumbers([FromQuery]int[] ids)
        {
            var data = imedicine.GetShelFNumbers(ids);

            return new CustomResponse<IEnumerable<int>> { StatusCode = 200, Data = data, Message = "ShelfNumbers Retreived Successfully" };
        }

        [HttpGet("GetSoonExpiredAndSoonOutOfStoock")]
        public CustomResponse<IEnumerable<MedicineVM>> GetSoonExpiredAndSoonOutOfStoock()
        {
            var data = imedicine.GetDangerData();
            var result = mapper.Map<IEnumerable<MedicineVM>>(data);
            return new CustomResponse<IEnumerable<MedicineVM>> { StatusCode = 200, Data = result, Message = "Data Retreived Successfully" };
        }

    }
}

using System;
using System.Threading.Tasks;
using CianPlatform.Interface;
using CianPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;

namespace CianPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CianController : ControllerBase
    {
        IDataProcessingCian _processingCian;
        public CianController(IDataProcessingCian processingCian)
        {
            _processingCian = processingCian;
        }

        [HttpGet]
        public async Task<ActionResult<ResultModel>> UpdateProjects([FromQuery(Name = "project")] string projectId, [FromQuery(Name = "building")] string buildingId)
        {
            Console.WriteLine("####################");
            Console.WriteLine("START PROCESSING CIAN DATA");
            Console.WriteLine($"PROJECT: {projectId}");
            Console.WriteLine($"BUILDING {buildingId}");
            Console.WriteLine("####################");

            ResultModel resultModel = await _processingCian.DataProcessingAsync(projectId, buildingId);

            Console.WriteLine("####################");
            Console.WriteLine("PROCESSING COMPLETED CIAN DATA");
            Console.WriteLine($"BUILDING {resultModel.Count}");
            Console.WriteLine("####################");
            
            return resultModel;
        }
    }
}
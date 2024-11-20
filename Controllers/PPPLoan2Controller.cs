using Microsoft.AspNetCore.Mvc;
using SODA;
using Newtonsoft.Json.Linq;



namespace ODGAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PPPLoan2Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PPPLoan2Controller> _logger; 
        public PPPLoan2Controller(IConfiguration configuration, ILogger<PPPLoan2Controller> logger) 
        { 
            _configuration = configuration; 
            _logger = logger;
        }
       
        [HttpGet("test")]
        public IActionResult GetTestMessage()
        {
            return Ok("Hello from PPPLoan2!");
        }

        // Add more actions (methods) as needed
        [HttpGet (Name = "GetPPPLoanData")]
        public IActionResult GetLoanData()
        {
           try
           {

                var securityKey = _configuration["SodaApiKey"];
                var ppploanurl = _configuration["PPPLoan2URL"];
                var pppresourceId = _configuration["PPPLoan2ResourceId"];
                var client = new SodaClient(ppploanurl, securityKey);


                //read metadata of a dataset using the resource identifier (Socrata 4x4)
                var metadata = client.GetMetadata(pppresourceId);
        
                //a custom type can be used as long as it is JSON serializable
                var dataset =  client.GetResource<PPPLoan2>(pppresourceId);

                var allRows = dataset.GetRows();
    
                Console.WriteLine("{0} has {1} views and contains {2} rows of data.", metadata.Name, metadata.ViewsCount, dataset.GetRows().Count().ToString());

                return Ok(allRows);  
           }
           catch(Exception ex)
           {
                _logger.LogError(ex,"An error occurred while getting data");
                return StatusCode(500,"An internal server error occurred");
           }   
        }
        [HttpGet("paginated/{pageIndex}/{pageSize}")]
        public IActionResult GetPaginatedData(int pageIndex, int pageSize)
        {                             
           try
           {
                var securityKey = _configuration["SodaApiKey"];
                var ppploanurl = _configuration["PPPLoan2URL"];
                var pppresourceId = _configuration["PPPLoan2ResourceId"];
                var client = new SodaClient(ppploanurl, securityKey);
                //a custom type can be used as long as it is JSON serializable
                var resource =  client.GetResource<PPPLoan2>(pppresourceId);

                var jsondata = resource.GetRows().OrderByDescending(p => p.JobsRetained).ThenBy(p => p.CD);

                var pagData = jsondata.Skip(pageIndex * pageSize)
                                    .Take(pageSize);
                Console.WriteLine("Total Records = " + jsondata.Count().ToString());
            return Ok(pagData);
           }
           catch(Exception ex)
           {
                _logger.LogError(ex,"An error occurred while getting data");
                return StatusCode(500,"An internal server error occurred");
           }  
        }
    }
}

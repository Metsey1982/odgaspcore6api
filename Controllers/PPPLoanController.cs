using Microsoft.AspNetCore.Mvc;
using SODA.Utilities;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Castle.Components.DictionaryAdapter;
using System.Text;
using System.IO.Compression;

namespace ODGAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PPPLoanController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PPPLoanController> _logger; 
        private readonly HttpClient _httpClient;
        public PPPLoanController(IConfiguration configuration, ILogger<PPPLoanController> logger, HttpClient httpClient) 
        { 
            _configuration = configuration; 
            _logger = logger;
            _httpClient = httpClient;
        }
        private string buildFilterURL(string filter)
        {
            //filter=Uri.EscapeDataString("zip=08055,businesstype=Non-Profit Organization");
            string[] filterURI = filter.Split(',');
            string filterURL = "&";
            StringBuilder sb = new StringBuilder(filterURL);
            foreach(string u in filterURI)
            {
                if(filterURL == "&")
                    sb.Append(u);
                else
                    sb.Append("&" + u);
                         
                filterURL = sb.ToString(); 
            }      
            return filterURL;
        }
        private string buildOrderByURL(string orderby)
        {            
            string[] orderByURI = orderby.Split(',');
            string orderByURL = "";
            StringBuilder sb = new StringBuilder(orderByURL);
            sb.Append("&$order=");
            orderByURL = sb.ToString();
            foreach(string u in orderByURI)
            {
                _logger.LogInformation("-1: " + u.Substring(u.Length - 1));
                _logger.LogInformation("-2: " + u.Substring(u.Length - 2));
                if(u.Substring(u.Length - 2) == "_d")
                {
                    _logger.LogInformation("if");
                    if(orderByURL == "&$order=")
                    {
                        sb.Append(u[..^2]);
                    }
                    else
                    {
                        sb.Append("," + u[..^2]);
                    }
                    sb.Append("+DESC");

                }
                else
                {
                    _logger.LogInformation("else");
                    if(orderByURL == "&$order=")
                    {
                        _logger.LogInformation("To append: " + u[..]);
                        sb.Append(u[..]);
                    }
                    else
                    {                           
                        _logger.LogInformation("To append: " + u[..]);
                        sb.Append("," + u[..]);
                    }

                }
                orderByURL = sb.ToString();  
            } 
                return orderByURL;
        }
        [HttpGet("test")]
        public IActionResult GetTestMessage()
        {
            return Ok("Hello from MyNewController!");
        }

        // Add more actions (methods) as needed
        [HttpGet("getwithfilter/{filter}")]
		[HttpGet(Name = "GetPPPLoan")]
		public async Task<IActionResult> GetPPPLoanData(string filter)
		{
            try 
            {
                var securityKey = _configuration["SodaApiKey"];
                var ppploanurl = _configuration["PPPLoanURL"];
                var pppresourceId = _configuration["PPPLoanResourceId"];
                var urlQS = "?$limit=25&$offset=0&$order=loanrange,JobsRetained+DESC";
                var urlFilter = "";
                 if(filter.Trim() != "nofilter")
                {
                    urlFilter = buildFilterURL(filter);
                }

                var url = $"{ppploanurl}{pppresourceId}{urlQS}{urlFilter}";
                _logger.LogInformation("url is " + url);
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsondata = await response.Content.ReadAsStringAsync();

                urlQS = "?$select=count+(0)";

                url = $"{ppploanurl}{pppresourceId}" + urlQS.Replace("(", "%28").Replace(")", "%29") + urlFilter;

                _logger.LogInformation("url count: " + url);

                var countresponse = await _httpClient.GetAsync(url);

                var jsonCount = await countresponse.Content.ReadAsStringAsync();

                _logger.LogInformation(jsonCount);

                var combinedJSON = new 
                {
                    jsondata,
                    jsonCount
                };
                return Ok(combinedJSON);
           }
           catch(Exception ex)
           {
                _logger.LogError(ex,"An error occurred while getting data");
                return StatusCode(500,"An internal server error occurred" + ex.Message.ToString());
           }   
 
		}

        [HttpGet("serverpaginated/{pageSize}/{page}/{filter}/{orderby}")]
        public async Task<IActionResult> GetPPPLoanServerPaginatedData(int page, int pageSize,string filter, string orderby)
        { 
           try 
            {
                var securityKey = _configuration["SodaApiKey"];
                var ppploanurl = _configuration["PPPLoanURL"];
                var pppresourceId = _configuration["PPPLoanResourceId"];

                var urlQS = $"?$limit={pageSize}&$offset={page}"; //&$order=loanrange,JobsRetained+DESC";
                _logger.LogInformation("urlQS: " + urlQS);
                var url = $"{ppploanurl}{pppresourceId}";
                StringBuilder sb = new StringBuilder(url);
                sb.Append(urlQS);
                url = sb.ToString();
                _logger.LogInformation("url + urlQS: " + url);
                _logger.LogInformation("filter param: " + filter);
                _logger.LogInformation("orderby param: " + orderby);
                var urlFilter = "";
                if(filter.Trim() != "nofilter")
                {
                    urlFilter = buildFilterURL(filter);
                    _logger.LogInformation("urlFilter PostBuild: " + urlFilter);
                    sb.Append(urlFilter);
                }
                var urlOrderBy = "";
                if(orderby.Trim() != "noorderby")
                {
                    urlOrderBy = buildOrderByURL(orderby);
                    _logger.LogInformation("urlFilter PostBuild: " + urlOrderBy);
                    sb.Append(urlOrderBy);
                }
                url = sb.ToString();

                _logger.LogInformation("url1 is " + url);
                var response = await _httpClient.GetAsync(url);
                
                response.EnsureSuccessStatusCode();
                var jsondata = await response.Content.ReadAsStringAsync();

                urlQS = "$select=count+(0)";
                
                if(filter.Trim() != "nofilter")
                    url = $"{ppploanurl}{pppresourceId}?" + urlFilter + "&" + urlQS.Replace("(", "%28").Replace(")", "%29");
                else
                    url = $"{ppploanurl}{pppresourceId}?" + urlQS.Replace("(", "%28").Replace(")", "%29");

                _logger.LogInformation("url count: " + url);

                var countresponse = await _httpClient.GetAsync(url);

                var jsonCount = await countresponse.Content.ReadAsStringAsync();

                _logger.LogInformation(jsonCount);

                var combinedJSON = new 
                {
                    jsondata,
                    jsonCount
                };
                return Ok(combinedJSON);
           }
           catch(Exception ex)
           {
                _logger.LogError(ex,"An error occurred while getting data");
                return StatusCode(500,"An internal server error occurred" + ex.Message.ToString());
           }   
 
        }
       [HttpGet("paginated/{page}/{pageSize}")]
        public async Task<IActionResult> GetPPPLoanPaginatedData(int page, int pageSize)
        { 
            try 
            {
                var securityKey = _configuration["SodaApiKey"];
                var ppploanurl = _configuration["PPPLoanURL"];
                var pppresourceId = _configuration["PPPLoanResourceId"];
                var url = $"{ppploanurl}{pppresourceId}?$limit={pageSize}&$offset={page * pageSize}&$order=loanrange,JobsRetained+DESC";
                _logger.LogInformation("url is " + url);
                Console.Write(url);
                //var url = $"https://data.nj.gov/resource/riep-z5cp.json?$limit={pageSize}&$offset={page * pageSize}&$order=naicscode";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); 
                var jsondata = await response.Content.ReadAsStringAsync();

                return Ok(jsondata);
            }
           catch(Exception ex)
           {
                _logger.LogError(ex,"An error occurred while getting data");
                return StatusCode(500,"An internal server error occurred");
           }   
        }
    }
}

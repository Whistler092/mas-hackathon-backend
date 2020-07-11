using MAS.Hackathon.BackEnd.Common;
using MAS.Hackathon.BackEnd.HubConfig;
using MAS.Hackathon.BackEnd.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MAS.Hackathon.BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IHubContext<MainHub> _hub;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webEnvironment;
        private readonly ILogger<MainController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public MainController(IHubContext<MainHub> hub, IConfiguration configuration, IWebHostEnvironment webEnvironment, ILogger<MainController> logger, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpFactory)
        {
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _webEnvironment = webEnvironment ?? throw new ArgumentNullException(nameof(webEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClientFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
        }

        [HttpPost("start")]
        public async Task<ActionResult> Start([FromBody] ReceiveStarModel requestBase64Image)
        {
            if (string.IsNullOrWhiteSpace(requestBase64Image.Image))
                return BadRequest();

            string urlImage = string.Empty;

            try
            {
                var imagesFolder = _configuration.GetValue<string>("ImagesConfiguration:ImagesFolder");
                var imagesPath = Path.Combine(_webEnvironment.WebRootPath, imagesFolder);
                var imagesExtension = _configuration.GetValue<string>("ImagesConfiguration:ImageExtension");

                var fileName = await Helper.SaveImage(requestBase64Image.Image, imagesPath, imagesExtension);

                urlImage = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}/{imagesFolder}/{fileName}";

                return Ok(new {Message = $"Image Received {urlImage}"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"MainController-Start::Exception:{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                ReturnImage(urlImage);
            }
        }

        private async void ReturnImage(string urlImage)
        {
            if (string.IsNullOrWhiteSpace(urlImage))
                return;
            
            var contentRequest = new RequestPredictionModel{Url = urlImage};
            var predictionModel = RunHttpStarterPredictionModel(contentRequest);

            await _hub.Clients.All.SendAsync(_configuration.GetValue<string>("HubConfiguration:BroadcastDataMethod"), urlImage);
        }

        private async Task<MainModel> RunHttpStarterPredictionModel(RequestPredictionModel contentRequest)
        {
            var requestUrl = $"{_configuration.GetValue<string>("PredictionConfiguration:UrlBase")}";
            _logger.LogInformation($"Main Process Request URL: {requestUrl}");
            var content = await Helper.CallEndpointPrediction(contentRequest, requestUrl, _httpClientFactory);
            _logger.LogInformation($"RatingBatchService Response content: {content}");
            return string.IsNullOrWhiteSpace(content) ? null : JsonConvert.DeserializeObject<MainModel>(content);
        }
    }
}

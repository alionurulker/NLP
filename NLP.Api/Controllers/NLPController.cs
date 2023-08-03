using Microsoft.AspNetCore.Mvc;
using NLP.Model.RequestModel;
using NLP.Model.Utils;
using NLP.Model.ViewModel;

namespace NLP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NLPController : ControllerBase
    {
        private readonly ILogger<NLPController> _logger;

        public NLPController(ILogger<NLPController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> SearchAsync([FromBody] NLPRequestModel requestModel)
        {
            string action = "Convert Text to Numerical Values";
            try
            {
                if (requestModel != null)
                {
                    string text = DataHelper.ConvertText(requestModel.UserText);

                    NLPViewModel viewModel = new NLPViewModel();

                    viewModel.Output = text;

                    _logger.Log(LogLevel.Information, action, viewModel);

                    return Ok(viewModel);
                }

                _logger.LogError("Empty Input", requestModel);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"NLP Exception Error", requestModel);
                return BadRequest(ex);
            }
        }
    }
}
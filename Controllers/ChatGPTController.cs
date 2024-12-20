using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeSync.Core.DTO.functionalDTO;
using TimeSync.Services;

namespace TimeSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        private readonly ChatGPTService _chatGPTService;

        public ChatGPTController(ChatGPTService chatGPTService)
        {
            _chatGPTService = chatGPTService;
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatGPTRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            try
            {
                var response = await _chatGPTService.GetChatGPTResponse(request.Message);
                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
  
}

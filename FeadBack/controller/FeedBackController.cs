using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/feadback")]
public class FeedBackController : ControllerBase
{
    private FeedBackService feedBackService;
    public FeedBackController(FeedBackService feedBackService)
    {
        this.feedBackService= feedBackService;
    }
    [HttpPost]
    [Route("auto")]
    public IActionResult PostData([FromBody] FeedBackAutoDtoRequest request)
    {
        return Ok(feedBackService.createFeadBackAuto(request));
    }

    [HttpPost]
    [Route("empoloee")]
    public IActionResult PostFeedBackEmployee()
    {
        return Ok();
    }
}
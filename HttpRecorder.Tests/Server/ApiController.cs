using Microsoft.AspNetCore.Mvc;

namespace HttpRecorder.Tests.Server
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        public const string JsonUri = "json";
        public const string FormDataUri = "formdata";
        public const string BinaryUri = "binary";
        public const string StatusCodeUri = "status";

        [HttpGet(JsonUri)]
        public IActionResult GetJson([FromQuery] string name = null)
            => Ok(new SampleModel { Name = name ?? SampleModel.DefaultName });

        [HttpPost(JsonUri)]
        public IActionResult PostJson(SampleModel model)
            => Ok(model);

        [HttpPost(FormDataUri)]
        public IActionResult PostFormData([FromForm] SampleModel model)
            => Ok(model);

        [HttpGet(BinaryUri)]
        public IActionResult GetBinary()
            => PhysicalFile(typeof(ApiController).Assembly.Location, "application/octet-stream");

        [HttpGet(StatusCodeUri)]
        public IActionResult GetStatus([FromQuery] int? statusCode = 200)
            => StatusCode(statusCode.Value);
    }
}

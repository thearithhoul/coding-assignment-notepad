
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace notepad_backend.Controllers;

[ApiController]
[Route("api/v1/note")]
[Authorize]
public class NotePadController : ControllerBase
{


}
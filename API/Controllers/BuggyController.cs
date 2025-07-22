using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Errors;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("auth")]
    public IActionResult GetAuth()
    {
        throw new UnauthorizedException("Not authorized");
    }

    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        throw new NotFoundException("Resource not found");
    }

    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        throw new Exception("This is a server error");
    }

    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        throw new BadRequestException("This was not a good request");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-secret")]
    public ActionResult<string> GetSecretAdmin()
    {
        return Ok("Only admins should see this");
    }
}
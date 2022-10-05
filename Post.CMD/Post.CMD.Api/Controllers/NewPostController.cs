﻿using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.CMD.Api.Commands;
using Post.CMD.Api.DTOs;
using Post.Common.DTOs;

namespace Post.CMD.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class NewPostController : ControllerBase
{
    private readonly ILogger<NewPostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
        var id = Guid.NewGuid();
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse { Id = id, Message = "New post creation request completed successfully!" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAVE_ERROR_MESSAGE = "Error while processing the request to create a new post!";
            _logger.Log(LogLevel.Error, ex, SAVE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Id = id, Message = SAVE_ERROR_MESSAGE });
        }
    }
}

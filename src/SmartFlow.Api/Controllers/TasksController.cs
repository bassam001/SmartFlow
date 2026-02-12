using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Application.Features.Tasks.CreateTask;
using SmartFlow.Application.Features.Tasks.GetFocusTasks;
using SmartFlow.Application.Features.Tasks.GetTasks;

namespace SmartFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TasksController : ControllerBase
{ 
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTasksQuery(), ct);
        return Ok(result);
    }

    [HttpGet("focus")]
    public async Task<IActionResult> Focus([FromQuery] int take = 3, CancellationToken ct = default)
    {
        if (take < 1) take = 1;
        if (take > 10) take = 10;

        var result = await _mediator.Send(new GetFocusTasksQuery(take), ct);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskCommand cmd, CancellationToken ct)
    {
        var created = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}

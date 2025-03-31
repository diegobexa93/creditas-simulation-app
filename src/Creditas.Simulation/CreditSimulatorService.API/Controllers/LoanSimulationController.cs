using CreditSimulatorService.Application.Commands;
using CreditSimulatorService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CreditSimulatorService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanSimulationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoanSimulationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateBatch")]
        public async Task<IActionResult> CreateBatch([FromBody] CreateLoanSimulationBatchCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("GetBatchById/{id}")]
        public async Task<IActionResult> GetBatchById(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllSimulationByBatchIdQuery
            {
                BatchId = id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (result.Items.Any())
                return Ok(result);

            return NotFound();
        }

        [HttpGet("GetBatchByEmail/{email}")]
        public async Task<IActionResult> GetBatchByEmail(string email, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllSimulationByEmailQuery
            {
                Email = email,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (result.Items.Any())
                return Ok(result);

            return NotFound();
        }
    }
}

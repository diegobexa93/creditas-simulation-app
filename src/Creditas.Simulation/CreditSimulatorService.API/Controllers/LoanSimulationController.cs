using CreditSimulatorService.Application.Commands;
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

        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatch([FromBody] CreateLoanSimulationBatchCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("batch/{id}")]
        public async Task<IActionResult> GetBatchById(Guid id)
        {
            // Implementação para buscar detalhes do batch
            return Ok(new { BatchId = id });
        }
    }
}

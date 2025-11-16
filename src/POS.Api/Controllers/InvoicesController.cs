using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Commands;
using POS.Application.Handlers;

namespace POS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly CreateInvoiceHandler _handler;

        public InvoicesController(CreateInvoiceHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command)
        {
            try
            {
                var id = await _handler.HandleAsync(command);
                return CreatedAtAction(nameof(GetById), new { id }, new { invoiceId = id });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Minimal GET to satisfy CreatedAtAction target -- returns a simple placeholder.
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(new { invoiceId = id });
        }
    }
}

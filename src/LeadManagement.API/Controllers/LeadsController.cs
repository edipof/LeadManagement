using LeadManagement.Application.Interfaces;
using LeadManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LeadManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<LeadsController> _logger;

        public LeadsController(
            ILeadRepository leadRepository,
            IEmailService emailService,
            ILogger<LeadsController> logger)
        {
            _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("invited")]
        [ProducesResponseType(typeof(List<Domain.Entities.Lead>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInvitedLeads()
        {
            try
            {
                var leads = await _leadRepository.GetLeadsByStatusAsync("Invited");
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invited leads");
                return Problem(
                    title: "Internal Server Error",
                    detail: "An error occurred while retrieving invited leads",
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("accepted")]
        [ProducesResponseType(typeof(List<Domain.Entities.Lead>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAcceptedLeads()
        {
            try
            {
                var leads = await _leadRepository.GetLeadsByStatusAsync("Accepted");
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving accepted leads");
                return Problem(
                    title: "Internal Server Error",
                    detail: "An error occurred while retrieving accepted leads",
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("accept/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AcceptLead(int id)
        {
            try
            {
                var lead = await _leadRepository.GetLeadByIdAsync(id);
                if (lead == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Lead with ID {id} not found",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                if (lead.Price > 500)
                {
                    lead.Price *= 0.9m;
                }

                lead.AcceptLead();
                await _leadRepository.UpdateLeadAsync(lead);

                try
                {
                    await _emailService.SendEmailNotificationAsync(
                        "sales@test.com",
                        "Lead Accepted",
                        $"O lead {lead.Id} foi aceito.");
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "Failed to send email notification for lead {LeadId}", lead.Id);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting lead {LeadId}", id);
                return Problem(
                    title: "Internal Server Error",
                    detail: $"An error occurred while accepting lead with ID {id}",
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("decline/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeclineLead(int id)
        {
            try
            {
                var lead = await _leadRepository.GetLeadByIdAsync(id);
                if (lead == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Lead with ID {id} not found",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                lead.DeclineLead();
                await _leadRepository.UpdateLeadAsync(lead);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining lead {LeadId}", id);
                return Problem(
                    title: "Internal Server Error",
                    detail: $"An error occurred while declining lead with ID {id}",
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
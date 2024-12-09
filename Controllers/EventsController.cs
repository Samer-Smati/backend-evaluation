using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PfeProject.Data;
using PfeProject.Dtos;
using PfeProject.Models;
using PfeProject.Utils;

namespace PfeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // Create Event
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            var newEvent = new Event
            {
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ManagerId = dto.ManagerId,
                EmployeeId = dto.EmployeeId
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            try
            {
                EmailService emailService = new EmailService();
                var employee = await _context.Users.FindAsync(dto.EmployeeId);

                if (employee != null)
                {
                    try
                    {
                        var subject = "New Event Invitation";
                        var body = $"Hello {employee.FirstName},\n\nYou have been invited to a new event:\n\n" +
                                   $"Title: {newEvent.Title}\n" +
                                   $"Start Date: {newEvent.StartDate.ToShortDateString()}\n" +
                                   $"End Date: {newEvent.EndDate.ToShortDateString()}\n" +
                                   $"Created by: Manager (ID: {newEvent.ManagerId})\n\n" +
                                   "Please check your calendar for details.\n\n" +
                                   "Best regards,\nYour Team";

                        // Send email to the employee
                        await emailService.SendEmailAsync(employee.Email ?? "", subject, body);
                    }
                    catch (Exception ex)
                    {
                        // Log the error or handle it appropriately
                        Console.WriteLine($"Error sending email to employee {dto.EmployeeId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(newEvent);
        }

        // Get All Events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _context.Events
                .Include(e => e.Manager)
                .Include(e => e.Employee)
                .ToListAsync();

            var eventDtos = events.Select(e => new EventReturnDto
            {
                Id = e.Id,
                Title = e.Title,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Manager = e.Manager != null ? new EventUserDto
                {
                    Id = e.Manager?.Id??"",
                    Name = e.Manager?.FirstName+" "+e.Manager?.LastName, 
                    Email = e.Manager.Email??"" 
                } : null,
                Employee = e.Employee != null ? new EventUserDto
                {
                    Id = e.Employee?.Id??"",
                    Name = e.Employee.FirstName + " " + e.Employee.LastName,
                    Email = e.Employee.Email 
                } : null
            }).ToList();

            return Ok(eventDtos);
        }


        // Get Events by Filter
        [HttpGet("filter")]
        public async Task<IActionResult> GetEventsByFilter([FromQuery] string managerId, [FromQuery] string employeeId, [FromQuery] string title)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(managerId))
                query = query.Where(e => e.ManagerId == managerId);

            if (!string.IsNullOrEmpty(employeeId))
                query = query.Where(e => e.EmployeeId == employeeId);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(e => e.Title.Contains(title));

            var events = await query
                .Include(e => e.Manager)
                .Include(e => e.Employee)
                .ToListAsync();

            var eventDtos = events.Select(e => new EventReturnDto
            {
                Id = e.Id,
                Title = e.Title,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Manager = e.Manager != null ? new EventUserDto
                {
                    Id = e.Manager.Id,
                    Name = e.Manager.FirstName + " " + e.Manager.LastName,   
                    Email = e.Manager.Email
                } : null,
                Employee = e.Employee != null ? new EventUserDto
                {
                    Id = e.Employee?.Id ?? "",
                    Name = e.Employee.FirstName + " " + e.Employee.LastName,
                    Email = e.Employee.Email 
                } : null
            }).ToList();

            return Ok(eventDtos);
        }

        // Update Event
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(string id, [FromBody] UpdateEventDto dto)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
                return NotFound();

            existingEvent.Title = dto.Title;
            existingEvent.StartDate = dto.StartDate;
            existingEvent.EndDate = dto.EndDate;
            existingEvent.EmployeeId = dto.EmployeeId;
            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();
            return Ok(existingEvent);
        }

        // Delete Event
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
                return NotFound();

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
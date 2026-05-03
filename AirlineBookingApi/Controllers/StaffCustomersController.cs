using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/staff/customers")]
[Authorize(Roles = "Staff")]
public class StaffCustomersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public StaffCustomersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> SearchCustomers([FromQuery] string? query, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var q = (query ?? string.Empty).Trim();

        var p = page.GetValueOrDefault(1);
        var ps = pageSize.GetValueOrDefault(20);
        if (p < 1) p = 1;
        if (ps < 1) ps = 1;
        if (ps > 100) ps = 100;

        var customersQuery = _dbContext.Accounts
            .AsNoTracking()
            .Include(x => x.Role)
            .Include(x => x.Profile)
            .Where(x => x.IsActive)
            .Where(x => x.Role != null && x.Role.Name == "Customer");

        if (!string.IsNullOrWhiteSpace(q))
        {
            customersQuery = customersQuery.Where(x =>
                x.Email != null && x.Email.Contains(q)
                || (x.Profile != null && x.Profile.FullName != null && x.Profile.FullName.Contains(q))
                || (x.Profile != null && x.Profile.PhoneNumber != null && x.Profile.PhoneNumber.Contains(q)));
        }

        var customers = await customersQuery
            .OrderByDescending(x => x.Id)
            .Skip((p - 1) * ps)
            .Take(ps)
            .Select(x => new StaffCustomerLookupDto
            {
                AccountId = x.Id,
                FullName = x.Profile != null ? x.Profile.FullName : string.Empty,
                Email = x.Email,
                PhoneNumber = x.Profile != null ? x.Profile.PhoneNumber : string.Empty
            })
            .ToListAsync();

        return Ok(customers);
    }
}

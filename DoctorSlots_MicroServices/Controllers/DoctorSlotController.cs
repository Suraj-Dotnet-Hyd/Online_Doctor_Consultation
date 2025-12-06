using DoctorSlots_MicroServices.DTOs;
using DoctorSlots_MicroServices.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DoctorSlots_MicroServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSlotsController : ControllerBase
    {
        private readonly ISlotRepository _slots;

        public DoctorSlotsController(ISlotRepository slots)
        {
            _slots = slots ?? throw new ArgumentNullException(nameof(slots));
        }

        // Helper to parse incoming date string (expects yyyy-MM-dd)
        private bool TryParseDate(string? dateStr, out DateOnly date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(dateStr)) return false;
            // Try DateOnly parse (ISO format)
            if (DateOnly.TryParseExact(dateStr.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return true;

            // fallback: try general parse then convert
            if (DateTime.TryParse(dateStr.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
            {
                date = DateOnly.FromDateTime(dt);
                return true;
            }

            return false;
        }

        /// <summary>
        /// GET /api/doctors/{doctorId}/slots?date=2025-11-26
        /// Returns the slot-status dictionary for the doctor on that date.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDaySlots(int doctorId, [FromQuery] string? date)
        {
            if (!TryParseDate(date, out var d))
                return BadRequest("Invalid or missing date. Use yyyy-MM-dd.");

            var dto = await _slots.GetDaySlotsAsync(doctorId, d);
            if (dto == null) return NotFound($"No slot record found for doctorId={doctorId} on {d:yyyy-MM-dd}");

            return Ok(dto);
        }

        /// <summary>
        /// GET /api/doctors/{doctorId}/slots/range?start=2025-11-26&days=7
        /// Returns slots for a range starting at start (inclusive) for 'days' days.
        /// </summary>
        [HttpGet("range")]
        public async Task<IActionResult> GetSlotsRange(int doctorId, [FromQuery] string? start, [FromQuery] int days = 7)
        {
            if (!TryParseDate(start, out var s))
                return BadRequest("Invalid or missing start date. Use yyyy-MM-dd.");

            if (days <= 0 || days > 90) return BadRequest("Days must be between 1 and 90.");

            var res = await _slots.GetSlotsRangeAsync(doctorId, s, days);
            return Ok(res);
        }

        /// <summary>
        /// POST /api/doctors/{doctorId}/slots/book
        /// Body: { "date": "2025-11-26", "hour": 10 }
        /// Attempts to book the slot. Returns 200 OK when booked, 409 Conflict when already booked/unavailable.

        [HttpPost("book")]
        public async Task<IActionResult> BookSlot(int doctorId, [FromBody] BookSlotRequest req)
        {
            if (req == null) return BadRequest("Request body required.");

            // req.Date is already DateOnly → no parsing needed
            var d = req.Date;

            if (req.Hour < 0 || req.Hour > 23)
                return BadRequest("Hour must be between 0 and 23.");

            var ok = await _slots.BookSlotAsync(doctorId, d, req.Hour);
            if (!ok)
                return Conflict("Slot is not available for booking (either not available or already booked).");

            return Ok(new
            {
                Success = true,
                DoctorId = doctorId,
                Date = d.ToString("yyyy-MM-dd"),
                Hour = req.Hour
            });
        }


        //[HttpPost("update")]
        //public async Task<IActionResult> UpdateSlots(int doctorId, [FromBody] UpdateSlotStatusDto req, [FromQuery] bool protectBooked = true)
        //{
        //    if (req == null) return BadRequest("Request body required.");
        //    if (!TryParseDate(req.Date, out var d)) return BadRequest("Invalid date. Use yyyy-MM-dd.");
        //    if (req.SlotUpdates == null || req.SlotUpdates.Count == 0) return BadRequest("slotUpdates required.");

        //    // Validate payload hours & values
        //    foreach (var kv in req.SlotUpdates)
        //    {
        //        if (kv.Key < 0 || kv.Key > 23) return BadRequest("Slot hours must be between 0 and 23.");
        //        if (kv.Value < 0 || kv.Value > 2) return BadRequest("Slot values must be 0/1/2.");
        //    }

        //    var affected = await _slots.UpdateAvailabilityAsync(doctorId, d, req.SlotUpdates, protectBooked);
        //    return Ok(new { RowsAffected = affected });
        //}

        /// <summary>
        /// GET /api/doctors/available?date=2025-11-26&hour=9
        /// Returns list of doctorIds who are available at given date & hour (slot value == 1).
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDoctors([FromQuery] string? date, [FromQuery] int hour = -1)
        {
            if (!TryParseDate(date, out var d)) return BadRequest("Invalid or missing date. Use yyyy-MM-dd.");
            if (hour < 0 || hour > 23) return BadRequest("Hour must be between 0 and 23.");

            var list = await _slots.GetAvailableDoctorIdsAsync(d, hour);
            return Ok(list);
        }
    }
}
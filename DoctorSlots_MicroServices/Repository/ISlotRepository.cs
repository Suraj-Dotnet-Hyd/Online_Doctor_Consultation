using DoctorSlots_MicroServices.DTOs;

namespace DoctorSlots_MicroServices.Repository
{
    public interface ISlotRepository
    {
        /// <summary>Return slot-status dictionary for a given doctor + date. Key = hour (0-23), Value = 0/1/2</summary>
        Task<DaySlotStatusDto?> GetDaySlotsAsync(int doctorId, DateOnly date);

        /// <summary>Return slots for a doctor across a range of days (start inclusive, count = days)</summary>
        Task<DoctorSlotWeekDto> GetSlotsRangeAsync(int doctorId, DateOnly startDate, int days);

        /// <summary>Atomically book a slot: only succeeds when current value == 1 (available). Returns true when booking succeeded.</summary>
        Task<bool> BookSlotAsync(int doctorId, DateOnly date, int hour);

        /// <summary>Update availability for one or more hours. If protectBooked = true, booked slots (2) are not changed.</summary>
        Task<int> UpdateAvailabilityAsync(int doctorId, DateOnly date, IDictionary<int, int> updates, bool protectBooked = true);

        /// <summary>Get doctor ids available (value == 1) for given date and hour.</summary>
        Task<List<int>> GetAvailableDoctorIdsAsync(DateOnly date, int hour);
    }
}

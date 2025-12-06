// SlotRepository.cs
using DoctorSlots_MicroServices.DTOs;
using DoctorSlots_MicroServices.Models;
using DoctorSlots_MicroServices.Repository;
using Microsoft.EntityFrameworkCore;


namespace DoctorRegistration.Repository
{
    public class SlotRepository : ISlotRepository
    {
        private readonly AppDbContext _db;
        private static readonly HashSet<string> ValidSlotColumns =
            Enumerable.Range(0, 24).Select(h => $"Slot_{h:D2}").ToHashSet();

        public SlotRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // --------- Public API ---------

        public async Task<DaySlotStatusDto?> GetDaySlotsAsync(int doctorId, DateOnly date)
        {
            // Map DayOfWeek to typed query for that DbSet (no dynamic)
            var dow = date.ToDateTime(TimeOnly.MinValue).DayOfWeek;
            switch (dow)
            {
                case DayOfWeek.Monday:
                    return await GetDaySlotsTypedAsync(_db.MondaySlots, doctorId, date);
                case DayOfWeek.Tuesday:
                    return await GetDaySlotsTypedAsync(_db.TuesdaySlots, doctorId, date);
                case DayOfWeek.Wednesday:
                    return await GetDaySlotsTypedAsync(_db.WednesdaySlots, doctorId, date);
                case DayOfWeek.Thursday:
                    return await GetDaySlotsTypedAsync(_db.ThursdaySlots, doctorId, date);
                case DayOfWeek.Friday:
                    return await GetDaySlotsTypedAsync(_db.FridaySlots, doctorId, date);
                case DayOfWeek.Saturday:
                    return await GetDaySlotsTypedAsync(_db.SaturdaySlots, doctorId, date);
                case DayOfWeek.Sunday:
                    return await GetDaySlotsTypedAsync(_db.SundaySlots, doctorId, date);
                default:
                    return null;
            }
        }

        public async Task<DoctorSlotWeekDto> GetSlotsRangeAsync(int doctorId, DateOnly startDate, int days)
        {
            var result = new DoctorSlotWeekDto { DoctorId = doctorId };
            for (int i = 0; i < days; i++)
            {
                var d = startDate.AddDays(i);
                var s = await GetDaySlotsAsync(doctorId, d);
                if (s != null) result.Days.Add(s);
            }

            return result;
        }

        public async Task<bool> BookSlotAsync(int doctorId, DateOnly date, int hour)
        {
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
            var col = $"Slot_{hour:D2}";
            if (!ValidSlotColumns.Contains(col)) throw new ArgumentException("Invalid hour");

            var table = GetTableNameForDate(date);
            var sql = $"UPDATE [{table}] SET [{col}] = 2 WHERE DoctorId = @p0 AND SlotDate = @p1 AND [{col}] = 1";
            var affected = await _db.Database.ExecuteSqlRawAsync(sql, doctorId, date.ToString("yyyy-MM-dd"));
            return affected > 0;
        }

        public async Task<int> UpdateAvailabilityAsync(int doctorId, DateOnly date, IDictionary<int, int> updates, bool protectBooked = true)
        {
            if (updates == null || updates.Count == 0) return 0;
            var table = GetTableNameForDate(date);

            var setClauses = new List<string>();
            var parameters = new List<object> { doctorId, date.ToString("yyyy-MM-dd") };
            int pi = 2;
            foreach (var kv in updates)
            {
                var h = kv.Key;
                var v = kv.Value;
                if (h < 0 || h > 23) throw new ArgumentOutOfRangeException(nameof(updates));
                if (v < 0 || v > 2) throw new ArgumentOutOfRangeException(nameof(updates));
                var col = $"[{($"Slot_{h:D2}")}]";
                var paramName = $"@p{pi++}";
                setClauses.Add($"{col} = {paramName}");
                parameters.Add(v);
            }

            var whereExtra = "";
            if (protectBooked)
            {
                var checks = updates.Keys.Select(h => $"[{($"Slot_{h:D2}")}] != 2");
                whereExtra = " AND " + string.Join(" AND ", checks);
            }

            var updateSql = $"UPDATE [{table}] SET {string.Join(", ", setClauses)} WHERE DoctorId = @p0 AND SlotDate = @p1{whereExtra}";
            var affected = await _db.Database.ExecuteSqlRawAsync(updateSql, parameters.ToArray());
            return affected;
        }

        public async Task<List<int>> GetAvailableDoctorIdsAsync(DateOnly date, int hour)
        {
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException(nameof(hour));
            var col = $"Slot_{hour:D2}";
            if (!ValidSlotColumns.Contains(col)) throw new ArgumentException("Invalid hour");

            var dow = date.ToDateTime(TimeOnly.MinValue).DayOfWeek;
            switch (dow)
            {
                case DayOfWeek.Monday:
                    return await _db.MondaySlots
                        .Where(s => s.SlotDate == date && EF.Property<int>(s, col) == 1)
                        .Select(s => s.DoctorId)
                        .Distinct()
                        .ToListAsync();

                case DayOfWeek.Tuesday:
                    return await _db.TuesdaySlots
                        .Where(s => s.SlotDate == date && EF.Property<int>(s, col) == 1)
                        .Select(s => s.DoctorId)
                        .Distinct()
                        .ToListAsync();

                case DayOfWeek.Wednesday:
                    return await _db.WednesdaySlots
                        .Where(s => s.SlotDate == date && EF.Property<int>(s, col) == 1)
                        .Select(s => s.DoctorId)
                        .Distinct()
                        .ToListAsync();

                case DayOfWeek.Thursday:
                    return await _db.ThursdaySlots
                        .Where(s => s.SlotDate == date && EF.Property<int>(s, col) == 1)
                        .Select(s => s.DoctorId)
                        .Distinct()
                        .ToListAsync();

                case DayOfWeek.Friday:
                    return await _db.FridaySlots
                        .Where(s => s.SlotDate == date && EF.Property<int>(s, col) == 1)
                        .Select(s => s.DoctorId)
                        .Distinct()
                        .ToListAsync();

                case DayOfWeek.Saturday:
                    return await _db.SaturdaySlots
                        .Where(s => s.SlotDate == date && EF.Property<int>(s, col) == 1)
                        .Select(s => s.DoctorId)
                        .Distinct()
                        .ToListAsync();

                case DayOfWeek.Sunday:
                    return await _db.SundaySlots
                        .Where(s => s.SlotDate == date && EF.Property<int>(s, col) == 1)
                        .Select(s => s.DoctorId)
                        .Distinct()
                        .ToListAsync();

                default:
                    return new List<int>();
            }
        }

        // --------- Private helpers ---------

        private static string GetTableNameForDate(DateOnly date)
        {
            return date.ToDateTime(TimeOnly.MinValue).DayOfWeek switch
            {
                DayOfWeek.Monday => "MondaySlots",
                DayOfWeek.Tuesday => "TuesdaySlots",
                DayOfWeek.Wednesday => "WednesdaySlots",
                DayOfWeek.Thursday => "ThursdaySlots",
                DayOfWeek.Friday => "FridaySlots",
                DayOfWeek.Saturday => "SaturdaySlots",
                DayOfWeek.Sunday => "SundaySlots",
                _ => throw new InvalidOperationException("Invalid DayOfWeek")
            };
        }

        // typed helper that runs a typed query for a DbSet<TEntity> where TEntity : DoctorDaySlotsBase
        private async Task<DaySlotStatusDto?> GetDaySlotsTypedAsync<TEntity>(DbSet<TEntity> dbSet, int doctorId, DateOnly date)
            where TEntity : DoctorDaySlotsBase
        {
            // Note: AppDbContext configured DateOnly mapping to SQL date. EF will translate SlotDate == date.
            var row = await dbSet
                .Where(r => r.DoctorId == doctorId && r.SlotDate == date)
                .Select(r => new
                {
                    S00 = EF.Property<int>(r, "Slot_00"),
                    S01 = EF.Property<int>(r, "Slot_01"),
                    S02 = EF.Property<int>(r, "Slot_02"),
                    S03 = EF.Property<int>(r, "Slot_03"),
                    S04 = EF.Property<int>(r, "Slot_04"),
                    S05 = EF.Property<int>(r, "Slot_05"),
                    S06 = EF.Property<int>(r, "Slot_06"),
                    S07 = EF.Property<int>(r, "Slot_07"),
                    S08 = EF.Property<int>(r, "Slot_08"),
                    S09 = EF.Property<int>(r, "Slot_09"),
                    S10 = EF.Property<int>(r, "Slot_10"),
                    S11 = EF.Property<int>(r, "Slot_11"),
                    S12 = EF.Property<int>(r, "Slot_12"),
                    S13 = EF.Property<int>(r, "Slot_13"),
                    S14 = EF.Property<int>(r, "Slot_14"),
                    S15 = EF.Property<int>(r, "Slot_15"),
                    S16 = EF.Property<int>(r, "Slot_16"),
                    S17 = EF.Property<int>(r, "Slot_17"),
                    S18 = EF.Property<int>(r, "Slot_18"),
                    S19 = EF.Property<int>(r, "Slot_19"),
                    S20 = EF.Property<int>(r, "Slot_20"),
                    S21 = EF.Property<int>(r, "Slot_21"),
                    S22 = EF.Property<int>(r, "Slot_22"),
                    S23 = EF.Property<int>(r, "Slot_23")
                })
                .FirstOrDefaultAsync();

            if (row == null) return null;

            return new DaySlotStatusDto
            {
                SlotDate = date,
                Slots = new Dictionary<int, int>
                {
                    [0] = row.S00,
                    [1] = row.S01,
                    [2] = row.S02,
                    [3] = row.S03,
                    [4] = row.S04,
                    [5] = row.S05,
                    [6] = row.S06,
                    [7] = row.S07,
                    [8] = row.S08,
                    [9] = row.S09,
                    [10] = row.S10,
                    [11] = row.S11,
                    [12] = row.S12,
                    [13] = row.S13,
                    [14] = row.S14,
                    [15] = row.S15,
                    [16] = row.S16,
                    [17] = row.S17,
                    [18] = row.S18,
                    [19] = row.S19,
                    [20] = row.S20,
                    [21] = row.S21,
                    [22] = row.S22,
                    [23] = row.S23
                }
            };
        }
    }
}

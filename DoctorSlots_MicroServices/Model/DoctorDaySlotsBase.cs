using DoctorSlots_MicroServices.Models;

namespace DoctorSlots_MicroServices.Models
{
    public class DoctorDaySlotsBase
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateOnly SlotDate { get; set; }

        // Slot status:
        // 0 = not available, 1 = available, 2 = booked
        public int Slot_00 { get; set; }
        public int Slot_01 { get; set; }
        public int Slot_02 { get; set; }
        public int Slot_03 { get; set; }
        public int Slot_04 { get; set; }
        public int Slot_05 { get; set; }
        public int Slot_06 { get; set; }
        public int Slot_07 { get; set; }
        public int Slot_08 { get; set; }
        public int Slot_09 { get; set; }
        public int Slot_10 { get; set; }
        public int Slot_11 { get; set; }
        public int Slot_12 { get; set; }
        public int Slot_13 { get; set; }
        public int Slot_14 { get; set; }
        public int Slot_15 { get; set; }
        public int Slot_16 { get; set; }
        public int Slot_17 { get; set; }
        public int Slot_18 { get; set; }
        public int Slot_19 { get; set; }
        public int Slot_20 { get; set; }
        public int Slot_21 { get; set; }
        public int Slot_22 { get; set; }
        public int Slot_23 { get; set; }
    }
}
public class MondaySlots : DoctorDaySlotsBase { }
public class TuesdaySlots : DoctorDaySlotsBase { }
public class WednesdaySlots : DoctorDaySlotsBase { }
public class ThursdaySlots : DoctorDaySlotsBase { }
public class FridaySlots : DoctorDaySlotsBase { }
public class SaturdaySlots : DoctorDaySlotsBase { }
public class SundaySlots : DoctorDaySlotsBase { }


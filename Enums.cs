// ============================================================
//  Enums.cs  –  All enumerations for the Hospital Med System
// ============================================================

namespace HospitalMedSystem
{
    /// <summary>
    /// Represents the physical form a drug is delivered in.
    /// Satisfies the ENUMERATION requirement.
    /// </summary>
    public enum DrugType
    {
        Tablet,
        Injection,
        Syrup,
        Drops,
        Capsule,
        Patch,
        Inhaler
    }

    /// <summary>
    /// Represents the scheduled time at which a medication is administered.
    /// </summary>
    public enum AdministrationTime
    {
        Morning,
        Afternoon,
        Evening,
        Night,
        AsNeeded,
        WithMeals,
        BeforeMeals,
        AfterMeals
    }
}

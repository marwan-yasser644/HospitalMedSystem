// ============================================================
//  Nurse.cs  –  Inherits from Person (second derived class)
// ============================================================

using System;

namespace HospitalMedSystem
{
    /// <summary>
    /// A Nurse IS-A Person (INHERITANCE – second derived class).
    /// Demonstrates that the inheritance hierarchy has multiple leaves.
    /// </summary>
    public class Nurse : Person
    {
        // ── Private backing fields ───────────────────────────────
        private string _employeeId;
        private string _department;

        // ── Properties ───────────────────────────────────────────

        public string EmployeeId
        {
            get => _employeeId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Employee ID cannot be empty.");
                _employeeId = value.Trim();
            }
        }

        public string Department
        {
            get => _department;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Department cannot be empty.");
                _department = value.Trim();
            }
        }

        public string Shift { get; set; }  // e.g. "Day", "Night"

        // ── Constructor ──────────────────────────────────────────

        public Nurse(string employeeId, string firstName, string lastName,
                     int age, string department, string shift = "Day")
            : base(firstName, lastName, age)   // calls Person constructor
        {
            EmployeeId = employeeId;
            Department = department;
            Shift      = shift;
        }

        // ── Overridden method ─────────────────────────────────────

        public override string GetDisplayInfo()
        {
            return $"Nurse {FullName} | EmpID: {EmployeeId} | " +
                   $"Dept: {Department} | Shift: {Shift}";
        }

        public override string ToString() =>
            $"Nurse {FullName} ({EmployeeId}) – {Department}";
    }
}

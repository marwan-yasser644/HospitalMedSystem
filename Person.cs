// ============================================================
//  Person.cs  –  Abstract base class (INHERITANCE root)
// ============================================================

using System;

namespace HospitalMedSystem
{
    /// <summary>
    /// Abstract base class for all people in the system.
    /// Demonstrates INHERITANCE (Person → Patient, Nurse)
    /// and ENCAPSULATION (private fields + public properties).
    /// </summary>
    public abstract class Person
    {
        // ── Private backing fields (ENCAPSULATION) ──────────────
        private string _firstName;
        private string _lastName;
        private int    _age;

        // ── Properties with validation ───────────────────────────

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("First name cannot be empty.");
                _firstName = value.Trim();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Last name cannot be empty.");
                _lastName = value.Trim();
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                if (value < 0 || value > 150)
                    throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 0 and 150.");
                _age = value;
            }
        }

        /// <summary>Read-only computed full name.</summary>
        public string FullName => $"{FirstName} {LastName}";

        // ── Constructor ──────────────────────────────────────────

        /// <summary>
        /// Base constructor – called by derived class constructors via : base(...)
        /// </summary>
        protected Person(string firstName, string lastName, int age)
        {
            FirstName = firstName;   // uses property setter (validation runs)
            LastName  = lastName;
            Age       = age;
        }

        // ── Virtual method (can be overridden in derived classes) ─
        public virtual string GetDisplayInfo()
        {
            return $"{FullName}, Age: {Age}";
        }

        public override string ToString() => FullName;
    }
}

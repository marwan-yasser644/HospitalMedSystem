// ============================================================
//  Patient.cs  –  Inherits from Person, owns a medication list
// ============================================================

using System;
using System.Collections.Generic;

namespace HospitalMedSystem
{
    /// <summary>
    /// A Patient IS-A Person (INHERITANCE).
    /// Contains a List&lt;Medication&gt; (LIST OF OBJECTS requirement).
    /// </summary>
    public class Patient : Person
    {
        // ── Private backing fields ───────────────────────────────
        private string _patientId;
        private string _ward;

        // ── Properties ───────────────────────────────────────────

        public string PatientId
        {
            get => _patientId;
            private set   // only settable inside this class
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Patient ID cannot be empty.");
                _patientId = value.Trim();
            }
        }

        public string Ward
        {
            get => _ward;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ward cannot be empty.");
                _ward = value.Trim();
            }
        }

        public DateTime AdmissionDate { get; set; }

        /// <summary>
        /// The list of all medications assigned to this patient.
        /// Satisfies the LIST OF OBJECTS requirement.
        /// </summary>
        public List<Medication> Medications { get; private set; }

        // ── Constructor ──────────────────────────────────────────

        public Patient(string patientId, string firstName, string lastName,
                       int age, string ward, DateTime admissionDate)
            : base(firstName, lastName, age)   // calls Person constructor
        {
            PatientId     = patientId;
            Ward          = ward;
            AdmissionDate = admissionDate;
            Medications   = new List<Medication>();  // initialise empty list
        }

        // ── Methods ──────────────────────────────────────────────

        /// <summary>Add a medication to this patient's list.</summary>
        public void AddMedication(Medication medication)
        {
            if (medication == null)
                throw new ArgumentNullException(nameof(medication), "Medication cannot be null.");
            Medications.Add(medication);
        }

        /// <summary>Remove a medication by its drug name.</summary>
        public bool RemoveMedication(string drugName)
        {
            var med = Medications.Find(m =>
                m.DrugName.Equals(drugName, StringComparison.OrdinalIgnoreCase));
            if (med != null)
            {
                Medications.Remove(med);
                return true;
            }
            return false;
        }

        /// <summary>Override to include patient-specific display info.</summary>
        public override string GetDisplayInfo()
        {
            return $"ID: {PatientId} | {FullName} | Age: {Age} | " +
                   $"Ward: {Ward} | Admitted: {AdmissionDate:dd/MM/yyyy}";
        }

        // ── Serialization helpers ─────────────────────────────────

        /// <summary>
        /// Serializes the patient header (no medications) to a file line.
        /// Format:  PATIENT|Id|FirstName|LastName|Age|Ward|AdmissionDate
        /// </summary>
        public string ToFileString()
        {
            return $"PATIENT|{PatientId}|{FirstName}|{LastName}|" +
                   $"{Age}|{Ward}|{AdmissionDate:yyyy-MM-dd}";
        }

        /// <summary>Parses a PATIENT| line back into a Patient object.</summary>
        public static Patient FromFileString(string line)
        {
            string[] parts = line.Split('|');
            if (parts.Length < 7)
                throw new FormatException("Patient data line is malformed.");

            string   id        = parts[1];
            string   firstName = parts[2];
            string   lastName  = parts[3];
            int      age       = int.Parse(parts[4]);
            string   ward      = parts[5];
            DateTime admitted  = DateTime.Parse(parts[6]);

            return new Patient(id, firstName, lastName, age, ward, admitted);
        }

        public override string ToString() =>
            $"{FullName}  (ID: {PatientId}, Ward: {Ward})";
    }
}

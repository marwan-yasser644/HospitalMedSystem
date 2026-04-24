// ============================================================
//  Medication.cs  –  Represents a single prescribed medication
// ============================================================

using System;

namespace HospitalMedSystem
{
    /// <summary>
    /// Stores all details about one medication entry.
    /// Uses DrugType and AdministrationTime enums.
    /// </summary>
    public class Medication
    {
        // ── Private backing fields ───────────────────────────────
        private string _drugName;
        private string _dosage;

        // ── Properties ───────────────────────────────────────────

        public string DrugName
        {
            get => _drugName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Drug name cannot be empty.");
                _drugName = value.Trim();
            }
        }

        public string Dosage
        {
            get => _dosage;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Dosage cannot be empty.");
                _dosage = value.Trim();
            }
        }

        /// <summary>The physical form of the drug (Tablet, Injection, etc.)</summary>
        public DrugType Type { get; set; }

        /// <summary>When during the day the drug should be taken.</summary>
        public AdministrationTime TimeOfAdministration { get; set; }

        /// <summary>Optional instructions (e.g. "Take with food").</summary>
        public string Notes { get; set; }

        // ── Constructor ──────────────────────────────────────────

        public Medication(string drugName, string dosage,
                          DrugType type, AdministrationTime timeOfAdministration,
                          string notes = "")
        {
            DrugName             = drugName;
            Dosage               = dosage;
            Type                 = type;
            TimeOfAdministration = timeOfAdministration;
            Notes                = notes ?? string.Empty;
        }

        // ── Serialization helpers (used by HospitalDataService) ──

        /// <summary>
        /// Converts the medication to a pipe-delimited string for file storage.
        /// Format:  DrugName|Dosage|Type|TimeOfAdministration|Notes
        /// </summary>
        public string ToFileString()
        {
            return $"{DrugName}|{Dosage}|{(int)Type}|{(int)TimeOfAdministration}|{Notes}";
        }

        /// <summary>
        /// Parses a pipe-delimited line back into a Medication object.
        /// </summary>
        public static Medication FromFileString(string line)
        {
            string[] parts = line.Split('|');
            if (parts.Length < 4)
                throw new FormatException("Medication data is malformed.");

            string name    = parts[0];
            string dosage  = parts[1];
            var    type    = (DrugType)int.Parse(parts[2]);
            var    time    = (AdministrationTime)int.Parse(parts[3]);
            string notes   = parts.Length >= 5 ? parts[4] : string.Empty;

            return new Medication(name, dosage, type, time, notes);
        }

        public override string ToString()
        {
            return $"{DrugName} ({Dosage}) – {Type} – {TimeOfAdministration}";
        }
    }
}

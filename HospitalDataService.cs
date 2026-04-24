// ============================================================
//  HospitalDataService.cs  –  File handling & business logic
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace HospitalMedSystem
{
    /// <summary>
    /// Static service class that:
    ///  1. Manages the in-memory List&lt;Patient&gt; (the master patient store).
    ///  2. Provides Save / Load to a plain-text file (FILE HANDLING requirement).
    ///  3. Provides Search logic.
    /// All risky operations use try/catch (EXCEPTION HANDLING requirement).
    /// </summary>
    public static class HospitalDataService
    {
        // ── In-memory store (LIST OF OBJECTS) ────────────────────
        private static List<Patient> _patients = new List<Patient>();

        /// <summary>Read-only access to the patient collection.</summary>
        public static IReadOnlyList<Patient> Patients => _patients.AsReadOnly();

        // ── Default data-file path ────────────────────────────────
        private static readonly string DataFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patients.dat");

        // ── Patient CRUD ──────────────────────────────────────────

        /// <summary>Adds a new patient. Throws if duplicate ID exists.</summary>
        public static void AddPatient(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            // Check for duplicate ID
            if (_patients.Exists(p =>
                    p.PatientId.Equals(patient.PatientId,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"A patient with ID '{patient.PatientId}' already exists.");
            }

            _patients.Add(patient);
        }

        /// <summary>Removes a patient by ID. Returns true if removed.</summary>
        public static bool RemovePatient(string patientId)
        {
            var p = FindPatientById(patientId);
            if (p != null)
            {
                _patients.Remove(p);
                return true;
            }
            return false;
        }

        // ── Search ────────────────────────────────────────────────

        /// <summary>
        /// Returns all patients whose full name contains the search term
        /// (case-insensitive, partial match).
        /// </summary>
        public static List<Patient> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Search term cannot be empty.");

            string term = name.Trim().ToLower();

            return _patients.FindAll(p =>
                p.FullName.ToLower().Contains(term) ||
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term));
        }

        /// <summary>Finds a patient by exact ID (case-insensitive).</summary>
        public static Patient FindPatientById(string id)
        {
            return _patients.Find(p =>
                p.PatientId.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        // ── File Save ─────────────────────────────────────────────

        /// <summary>
        /// Saves all patients and their medications to a plain-text file.
        /// FILE HANDLING + EXCEPTION HANDLING demonstrated here.
        /// 
        /// File format:
        ///   PATIENT|id|first|last|age|ward|date
        ///   MED|drugName|dosage|typeInt|timeInt|notes
        ///   MED|...
        ///   PATIENT|...
        ///   ...
        /// </summary>
        public static void SaveToFile(string filePath = null)
        {
            filePath ??= DataFilePath;   // use default if not specified

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, append: false))
                {
                    foreach (Patient patient in _patients)
                    {
                        // Write patient header line
                        writer.WriteLine(patient.ToFileString());

                        // Write each medication on its own line
                        foreach (Medication med in patient.Medications)
                        {
                            writer.WriteLine($"MED|{med.ToFileString()}");
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException(
                    $"Permission denied writing to '{filePath}'.", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new IOException(
                    $"Directory not found for path '{filePath}'.", ex);
            }
            // Other IOExceptions propagate as-is so the UI can display them.
        }

        // ── File Load ─────────────────────────────────────────────

        /// <summary>
        /// Loads patients from the data file into the in-memory list.
        /// Called once when the application starts.
        /// FILE HANDLING + EXCEPTION HANDLING demonstrated here.
        /// </summary>
        public static void LoadFromFile(string filePath = null)
        {
            filePath ??= DataFilePath;

            _patients.Clear();

            // If no file exists yet, start with empty list (first run)
            if (!File.Exists(filePath))
                return;

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                Patient  currentPatient = null;

                foreach (string rawLine in lines)
                {
                    string line = rawLine.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.StartsWith("PATIENT|"))
                    {
                        // Parse patient header
                        currentPatient = Patient.FromFileString(line);
                        _patients.Add(currentPatient);
                    }
                    else if (line.StartsWith("MED|") && currentPatient != null)
                    {
                        // Strip the "MED|" prefix and parse the medication
                        string medData = line.Substring(4);
                        Medication med = Medication.FromFileString(medData);
                        currentPatient.AddMedication(med);
                    }
                }
            }
            catch (FormatException ex)
            {
                // Data file is corrupted; start fresh and warn caller
                _patients.Clear();
                throw new IOException(
                    "Data file appears corrupted and has been reset. " +
                    "Details: " + ex.Message, ex);
            }
        }

        // ── Sample Data (first-run convenience) ───────────────────

        /// <summary>
        /// Populates the system with demo data so the student can see
        /// the app working immediately without manually entering records.
        /// </summary>
        public static void LoadSampleData()
        {
            _patients.Clear();

            var p1 = new Patient("P001", "Ahmed",  "Hassan",  45,
                                 "Cardiology", new DateTime(2025, 3, 10));
            p1.AddMedication(new Medication("Aspirin",    "100 mg", DrugType.Tablet,
                             AdministrationTime.Morning,  "Take with water"));
            p1.AddMedication(new Medication("Lisinopril", "10 mg",  DrugType.Tablet,
                             AdministrationTime.Evening,  "Monitor blood pressure"));
            p1.AddMedication(new Medication("Atorvastatin","20 mg", DrugType.Tablet,
                             AdministrationTime.Night,    "Take at bedtime"));

            var p2 = new Patient("P002", "Fatima", "Ali",     32,
                                 "Pediatrics", new DateTime(2025, 4, 1));
            p2.AddMedication(new Medication("Amoxicillin","250 mg/5ml", DrugType.Syrup,
                             AdministrationTime.WithMeals,"Shake well before use"));
            p2.AddMedication(new Medication("Paracetamol","500 mg",    DrugType.Tablet,
                             AdministrationTime.AsNeeded, "Max 4 times daily"));

            var p3 = new Patient("P003", "Omar",   "Khalil",  60,
                                 "Orthopedics", new DateTime(2025, 2, 20));
            p3.AddMedication(new Medication("Morphine",  "10 mg",  DrugType.Injection,
                             AdministrationTime.AsNeeded, "PRN for severe pain"));
            p3.AddMedication(new Medication("Ibuprofen", "400 mg", DrugType.Tablet,
                             AdministrationTime.AfterMeals, "Do not exceed 1200mg/day"));

            var p4 = new Patient("P004", "Sara",   "Mostafa", 28,
                                 "General",   new DateTime(2025, 4, 15));
            p4.AddMedication(new Medication("Insulin Glargine", "20 units",
                             DrugType.Injection, AdministrationTime.Night,
                             "Inject into abdomen"));
            p4.AddMedication(new Medication("Metformin", "500 mg", DrugType.Tablet,
                             AdministrationTime.WithMeals, "Take with meals"));

            _patients.AddRange(new[] { p1, p2, p3, p4 });
        }

        /// <summary>Returns how many patients are currently loaded.</summary>
        public static int PatientCount => _patients.Count;
    }
}



using System;
using System.Collections.Generic;
using System.IO;

namespace HospitalMedSystem
{
    
    public static class HospitalDataService
    {

        private static List<Patient> _patients = new List<Patient>();


        public static IReadOnlyList<Patient> Patients => _patients.AsReadOnly();


        private static readonly string DataFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patients.dat");

        
        public static void AddPatient(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));


            if (_patients.Exists(p =>
                    p.PatientId.Equals(patient.PatientId,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"A patient with ID '{patient.PatientId}' already exists.");
            }

            _patients.Add(patient);
        }


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


        public static Patient FindPatientById(string id)
        {
            return _patients.Find(p =>
                p.PatientId.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

      

        public static void SaveToFile(string filePath = null)
        {
            filePath ??= DataFilePath;  


            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, append: false))
                {
                    foreach (Patient patient in _patients)
                    {

                        writer.WriteLine(patient.ToFileString());


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
            
        }

        
        public static void LoadFromFile(string filePath = null)
        {
            filePath ??= DataFilePath;

            _patients.Clear();

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

                        currentPatient = Patient.FromFileString(line);
                        _patients.Add(currentPatient);
                    }
                    else if (line.StartsWith("MED|") && currentPatient != null)
                    {

                        string medData = line.Substring(4);
                        Medication med = Medication.FromFileString(medData);
                        currentPatient.AddMedication(med);
                    }
                }
            }
            catch (FormatException ex)
            {

                _patients.Clear();
                throw new IOException(
                    "Data file appears corrupted and has been reset. " +
                    "Details: " + ex.Message, ex);
            }
        }

     

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


        public static int PatientCount => _patients.Count;
    }
}


using System;
using System.Collections.Generic;

namespace HospitalMedSystem
{


    public class Patient : Person
    {
        private string _patientId;
        private string _ward;


        public string PatientId
        {
            get => _patientId;
            private set   
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



        public List<Medication> Medications { get; private set; }


        public Patient(string patientId, string firstName, string lastName,
                       int age, string ward, DateTime admissionDate)
            : base(firstName, lastName, age)   
        {
            PatientId     = patientId;
            Ward          = ward;
            AdmissionDate = admissionDate;
            Medications   = new List<Medication>();  
        }


        
        public void AddMedication(Medication medication)
        {
            if (medication == null)
                throw new ArgumentNullException(nameof(medication), "Medication cannot be null.");
            Medications.Add(medication);
        }

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

        public override string GetDisplayInfo()
        {
            return $"ID: {PatientId} | {FullName} | Age: {Age} | " +
                   $"Ward: {Ward} | Admitted: {AdmissionDate:dd/MM/yyyy}";
        }



        public string ToFileString()
        {
            return $"PATIENT|{PatientId}|{FirstName}|{LastName}|" +
                   $"{Age}|{Ward}|{AdmissionDate:yyyy-MM-dd}";
        }

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

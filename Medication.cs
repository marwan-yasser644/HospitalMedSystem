


using System;

namespace HospitalMedSystem
{


    public class Medication
    {

        private string _drugName;
        private string _dosage;



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


        public DrugType Type { get; set; }


        public AdministrationTime TimeOfAdministration { get; set; }


        public string Notes { get; set; }



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

      

        public string ToFileString()
        {
            return $"{DrugName}|{Dosage}|{(int)Type}|{(int)TimeOfAdministration}|{Notes}";
        }



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

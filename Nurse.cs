


using System;

namespace HospitalMedSystem
{


    public class Nurse : Person
    {

        private string _employeeId;
        private string _department;



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

        public string Shift { get; set; }  




        public Nurse(string employeeId, string firstName, string lastName,
                     int age, string department, string shift = "Day")
            : base(firstName, lastName, age)   
        {
            EmployeeId = employeeId;
            Department = department;
            Shift      = shift;
        }


        public override string GetDisplayInfo()
        {
            return $"Nurse {FullName} | EmpID: {EmployeeId} | " +
                   $"Dept: {Department} | Shift: {Shift}";
        }

        public override string ToString() =>
            $"Nurse {FullName} ({EmployeeId}) – {Department}";
    }
}

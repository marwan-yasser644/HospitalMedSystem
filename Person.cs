


using System;

namespace HospitalMedSystem
{


    public abstract class Person
    {

        private string _firstName;
        private string _lastName;
        private int    _age;



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


        public string FullName => $"{FirstName} {LastName}";

      

        protected Person(string firstName, string lastName, int age)
        {
            FirstName = firstName;   
            LastName  = lastName;
            Age       = age;
        }


        public virtual string GetDisplayInfo()
        {
            return $"{FullName}, Age: {Age}";
        }

        public override string ToString() => FullName;
    }
}

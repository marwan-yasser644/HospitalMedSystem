using System;
using System.IO;
using HospitalMedSystem.Models;

namespace HospitalMedSystem.Data
{
    public class UserRepository
    {
        private static User[] _users = new User[100]; 
        private static int _count = 0;

        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.dat");

        public void Load()
        {
            _count = 0;

            if (!File.Exists(FilePath))
                return;

            var lines = File.ReadAllLines(FilePath);

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length == 3)
                {
                    _users[_count] = new User
                    {
                        Username = parts[0],
                        PasswordHash = parts[1],
                        Role = parts[2]
                    };

                    _count++;
                }
            }
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(FilePath, false))
            {
                for (int i = 0; i < _count; i++)
                {
                    writer.WriteLine($"{_users[i].Username}|{_users[i].PasswordHash}|{_users[i].Role}");
                }
            }
        }

        public void Add(User user)
        {
            if (_count >= _users.Length)
                throw new Exception("Array is full");

            for (int i = 0; i < _count; i++)
            {
                if (_users[i].Username == user.Username)
                    throw new Exception("User already exists");
            }

            _users[_count] = user;
            _count++;

            Save();
        }

        public User Get(string username, string passwordHash)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_users[i].Username == username &&
                    _users[i].PasswordHash == passwordHash)
                {
                    return _users[i];
                }
            }

            return null;
        }
    }
}

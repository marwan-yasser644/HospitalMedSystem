using System;
using System.Collections.Generic;
using System.IO;
using HospitalMedSystem.Models;

namespace HospitalMedSystem.Services
{
    public class UserRepository
    {
        private static List<User> _users = new List<User>();

        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.dat");

        public void Load()
        {
            _users.Clear();

            if (!File.Exists(FilePath))
                return;

            var lines = File.ReadAllLines(FilePath);

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts.Length == 3)
                {
                    _users.Add(new User
                    {
                        Username = parts[0],
                        PasswordHash = parts[1],
                        Role = parts[2]
                    });
                }
            }
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(FilePath, false))
            {
                foreach (var user in _users)
                {
                    writer.WriteLine($"{user.Username}|{user.PasswordHash}|{user.Role}");
                }
            }
        }

        public void Add(User user)
        {
            if (_users.Exists(u => u.Username == user.Username))
                throw new Exception("User already exists");

            _users.Add(user);
            Save();
        }

        
        public User Get(string username, string passwordHash)
        {
            return _users.Find(u =>
                u.Username == username &&
                u.PasswordHash == passwordHash);
        }
    }
}
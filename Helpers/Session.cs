using HospitalMedSystem.Models;

namespace HospitalMedSystem.Helpers
{
    public static class Session
    {
        public static User CurrentUser { get; set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
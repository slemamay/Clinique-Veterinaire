using CliniqueVet.Models;

namespace CliniqueVet
{
    public static class Session
    {
        public static Utilisateur CurrentUser { get; set; }

        public static bool IsAdmin
        {
            get { return CurrentUser != null && CurrentUser.Role == "Administrateur"; }
        }

        public static bool IsSecretaire
        {
            get { return CurrentUser != null && CurrentUser.Role == "Secretaire"; }
        }

        public static bool IsVeterinaire
        {
            get { return CurrentUser != null && CurrentUser.Role == "Veterinaire"; }
        }
    }
}

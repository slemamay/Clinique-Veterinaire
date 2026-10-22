using System;

namespace CliniqueVet.Models
{
    public class Consultation
    {
        public int IdConsult { get; set; }
        public DateTime DateConsult { get; set; }
        public string Diagnostic { get; set; }
        public int IdAnimal { get; set; }
        public string NomAnimal { get; set; }
        public string NomProprietaire { get; set; }
    }

    public class Facture
    {
        public int IdFacture { get; set; }
        public DateTime DateFacture { get; set; }
        public decimal Montant { get; set; }
        public int IdConsult { get; set; }
        public string NomAnimal { get; set; }
        public string NomProprietaire { get; set; }
    }

    public class RDV
    {
        public int IdRdv { get; set; }
        public DateTime DateRdv { get; set; }
        public TimeSpan HeureRdv { get; set; }
        public string Motif { get; set; }
        public int IdAnimal { get; set; }
        public string NomAnimal { get; set; }
        public string NomProprietaire { get; set; }
    }

    public class Medicament
    {
        public int IdMedicament { get; set; }
        public string Nom { get; set; }
        public int Quantite { get; set; }
        public DateTime? DateExpiration { get; set; }

        public override string ToString()
        {
            return Nom;
        }
    }

    public class Vaccination
    {
        public int IdVaccin { get; set; }
        public string Nom { get; set; }
        public DateTime DateVaccin { get; set; }
        public DateTime? DateRappel { get; set; }
        public int IdAnimal { get; set; }
        public string NomAnimal { get; set; }
    }

    public class Utilisateur
    {
        public int IdUser { get; set; }
        public string CIN { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public string NomComplet
        {
            get { return Prenom + " " + Nom; }
        }
    }
}

namespace CliniqueVet.Models
{
    public class Animal
    {
        public int IdAnimal { get; set; }
        public string Nom { get; set; }
        public string Espece { get; set; }
        public int IdProp { get; set; }
        public string NomProprietaire { get; set; }

        public override string ToString()
        {
            return Nom + " (" + Espece + ")";
        }
    }
}

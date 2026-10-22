namespace CliniqueVet.Models
{
    public class Proprietaire
    {
        public int IdProp { get; set; }
        public string CIN { get; set; }
        public string Nom { get; set; }

        public override string ToString()
        {
            return Nom;
        }
    }
}

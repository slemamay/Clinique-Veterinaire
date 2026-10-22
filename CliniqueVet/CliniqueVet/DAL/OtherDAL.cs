using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CliniqueVet.Models;

namespace CliniqueVet.DAL
{
    public class FactureDAL
    {
        public List<Facture> GetAll()
        {
            var list = new List<Facture>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT f.id_facture, f.date_facture, f.montant, f.id_consult,
                                      a.nom AS nom_animal, p.nom AS nom_prop
                               FROM Facture f
                               JOIN Consultation c ON f.id_consult = c.id_consult
                               JOIN Animal a ON c.id_animal = a.id_animal
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               ORDER BY f.date_facture DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Facture
                        {
                            IdFacture = reader.GetInt32(0),
                            DateFacture = reader.GetDateTime(1),
                            Montant = reader.GetDecimal(2),
                            IdConsult = reader.GetInt32(3),
                            NomAnimal = reader.GetString(4),
                            NomProprietaire = reader.GetString(5)
                        });
                    }
                }
            }
            return list;
        }

        public void Insert(Facture f)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Facture (date_facture, montant, id_consult) VALUES (@date, @montant, @idconsult)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", f.DateFacture);
                    cmd.Parameters.AddWithValue("@montant", f.Montant);
                    cmd.Parameters.AddWithValue("@idconsult", f.IdConsult);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Facture f)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Facture SET date_facture=@date, montant=@montant WHERE id_facture=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", f.DateFacture);
                    cmd.Parameters.AddWithValue("@montant", f.Montant);
                    cmd.Parameters.AddWithValue("@id", f.IdFacture);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Facture WHERE id_facture=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetRevenuTotal()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ISNULL(SUM(montant), 0) FROM Facture";
                using (var cmd = new SqlCommand(sql, conn))
                    return (decimal)cmd.ExecuteScalar();
            }
        }

        public decimal GetRevenuMois()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ISNULL(SUM(montant), 0) FROM Facture WHERE MONTH(date_facture)=MONTH(GETDATE()) AND YEAR(date_facture)=YEAR(GETDATE())";
                using (var cmd = new SqlCommand(sql, conn))
                    return (decimal)cmd.ExecuteScalar();
            }
        }

        public bool ExistsForConsultation(int idConsult)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Facture WHERE id_consult=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idConsult);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }
    }

    public class RDVDAL
    {
        public List<RDV> GetAll()
        {
            var list = new List<RDV>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT r.id_rdv, r.date_rdv, r.heure_rdv, r.motif, r.id_animal,
                                      a.nom AS nom_animal, p.nom AS nom_prop
                               FROM RDV r
                               JOIN Animal a ON r.id_animal = a.id_animal
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               ORDER BY r.date_rdv, r.heure_rdv";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RDV
                        {
                            IdRdv = reader.GetInt32(0),
                            DateRdv = reader.GetDateTime(1),
                            HeureRdv = reader.GetTimeSpan(2),
                            Motif = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            IdAnimal = reader.GetInt32(4),
                            NomAnimal = reader.GetString(5),
                            NomProprietaire = reader.GetString(6)
                        });
                    }
                }
            }
            return list;
        }

        public List<RDV> GetUpcoming()
        {
            var list = new List<RDV>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT r.id_rdv, r.date_rdv, r.heure_rdv, r.motif, r.id_animal,
                                      a.nom AS nom_animal, p.nom AS nom_prop
                               FROM RDV r
                               JOIN Animal a ON r.id_animal = a.id_animal
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               WHERE r.date_rdv >= CAST(GETDATE() AS DATE)
                               ORDER BY r.date_rdv, r.heure_rdv";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RDV
                        {
                            IdRdv = reader.GetInt32(0),
                            DateRdv = reader.GetDateTime(1),
                            HeureRdv = reader.GetTimeSpan(2),
                            Motif = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            IdAnimal = reader.GetInt32(4),
                            NomAnimal = reader.GetString(5),
                            NomProprietaire = reader.GetString(6)
                        });
                    }
                }
            }
            return list;
        }

        public int GetCountToday()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM RDV WHERE date_rdv = CAST(GETDATE() AS DATE)";
                using (var cmd = new SqlCommand(sql, conn))
                    return (int)cmd.ExecuteScalar();
            }
        }

        public void Insert(RDV r)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO RDV (date_rdv, heure_rdv, motif, id_animal) VALUES (@date, @heure, @motif, @idanimal)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", r.DateRdv);
                    cmd.Parameters.AddWithValue("@heure", r.HeureRdv);
                    cmd.Parameters.AddWithValue("@motif", (object)r.Motif ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@idanimal", r.IdAnimal);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(RDV r)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE RDV SET date_rdv=@date, heure_rdv=@heure, motif=@motif, id_animal=@idanimal WHERE id_rdv=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", r.DateRdv);
                    cmd.Parameters.AddWithValue("@heure", r.HeureRdv);
                    cmd.Parameters.AddWithValue("@motif", (object)r.Motif ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@idanimal", r.IdAnimal);
                    cmd.Parameters.AddWithValue("@id", r.IdRdv);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM RDV WHERE id_rdv=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class MedicamentDAL
    {
        public List<Medicament> GetAll()
        {
            var list = new List<Medicament>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id_medicament, nom, quantite, date_expiration FROM Medicament ORDER BY nom";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Medicament
                        {
                            IdMedicament = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Quantite = reader.GetInt32(2),
                            DateExpiration = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3)
                        });
                    }
                }
            }
            return list;
        }

        public void Insert(Medicament m)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Medicament (nom, quantite, date_expiration) VALUES (@nom, @qte, @exp)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", m.Nom);
                    cmd.Parameters.AddWithValue("@qte", m.Quantite);
                    cmd.Parameters.AddWithValue("@exp", (object)m.DateExpiration ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Medicament m)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Medicament SET nom=@nom, quantite=@qte, date_expiration=@exp WHERE id_medicament=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", m.Nom);
                    cmd.Parameters.AddWithValue("@qte", m.Quantite);
                    cmd.Parameters.AddWithValue("@exp", (object)m.DateExpiration ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", m.IdMedicament);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Medicament WHERE id_medicament=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class VaccinationDAL
    {
        public List<Vaccination> GetAll()
        {
            var list = new List<Vaccination>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT v.id_vaccin, v.nom, v.date_vaccin, v.date_rappel, v.id_animal, a.nom AS nom_animal
                               FROM Vaccination v
                               JOIN Animal a ON v.id_animal = a.id_animal
                               ORDER BY v.date_vaccin DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Vaccination
                        {
                            IdVaccin = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            DateVaccin = reader.GetDateTime(2),
                            DateRappel = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            IdAnimal = reader.GetInt32(4),
                            NomAnimal = reader.GetString(5)
                        });
                    }
                }
            }
            return list;
        }

        public List<Vaccination> GetByAnimal(int idAnimal)
        {
            var list = new List<Vaccination>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT v.id_vaccin, v.nom, v.date_vaccin, v.date_rappel, v.id_animal, a.nom AS nom_animal
                               FROM Vaccination v
                               JOIN Animal a ON v.id_animal = a.id_animal
                               WHERE v.id_animal = @id ORDER BY v.date_vaccin DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idAnimal);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Vaccination
                            {
                                IdVaccin = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                DateVaccin = reader.GetDateTime(2),
                                DateRappel = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                IdAnimal = reader.GetInt32(4),
                                NomAnimal = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(Vaccination v)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Vaccination (nom, date_vaccin, date_rappel, id_animal) VALUES (@nom, @date, @rappel, @idanimal)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", v.Nom);
                    cmd.Parameters.AddWithValue("@date", v.DateVaccin);
                    cmd.Parameters.AddWithValue("@rappel", (object)v.DateRappel ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@idanimal", v.IdAnimal);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Vaccination v)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Vaccination SET nom=@nom, date_vaccin=@date, date_rappel=@rappel, id_animal=@idanimal WHERE id_vaccin=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", v.Nom);
                    cmd.Parameters.AddWithValue("@date", v.DateVaccin);
                    cmd.Parameters.AddWithValue("@rappel", (object)v.DateRappel ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@idanimal", v.IdAnimal);
                    cmd.Parameters.AddWithValue("@id", v.IdVaccin);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Vaccination WHERE id_vaccin=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Vaccination> GetUpcomingRappels(int days = 30)
        {
            var list = new List<Vaccination>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT v.id_vaccin, v.nom, v.date_vaccin, v.date_rappel, v.id_animal, a.nom AS nom_animal
                               FROM Vaccination v
                               JOIN Animal a ON v.id_animal = a.id_animal
                               WHERE v.date_rappel IS NOT NULL
                               AND v.date_rappel BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, @days, CAST(GETDATE() AS DATE))
                               ORDER BY v.date_rappel";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@days", days);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Vaccination
                            {
                                IdVaccin = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                DateVaccin = reader.GetDateTime(2),
                                DateRappel = reader.GetDateTime(3),
                                IdAnimal = reader.GetInt32(4),
                                NomAnimal = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return list;
        }
    }

    public class UtilisateurDAL
    {
        public Utilisateur Authenticate(string email, string password)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id_user, CIN, nom, prenom, email, password, role FROM Utilisateur WHERE email=@email AND password=@pwd";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pwd", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Utilisateur
                            {
                                IdUser = reader.GetInt32(0),
                                CIN = reader.GetString(1),
                                Nom = reader.GetString(2),
                                Prenom = reader.GetString(3),
                                Email = reader.GetString(4),
                                Password = reader.GetString(5),
                                Role = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Utilisateur> GetAll()
        {
            var list = new List<Utilisateur>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id_user, CIN, nom, prenom, email, password, role FROM Utilisateur ORDER BY nom";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Utilisateur
                        {
                            IdUser = reader.GetInt32(0),
                            CIN = reader.GetString(1),
                            Nom = reader.GetString(2),
                            Prenom = reader.GetString(3),
                            Email = reader.GetString(4),
                            Password = reader.GetString(5),
                            Role = reader.GetString(6)
                        });
                    }
                }
            }
            return list;
        }

        public void Insert(Utilisateur u)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Utilisateur (CIN, nom, prenom, email, password, role) VALUES (@cin, @nom, @prenom, @email, @pwd, @role)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cin", u.CIN);
                    cmd.Parameters.AddWithValue("@nom", u.Nom);
                    cmd.Parameters.AddWithValue("@prenom", u.Prenom);
                    cmd.Parameters.AddWithValue("@email", u.Email);
                    cmd.Parameters.AddWithValue("@pwd", u.Password);
                    cmd.Parameters.AddWithValue("@role", u.Role);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Utilisateur u)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Utilisateur SET CIN=@cin, nom=@nom, prenom=@prenom, email=@email, password=@pwd, role=@role WHERE id_user=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cin", u.CIN);
                    cmd.Parameters.AddWithValue("@nom", u.Nom);
                    cmd.Parameters.AddWithValue("@prenom", u.Prenom);
                    cmd.Parameters.AddWithValue("@email", u.Email);
                    cmd.Parameters.AddWithValue("@pwd", u.Password);
                    cmd.Parameters.AddWithValue("@role", u.Role);
                    cmd.Parameters.AddWithValue("@id", u.IdUser);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Utilisateur WHERE id_user=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

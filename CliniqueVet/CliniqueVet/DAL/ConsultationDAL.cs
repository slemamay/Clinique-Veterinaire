using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CliniqueVet.Models;

namespace CliniqueVet.DAL
{
    public class ConsultationDAL
    {
        public List<Consultation> GetAll()
        {
            var list = new List<Consultation>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.id_consult, c.date_consult, c.diagnostic, c.id_animal,
                                      a.nom AS nom_animal, p.nom AS nom_prop
                               FROM Consultation c
                               JOIN Animal a ON c.id_animal = a.id_animal
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               ORDER BY c.date_consult DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Consultation
                        {
                            IdConsult = reader.GetInt32(0),
                            DateConsult = reader.GetDateTime(1),
                            Diagnostic = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            IdAnimal = reader.GetInt32(3),
                            NomAnimal = reader.GetString(4),
                            NomProprietaire = reader.GetString(5)
                        });
                    }
                }
            }
            return list;
        }

        public List<Consultation> GetByAnimal(int idAnimal)
        {
            var list = new List<Consultation>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT c.id_consult, c.date_consult, c.diagnostic, c.id_animal,
                                      a.nom AS nom_animal, p.nom AS nom_prop
                               FROM Consultation c
                               JOIN Animal a ON c.id_animal = a.id_animal
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               WHERE c.id_animal = @id
                               ORDER BY c.date_consult DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idAnimal);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Consultation
                            {
                                IdConsult = reader.GetInt32(0),
                                DateConsult = reader.GetDateTime(1),
                                Diagnostic = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                IdAnimal = reader.GetInt32(3),
                                NomAnimal = reader.GetString(4),
                                NomProprietaire = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int Insert(Consultation c)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO Consultation (date_consult, diagnostic, id_animal)
                               VALUES (@date, @diag, @idanimal);
                               SELECT SCOPE_IDENTITY();";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", c.DateConsult);
                    cmd.Parameters.AddWithValue("@diag", (object)c.Diagnostic ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@idanimal", c.IdAnimal);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void Update(Consultation c)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Consultation SET date_consult=@date, diagnostic=@diag, id_animal=@idanimal WHERE id_consult=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@date", c.DateConsult);
                    cmd.Parameters.AddWithValue("@diag", (object)c.Diagnostic ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@idanimal", c.IdAnimal);
                    cmd.Parameters.AddWithValue("@id", c.IdConsult);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Consultation_Medicament WHERE id_consult=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM Facture WHERE id_consult=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand("DELETE FROM Consultation WHERE id_consult=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetCount()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Consultation", conn))
                    return (int)cmd.ExecuteScalar();
            }
        }

        public int GetCountThisMonth()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Consultation WHERE MONTH(date_consult)=MONTH(GETDATE()) AND YEAR(date_consult)=YEAR(GETDATE())";
                using (var cmd = new SqlCommand(sql, conn))
                    return (int)cmd.ExecuteScalar();
            }
        }

        public void AddMedicament(int idConsult, int idMedicament, int quantite)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"IF EXISTS (SELECT 1 FROM Consultation_Medicament WHERE id_consult=@ic AND id_medicament=@im)
                                   UPDATE Consultation_Medicament SET quantite_utilisee=@q WHERE id_consult=@ic AND id_medicament=@im
                               ELSE
                                   INSERT INTO Consultation_Medicament (id_consult, id_medicament, quantite_utilisee) VALUES (@ic, @im, @q)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ic", idConsult);
                    cmd.Parameters.AddWithValue("@im", idMedicament);
                    cmd.Parameters.AddWithValue("@q", quantite);
                    cmd.ExecuteNonQuery();
                }
                string sqlStock = "UPDATE Medicament SET quantite = quantite - @q WHERE id_medicament=@im";
                using (var cmd = new SqlCommand(sqlStock, conn))
                {
                    cmd.Parameters.AddWithValue("@q", quantite);
                    cmd.Parameters.AddWithValue("@im", idMedicament);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Medicament> GetMedicamentsForConsultation(int idConsult)
        {
            var list = new List<Medicament>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT m.id_medicament, m.nom, cm.quantite_utilisee
                               FROM Consultation_Medicament cm
                               JOIN Medicament m ON cm.id_medicament = m.id_medicament
                               WHERE cm.id_consult = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idConsult);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Medicament
                            {
                                IdMedicament = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Quantite = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}

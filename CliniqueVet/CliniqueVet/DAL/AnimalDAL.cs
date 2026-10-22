using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CliniqueVet.Models;

namespace CliniqueVet.DAL
{
    public class AnimalDAL
    {
        public List<Animal> GetAll()
        {
            var list = new List<Animal>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT a.id_animal, a.nom, a.espece, a.id_prop, p.nom AS nom_prop
                               FROM Animal a
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               ORDER BY a.nom";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Animal
                        {
                            IdAnimal = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Espece = reader.GetString(2),
                            IdProp = reader.GetInt32(3),
                            NomProprietaire = reader.GetString(4)
                        });
                    }
                }
            }
            return list;
        }

        public List<Animal> GetByProprietaire(int idProp)
        {
            var list = new List<Animal>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT a.id_animal, a.nom, a.espece, a.id_prop, p.nom AS nom_prop
                               FROM Animal a
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               WHERE a.id_prop = @id ORDER BY a.nom";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idProp);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Animal
                            {
                                IdAnimal = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Espece = reader.GetString(2),
                                IdProp = reader.GetInt32(3),
                                NomProprietaire = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Animal> Search(string keyword)
        {
            var list = new List<Animal>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT a.id_animal, a.nom, a.espece, a.id_prop, p.nom AS nom_prop
                               FROM Animal a
                               JOIN Proprietaire p ON a.id_prop = p.id_prop
                               WHERE a.nom LIKE @kw OR a.espece LIKE @kw OR p.nom LIKE @kw
                               ORDER BY a.nom";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Animal
                            {
                                IdAnimal = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Espece = reader.GetString(2),
                                IdProp = reader.GetInt32(3),
                                NomProprietaire = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(Animal a)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Animal (nom, espece, id_prop) VALUES (@nom, @espece, @idprop)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", a.Nom);
                    cmd.Parameters.AddWithValue("@espece", a.Espece);
                    cmd.Parameters.AddWithValue("@idprop", a.IdProp);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Animal a)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Animal SET nom=@nom, espece=@espece, id_prop=@idprop WHERE id_animal=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", a.Nom);
                    cmd.Parameters.AddWithValue("@espece", a.Espece);
                    cmd.Parameters.AddWithValue("@idprop", a.IdProp);
                    cmd.Parameters.AddWithValue("@id", a.IdAnimal);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM Animal WHERE id_animal=@id";
                using (var cmd = new SqlCommand(sql, conn))
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
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Animal", conn))
                    return (int)cmd.ExecuteScalar();
            }
        }
    }
}

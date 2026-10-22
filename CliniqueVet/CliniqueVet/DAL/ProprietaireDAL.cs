using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CliniqueVet.Models;

namespace CliniqueVet.DAL
{
    public class ProprietaireDAL
    {
        public List<Proprietaire> GetAll()
        {
            var list = new List<Proprietaire>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id_prop, CIN, nom FROM Proprietaire ORDER BY nom";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Proprietaire
                        {
                            IdProp = reader.GetInt32(0),
                            CIN = reader.GetString(1),
                            Nom = reader.GetString(2)
                        });
                    }
                }
            }
            return list;
        }

        public List<Proprietaire> Search(string keyword)
        {
            var list = new List<Proprietaire>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id_prop, CIN, nom FROM Proprietaire WHERE nom LIKE @kw OR CIN LIKE @kw ORDER BY nom";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Proprietaire
                            {
                                IdProp = reader.GetInt32(0),
                                CIN = reader.GetString(1),
                                Nom = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(Proprietaire p)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO Proprietaire (CIN, nom) VALUES (@cin, @nom)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cin", p.CIN);
                    cmd.Parameters.AddWithValue("@nom", p.Nom);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Proprietaire p)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Proprietaire SET CIN=@cin, nom=@nom WHERE id_prop=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cin", p.CIN);
                    cmd.Parameters.AddWithValue("@nom", p.Nom);
                    cmd.Parameters.AddWithValue("@id", p.IdProp);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM Proprietaire WHERE id_prop=@id";
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
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Proprietaire", conn))
                    return (int)cmd.ExecuteScalar();
            }
        }
    }
}

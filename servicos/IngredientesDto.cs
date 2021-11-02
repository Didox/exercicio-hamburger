using System;
using System.Collections.Generic;
using Npgsql;

namespace hamburger_exercicio
{
    public class IngredientesDto
    {
        public static List<Ingrediente> Todos()
        {
            var ingredientes = new List<Ingrediente>();
            
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("select * from ingredientes", conn))
                {
                    var dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        ingredientes.Add(new Ingrediente
                        {
                            Codigo = Convert.ToInt16(dr["codigo"]),
                            Nome = dr["nome"].ToString(),
                        });
                    }
                    dr.Close();
                }
                conn.Close();
                conn.Dispose();
            }

            return ingredientes;
        }

        internal static void Salvar(List<Ingrediente> ingredientes)
        {
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                foreach (var ingrediente in ingredientes)
                {
                    var command = new NpgsqlCommand("select count (1) from ingredientes where codigo=" + ingrediente.Codigo, conn);
                    var qtdIngrediente = Convert.ToInt16(command.ExecuteScalar());
                    if (qtdIngrediente > 0) continue;

                    command = new NpgsqlCommand("insert into ingredientes (codigo, nome) values (@codigo, @nome)", conn);
                    command.Parameters.AddWithValue("@codigo", ingrediente.Codigo);
                    command.Parameters.AddWithValue("@nome", ingrediente.Nome);
                    command.ExecuteNonQuery();
                }
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Npgsql;

namespace hamburger_exercicio
{
    public class HamburgerDto
    {
        internal static List<Hamburger> Todos()
        {
            var hamburgeres = new List<Hamburger>();
            
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("select * from hamburgeres", conn))
                {
                    var dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        hamburgeres.Add(new Hamburger
                        {
                            Codigo = Convert.ToInt16(dr["codigo"]),
                            Nome = dr["nome"].ToString(),
                            Valor = Convert.ToDouble(dr["valor"])
                        });
                    }
                    dr.Close();
                }

                foreach (var hamburger in hamburgeres)
                {
                    using (var command = new NpgsqlCommand("select ingredientes.* from ingredientes inner join hamburgeres_ingredientes on hamburgeres_ingredientes.codigo_ingrediente = ingredientes.codigo where hamburgeres_ingredientes.codigo_hamburger = " + hamburger.Codigo, conn))
                    {
                        hamburger.Ingredientes = new List<Ingrediente>();
                        var dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            hamburger.Ingredientes.Add(new Ingrediente
                            {
                                Codigo = Convert.ToInt16(dr["codigo"]),
                                Nome = dr["nome"].ToString()
                            });
                        }
                        dr.Close();
                    }
                }

                conn.Close();
                conn.Dispose();
            }
            
            return hamburgeres;
        }

        internal static void Salvar(List<Hamburger> hamburgeres)
        {
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                foreach (var hamburger in hamburgeres)
                {
                    var command = new NpgsqlCommand("select count (1) from hamburgeres where codigo=" + hamburger.Codigo, conn);
                    var qtdHamburguer = Convert.ToInt16(command.ExecuteScalar());
                    if (qtdHamburguer > 0) continue;

                    command = new NpgsqlCommand("insert into hamburgeres (codigo, nome, valor) values (@codigo, @nome, @valor)", conn);
                    command.Parameters.AddWithValue("@codigo", hamburger.Codigo);
                    command.Parameters.AddWithValue("@nome", hamburger.Nome);
                    command.Parameters.AddWithValue("@valor", hamburger.Valor);
                    command.ExecuteNonQuery();

                    foreach (var ingrediente in hamburger.Ingredientes)
                    {
                        command = new NpgsqlCommand($"select count (1) from hamburgeres_ingredientes where codigo_hamburger = {hamburger.Codigo} and codigo_ingrediente = {ingrediente.Codigo}", conn);
                        var qtdHamb_ingre = Convert.ToInt16(command.ExecuteScalar());
                        if (qtdHamb_ingre > 0) continue;

                        command = new NpgsqlCommand("insert into hamburgeres_ingredientes (codigo_hamburger, codigo_ingrediente) values (@codigo_hamburger, @codigo_ingrediente)", conn);
                        command.Parameters.AddWithValue("@codigo_hamburger", hamburger.Codigo);
                        command.Parameters.AddWithValue("@codigo_ingrediente", ingrediente.Codigo);
                        command.ExecuteNonQuery();
                    }

                }
                conn.Close();
                conn.Dispose();
            }
        }

    }
}
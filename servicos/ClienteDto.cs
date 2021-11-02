using System;
using System.Collections.Generic;
using Npgsql;

namespace hamburger_exercicio
{
    public class ClienteDto
    {
        public static List<Cliente> Todos()
        {
            var clientes = new List<Cliente>();
            
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("select * from clientes", conn))
                {
                    var dr = command.ExecuteReader();
                    while(dr.Read())
                    {
                        clientes.Add(new Cliente{
                            Codigo = Convert.ToInt16(dr["codigo"]), 
                            Nome = dr["nome"].ToString(),
                            Endereco = dr["endereco"].ToString(),
                            Telefone = dr["telefone"].ToString()
                        });
                    }
                    dr.Close();
                }
                conn.Close();
                conn.Dispose();
            }

            return clientes;
        }

        internal static void Salvar(List<Cliente> clientes)
        {
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                foreach (var cliente in clientes)
                {
                    var command = new NpgsqlCommand("select count (1) from clientes where codigo=" + cliente.Codigo, conn);
                    var qtdCliente = Convert.ToInt16(command.ExecuteScalar());
                    if (qtdCliente > 0) continue;

                    command = new NpgsqlCommand("insert into clientes (codigo, nome, endereco, telefone) values (@codigo, @nome, @endereco, @telefone)", conn);
                    command.Parameters.AddWithValue("@codigo", cliente.Codigo);
                    command.Parameters.AddWithValue("@nome", cliente.Nome);
                    command.Parameters.AddWithValue("@endereco", cliente.Endereco);
                    command.Parameters.AddWithValue("@telefone", cliente.Telefone);
                    command.ExecuteNonQuery();
                }
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
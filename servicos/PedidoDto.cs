using System;
using System.Collections.Generic;
using Npgsql;

namespace hamburger_exercicio
{
    public class PedidoDto
    {
        internal static List<Pedido> Todos()
        {
            var pedidos = new List<Pedido>();
            
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("select * from pedidos", conn))
                {
                    var dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        pedidos.Add(new Pedido
                        {
                            Codigo = Convert.ToInt16(dr["codigo"]),
                            Cliente = ClienteDto.Todos().Find(c =>  c.Codigo == Convert.ToInt16(dr["codigo_cliente"])),
                        });
                    }
                    dr.Close();
                }

                foreach (var pedido in pedidos)
                {
                    using (var command = new NpgsqlCommand("select hamburgeres.*, pedido_hamburgeres.quantidade, pedido_hamburgeres.valor as valor_vendido from hamburgeres inner join pedido_hamburgeres on pedido_hamburgeres.codigo_hamburger = hamburgeres.codigo where pedido_hamburgeres.codigo_pedido = " + pedido.Codigo, conn))
                    {
                        pedido.Itens = new List<Hamburger>();
                        var dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            pedido.Itens.Add(new Hamburger
                            {
                                Codigo = Convert.ToInt16(dr["codigo"]),
                                Nome = dr["nome"].ToString(),
                                Valor = Convert.ToDouble(dr["valor_vendido"].ToString()),
                                Quantidade = Convert.ToInt16(dr["quantidade"].ToString())
                            });
                        }
                        dr.Close();
                    }

                    foreach (var hamburger in pedido.Itens)
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
                }

                conn.Close();
                conn.Dispose();
            }
            
            return pedidos;
        }

        internal static void Salvar(List<Pedido> pedidos)
        {
            string connString = Config.ConnectionString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                foreach (var pedido in pedidos)
                {
                    var command = new NpgsqlCommand("select count (1) from pedidos where codigo=" + pedido.Codigo, conn);
                    var qtdPedido = Convert.ToInt16(command.ExecuteScalar());
                    if (qtdPedido > 0) continue;

                    command = new NpgsqlCommand("insert into pedidos (codigo, codigo_cliente, valor_total) values (@codigo, @codigo_cliente, @valor_total)", conn);
                    command.Parameters.AddWithValue("@codigo", pedido.Codigo);
                    command.Parameters.AddWithValue("@codigo_cliente", pedido.Cliente.Codigo);
                    command.Parameters.AddWithValue("@valor_total", pedido.ValorTotal());
                    command.ExecuteNonQuery();

                    foreach (var item in pedido.Itens)
                    {
                        command = new NpgsqlCommand($"select count (1) from pedido_hamburgeres where codigo_hamburger = {item.Codigo} and codigo_pedido = {pedido.Codigo}", conn);
                        var qtdHamb_ingre = Convert.ToInt16(command.ExecuteScalar());
                        if (qtdHamb_ingre > 0) continue;

                        command = new NpgsqlCommand("insert into pedido_hamburgeres (codigo_hamburger, codigo_pedido, valor, quantidade) values (@codigo_hamburger, @codigo_pedido, @valor, @quantidade)", conn);
                        command.Parameters.AddWithValue("@codigo_hamburger", item.Codigo);
                        command.Parameters.AddWithValue("@codigo_pedido", pedido.Codigo);
                        command.Parameters.AddWithValue("@valor", item.Valor);
                        command.Parameters.AddWithValue("@quantidade", item.Quantidade);
                        command.ExecuteNonQuery();
                    }

                }
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
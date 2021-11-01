using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using Npgsql;

namespace hamburger_exercicio
{
    class Program
    {
        private static List<Ingrediente> ingredientes = new List<Ingrediente>();
        private static List<Pedido> pedidos = new List<Pedido>();
        private static List<Hamburger> hamburgeres = new List<Hamburger>();
        private static List<Cliente> clientes = new List<Cliente>();
        static void Main(string[] args)
        {
            //carregarClientesDoDiscoEmCsv();
            carregarClientesDoPostgreSql();
            //carregarIngredientesDoDiscoEmCsv();
            carregarIngredientesDoPostgreSql();
            carregarHamburgeresDoPostgreSql();
            //carregarHamburgeresDoDiscoEmCsv();
            //carregarPedidosDoDiscoEmCsv();

            // carregarClientesDoDiscoEmJson();
            // carregarIngredientesDoDiscoEmJson();
            // carregarHamburgeresDoDiscoEmJson();
            // carregarPedidosDoDiscoEmJson();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Bem vindo ao programa do Hamburger o que você deseja fazer ?");
                Console.WriteLine("1 - Cadastrar ingredientes");
                Console.WriteLine("2 - Cadastrar hamburger");
                Console.WriteLine("3 - Cadastrar pedido");
                Console.WriteLine("4 - Listar pedidos");
                Console.WriteLine("5 - Listar ingredientes");
                Console.WriteLine("6 - Listar Hamburgeres");
                Console.WriteLine("7 - Listar Clientes");
                Console.WriteLine("0 - Sair");

                int opcao = Convert.ToInt16(Console.ReadLine());

                switch(opcao)
                {
                    case 1:
                        cadastrarIngredientes();
                        break;
                    case 2:
                        cadastrarHamburger();
                        break;
                    case 3:
                        cadastrarPedido();
                        break;
                    case 4:
                        listarPedidos();
                        break;
                    case 5:
                        listarIngredientes();
                        break;
                    case 6:
                        listarHamburgeres();
                        break;
                    case 7:
                        listarClientes();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void salvarPedidosPostgreSql(List<Pedido> pedidos)
        {
            string connString = "Server=localhost;Username=danilo;Database=pedido_hamburger;Port=5432;Password=;SSLMode=Prefer";
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

        private static void salvarIngredientesPostgreSql(List<Ingrediente> ingredientes)
        {
            string connString = "Server=localhost;Username=danilo;Database=pedido_hamburger;Port=5432;Password=;SSLMode=Prefer";
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

        private static void salvarHamburgueresPostgreSql(List<Hamburger> hamburgeres)
        {
            string connString = "Server=localhost;Username=danilo;Database=pedido_hamburger;Port=5432;Password=;SSLMode=Prefer";
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

        private static void salvarClientesPostgreSql(List<Cliente> clientes)
        {
            string connString = "Server=localhost;Username=danilo;Database=pedido_hamburger;Port=5432;Password=;SSLMode=Prefer";
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

        private static void carregarHamburgeresDoPostgreSql()
        {
            string connString = "Server=localhost;Username=danilo;Database=pedido_hamburger;Port=5432;Password=;SSLMode=Prefer";
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
        }
        private static void carregarHamburgeresDoDiscoEmCsv()
        {
            string readText = File.ReadAllText("hamburgeres.csv");
            var linhas = readText.Split('\n');
            foreach(var linha in linhas)
            {
                var colunas = linha.Split(';');
                if(colunas[0] == "" || colunas[0].ToLower() == "codigo") continue;

                var ingredientesHamburger = new List<Ingrediente>();
                var codigosIngredientes = colunas[4].Split(',');
                foreach(var codIngre in codigosIngredientes)
                {
                    var ingrediente = ingredientes.Find(ing => ing.Codigo == Convert.ToInt16(codIngre));
                    ingredientesHamburger.Add(ingrediente);
                }

                hamburgeres.Add(new Hamburger{
                    Codigo = Convert.ToInt16(colunas[0]), 
                    Nome = colunas[1],
                    Quantidade = Convert.ToInt16(colunas[2]),
                    Valor = Convert.ToDouble(colunas[3]),
                    Ingredientes = ingredientesHamburger,
                });
            }
        }

        private static void carregarPedidosDoDiscoEmCsv()
        {
            string readText = File.ReadAllText("pedidos.csv");
            var linhas = readText.Split('\n');
            foreach(var linha in linhas)
            {
                var colunas = linha.Split(';');
                if(colunas[0] == "" || colunas[0].ToLower() == "codigo") continue;

                var hamburgeresDoPedido = new List<Hamburger>();
                var codigosHamburgeres = colunas[1].Split(',');
                foreach(var cod in codigosHamburgeres)
                {
                    var codQtd = cod.Split('|');
                    var ham = hamburgeres.Find(ham => ham.Codigo == Convert.ToInt16(codQtd[0]));
                    ham.Quantidade = Convert.ToInt16(codQtd[1]);
                    hamburgeresDoPedido.Add(ham);
                }

                pedidos.Add(new Pedido{
                    Codigo = Convert.ToInt16(colunas[0]), 
                    Itens = hamburgeresDoPedido,
                    Cliente = clientes.Find(c => c.Codigo == Convert.ToInt16(colunas[2])),
                });
            }
        }

        private static void carregarHamburgeresDoDiscoEmJson()
        {
            string readText = File.ReadAllText("hamburgeres.json");
            hamburgeres = JsonSerializer.Deserialize<List<Hamburger>>(readText);
        }

        private static void carregarPedidosDoDiscoEmJson()
        {
            string readText = File.ReadAllText("pedidos.json");
            pedidos = JsonSerializer.Deserialize<List<Pedido>>(readText);
        }

        private static void carregarIngredientesDoDiscoEmCsv()
        {
            string readText = File.ReadAllText("ingredientes.csv");
            var linhas = readText.Split('\n');
            foreach(var linha in linhas)
            {
                var colunas = linha.Split(';');
                if(colunas[0] == "" || colunas[0].ToLower() == "codigo") continue;
                ingredientes.Add(new Ingrediente{
                    Codigo = Convert.ToInt16(colunas[0]), 
                    Nome = colunas[1]
                });
            }
        }

        private static void carregarClientesDoDiscoEmCsv()
        {
            string readText = File.ReadAllText("clientes.csv");
            var linhas = readText.Split('\n');
            foreach(var linha in linhas)
            {
                var colunas = linha.Split(';');
                if(colunas[0] == "" || colunas[0].ToLower() == "codigo") continue;
                clientes.Add(new Cliente{
                    Codigo = Convert.ToInt16(colunas[0]), 
                    Nome = colunas[1],
                    Endereco = colunas[2],
                    Telefone = colunas[3]
                });
            }
        }

        private static void carregarClientesDoPostgreSql()
        {
            string connString ="Server=localhost;Username=danilo;Database=pedido_hamburger;Port=5432;Password=;SSLMode=Prefer";
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
        }

        private static void carregarIngredientesDoPostgreSql()
        {
            string connString = "Server=localhost;Username=danilo;Database=pedido_hamburger;Port=5432;Password=;SSLMode=Prefer";
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
        }

        private static void carregarClientesDoDiscoEmJson()
        {
            string readText = File.ReadAllText("clientes.json");
            clientes = JsonSerializer.Deserialize<List<Cliente>>(readText);
        }

        private static void carregarIngredientesDoDiscoEmJson()
        {
            string readText = File.ReadAllText("ingredientes.json");
            ingredientes = JsonSerializer.Deserialize<List<Ingrediente>>(readText);
        }

        private static void listarIngredientes()
        {
            Console.Clear();

            Console.WriteLine("======== Lista de ingredientes ========");
           
            foreach(var ingrediente in ingredientes)
            {
                Console.WriteLine($"{ingrediente.Codigo} - {ingrediente.Nome}");
                Console.WriteLine("----------------------------------");
            }

            Thread.Sleep(10000);
        }
        
        private static void listarClientes()
        {
            Console.Clear();

            Console.WriteLine("======== Lista de clientes ========");
           
            foreach(var cliente in clientes)
            {
                Console.WriteLine($"{cliente.Codigo} - {cliente.Nome} - {cliente.Endereco} - {cliente.Telefone}");
                Console.WriteLine("----------------------------------");
            }

            Thread.Sleep(10000);
        }

        private static void listarHamburgeres()
        {
            Console.Clear();

            Console.WriteLine("======== Lista de hamburgeres ========");
           
            foreach(var hamburger in hamburgeres)
            {
                Console.WriteLine($"{hamburger.Codigo} - {hamburger.Nome}");
                foreach(var ingre in hamburger.Ingredientes)
                {
                    Console.WriteLine($"   - {ingre.Nome}");
                }
                Console.WriteLine($"Valor R$ {hamburger.Valor}");

                Console.WriteLine("----------------------------------");
            }

            Thread.Sleep(10000);
        }

        private static void listarPedidos()
        {
            Console.Clear();

            Console.WriteLine("======== Lista de pedidos ========");
           
            foreach(var pedido in pedidos)
            {
                Console.WriteLine($"Pedido numero {pedido.Codigo} do(a) {pedido.Cliente.Nome}");
                Console.WriteLine($"Escolheu os hamburgeres");
                foreach(var item in pedido.Itens)
                {
                    Console.WriteLine($" - {item.Nome}");
                    foreach(var ingre in item.Ingredientes)
                    {
                        Console.WriteLine($"   - {ingre.Nome}");
                    }
                }

                Console.WriteLine($"Valor total de R$ {pedido.ValorTotal().ToString("#.##")} reais");
                Console.WriteLine("----------------------------------");
            }

            Thread.Sleep(15000);
        }

        private static void cadastrarPedido()
        {
            Console.Clear();

            if(hamburgeres.Count == 0)
            {
                Console.WriteLine("Não existem hamburgueres cadastrados!");
                Thread.Sleep(3000);
                return;
            }

            var pedido = new Pedido(){ Codigo = (pedidos.Count + 1) ,Cliente = new Cliente(), Itens = new List<Hamburger>()};
            Console.WriteLine("Digite o nome do cliente:");
            pedido.Cliente.Nome = Console.ReadLine();

            var cli = clientes.Find(c => c.Nome.ToLower() == pedido.Cliente.Nome.ToLower());
            if(cli != null)
                pedido.Cliente = cli;
            else
            {
                Console.WriteLine("Digite o endereço do cliente:");
                pedido.Cliente.Endereco = Console.ReadLine();

                Console.WriteLine("Digite o telefone do cliente:");
                pedido.Cliente.Telefone = Console.ReadLine();

                pedido.Cliente.Codigo = clientes.Count + 1;

                clientes.Add(pedido.Cliente);
                //salvarClientesCsv(clientes);
                //salvarClientesJson(clientes);
                salvarClientesPostgreSql(clientes);
            }

            selectionaHamburgeres(pedido);

            pedidos.Add(pedido);

            // salvarPedidosJson(pedidos);
            //salvarPedidosCsv(pedidos);
            salvarPedidosPostgreSql(pedidos);

            Console.WriteLine("Pedido cadastrado com sucesso!");
            Thread.Sleep(1000);
        }

        private static void selectionaHamburgeres(Pedido pedido)
        {
            Console.WriteLine("Seleciona um dos hamburgeres abaixo:");
            foreach(var hamburger in hamburgeres)
            {
                Console.WriteLine($"{hamburger.Codigo} - {hamburger.Nome}");
            }
            int codigo = Convert.ToInt16(Console.ReadLine());

            var hamb = hamburgeres.Find(i =>  i.Codigo == codigo);
            if(hamb == null)
            {
                Console.WriteLine("Código do hamburguer inválido!");
                Thread.Sleep(2000);
                selectionaHamburgeres(pedido);
                return;
            }

            Console.WriteLine($"Digite a quantidade do {hamb.Nome}:");
            hamb.Quantidade = Convert.ToInt16(Console.ReadLine());

            pedido.Itens.Add(hamb);

            Console.WriteLine("Digite 1 para selectionar mais hamburgueres ou 0 para sair");
            int opcao = Convert.ToInt16(Console.ReadLine());
            if(opcao == 1) selectionaHamburgeres(pedido);
        }

        private static void cadastrarHamburger()
        {
            Console.Clear();

            if(ingredientes.Count == 0)
            {
                Console.WriteLine("Não existem ingredientes cadastrados!");
                Thread.Sleep(3000);
                return;
            }

            var hamburger = new Hamburger();
            Console.WriteLine("Digite o nome do hamburger");
            hamburger.Nome = Console.ReadLine();

            Console.WriteLine($"Digite o valor do hamburger {hamburger.Nome}");
            hamburger.Valor = Convert.ToDouble(Console.ReadLine());
            
            hamburger.Codigo = hamburgeres.Count + 1;

            selectionaIngredientes(hamburger);

            hamburgeres.Add(hamburger);

            salvarHamburgueresPostgreSql(hamburgeres);
            //salvarHamburgeresCsv(hamburgeres);
            // salvarHamburgeresJson(hamburgeres);

            Console.WriteLine("Hamburguer cadastrado com sucesso!");
            Thread.Sleep(1000);
        }

        private static void selectionaIngredientes(Hamburger hamburger)
        {
            Console.WriteLine("Selecione um dos ingredientes abaixo:");
            foreach(var ingrediente in ingredientes)
            {
                Console.WriteLine($"{ingrediente.Codigo} - {ingrediente.Nome}");
            }
            int codigo = Convert.ToInt16(Console.ReadLine());

            var ingre = ingredientes.Find(i =>  i.Codigo == codigo);
            if(ingre == null)
            {
                Console.WriteLine("Código de ingrediente inválido!");
                Thread.Sleep(2000);
                selectionaIngredientes(hamburger);
                return;
            }

            if(hamburger.Ingredientes == null) hamburger.Ingredientes = new List<Ingrediente>();
            hamburger.Ingredientes.Add(ingre);

            Console.WriteLine("Digite 1 para selecionar mais ingredientes ou 0 para sair");
            int opcao = Convert.ToInt16(Console.ReadLine());
            if(opcao == 1) selectionaIngredientes(hamburger);
        }

        private static void cadastrarIngredientes()
        {
            Console.Clear();
            Console.WriteLine("Digite o nome do ingrediente");
            var ingrediente = new Ingrediente();
            ingrediente.Nome = Console.ReadLine();
            ingrediente.Codigo = ingredientes.Count + 1;
            ingredientes.Add(ingrediente);

            //salvarIngredientesCsv(ingredientes);
            // salvarIngredientesJson(ingredientes);
            salvarIngredientesPostgreSql(ingredientes);

            Console.WriteLine("Ingrediente cadastrado com sucesso");
            Thread.Sleep(1000);
        }

        private static void salvarHamburgeresCsv(List<Hamburger> hamburgeres)
        {
            string conteudoCsv = "Codigo;Nome;Quantidade;Valor;Ingredientes\n";
            foreach(var hamburger in hamburgeres)
            {
                List<int> codigosIngredientes = new List<int>();
                foreach(var ingre in hamburger.Ingredientes)
                {
                    codigosIngredientes.Add(ingre.Codigo);
                }
                conteudoCsv += $"{hamburger.Codigo};{hamburger.Nome};{hamburger.Quantidade};{hamburger.Valor};{string.Join(",", codigosIngredientes.ToArray())}\n";
            }
            
            File.WriteAllText("hamburgeres.csv", conteudoCsv);
        }

        private static void salvarPedidosCsv(List<Pedido> pedidos)
        {
            string conteudoCsv = "Codigo;Itens;CodigoCliente\n";
            foreach(var pedido in pedidos)
            {
                List<string> codigoQtdPedidos = new List<string>();
                foreach(var ham in pedido.Itens)
                {
                    codigoQtdPedidos.Add($"{ham.Codigo}|{ham.Quantidade}");
                }
                conteudoCsv += $"{pedido.Codigo};{string.Join(",", codigoQtdPedidos.ToArray())};{pedido.Cliente.Codigo}\n";
            }
            
            File.WriteAllText("pedidos.csv", conteudoCsv);
        }
        
        private static void salvarClientesCsv(List<Cliente> clientes)
        {
            string conteudoCsv = "Codigo;Nome;Endereco;Telefone\n";
            foreach(var cliente in clientes)
            {
                conteudoCsv += $"{cliente.Codigo};{cliente.Nome};{cliente.Endereco};{cliente.Telefone}\n";
            }
            
            File.WriteAllText("clientes.csv", conteudoCsv);
        }
        
        private static void salvarHamburgeresJson(List<Hamburger> hamburgeres)
        {
            File.WriteAllText("hamburgeres.json", JsonSerializer.Serialize(hamburgeres));
        }

        private static void salvarIngredientesCsv(List<Ingrediente> ingredientes)
        {
            string conteudoCsv = "codigo;nome\n";
            foreach(var ingrediente in ingredientes)
            {
                conteudoCsv += $"{ingrediente.Codigo};{ingrediente.Nome}\n";
            }
            
            File.WriteAllText("ingredientes.csv", conteudoCsv);
        }

        private static void salvarIngredientesJson(List<Ingrediente> ingredientes)
        {
            File.WriteAllText("ingredientes.json", JsonSerializer.Serialize(ingredientes));
        }

        private static void salvarClientesJson(List<Cliente> clientes)
        {
            File.WriteAllText("clientes.json", JsonSerializer.Serialize(clientes));
        }

        private static void salvarPedidosJson(List<Pedido> pedidos)
        {
            File.WriteAllText("pedidos.json", JsonSerializer.Serialize(pedidos));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace hamburger_exercicio
{
    class Program
    {
        private static List<Ingrediente> ingredientes = new List<Ingrediente>();
        private static List<Pedido> pedidos = new List<Pedido>();
        private static List<Hamburger> hamburgeres = new List<Hamburger>();
        static void Main(string[] args)
        {
            carregarIngredientesDoDiscoEmCsv();
            carregarHamburgeresDoDiscoEmCsv();
            carregarPedidosDoDiscoEmCsv();

            // carregarIngredientesDoDiscoEmJson();
            // carregarHamburgeresDoDiscoEmJson();
            // carregarPedidosDoDiscoEmJson();

            while(true)
            {
                Console.Clear();
                Console.WriteLine("Bem vindo ao programa do Hamburger o que você deseja fazer ?");
                Console.WriteLine("1 - Cadastrar ingredientes");
                Console.WriteLine("2 - Cadastrar hamburger");
                Console.WriteLine("3 - Cadastrar pedido");
                Console.WriteLine("4 - Listar pedidos");
                Console.WriteLine("5 - Listar ingredientes");
                Console.WriteLine("6 - Listar Hamburgeres");
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
                    case 0:
                        return;
                }
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
                    Cliente = new Cliente{ Nome = colunas[2], Endereco = colunas[3] },
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
                Console.WriteLine("Nao existe hamburgeres cadastrados");
                Thread.Sleep(3000);
                return;
            }

            var pedido = new Pedido(){ Codigo = (pedidos.Count + 1) ,Cliente = new Cliente(), Itens = new List<Hamburger>()};
            Console.WriteLine("Digite o nome do cliente");
            pedido.Cliente.Nome = Console.ReadLine();

            Console.WriteLine("Digite o endereço do cliente");
            pedido.Cliente.Endereco = Console.ReadLine();

            selectionaHamburgeres(pedido);

            pedidos.Add(pedido);

            // salvarPedidosJson(pedidos);
            salvarPedidosCsv(pedidos);

            Console.WriteLine("Pedido cadastrado com sucesso");
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
                Console.WriteLine("Código do hamburger inválido");
                Thread.Sleep(2000);
                selectionaHamburgeres(pedido);
                return;
            }

            Console.WriteLine($"Digite a quantidade do {hamb.Nome}:");
            hamb.Quantidade = Convert.ToInt16(Console.ReadLine());

            pedido.Itens.Add(hamb);

            Console.WriteLine("Digite 1 para selectionar mais hamburgeres ou 0 para sair");
            int opcao = Convert.ToInt16(Console.ReadLine());
            if(opcao == 1) selectionaHamburgeres(pedido);
        }

        private static void cadastrarHamburger()
        {
            Console.Clear();

            if(ingredientes.Count == 0)
            {
                Console.WriteLine("Nao existe ingredientes cadastrados");
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

            salvarHamburgeresCsv(hamburgeres);
            // salvarHamburgeresJson(hamburgeres);

            Console.WriteLine("Hamburger cadastrado com sucesso");
            Thread.Sleep(1000);
        }

        private static void selectionaIngredientes(Hamburger hamburger)
        {
            Console.WriteLine("Seleciona um dos ingredientes abaixo:");
            foreach(var ingrediente in ingredientes)
            {
                Console.WriteLine($"{ingrediente.Codigo} - {ingrediente.Nome}");
            }
            int codigo = Convert.ToInt16(Console.ReadLine());

            var ingre = ingredientes.Find(i =>  i.Codigo == codigo);
            if(ingre == null)
            {
                Console.WriteLine("Código de ingrediente inválido");
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

            salvarIngredientesCsv(ingredientes);
            // salvarIngredientesJson(ingredientes);

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
            string conteudoCsv = "Codigo;Itens;ClienteNome;ClienteEndereco\n";
            foreach(var pedido in pedidos)
            {
                List<string> codigoQtdPedidos = new List<string>();
                foreach(var ham in pedido.Itens)
                {
                    codigoQtdPedidos.Add($"{ham.Codigo}|{ham.Quantidade}");
                }
                conteudoCsv += $"{pedido.Codigo};{string.Join(",", codigoQtdPedidos.ToArray())};{pedido.Cliente.Nome};{pedido.Cliente.Endereco}\n";
            }
            
            File.WriteAllText("pedidos.csv", conteudoCsv);
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

        private static void salvarPedidosJson(List<Pedido> pedidos)
        {
            File.WriteAllText("pedidos.json", JsonSerializer.Serialize(pedidos));
        }
    }
}

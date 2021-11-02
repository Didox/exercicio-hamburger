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
        private static List<Cliente> clientes;
        private static List<Ingrediente> ingredientes;
        private static List<Hamburger> hamburgeres;
        private static List<Pedido> pedidos;
        static void Main(string[] args)
        {
            clientes = ClienteDto.Todos();
            ingredientes = IngredientesDto.Todos();
            hamburgeres = HamburgerDto.Todos();
            pedidos = PedidoDto.Todos();
            
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

        private static void listarIngredientes()
        {
            Console.Clear();

            Console.WriteLine("======== Lista de ingredientes ========");
           
            foreach(var ingrediente in ingredientes)
            {
                Console.WriteLine($"{ingrediente.Codigo} - {ingrediente.Nome}");
                Console.WriteLine("----------------------------------");
            }

            Thread.Sleep(5000);
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

            Thread.Sleep(5000);
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

            Thread.Sleep(5000);
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
                    Console.WriteLine($" - {item.Nome} - {item.Quantidade} - R${item.Valor}");
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
                ClienteDto.Salvar(clientes);
            }

            selectionaHamburgeres(pedido);

            pedidos.Add(pedido);

            PedidoDto.Salvar(pedidos);

            Console.WriteLine("Pedido cadastrado com sucesso!");
            Thread.Sleep(1000);
        }

        private static void selectionaHamburgeres(Pedido pedido)
        {
            Console.WriteLine("Selecione um dos hamburgeres abaixo:");
            foreach(var hamburger in hamburgeres)
            {
                Console.WriteLine($"{hamburger.Codigo} - {hamburger.Nome} R$ {hamburger.Valor}");
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

            HamburgerDto.Salvar(hamburgeres);

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

            IngredientesDto.Salvar(ingredientes);

            Console.WriteLine("Ingrediente cadastrado com sucesso");
            Thread.Sleep(1000);
        }
    }
}

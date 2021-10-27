using System.Collections.Generic;

namespace hamburger_exercicio
{
    internal class Pedido
    {
        public int Codigo {get;set;}
        public List<Hamburger> Itens {get;set;}
        public Cliente Cliente {get;set;}

        public double ValorTotal()
        {
            if(Itens == null || Itens.Count == 0) return 0;
            double valorTotal = 0;
            foreach(var item in Itens)
            {
                valorTotal += (item.Valor * item.Quantidade);
            }

            return valorTotal;
        }

    }
}
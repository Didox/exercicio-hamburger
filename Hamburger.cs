using System.Collections.Generic;

namespace hamburger_exercicio
{
    internal class Hamburger
    {
        public int Codigo {get;set;}
        public string Nome {get;set;}
        public int Quantidade {get;set;}
        public double Valor {get;set;}
        public List<Ingrediente> Ingredientes {get;set;}
    }
}
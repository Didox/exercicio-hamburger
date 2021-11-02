using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Npgsql;

namespace hamburger_exercicio
{
    public class Config
    {
        public static string ConnectionString()
        {
            string readText = File.ReadAllText("appsettings.json");
            CnnStr dado = JsonSerializer.Deserialize<CnnStr>(readText);
            return dado.ConnectionString;
        }

        class CnnStr
        {
            public string ConnectionString{get;set;}
        }
    }
}
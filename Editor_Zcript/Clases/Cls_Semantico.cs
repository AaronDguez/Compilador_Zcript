using System;
using System.Collections.Generic;
using System.Data;

namespace Editor_Zcript.Clases
{
    class Cls_Semantico
    {
        public static List<Tuple<int, string, int>> Errores = new List<Tuple<int, string, int>>();
        private static DataTable Simbolos = new DataTable();
        private static int alcance;
        private static Dictionary<int, string> error = new Dictionary<int, string>
        {
            { 450, "Variable no declarada" },
            { 451, "Variable ya declarada" },
            { 452, "Incompatibilidad de Tipos" },
            { 453, "Division entre 0" }
        };
        public static void Analizar(List<Tuple<string, int, string, int>> resulLexico) //Item1 = Palabra, Item2 = Token, Item4 = Linea
        {
            Errores.Clear(); Simbolos.Clear();
            Stack<string> llaves = new Stack<string>(); //Pilas para saber si lugar en el que se está es dentro de alguna condicion, funcion o ciclo
            Simbolos.Columns.Add("Tipo", typeof(string)); //Campo para saber si la variable es Entera, Double, Bool o Cadena
            Simbolos.Columns.Add("Nombre", typeof(string)); //Nombre de la variable
            Simbolos.Columns.Add("Alcance", typeof(int)); //Campo para ver si la variable es Global o local de una funcion, condicional o ciclo;
            Simbolos.Columns.Add("Cambio", typeof(string)); //Campo para poner Si la variable tiene permitido cambiar de valor del mismo tipo (LET), de cualquier tipo (VAR) o de ningun modo (CONST)
            alcance = 0;
        }
    }
}

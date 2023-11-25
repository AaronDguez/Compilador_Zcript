using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor_Zcript.Clases
{
    static class Postfijo
    {
        public static string ConvertirExpresion(string Expresion)
        {
            string Post = string.Empty;
            string[] ExpresionSeparada = Expresion.Split(' ');
            Stack<Tuple<string, int>> PilaDeOperadores = new Stack<Tuple<string, int>>();
            int Jerarquia;
            for (int i = 0; i < ExpresionSeparada.Length; i++)
            {
                if (ExpresionSeparada[i].All(char.IsLetterOrDigit))
                    Post += $"{ExpresionSeparada[i]} ";
                else //Es operador
                {
                    Jerarquia = ObtenerJerarquia(ExpresionSeparada[i]);

                    if (Jerarquia == -1) // Error
                    {
                        Console.WriteLine($"Palabra {i}: No se encontró el operador {ExpresionSeparada[i]}, cheque que la expresion esté correcta");
                        continue;
                    }
                    else if (Jerarquia == 1) // Parentesis que abre
                        PilaDeOperadores.Push(new Tuple<string, int>(ExpresionSeparada[i], Jerarquia));
                    else if (Jerarquia == 2) // Parentesis que cierra
                    {
                        while (PilaDeOperadores.Count > 0 && PilaDeOperadores.Peek().Item2 != 1) //Sacar todo hasta que la pila encuentra un ( o hasta que se vacia
                        {
                            Post += $"{PilaDeOperadores.Pop().Item1} ";
                        }
                        if (PilaDeOperadores.Count > 0)
                            PilaDeOperadores.Pop();
                    }
                    else //Operador cualquiera
                    {
                        if (PilaDeOperadores.Count > 0 && Jerarquia > PilaDeOperadores.Peek().Item2)
                            PilaDeOperadores.Push(new Tuple<string, int>(ExpresionSeparada[i], Jerarquia));
                        else
                        {
                            while (PilaDeOperadores.Count > 0 && PilaDeOperadores.Peek().Item2 >= Jerarquia)
                            {
                                Post += $"{PilaDeOperadores.Pop().Item1} ";
                            }
                            PilaDeOperadores.Push(new Tuple<string, int>(ExpresionSeparada[i], Jerarquia));
                        }
                    }
                }
            }
            while (PilaDeOperadores.Count > 0)
            {
                if (PilaDeOperadores.Count > 0 && PilaDeOperadores.Peek().Item2 != 1 && PilaDeOperadores.Peek().Item2 != 2)
                    Post += $"{PilaDeOperadores.Pop().Item1}  ";
                else
                    PilaDeOperadores.Pop();
            }
            return Post;
        }

        private static int ObtenerJerarquia(string operador)
        {
            switch (operador) //Asignar un valor de acuerdo a la jerarquia del operador
            {
                case "(":
                    return 1;
                case ")":
                    return 2;
                case "=":
                    return 3;
                case "or":
                    return 4;
                case "and":
                    return 5;
                case "not":
                    return 6;
                case "!=" or "==" or "<" or ">" or "<=" or ">=":
                    return 7;
                case "+"or "-":
                    return 8;
                case "*" or "/":
                    return 9;
                case "^":
                    return 10;
                default: // No se encontró el operador
                    return -1;
            }
        }
    }
}

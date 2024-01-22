using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor_Zcript.Clases
{
    static class Postfijo
    {
        public static string ConvertirExpresion(string Expresion) //Convierte una expresion infija a postfija
        {
            string Post = string.Empty; //Expresion postfija
            string[] ExpresionSeparada = Expresion.Split(' '); //Separar la expresion por espacios
            Stack<Tuple<string, int>> PilaDeOperadores = new Stack<Tuple<string, int>>(); //Pila de operadores
            int Jerarquia; //Jerarquia del operador
            for (int i = 0; i < ExpresionSeparada.Length; i++) //Recorrer la expresion
            {
                if (ExpresionSeparada[i].All(char.IsLetterOrDigit)) //Es operando
                    Post += $"{ExpresionSeparada[i]} "; //Agregar a la expresion postfija
                else //Es operador
                {
                    Jerarquia = ObtenerJerarquia(ExpresionSeparada[i]); //Obtener la jerarquia del operador

                    if (Jerarquia == -1) // Error
                    {
                        Console.WriteLine($"Palabra {i}: No se encontró el operador {ExpresionSeparada[i]}, cheque que la expresion esté correcta"); //Mostrar error
                        continue; //Continuar con la siguiente palabra
                    }
                    else if (Jerarquia == 1) // Parentesis que abre
                        PilaDeOperadores.Push(new Tuple<string, int>(ExpresionSeparada[i], Jerarquia)); //Agregar a la pila
                    else if (Jerarquia == 2) // Parentesis que cierra
                    {
                        while (PilaDeOperadores.Count > 0 && PilaDeOperadores.Peek().Item2 != 1) //Sacar todo hasta que la pila encuentra un ( o hasta que se vacia
                        {
                            Post += $"{PilaDeOperadores.Pop().Item1} "; //Agregar a la expresion postfija
                        }
                        if (PilaDeOperadores.Count > 0) //Si la pila no está vacia
                            PilaDeOperadores.Pop(); //Sacar el parentesis que abre
                    }
                    else //Operador cualquiera
                    {
                        if (PilaDeOperadores.Count > 0 && Jerarquia > PilaDeOperadores.Peek().Item2) //Si la pila no está vacia y la jerarquia del operador es mayor a la del tope de la pila
                            PilaDeOperadores.Push(new Tuple<string, int>(ExpresionSeparada[i], Jerarquia)); //Agregar a la pila
                        else
                        {
                            while (PilaDeOperadores.Count > 0 && PilaDeOperadores.Peek().Item2 >= Jerarquia) //Sacar todo hasta que la pila encuentra un operador de menor jerarquia o hasta que se vacia
                            {
                                Post += $"{PilaDeOperadores.Pop().Item1} "; //Agregar a la expresion postfija
                            }
                            PilaDeOperadores.Push(new Tuple<string, int>(ExpresionSeparada[i], Jerarquia)); //Agregar a la pila
                        }
                    }
                }
            }
            while (PilaDeOperadores.Count > 0) //Sacar todo lo que queda en la pila
            {
                if (PilaDeOperadores.Count > 0 && PilaDeOperadores.Peek().Item2 != 1 && PilaDeOperadores.Peek().Item2 != 2) //Si la pila no está vacia y no es un parentesis
                    Post += $"{PilaDeOperadores.Pop().Item1}  "; //Agregar a la expresion postfija
                else
                    PilaDeOperadores.Pop(); //Sacar el parentesis
            }
            return Post; //Devolver la expresion postfija
        }

        private static int ObtenerJerarquia(string operador) //Obtener la jerarquia del operador
        {
            switch (operador) //Asignar un valor de acuerdo a la jerarquia del operador
            {
                case "(": //Parentesis que abre
                    return 1;
                case ")": //Parentesis que cierra
                    return 2;
                case "=": //Asignacion
                    return 3;
                case "or": //Operador or
                    return 4;
                case "and": //Operador and
                    return 5;
                case "not": //Operador not
                    return 6;
                case "!=" or "==" or "<" or ">" or "<=" or ">=": //Operadores de comparacion
                    return 7;
                case "+"or "-": //Operadores de suma y resta
                    return 8;
                case "*" or "/": //Operadores de multiplicacion y division
                    return 9;
                case "^": //Operador de potencia
                    return 10;
                default: // No se encontró el operador
                    return -1;
            }
        }
    }
}

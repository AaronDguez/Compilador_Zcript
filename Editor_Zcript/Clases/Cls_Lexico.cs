using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor_Zcript
{
    class Cls_Lexico
    {
        static int[,] matriz = {
          //  0  1   2    3    4    5   6  7  8  9   10  11  12   13   14   15   16    17   18   19  20    21  22  23 24  25
            { 1, 2, 401, 103, 104, 105, 5, 8, 9, 10, 11, 12, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 13, 0, 0, 401 },
            { 1, 1, 1, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 },
            { 101, 2, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 3, 101, 101, 101, 101, 101, 101, 101 },
            { 403, 4, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403, 403 },
            { 102, 4, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102 },
            { 106, 106, 106, 106, 106, 6, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106, 106 },
            { 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },
            { 6, 6, 6, 6, 6, 6, 127, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 },
            { 107, 107, 107, 107, 107, 107, 107, 107, 107, 108, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107, 107 },
            { 109, 109, 109, 109, 109, 109, 109, 109, 109, 110, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109, 109 },
            { 112, 112, 112, 112, 112, 112, 112, 113, 112, 111, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112, 112 },
            { 114, 114, 114, 114, 114, 114, 114, 114, 114, 115, 114, 114, 114, 114, 114, 114, 114, 114, 114, 114, 114, 114, 114, 114, 114, 114 },
            { 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 127, 12, 12 },
            { 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 126, 402, 13, 13 }
        };
        static int[] Finales = { 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127 }, Errores = { 401, 402, 403, 404, 405 };
        static int[] FinalesNoDirectos = { 100, 101, 102, 126, 127 };
        public static List<Tuple<string, int, string, int>> queue = new List<Tuple<string, int, string, int>>();

        public static void verificar(string texto)
        {
            queue.Clear();
            int filaMatriz = 0, columnaMatriz, linea = 0, estado; string palabraEncontrada = string.Empty;
            for (int i = 0; i < texto.Length; i++)
            {
                char caracterLeido = texto[i];
                if (char.IsLetter(caracterLeido))
                    columnaMatriz = 0;
                else if (char.IsDigit(caracterLeido))
                    columnaMatriz = 1;
                else
                {
                    switch (caracterLeido)
                    {
                        case '_':
                            columnaMatriz = 2;
                            break;
                        case '+':
                            columnaMatriz = 3;
                            break;
                        case '-':
                            columnaMatriz = 4;
                            break;
                        case '*':
                            columnaMatriz = 5;
                            break;
                        case '/':
                            columnaMatriz = 6;
                            break;
                        case '>':
                            columnaMatriz = 7;
                            break;
                        case '<':
                            columnaMatriz = 8;
                            break;
                        case '=':
                            columnaMatriz = 9;
                            break;
                        case '!':
                            columnaMatriz = 10;
                            break;
                        case '#':
                            columnaMatriz = 11;
                            break;
                        case '$':
                            columnaMatriz = 12;
                            break;
                        case '\\':
                            columnaMatriz = 13;
                            break;
                        case '{':
                            columnaMatriz = 14;
                            break;
                        case '}':
                            columnaMatriz = 15;
                            break;
                        case '(':
                            columnaMatriz = 16;
                            break;
                        case ')':
                            columnaMatriz = 17;
                            break;
                        case '.':
                            columnaMatriz = 18;
                            break;
                        case ',':
                            columnaMatriz = 19;
                            break;
                        case ';':
                            columnaMatriz = 20;
                            break;
                        case ':':
                            columnaMatriz = 21;
                            break;
                        case (char)0x0027: // '
                            columnaMatriz = 22;
                            break;
                        case '\n':
                            columnaMatriz = 23;
                            break;
                        case '\t':
                        case '\0':
                        case ' ':
                        case (char)0x2408: //BackSpace
                            columnaMatriz = 24;
                            break;
                        default:
                            columnaMatriz = 25;
                            break;
                    }
                }
                palabraEncontrada += caracterLeido;
                filaMatriz = matriz[filaMatriz, columnaMatriz];
                if (filaMatriz >= 100)
                {
                    estado = filaMatriz;
                    if (palabraEncontrada.Length > 1) 
                    {
                        if ((estado != 127 && estado != 126 && FinalesNoDirectos.Contains(estado)) || estado == 112 )
                        {
                            palabraEncontrada = palabraEncontrada.Remove(palabraEncontrada.Length - 1, 1);
                            i--;
                        }
                    }
                    if (Finales.Contains(estado))
                    {
                        string tipo = "Tipo Sin Determinar";
                        if (estado == 100)
                        {
                            if (palabraEncontrada == "var" || palabraEncontrada == "let" || palabraEncontrada == "const")
                            {
                                tipo = "Tipo de Variable";
                                switch (palabraEncontrada)
                                {
                                    case "var":
                                        estado = 200;
                                        break;
                                    case "let":
                                        estado = 201;
                                        break;
                                    case "const":
                                        estado = 202;
                                        break;
                                }
                            }
                            else if (palabraEncontrada == "if" || palabraEncontrada == "else" || palabraEncontrada == "elif")
                            {
                                tipo = "Condicional";
                                switch (palabraEncontrada)
                                {
                                    case "if":
                                        estado = 203;
                                        break;
                                    case "else":
                                        estado = 204;
                                        break;
                                    case "elif":
                                        estado = 205;
                                        break;
                                }
                            }
                            else if (palabraEncontrada == "while")
                            {
                                tipo = "Ciclo";
                                estado = 206;
                            }
                            else if (palabraEncontrada == "and" || palabraEncontrada == "or")
                            {
                                tipo = "Operador Logico";
                                switch (palabraEncontrada)
                                {
                                    case "and":
                                        estado = 212;
                                        break;
                                    case "or":
                                        estado = 213;
                                        break;
                                }
                            }
                            else if (palabraEncontrada == "mod")
                            {
                                tipo = "modulo";
                                estado = 216;
                            }
                            else if (palabraEncontrada == "write" || palabraEncontrada == "read")
                            {
                                tipo = "Lectura/Escritura";
                                switch (palabraEncontrada)
                                {
                                    case "write":
                                        estado = 210;
                                        break;
                                    case "read":
                                        estado = 211;
                                        break;
                                }
                            }
                            else
                            {
                                switch (palabraEncontrada)
                                {
                                    case "break":
                                        estado = 207;
                                        tipo = palabraEncontrada;
                                        break;
                                    case "null":
                                        estado = 209;
                                        tipo = palabraEncontrada;
                                        break;
                                    case "true":
                                        estado = 214;
                                        tipo = "Booleano";
                                        break;
                                    case "false":
                                        estado = 215;
                                        tipo = "Booleano";
                                        break;
                                    case "main":
                                        estado = 208;
                                        tipo = "MAIN";
                                        break;
                                    default:
                                        tipo = "Variable";
                                        break;
                                }
                            }
                        }
                        else if (estado == 101)
                            tipo = "Numero Entero";
                        else if (estado == 102)
                            tipo = "Numero Decimal";
                        else if (estado >= 103 && estado <= 106)
                        {
                            switch (estado)
                            {
                                case 103:
                                    tipo = "Suma | Concatenacion";
                                    break;
                                case 104:
                                    tipo = "Resta";
                                    break;
                                case 105:
                                    tipo = "Multiplicacion";
                                    break;
                                case 106:
                                    tipo = "Division";
                                    break;
                            }
                        }
                        else if (estado >= 107 && estado <= 115)
                        {
                            switch (estado)
                            {
                                case 107:
                                    tipo = "Mayor que";
                                    break;
                                case 108:
                                    tipo = "Mayor o igual que";
                                    break;
                                case 109:
                                    tipo = "Menor que";
                                    break;
                                case 110:
                                    tipo = "Menor o igual que";
                                    break;
                                case 111:
                                    tipo = "Igual que";
                                    break;
                                case 112:
                                    tipo = "Igualacion";
                                    break;
                                case 113:
                                    tipo = "Declaracion de Metodo";
                                    break;
                                case 114:
                                    tipo = "No";
                                    break;
                                case 115:
                                    tipo = "Distinto de";
                                    break;
                            }
                        }
                        else if (estado == 126)
                            tipo = "Cadena";
                        else if (estado == 120 || estado == 121)
                            tipo = "Parentesis";
                        else if (estado == 118 || estado == 119)
                            tipo = "Llaves";
                        if (estado != 127)
                        {
                            queue.Add(new Tuple<string, int, string, int>(palabraEncontrada, estado, tipo, linea + 1)); //Guardar Token, palabra, tipo y linea
                        }
                        filaMatriz = 0; palabraEncontrada = string.Empty;
                    }
                    else if (Errores.Contains(estado))
                    {
                        string tipo = string.Empty;
                        switch (estado)
                        {
                            case 401:
                                tipo = "Unknown ID";
                                break;
                            case 402:
                                tipo = "' Expected";
                                break;
                            case 403:
                                tipo = "Digit Error";
                                break;
                            case 404:
                                tipo = "Digit or Operator Expected";
                                break;
                            case 405:
                                tipo = "ID or Name too long";
                                break;
                        }
                        queue.Add(new Tuple<string, int, string, int>(palabraEncontrada, estado, tipo, linea + 1));
                        filaMatriz = 0; palabraEncontrada = string.Empty;
                    }
                }
                else if (filaMatriz == 0)
                {
                    if (caracterLeido == '\n')
                        linea++;
                    palabraEncontrada = string.Empty;
                }
            }
        }
    }
}

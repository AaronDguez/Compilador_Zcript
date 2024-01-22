using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor_Zcript.Clases
{
    class Asm
    {
        List<string> token_variables = new List<string>(); // tokens de las variables
        List<string> variables = new List<string>(); // nombre de las variables
        List<string> contenido_vars = new List<string>(); // contenido de las variables
        List<string> tokens = new List<string>(); // tokens del codigo
        List<string> lineas = new List<string>(); // lineas del codigo
        List<string> contenido = new List<string>(); // contenido del codigo

        List<string> tkn_ifelse = new List<string>(); // tokens de los if y else
        List<string> lista_ifelse = new List<string>(); // contenido de los if y else
        List<string> lista_lineas = new List<string>(); // lineas de los if y else
        public Asm(List<string> variables, List<string> tokens_code, List<string> contenido_code, List<string> lineas_code, List<string> tokens_variables, List<string> contenido_variables) // constructor
        {
            this.variables = variables; // asignacion de variables
            this.tokens = tokens_code; // asignacion de tokens
            this.contenido = contenido_code; // asignacion de contenido
            this.lineas = lineas_code; // asignacion de lineas
            this.token_variables = tokens_variables; // asignacion de tokens de las variables
            this.contenido_vars = contenido_variables; // asignacion de contenido de las variables
            for (int i = 0; i < tokens.Count; i++) // recorrido de los tokens
            {
                if (Convert.ToInt32(tokens[i]) == clsTokens.Cond_IF || Convert.ToInt32(tokens[i]) == clsTokens.Cond_WHILE || Convert.ToInt32(tokens[i]) == clsTokens.Cond_ELSE || Convert.ToInt32(tokens[i]) == clsTokens.Llave_Cerrar || Convert.ToInt32(tokens[i]) == clsTokens.MAIN) // si el token es un if, while, else o llave cerrar
                {
                    tkn_ifelse.Add(tokens_code[i]); // se agrega el token a la lista de tokens de if y else
                    lista_ifelse.Add(contenido_code[i]); // se agrega el contenido a la lista de contenido de if y else
                    lista_lineas.Add(lineas_code[i]); // se agrega la linea a la lista de lineas de if y else
                }
            }
            int num = BscarFin_IF(ref linea_finElse); // se busca el fin del if
            if (num > 0) // si el numero es mayor a 0
            {
                linea_finIf = num; // se asigna el numero a la linea fin del if
            }
        }
        int linea_finIf; // linea fin del if
        int linea_finElse; // linea fin del else
        private int BscarFin_IF(ref int linea_else) // metodo para buscar el fin del if
        {
            int ln = 0; // variable para guardar la linea
            Stack<int> stack = new Stack<int>(); // pila para guardar los tokens
            for (int i = 0; i < tkn_ifelse.Count; i++) // recorrido de los tokens de if y else
            {
                if (Convert.ToInt32(tkn_ifelse[i]) == clsTokens.Cond_ELSE) // si el token es un else
                {
                    ln = Convert.ToInt32(lista_lineas[i - 1]); // se guarda la linea
                    linea_finElse = Convert.ToInt32(lista_lineas[i + 1]); // se guarda la linea del else
                }
            }
            return ln; // se retorna la linea
        }
        /// <summary>
        /// Método utilizado para generar código ASM y retornarlo en una cadena
        /// </summary>
        /// <returns>Regresa la cadena de codigo generada </returns>
        public string Codigo_ASM() // metodo para generar el codigo asm
        {
            string codigo = ""; // variable para guardar el codigo
            codigo += ConvertirVariables(token_variables, variables, contenido_vars); // se convierten las variables
            codigo += BeginCode(); // se agrega el inicio del codigo
            codigo += LeerCodigo(); // se agrega el codigo
            codigo += EndCode(); // se agrega el fin del codigo
            return codigo; // se retorna el codigo
        }

        private string ConvertirVariables(List<string> tkn, List<string> vars, List<string> contenido) // metodo para convertir las variables
        {
            string code = ".Model Small\n.Stack 100h\n.Data\nBUFFER_NUMEROS DB 6 DUP(?)\r\nNL DB 0DH, 0AH, 24H"; // se agrega el modelo
            string ln, val; // variables para guardar el nombre y el valor
            code += "buffer db 12 dup('$')\n"; // se agrega el buffer
            for (int i = 0; i < vars.Count; i++) // recorrido de las variables
            {
                ln = vars[i]; // se guarda el nombre
                val = contenido[i]; // se guarda el valor
                code += Convert.ToInt32(tkn[i]) == clsTokens.Cadena ? $"{ln} dw {val}\n" : $"{ln} dw ?\n"; // se agrega la variable
            }
            return code; // se retorna el codigo
        }

        private string BeginCode() => $"\n.Code \nMain Proc \nmov ax, @Data \nmov ds, ax\n\nIMPRIMIRNUMERO MACRO VALOR\nMOV AX, VALOR\nMOV CX, 10\nMOV SI, OFFSET BUFFER_NUMEROS + 5\nMOV BYTE PTR [SI], 24H\nCONVERT_LOOP:\nDEC SI\nXOR DX, DX\nDIV CX\nADD DL, 30H\nMOV BYTE PTR [SI], DL\nTEST AX, AX\nJNZ CONVERT_LOOP\nMOV AH, 09H\nMOV DX, SI\nINT 21H\nENDM\n\n"; // metodo para agregar el inicio del codigo
        private string EndCode() => "mov ah, 4ch \nint 21h \nmain endp \nend main"; // metodo para agregar el fin del codigo
        int ubicacion_else; // ubicacion del else
        private string LeerCodigo() // metodo para leer el codigo
        {
            var tupla = CodigoLineas(contenido, lineas); // se obtiene la tupla de las lineas
            string[] tokens = tupla.Item1.Split('\n'); // se separan los tokens
            string[] codigo = tupla.Item2.Split('\n'); // se separa el codigo
            string[] ln; // variable para guardar la linea
            string[] tk; // variable para guardar el token
            int[] tkInt; // variable para guardar el token en entero
            char[] separadores = { ' ' }; // separadores
            string lineaCodigo = ""; // variable para guardar la linea de codigo
            string code = ""; // variable para guardar el codigo
            bool hay_If = false; // variable para saber si hay un if
            for (int i = 1; i < codigo.Length; i++) // recorrido del codigo
            {
                ln = codigo[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries); // se separa la linea
                tk = tokens[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries); // se separan los tokens
                tkInt = tk.Select(x => Convert.ToInt32(x)).ToArray(); // se convierten los tokens a enteros
                for (int j = 0; j < tk.Length; j++) // recorrido de los tokens
                {
                    if (Convert.ToInt32(tk[j]) == clsTokens.MAIN || Convert.ToInt32(tk[j]) == clsTokens.let) // si el token es un main o un let
                        break;
                    if (Convert.ToInt32(tk[j]) == clsTokens.Write && Convert.ToInt32(tk[j + 2]) == clsTokens.Cadena) // si el token es un write y el siguiente es una cadena
                    {
                        lineaCodigo = ImprimirMsg(); // se imprime el mensaje
                        code += lineaCodigo; // se agrega la linea de codigo
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == clsTokens.Writeln && Convert.ToInt32(tk[j + 2]) == clsTokens.Cadena) // si el token es un writeln y el siguiente es una cadena
                    {
                        code += "mov ah, 02h\nmov dl, 0dh\nint 21h\nmov dl, 0ah\nint 21h\n"; // se agrega el salto de linea
                        lineaCodigo = ImprimirMsg(); // se imprime el mensaje
                        code += lineaCodigo; // se agrega la linea de codigo
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == clsTokens.Write) // si el token es un write
                    {
                        lineaCodigo = AsignarValoresVars(tkInt, ln.ToArray()); // se asignan los valores a las variables
                        code += lineaCodigo + "\n"; // se agrega la linea de codigo
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == clsTokens.Writeln) // si el token es un writeln
                    {
                        lineaCodigo = AsignarValoresVars(tkInt, ln.ToArray()); // se asignan los valores a las variables
                        code += lineaCodigo + "\n"; // se agrega la linea de codigo
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == clsTokens.Variable) // si el token es una variable
                    {
                        string str = ""; // variable para guardar la linea
                        for (int k = 2; k < ln.Length - 1; k++) // recorrido de la linea
                        {
                            str += ln[k] + " "; // se agrega la linea
                        }
                        Dictionary<string, int> dicionarioTokens = CrearDiccionarioTokens(ln, tkInt); // se crea el diccionario de tokens
                        List<string> arrPostfix = Postfijo.ConvertirExpresion(str).Split(separadores, StringSplitOptions.RemoveEmptyEntries).ToList(); // se convierte la expresion a postfijo
                        arrPostfix.Add(ln[1]); // se agrega el token
                        arrPostfix.Add(ln[0]); // se agrega el token
                        arrPostfix.Add(ln[ln.Length - 1]); // se agrega el token
                        int[] arrTkn = RecuperarTokens(arrPostfix.ToArray(), dicionarioTokens); // se recuperan los tokens
                        lineaCodigo = AsignarValoresVars(arrTkn, arrPostfix.ToArray()); // se asignan los valores a las variables
                        code += lineaCodigo + "\n"; // se agrega la linea de codigo
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == clsTokens.Cond_IF) // si el token es un if
                    {
                        code += EvaluarOperador(tk, ln, $"entra_if{contador_if}"); // se evalua el if
                        PilaControl($"empieza_else{contador_else}:\n", clsTokens.Cond_IF); // se agrega a la pila de control
                        code += $"\nentra_if{contador_if}:\n"; // se agrega la linea de codigo
                        contador_if++; // se aumenta el contador
                        contador_else++; // se aumenta el contador
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == clsTokens.Cond_ELSE) // si el token es un else
                    {
                        hay_If = true; // hay un if
                        //contador_else--;
                        //code += $"jmp fin_if\n";
                        code += $"empieza_else{contador_else}:\n"; // se agrega la linea de codigo
                        //PilaControl($"sale_if{contador_if}", Convert.ToInt32(tk[j]));
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == clsTokens.Cond_WHILE) // si el token es un while
                    {
                        code += $"while{contador_while}:\n"; // se agrega la linea de codigo
                        code += EvaluarOperador(tk, ln); // se evalua el while
                        PilaControl($"jmp while{contador_while}\nfin_while{contador_while}:", clsTokens.Cond_WHILE); // se agrega a la pila de control
                        contador_while++; // se aumenta el contador
                        break;
                    }
                    if (i == linea_finIf - 1) // si la linea es igual a la linea fin del if
                        code += $"jmp fin_if\n"; // se agrega la linea de codigo
                    if (i == linea_finElse - 1) // si la linea es igual a la linea fin del else
                        code += $"fin_if:\n"; // se agrega la linea de codigo
                    if (Convert.ToInt32(tk[j]) == clsTokens.Llave_Cerrar) // si el token es una llave cerrar
                    {
                        string str = PilaControl("", clsTokens.Llave_Cerrar); // se obtiene la linea de codigo
                        code += str + "\n"; // se agrega la linea de codigo
                    }
                }
            }
            return code; // se retorna el codigo
        }

        private Dictionary<string, int> CrearDiccionarioTokens(string[] lexema, int[] tkn_var) // metodo para crear el diccionario de tokens
        {
            Dictionary<string, int> diccionario = new Dictionary<string, int>(); // se crea el diccionario
            for (int i = 0; i < lexema.Length; i++) // recorrido de los lexemas
            {
                string tkn = lexema[i]; // se guarda el lexema
                int valor = tkn_var[i]; // se guarda el token
                diccionario[tkn] = valor; // se agrega al diccionario
            }
            return diccionario; // se retorna el diccionario
        }
        private int[] RecuperarTokens(string[] lista1, Dictionary<string, int> diccionario) // metodo para recuperar los tokens
        {
            List<int> lista2 = new List<int>(); // lista para guardar los tokens
            int[] arr; // arreglo para guardar los tokens
            foreach (string token in lista1) // recorrido de los tokens
            {
                if (diccionario.TryGetValue(token, out int valor)) // si se encuentra el token
                    lista2.Add(valor); // se agrega a la lista
                else
                    continue;
            }
            arr = lista2.ToArray(); // se convierte la lista a arreglo
            return arr; // se retorna el arreglo
        }

        private string AsignarValoresVars(int[] tokens, string[] exp) // metodo para asignar valores a las variables
        {
            string ax = "", bx = "", cx = "", dx = ""; // variables para guardar los valores
            string code = ""; // variable para guardar el codigo
            string var = exp[exp.Length - 2]; // variable para guardar el nombre de la variable
            List<string> expre = new List<string>(); // lista para guardar la expresion
            List<int> tkn = new List<int>(); // lista para guardar los tokens
            for (int i = 0; i < tokens.Length; i++) // recorrido de los tokens
            {
                /* Suma 104   Resta 105   Multi 106   Div 107 */
                /* cadena  126  Num 101    dec 102  */
                if (tokens[i] == clsTokens.NoEntero || tokens[i] == clsTokens.Variable) // si el token es un numero o una variable
                {
                    if (string.IsNullOrEmpty(ax)) // si ax esta vacio
                    {
                        ax = exp[i]; // se guarda el valor
                        code += $"\nmov ax, {ax}"; // se agrega la linea de codigo
                    }
                    else // si ax no esta vacio
                    {
                        bx = exp[i]; // se guarda el valor
                        code += $"\nmov bx, {bx}"; // se agrega la linea de codigo
                    }
                }
                if (tokens[i] == clsTokens.Suma)// suma
                {
                    code += $"\nadd ax, bx"; // se agrega la linea de codigo
                    //code += $"\nmov {var}, ax";
                }
                if (tokens[i] == clsTokens.Resta) //resta
                {
                    code += $"\nsub ax, bx"; // se agrega la linea de codigo
                    //code += $"\nmov {var}, ax";
                }
                if (tokens[i] == clsTokens.Multiplicacion) //multiplicacion
                {
                    if (!string.IsNullOrEmpty(bx)) // si bx no esta vacio
                    {
                        code += $"\nmul bx"; // se agrega la linea de codigo
                        //code += $"\nmov {var}, ax";
                    }
                }
                if (tokens[i] == clsTokens.Division) //division
                {
                    if (!string.IsNullOrEmpty(bx)) // si bx no esta vacio
                    {
                        code += $"\ndiv bx"; // se agrega la linea de codigo
                        //code += $"\nmov {var}, ax";
                    }
                }
                if (tokens[i] == clsTokens.Igual) //igual
                {
                    code += $"\nmov {var}, ax"; // se agrega la linea de codigo
                    break;
                }
                if (tokens[i] == clsTokens.Write || tokens[i] == clsTokens.Writeln) //write o writeln
                {
                    while (tokens[i] != clsTokens.PuntoyComa) // mientras el token no sea un punto y coma
                    {
                        i++; // se aumenta el contador
                        expre.Add(exp[i]); // se agrega la expresion
                        tkn.Add(tokens[i]); // se agrega el token
                    }
                    code += Imprimir(tkn, expre); // se imprime
                }
            }
            return code; // se retorna el codigo
        }
        int cont = 0; // contador
        private string ImpVariosNums(string var) => $"IMPRIMIRNUMERO {var}\n"; // metodo para imprimir varios numeros

        private string Imprimir(List<int> tokens, List<string> exp) // metodo para imprimir
        {
            string imp = ""; // variable para guardar el valor
            string code = ""; // variable para guardar el codigo
            for (int i = 0; i < tokens.Count; i++) // recorrido de los tokens
            {
                if (tokens[i] == clsTokens.Variable) // si el token es una variable
                {
                    imp = exp[i]; // se guarda el valor
                    break;
                }
                else
                    continue;
            }
            //code += ImpVariosNums(imp);
            cont++; // se aumenta el contador
            code = $"\nmov dx, {imp}\nadd dx, 30h\nmov ah, 02h\nint 21h\n"; // se agrega la linea de codigo
            return code; // se retorna el codigo
        }

        int messg = 0; // contador
        private string ImprimirMsg() // metodo para imprimir mensaje
        {
            string msg = $"msg{messg}"; // variable para guardar el mensaje
            string code = $"\nmov ah, 09h\nlea dx, {msg}\nint 21h\nlea dx, NL\nint 21h\n"; // se agrega la linea de codigo
            messg++; // se aumenta el contador
            return code; // se retorna el codigo
        }

        public Tuple<string, string> CodigoLineas(List<string> contenido, List<string> linea) // metodo para obtener las lineas
        {
            int ln = 1; // variable para guardar la linea
            string lineas = ""; // variable para guardar las lineas
            string tknsLn = ""; // variable para guardar los tokens de las lineas
            for (int i = 0; i < linea.Count; i++) // recorrido de las lineas
            {
                if (ln == Convert.ToInt32(linea[i])) // si la linea es igual a la linea
                {
                    lineas += contenido[i] + " "; // se agrega la linea
                    tknsLn += tokens[i] + " "; // se agrega el token
                }
                if (i != linea.Count - 1) // si el contador es diferente al tamaño de la lista
                {
                    if (ln != Convert.ToInt32(linea[i + 1])) // si la linea es diferente a la linea
                    {
                        lineas += "\n"; // se agrega el salto de linea
                        tknsLn += "\n"; // se agrega el salto de linea
                        ln++; // se aumenta el contador
                    }
                }
            }
            return Tuple.Create(tknsLn, lineas); // se retorna la tupla
        }

        int contador_while = 1; // contador
        int contador_if = 1; // contador
        int contador_else = 1; // contador
        private string EvaluarIf(string[] arr_tokens, string[] arr_lexema, bool hay_else) // metodo para evaluar el if
        {
            string code = EvaluarOperador(arr_tokens, arr_lexema, $"entra_if{ContarIfs(contador_if)}"); // se evalua el operador
            return code; // se retorna el codigo
        }
        private string EvaluarOperador(string[] arrTokens, string[] arrLexema, string entra_if) // metodo para evaluar el operador
        {
            string code = "", op1 = "", op2 = ""; // variables para guardar los valores
            for (int i = 0; i < arrTokens.Length; i++) // recorrido de los tokens
            {
                if (Convert.ToInt32(arrTokens[i]) == clsTokens.EQUAL_TO)//==
                {
                    op1 = arrLexema[i - 1]; // se guarda el valor
                    op2 = arrLexema[i + 1]; // se guarda el valor
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\nje {entra_if}\n"; // se agrega la linea de codigo
                }
                if (Convert.ToInt32(arrTokens[i]) == clsTokens.Not_EQUAL)//!=
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njne {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == clsTokens.Mayor)//>
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njg {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == clsTokens.MayorIgual)//>=
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njge {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == clsTokens.Menor)//<
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njl {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == clsTokens.MenorIgual)//<=
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njle {entra_if}\n";
                }
            }
            code += $"jmp empieza_else{contador_else}\n"; // se agrega la linea de codigo
            return code; // se retorna el codigo
        }

        int cantidad_while = 1; // contador

        private string EvaluarOperador(string[] arrTokens, string[] arrLexema) // metodo para evaluar el operador
        {
            string code = "", op1 = "", op2 = ""; // variables para guardar los valores
            string ongoing = $"fin_while{contador_while}"; // variable para guardar el fin del while
            for (int i = 0; i < arrTokens.Length; i++) // recorrido de los tokens
            {
                switch (Convert.ToInt32(arrTokens[i])) // switch para evaluar los tokens
                {
                    case 111: // == is
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njne {ongoing}\n";
                        break;

                    case 115: // != nop
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\nje {ongoing}\n";
                        break;

                    case 107: // >
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njle {ongoing}\n";
                        break;

                    case 108: // >=
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njl {ongoing}\n";
                        break;

                    case 109: // <
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njge {ongoing}\n";
                        break;

                    case 110: // <=
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njg {ongoing}\n";
                        break;
                }
            }
            return code;
        }

        private int ContarIfs(int cant) // metodo para contar los if
        {
            Stack<int> cantidadIfs = new Stack<int>(); // pila para guardar los if
            cantidadIfs.Push(cant); // se agrega el if
            return cantidadIfs.Pop(); // se retorna el if
        }

        Stack<string> pila_control = new Stack<string>(); // pila para guardar el control
        private string PilaControl(string str, int tkn) // metodo para agregar a la pila de control
        {
            string res = ""; // variable para guardar el valor
            if (tkn == clsTokens.Cond_IF || tkn == clsTokens.Cond_ELSE || tkn == clsTokens.Cond_WHILE) // si el token es un if, else o while
                pila_control.Push(str); // se agrega a la pila
            if (pila_control.Any()) // si la pila tiene algo
            {
                if (tkn == clsTokens.Llave_Cerrar) // si el token es una llave cerrar
                    res = pila_control.Pop(); // se saca de la pila
            }
            return res; // se retorna el valor
        }
    }
}

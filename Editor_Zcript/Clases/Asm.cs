using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor_Zcript.Clases
{
    class Asm
    {
        List<string> token_variables = new List<string>();
        List<string> variables = new List<string>();
        List<string> contenido_vars = new List<string>();
        List<string> tokens = new List<string>();
        List<string> lineas = new List<string>();
        List<string> contenido = new List<string>();


        List<string> tkn_ifelse = new List<string>();
        List<string> lista_ifelse = new List<string>();
        List<string> lista_lineas = new List<string>();
        public Asm(List<string> variables, List<string> tokens_code, List<string> contenido_code, List<string> lineas_code, List<string> tokens_variables, List<string> contenido_variables)
        {
            this.variables = variables;
            this.tokens = tokens_code;
            this.contenido = contenido_code;
            this.lineas = lineas_code;
            this.token_variables = tokens_variables;
            this.contenido_vars = contenido_variables;


            for (int i = 0; i < tokens.Count; i++)
            {
                if (Convert.ToInt32(tokens[i]) == 205 || Convert.ToInt32(tokens[i]) == 209 || Convert.ToInt32(tokens[i]) == 207 || Convert.ToInt32(tokens[i]) == 221 || Convert.ToInt32(tokens[i]) == 224)
                {
                    tkn_ifelse.Add(tokens_code[i]);
                    lista_ifelse.Add(contenido_code[i]);
                    lista_lineas.Add(lineas_code[i]);
                }
            }
        }

        public string Codigo_ASM()
        {
            string codigo = "";
            codigo += ConvertirVariables(token_variables, variables, contenido_vars);
            codigo += BeginCode();
            codigo += LeerCodigo();
            codigo += EndCode();
            return codigo;
        }

        private string ConvertirVariables(List<string> tkn, List<string> vars, List<string> contenido)
        {
            string code = ".Model Small\n.Stack 100h\n.Data\n";
            string ln, val;
            code += "buffer db 12 dup('$')\n";
            for (int i = 0; i < vars.Count; i++)
            {
                ln = vars[i];
                val = contenido[i];
                code += Convert.ToInt32(tkn[i]) == Cls_Tokens.Cadena ? $"{ln} dw {val}\n" : $"{ln} dw ?\n";
            }
            return code;
        }

        private string BeginCode() => $"\n.Code \nMain Proc \nmov ax, @Data \nmov ds, ax \n";
        private string EndCode() => "mov ah, 4ch \nint 21h \nmain endp \nend main";
        int ubicacion_else;
        public string LeerCodigo()
        {
            var tupla = CodigoLineas(contenido, lineas);
            string[] tokens = tupla.Item1.Split('\n');
            string[] codigo = tupla.Item2.Split('\n');
            string[] ln;
            string[] tk;
            int[] tkInt;
            char[] separadores = { ' ' };
            string lineaCodigo = "";
            string code = "";
            bool hay_if = false;
            bool hay_else = false;
            bool if_anidado = false;
            bool else_anidado = false;
            bool hay_While = false;
            bool termina_if = false;
            List<string> codigo_limpio = new List<string>();
            List<string> tokens_limpio = new List<string>();
            List<string> lineas_ifElse = new List<string>();
            for (int i = 0; i < codigo.Length; i++)
            {
                if (string.IsNullOrEmpty(codigo[i]))
                {
                    continue;
                }
                else
                {
                    codigo_limpio.Add(codigo[i]);
                    tokens_limpio.Add(tokens[i]);
                }
            }
            for (int i = 0; i < codigo_limpio.Count; i++)
            {
                ln = codigo_limpio[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                tk = tokens_limpio[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                tkInt = tk.Select(x => Convert.ToInt32(x)).ToArray();
                for (int j = 0; j < tk.Length; j++)
                {
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Amogus || Convert.ToInt32(tk[j]) == Cls_Tokens.Decl)
                    {
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Write && Convert.ToInt32(tk[j + 2]) == Cls_Tokens.Cadena)
                    {
                        lineaCodigo = ImprimirMsg();
                        code += lineaCodigo;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Writeln && Convert.ToInt32(tk[j + 2]) == Cls_Tokens.Cadena)
                    {
                        code += "mov ah, 02h\nmov dl, 0dh\nint 21h\nmov dl, 0ah\nint 21h\n";
                        lineaCodigo = ImprimirMsg();
                        code += lineaCodigo;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Write)
                    {
                        lineaCodigo = AsignarValoresVars(tkInt, ln.ToArray());
                        code += lineaCodigo + "\n";
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Writeln)
                    {
                        lineaCodigo = AsignarValoresVars(tkInt, ln.ToArray());
                        code += lineaCodigo + "\n";
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Variable)
                    {
                        string str = "";
                        for (int k = 2; k < ln.Length - 1; k++)
                        {
                            str += ln[k] + " ";
                        }
                        Dictionary<string, int> dicionarioTokens = CrearDiccionarioTokens(ln, tkInt);
                        List<string> arrPostfix = Postfijo.ConvertirExpresion(str).Split(separadores, StringSplitOptions.RemoveEmptyEntries).ToList();
                        arrPostfix.Add(ln[1]);
                        arrPostfix.Add(ln[0]);
                        arrPostfix.Add(ln[ln.Length - 1]);
                        int[] arrTkn = RecuperarTokens(arrPostfix.ToArray(), dicionarioTokens);
                        lineaCodigo = AsignarValoresVars(arrTkn, arrPostfix.ToArray());
                        code += lineaCodigo + "\n";
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Condit)//if
                    {
                        if (hay_if == true)
                        {
                            if_anidado = true;
                            cant_if_anidados++;
                            code += "\n;---------------------------------------------------AQUI EMPIEZA UN IF ANIDADO-----------------------------------------------------\n";
                        }
                        hay_if = true;
                        if (if_anidado == false)
                        {
                            for (int x = i; x < tkn_ifelse.Count; x++)
                            {
                                if (Convert.ToInt32(tkn_ifelse[i]) == Cls_Tokens.Choice)
                                {
                                    lineas_ifElse.Add(lista_lineas[i]);
                                    hay_else = true;
                                    ubicacion_else = x;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int x = i; x < ubicacion_else - 1; x++)
                            {
                                if (Convert.ToInt32(tkn_ifelse[i]) == Cls_Tokens.Choice)
                                {
                                    lineas_ifElse.Add(lista_lineas[i]);
                                    else_anidado = true;
                                    break;
                                }
                            }
                        }
                        string str = "";
                        code += "\n;---------------------------------------------------AQUI EMPIEZA UN IF-----------------------------------------------------\n";
                        str = EvaluarIf(tk, ln, hay_else);
                        str += $"entra_if{ContarIfs(contador_if)}:\n";
                        code += str;
                        contador_if++;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Endit && hay_if == true && if_anidado == true)
                    {
                        code += $"jmp fin_if{ContarIfs(contador_if)}\nfin_if{ContarIfs(contador_if)}:\n";
                        if_anidado = false;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Endit && hay_if == true && hay_While == false)
                    {
                        // code += $"jmp fin_if{ContarIfs(contador_if)}:\n";
                        code += "\n;---------------------------------------------------AQUI TERMINA UN IF-----------------------------------------------------\n";
                        cant_if_anidados--;
                        hay_if = false;
                        termina_if = true;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Choice && termina_if == true)//else
                    {
                        string str = $"\nempieza_else{contador_else}:\n";
                        code += "\n;---------------------------------------------------AQUI EMPIEZA UN ELSE-----------------------------------------------------\n";
                        code += str;
                        hay_else = true;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Endit && hay_else == true && termina_if == true)
                    {
                        code += $"\njmp fin_else{contador_else}";
                        code += $"\nfin_else{contador_else}:\nfin_if{ContarIfs(contador_if)}:\n";
                        code += "\n;---------------------------------------------------AQUI TERMINA UN ELSE-----------------------------------------------------\n";
                        hay_else = false;
                        contador_else++;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Ongoing)//while
                    {
                        code += "\n;---------------------------------------------------AQUI EMPIEZA UN WHILE-----------------------------------------------------\n";
                        code += $"\nwhile{cantidad_while}:";
                        code += EvaluarOperador(tk, ln);
                        hay_While = true;
                        break;
                    }
                    if (Convert.ToInt32(tk[j]) == Cls_Tokens.Endit && hay_While == true)
                    {
                        code += $"jmp while{cantidad_while}\nfin_while{cantidad_while}:\n";
                        code += "\n;---------------------------------------------------AQUI TERMINA UN WHILE-----------------------------------------------------\n";
                        hay_While = false;
                        cantidad_while++;
                        break;
                    }
                }
            }
            return code;
        }

        private Dictionary<string, int> CrearDiccionarioTokens(string[] lexema, int[] tkn_var)
        {
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            for (int i = 0; i < lexema.Length; i++)
            {
                string tkn = lexema[i];
                int valor = tkn_var[i];
                diccionario[tkn] = valor;
            }
            return diccionario;
        }
        private int[] RecuperarTokens(string[] lista1, Dictionary<string, int> diccionario)
        {
            List<int> lista2 = new List<int>();
            int[] arr;
            foreach (string token in lista1)
            {
                if (diccionario.TryGetValue(token, out int valor))
                {
                    lista2.Add(valor);
                }
                else
                {
                    continue;
                }
            }
            arr = lista2.ToArray();
            return arr;
        }

        private string AsignarValoresVars(int[] tokens, string[] exp)
        {
            string ax = "", bx = "", cx = "", dx = "";
            string code = "";
            string var = exp[exp.Length - 2];
            List<string> expre = new List<string>();
            List<int> tkn = new List<int>();
            for (int i = 0; i < tokens.Length; i++)
            {
                /* Suma 104   Resta 105   Multi 106   Div 107 */
                /* cadena  126  Num 101    dec 102  */
                if (tokens[i] == Cls_Tokens.NoEntero || tokens[i] == Cls_Tokens.Variable)
                {
                    if (string.IsNullOrEmpty(ax))
                    {
                        ax = exp[i];
                        code += $"\nmov ax, {ax}";
                    }
                    else
                    {
                        bx = exp[i];
                        code += $"\nmov bx, {bx}";
                    }
                }
                if (tokens[i] == Cls_Tokens.Suma)// suma
                {
                    code += $"\nadd ax, bx";
                    //code += $"\nmov {var}, ax";
                }
                if (tokens[i] == Cls_Tokens.Resta) //resta
                {
                    code += $"\nsub ax, bx";
                    //code += $"\nmov {var}, ax";
                }
                if (tokens[i] == Cls_Tokens.Multiplicacion) //multiplicacion
                {
                    if (!string.IsNullOrEmpty(bx))
                    {
                        code += $"\nmul bx";
                        //code += $"\nmov {var}, ax";
                    }
                }
                if (tokens[i] == Cls_Tokens.Division) //division
                {
                    if (!string.IsNullOrEmpty(bx))
                    {
                        code += $"\ndiv bx";
                        //code += $"\nmov {var}, ax";
                    }
                }
                if (tokens[i] == Cls_Tokens.Igual)
                {
                    code += $"\nmov {var}, ax";
                    break;
                }
                if (tokens[i] == Cls_Tokens.Write || tokens[i] == Cls_Tokens.Writeln)
                {
                    while (tokens[i] != Cls_Tokens.PuntoyComa)
                    {
                        i++;
                        expre.Add(exp[i]);
                        tkn.Add(tokens[i]);
                    }
                    code += Imprimir(tkn, expre);
                }
            }
            return code;
        }
        int cont = 0;
        private string ImpVariosNums(string var) => $"\nmov {var}, ax\r\nmov bx, 10 \r\nmov si, offset buffer +11\r\nmov ax,{var}\r\nmov cl, 0 \r\nconvertir{cont}: \r\nxor dx, dx; \r\ndiv bx\r\nadd dl, '0' \r\ndec si\r\n mov [si],dl \r\ntest ax, ax\r\njnz convertir{cont} \r\nmov dx, si \r\nmov ah, 9 \r\nint 21h ";

        private string Imprimir(List<int> tokens, List<string> exp)
        {
            string imp = "";
            string code = ""; ;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == Cls_Tokens.Variable)
                {
                    imp = exp[i];
                    break;
                }
                else
                    continue;
            }
            //code += ImpVariosNums(imp);
            cont++;
            code = $"\nmov dx, {imp}\nadd dx, 30h\nmov ah, 02h\nint 21h\n";
            return code;
        }

        int messg = 0;
        private string ImprimirMsg()
        {
            string msg = $"msg{messg}";
            string code = $"\nmov ah, 09h\nlea dx, {msg}\nint 21h\n";
            messg++;
            return code;
        }

        public Tuple<string, string> CodigoLineas(List<string> contenido, List<string> linea)
        {
            int ln = 1;
            string lineas = "";
            string tknsLn = "";
            for (int i = 0; i < linea.Count; i++)
            {
                if (ln == Convert.ToInt32(linea[i]))
                {
                    lineas += contenido[i] + " ";
                    tknsLn += tokens[i] + " ";
                }
                if (i != linea.Count - 1)
                {
                    if (ln != Convert.ToInt32(linea[i + 1]))
                    {
                        lineas += "\n";
                        tknsLn += "\n";
                        ln++;
                    }
                }
            }
            return Tuple.Create(tknsLn, lineas);
        }
        int cant_if_anidados = 0;
        int contador_if = 1;
        int contador_else = 1;
        private string EvaluarIf(string[] arr_tokens, string[] arr_lexema, bool hay_else)
        {
            string code = EvaluarOperador(arr_tokens, arr_lexema, $"entra_if{ContarIfs(contador_if)}", hay_else);
            return code;
        }
        private string EvaluarIfAnidado(string[] arr_tokens, string[] arr_lexema, bool hay_else)
        {
            string code = EvaluarOperador(arr_tokens, arr_lexema, $"entra_if{ContarIfs(contador_if)}", hay_else);
            return code;
        }

        private string EvaluarOperador(string[] arrTokens, string[] arrLexema, string entra_if, bool hay_else)
        {
            string code = "", op1 = "", op2 = "";
            for (int i = 0; i < arrTokens.Length; i++)
            {
                if (Convert.ToInt32(arrTokens[i]) == Cls_Tokens.Is)//==
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\nje {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == Cls_Tokens.Nop)//!=
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njne {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == Cls_Tokens.Mayor)//>
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njg {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == 111)//>=
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njge {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == Cls_Tokens.Menor)//<
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njl {entra_if}\n";
                }
                if (Convert.ToInt32(arrTokens[i]) == Cls_Tokens.MenorIgual)//<=
                {
                    op1 = arrLexema[i - 1];
                    op2 = arrLexema[i + 1];
                    code += $"\nmov ax, {op2}\ncmp {op1}, ax\njle {entra_if}\n";
                }
            }
            code += hay_else == true ? $"jmp empieza_else{contador_else}\n" : $"jmp fin_if{ContarIfs(contador_if)}\n";
            return code;
        }

        int cantidad_while = 1;

        private string EvaluarOperador(string[] arrTokens, string[] arrLexema)
        {
            string code = "", op1 = "", op2 = "";
            string ongoing = $"fin_while{cantidad_while}";
            for (int i = 0; i < arrTokens.Length; i++)
            {
                switch (Convert.ToInt32(arrTokens[i]))
                {
                    case 231: // == is
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njne {ongoing}\n";
                        break;

                    case 216: // != nop
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\nje {ongoing}\n";
                        break;

                    case 110: // >
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njle {ongoing}\n";
                        break;

                    case 111: // >=
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njl {ongoing}\n";
                        break;

                    case 112: // <
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njge {ongoing}\n";
                        break;

                    case 113: // <=
                        op1 = arrLexema[i - 1];
                        op2 = arrLexema[i + 1];
                        code += $"\nmov ax, {op2}\ncmp {op1},ax\njg {ongoing}\n";
                        break;
                }
            }
            return code;
        }

        private int ContarIfs(int cant)
        {
            Stack<int> cantidadIfs = new Stack<int>();
            cantidadIfs.Push(cant);
            return cantidadIfs.Pop();
        }
    }
}

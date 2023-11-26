using System;
using System.Collections.Generic;
using System.Data;

namespace Editor_Zcript.Clases
{
    class Cls_Semantico
    {
        List<string> tokens = new List<string>();
        List<string> nombreTkn = new List<string>();
        List<string> tipo = new List<string>();
        List<string> ln = new List<string>();
        List<string> errores = new List<string>();
        DataTable tablaVars = new DataTable();
        DataTable tablaExp = new DataTable();

        public Cls_Semantico(DataTable tkn)
        {
            foreach (DataRow row in tkn.Rows)
            {
                tokens.Add(row[0].ToString());
                nombreTkn.Add(row[1].ToString());
                ln.Add(row[2].ToString());
            };

            string type = "";
            var exp = new Func<int, string>((id) =>
            {
                switch (id)
                {
                    case 200: return "number";
                    case 201: return "decimal";
                    case 202: return "text";
                    default: return "null";
                }
            });
            for (int i = 0; i < tokens.Count; i++)
            {
                switch (Convert.ToInt32(tokens[i]))
                {
                    case 100: type = exp(i > 0 ? Convert.ToInt32(tokens[i - 1]) : 0); break;
                    case 200: type = "number"; break;
                    case 201: type = "decimal"; break;
                    case 202: type = "text"; break;
                    case 101: type = "no_entero"; break;
                    case 102: type = "no_decimal"; break;
                    case 103: type = "no_decimal"; break;
                    case 126: type = "cadena"; break;
                    case 205: type = "if"; break;
                    case 207: type = "else"; break;
                    case 209: type = "while"; break;
                    case 226: type = "declaracion"; break;
                    case 119: type = "fin ln"; break;
                    default: type = "null"; break;
                }
                tipo.Add(type);
            }
            tablaVars.Columns.Add("Token");
            tablaVars.Columns.Add("Nombre");
            tablaVars.Columns.Add("Tipo");
            tablaVars.Columns.Add("Contenido");
            tablaVars.Columns.Add("Ln");
        }

        List<string> tknVar = new List<string>();
        List<string> nombreVar = new List<string>();
        List<string> contVar = new List<string>();
        public void EvaluarVariables(ref string mssg)
        {
            List<string> tipoVar = new List<string>();
            List<string> lineaVar = new List<string>();
            string contActual, contDespues;
            Func<string, string> getLnVar = (variable) =>
            {
                string ln = "";
                //nombreVar.Contains(variable) ? lineaVar[Array.IndexOf(nombreVar.ToArray(), variable)] : "";
                for (int i = 0; i < nombreVar.Count; i++)
                {
                    if (variable == nombreVar[i])
                    {
                        ln = lineaVar[i];
                        break;
                    }
                }
                return ln;
            };
            Func<string, string> getTipoVar = (variable) =>
            {
                string type = "";
                for (int i = 0; i < nombreVar.Count; i++)
                {
                    if (variable == nombreVar[i])
                    {
                        type = tipoVar[i];
                        break;
                    }
                }
                return type;
            };
            Predicate<string> verificarExistencia = (variable) => nombreVar.Contains(variable) ? true : false;
            Action<string, string> asignarValor = (variable, valor) =>
            {
                for (int i = 0; i < nombreVar.Count; i++)
                {
                    if (variable == nombreVar[i])
                    {
                        contVar[i] = valor;
                    }
                }
            };
            Func<string, string> getNombreVar = (str) =>
            {
                for (int i = 0; i < nombreVar.Count; i++)
                {
                    if (nombreVar[i] == str)
                    {
                        return nombreVar[i];
                    }
                }
                return "";
            };
            Func<string, string> getValor = (variable) => nombreVar.Contains(variable) ? contVar[Array.IndexOf(nombreVar.ToArray(), variable)] : "null";
            Predicate<int> existeFunc = (tkn) => tkn == 210 ? true : false;
            Func<string, string> removerComillas = (valor) =>
            {
                char[] arr = { '\'' };
                string[] txt = valor.Split(arr, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(" ", txt);
            };
            Func<int, string> asignarTipo = (valor) =>
            {
                switch (valor)
                {
                    case 200: return "number";
                    case 201: return "decimal";
                    case 202: return "text";
                }
                return "";
            };

            for (int i = 2; i < tokens.Count; i++)
            {
                //Declaracion
                if (Convert.ToInt32(tokens[i]) == Cls_Tokens.Decl)
                {
                    if (verificarExistencia(this.nombreTkn[i + 2]))
                    {
                        errores.Add($"Error 500:{ln[i]} => La variable '{this.nombreTkn[i + 2]}' ya fue declarada en la linea '{getLnVar(this.nombreTkn[i + 2])}'");
                    }
                    else
                    {
                        tknVar.Add(tokens[i + 2]);
                        nombreVar.Add(this.nombreTkn[i + 2]);
                        tipoVar.Add(asignarTipo(Convert.ToInt32(tokens[i + 1])));
                        lineaVar.Add(ln[i]);
                        switch (nombreTkn[i + 1])
                        {
                            case "number":
                                contVar.Add("0");
                                break;
                            case "decimal":
                                contVar.Add("0.0");
                                break;
                            case "text":
                                contVar.Add("' '");
                                break;
                        }
                    }
                }
                //Todo lo relacionado con una variale
                if (Convert.ToInt32(tokens[i]) == Cls_Tokens.Variable)
                {
                    //Igualacion
                    if (Convert.ToInt32(tokens[i + 1]) == Cls_Tokens.Igual)
                    {
                        asignarValor(getNombreVar(nombreTkn[i]), nombreTkn[i + 2]);
                    }
                    if (verificarExistencia(nombreTkn[i]) == false)
                    {
                        errores.Add($"Error 502: {ln[i]} => La variable {nombreTkn[i]} no ha sido declardada en este contexto");
                    }
                }
                //Asignacion / Igualacion
                if (Convert.ToInt32(tokens[i]) == Cls_Tokens.Variable && Convert.ToInt32(tokens[i + 1]) == Cls_Tokens.Igual)
                {
                    contActual = this.nombreTkn[i];
                    contDespues = nombreTkn[i + 2];
                    if (nombreVar.Contains(contActual))
                    {
                        if (getTipoVar(contActual) == "number")
                        {
                            if (Convert.ToInt32(tokens[i + 2]) != Cls_Tokens.NoEntero)
                            {
                                if (tipo[i + 2] != "null")
                                {
                                    errores.Add($"Error 501: {ln[i]} => Se esta asignando un valor {tipo[i + 2]} a una variable tipo {getTipoVar(contActual)}");
                                }
                            }
                        }
                        if (getTipoVar(contActual) == "decimal")
                        {
                            if (Convert.ToInt32(tokens[i + 2]) != Cls_Tokens.NoDecimal)
                            {
                                if (tipo[i + 2] != "null")
                                {
                                    errores.Add($"Error 501: {ln[i]} => Se esta asignando un valor {tipo[i + 2]} a una variable tipo {getTipoVar(contActual)}");
                                }
                            }
                        }
                        if (getTipoVar(contActual) == "text")
                        {
                            if (Convert.ToInt32(tokens[i + 2]) != Cls_Tokens.Cadena)
                            {
                                if (tipo[i + 2] != "null")
                                {
                                    errores.Add($"Error 501: {ln[i]} => Se esta asignando un valor {tipo[i + 2]} a una variable tipo {getTipoVar(contActual)}");
                                }
                            }
                        }
                    }
                    //Tipos incompatibles OP Aritmeticos
                    if (Convert.ToInt32(tokens[i + 2]) == Cls_Tokens.Variable && Convert.ToInt32(tokens[i + 4]) == Cls_Tokens.Variable)
                    {
                        if (Convert.ToInt32(tokens[i + 3]) >= 104 && Convert.ToInt32(tokens[i + 3]) <= 107)
                        {
                            string var1 = nombreTkn[i + 2], var2 = nombreTkn[i + 4];
                            string val1, val2;

                            if (getTipoVar(var1) == getTipoVar(var2))
                            {//suma
                                if (Convert.ToInt32(tokens[i + 3]) == Cls_Tokens.Suma)
                                {
                                    switch (getTipoVar(var1))
                                    {
                                        case "decimal":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            //asignarValor(contActual, (Convert.ToDouble(val1) + Convert.ToDouble(val2)).ToString());
                                            break;
                                        case "number":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            //asignarValor(contActual, (Convert.ToInt32(val1) + Convert.ToInt32(val2)).ToString());
                                            break;
                                        case "text":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            //asignarValor(contActual, ($"'{removerComillas(val1)}{removerComillas(val2)}'"));
                                            break;
                                    }
                                }//resta
                                if (Convert.ToInt32(tokens[i + 3]) == Cls_Tokens.Resta)
                                {
                                    switch (getTipoVar(var1))
                                    {
                                        case "decimal":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            //asignarValor(contActual, (Convert.ToDouble(val1) - Convert.ToDouble(val2)).ToString());
                                            break;
                                        case "number":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            //asignarValor(contActual, (Convert.ToInt32(val1) - Convert.ToInt32(val2)).ToString());
                                            break;
                                        case "text":
                                            errores.Add($"Error 503: Ln{ln[i]} Esta Operacion '{nombreTkn[i + 3]}' no es posible con datos tipo string");
                                            break;
                                    }
                                }//multiplicacion
                                if (Convert.ToInt32(tokens[i + 3]) == Cls_Tokens.Multiplicacion)
                                {
                                    switch (getTipoVar(var1))
                                    {
                                        case "decimal":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            asignarValor(contActual, (Convert.ToDouble(val1) * Convert.ToDouble(val2)).ToString());
                                            break;
                                        case "number":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            asignarValor(contActual, (Convert.ToInt32(val1) * Convert.ToInt32(val2)).ToString());
                                            break;
                                        case "text":
                                            errores.Add($"Error 503: Ln{ln[i]} Esta Operacion '{nombreTkn[i + 3]}' no es posible con datos tipo string");
                                            break;
                                    }
                                }//division
                                if (Convert.ToInt32(tokens[i + 3]) == Cls_Tokens.Division)
                                {
                                    switch (getTipoVar(var1))
                                    {
                                        case "decimal":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            if (Convert.ToInt32(val2) == 0)
                                                errores.Add($"Error 504: Ln {ln[i]} No es posible realizar una division entre 0");
                                            break;
                                        case "number":
                                            val1 = getValor(var1);
                                            val2 = getValor(var2);
                                            if (Convert.ToDouble(val2) == 0)
                                                errores.Add($"Error 504: Ln {ln[i]} No es posible realizar una division entre 0");
                                            break;
                                        case "text":
                                            errores.Add($"Error 503: Ln{ln[i]} Esta Operacion '{nombreTkn[i + 3]}' no es posible con datos tipo string");
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                errores.Add($"Error 504: Ln {ln[i]} => No se puede realizar una operacion aritmetica entre un {getTipoVar(var1)} y un {getTipoVar(var2)}");
                            }
                        }
                    }
                    if (Convert.ToInt32(tokens[i]) == Cls_Tokens.Condit)
                    {
                        if (Convert.ToInt32(tokens[i + 2]) == Cls_Tokens.Variable)
                        {
                            if (!verificarExistencia(nombreTkn[i + 1]))
                                errores.Add("Se esta Evaluando una variable que no existe dentro de un if");
                        }
                    }
                }
                if (Convert.ToInt32(tokens[i]) == Cls_Tokens.Write || Convert.ToInt32(tokens[i]) == Cls_Tokens.Writeln)
                {
                    if (Convert.ToInt32(tokens[i + 2]) == Cls_Tokens.Cadena)
                    {
                        string contenido_actual = nombreTkn[i + 2];
                        string cadena = removerComillas(contenido_actual);
                        string cadena_nueva = "'" + cadena + " $'";
                        tknVar.Add(126.ToString());
                        nombreVar.Add("msg" + msg);
                        tipoVar.Add("cadena");
                        contVar.Add(cadena_nueva);
                        lineaVar.Add(ln[i]);
                        msg++;
                    }
                }
            }
            for (int i = 0; i < tknVar.Count; i++)
            {
                tablaVars.Rows.Add(tknVar[i], nombreVar[i], tipoVar[i], contVar[i], lineaVar[i]);
            }
            for (int i = 0; i < errores.Count; i++)
            {
                mssg += errores[i] + "\n";
            }
        }
        int msg = 0;
        public string getExpresion(int i)
        {
            List<string> op = new List<string>();
            while (Convert.ToInt32(tokens[i]) != Cls_Tokens.PuntoyComa)
            {
                op.Add(nombreTkn[i]);
                i++;
            }
            return string.Join(" ", op);
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

        public List<string> getErroresSem() => errores;

        public DataTable getDatatable() => tablaVars;

        public Tuple<List<string>, List<string>, List<string>, List<string>, List<string>, List<string>> TuplaListas() => Tuple.Create(nombreVar, tokens, nombreTkn, ln, tknVar, contVar);
    }
}

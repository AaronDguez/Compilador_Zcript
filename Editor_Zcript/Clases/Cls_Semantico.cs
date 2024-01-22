using System;
using System.Collections.Generic;
using System.Data;

namespace Editor_Zcript.Clases
{
    class Semantico
    {
        List<string> tokens = new List<string>(); //Lista de tokens
        List<string> nombreTkn = new List<string>(); //Lista de nombres de tokens
        List<string> tipo = new List<string>(); //Lista de tipos de tokens
        List<string> ln = new List<string>(); //Lista de lineas de tokens
        List<string> errores = new List<string>(); //Lista de errores
        DataTable tablaVars = new DataTable(); //Tabla de variables
        DataTable tablaExp = new DataTable(); //Tabla de expresiones

        public Semantico(DataTable tkn) //Constructor
        {
            foreach (DataRow row in tkn.Rows) //Recorre las filas de la tabla de tokens
            {
                tokens.Add(row[0].ToString()); //Agrega el token a la lista de tokens
                nombreTkn.Add(row[1].ToString()); //Agrega el nombre del token a la lista de nombres de tokens
                ln.Add(row[2].ToString()); //Agrega la linea del token a la lista de lineas de tokens
            };

            string type = ""; //Variable que almacena el tipo de token
            var exp = new Func<int, string>((id) => //Funcion que devuelve el tipo de expresion
            {
                switch (id) //Switch que devuelve el tipo de expresion
                {
                    case 200: return "entero"; //Si el token es 200, devuelve "entero"
                    case 201: return "decimal"; //Si el token es 201, devuelve "decimal"
                    case 202: return "cadena"; //Si el token es 202, devuelve "cadena"
                    default: return "null"; //Si no, devuelve "null"
                }
            });
            for (int i = 0; i < tokens.Count; i++) //Recorre la lista de tokens
            {
                switch (Convert.ToInt32(tokens[i])) //Switch que devuelve el tipo de token
                {
                    case 100: type = exp(i > 0 ? Convert.ToInt32(tokens[i - 1]) : 0); break; //Si el token es 100, devuelve el tipo de expresion
                    case 200: type = "entero"; break; //Si el token es 200, devuelve "entero"
                    case 201: type = "decimal"; break; //Si el token es 201, devuelve "decimal"
                    case 202: type = "cadena"; break; //Si el token es 202, devuelve "cadena"
                    case 101: type = "no_entero"; break; //Si el token es 101, devuelve "no_entero"
                    case 102: type = "no_decimal"; break; //Si el token es 102, devuelve "no_decimal"
                    case 103: type = "no_decimal"; break; //Si el token es 103, devuelve "no_decimal"
                    case 126: type = "cadena"; break; //Si el token es 126, devuelve "cadena"
                    case 205: type = "if"; break; //Si el token es 205, devuelve "if"
                    case 207: type = "else"; break; //Si el token es 207, devuelve "else"
                    case 209: type = "while"; break; //Si el token es 209, devuelve "while"
                    case 226: type = "declaracion"; break; //Si el token es 226, devuelve "declaracion"
                    case 119: type = "fin ln"; break; //Si el token es 119, devuelve "fin ln"
                    default: type = "null"; break; //Si no, devuelve "null"
                }
                tipo.Add(type); //Agrega el tipo de token a la lista de tipos de tokens
            }
            tablaVars.Columns.Add("Token"); //Agrega la columna "Token" a la tabla de variables
            tablaVars.Columns.Add("Nombre"); //Agrega la columna "Nombre" a la tabla de variables
            tablaVars.Columns.Add("Tipo"); //Agrega la columna "Tipo" a la tabla de variables
            tablaVars.Columns.Add("Contenido"); //Agrega la columna "Contenido" a la tabla de variables
            tablaVars.Columns.Add("Ln"); //Agrega la columna "Ln" a la tabla de variables
        }

        List<string> tknVar = new List<string>(); //Lista de tokens de variables
        List<string> nombreVar = new List<string>(); //Lista de nombres de variables
        List<string> contVar = new List<string>(); //Lista de contenidos de variables
        public void EvaluarVariables(ref string mssg) //Funcion que evalua las variables
        {
            List<string> tipoVar = new List<string>(); //Lista de tipos de variables
            List<string> lineaVar = new List<string>(); //Lista de lineas de variables
            string contActual, contDespues; //Variables que almacenan el contenido de la variable actual y la variable siguiente
            Func<string, string> getLnVar = (variable) => //Funcion que devuelve la linea de la variable
            {
                string ln = "";
                //nombreVar.Contains(variable) ? lineaVar[Array.IndexOf(nombreVar.ToArray(), variable)] : "";
                for (int i = 0; i < nombreVar.Count; i++) //Recorre la lista de nombres de variables
                {
                    if (variable == nombreVar[i]) //Si el nombre de la variable es igual al nombre de la variable en la posicion i
                    {
                        ln = lineaVar[i]; //Almacena la linea de la variable
                        break; //Rompe el ciclo
                    }
                }
                return ln; //Devuelve la linea de la variable
            };
            Func<string, string> getTipoVar = (variable) => //Funcion que devuelve el tipo de la variable
            {
                string type = "";
                for (int i = 0; i < nombreVar.Count; i++) //Recorre la lista de nombres de variables
                {
                    if (variable == nombreVar[i]) //Si el nombre de la variable es igual al nombre de la variable en la posicion i
                    {
                        type = tipoVar[i]; //Almacena el tipo de la variable
                        break; //Rompe el ciclo
                    }
                }
                return type; //Devuelve el tipo de la variable
            };
            Predicate<string> verificarExistencia = (variable) => nombreVar.Contains(variable) ? true : false; //Funcion que verifica si la variable existe
            Action<string, string> asignarValor = (variable, valor) => //Funcion que asigna un valor a una variable
            {
                for (int i = 0; i < nombreVar.Count; i++) //Recorre la lista de nombres de variables
                {
                    if (variable == nombreVar[i]) //Si el nombre de la variable es igual al nombre de la variable en la posicion i
                        contVar[i] = valor; //Asigna el valor a la variable
                }
            };
            Func<string, string> getNombreVar = (str) => //Funcion que devuelve el nombre de la variable
            {
                for (int i = 0; i < nombreVar.Count; i++) //Recorre la lista de nombres de variables
                {
                    if (nombreVar[i] == str) //Si el nombre de la variable en la posicion i es igual al nombre de la variable
                        return nombreVar[i]; //Devuelve el nombre de la variable
                }
                return ""; //Si no, devuelve ""
            };
            Func<string, string> getValor = (variable) => nombreVar.Contains(variable) ? contVar[Array.IndexOf(nombreVar.ToArray(), variable)] : "null"; //Funcion que devuelve el valor de la variable
            Predicate<int> existeFunc = (tkn) => tkn == 210 ? true : false; //Funcion que verifica si existe una funcion
            Func<string, string> removerComillas = (valor) => //Funcion que remueve las comillas de una cadena
            {
                char[] arr = { '\'' }; //Arreglo de caracteres que contiene las comillas
                string[] txt = valor.Split(arr, StringSplitOptions.RemoveEmptyEntries); //Arreglo de cadenas que contiene el valor de la cadena sin comillas
                return string.Join(" ", txt); //Devuelve el valor de la cadena sin comillas
            };
            Func<int, string> asignarTipo = (valor) => //Funcion que asigna el tipo de variable
            {
                switch (valor) //Switch que devuelve el tipo de variable
                {
                    case 200: return "entero"; //Si el token es 200, devuelve "entero"
                    case 201: return "decimal"; //Si el token es 201, devuelve "decimal"
                    case 202: return "cadena"; //Si el token es 202, devuelve "cadena"
                }
                return ""; //Si no, devuelve ""
            };

            for (int i = 2; i < tokens.Count - 1; i++) //Recorre la lista de tokens
            {
                //Declaracion
                if (Convert.ToInt32(tokens[i]) == clsTokens.let) //Si el token es 226
                {
                    if (verificarExistencia(this.nombreTkn[i + 2])) //Si la variable ya existe
                        errores.Add($"Error 500:{ln[i]} => La variable '{this.nombreTkn[i + 2]}' ya fue declarada en la linea '{getLnVar(this.nombreTkn[i + 2])}'"); //Agrega el error a la lista de errores
                    else //Si no
                    {
                        string nombre_variable = nombreTkn[i + 1]; //Almacena el nombre de la variable
                        string contenido_variable = nombreTkn[i + 3]; //Almacena el contenido de la variable
                        int tkn_Var = Convert.ToInt32(tokens[i + 3]); //Almacena el token de la variable
                        string tipo = "nada"; //Almacena el tipo de la variable
                        switch (tkn_Var) //Switch que devuelve el tipo de variable
                        {
                            case 101:
                                tipo = "entero";
                                break;
                            case 126:
                                tipo = "cadena";
                                break;
                            case 102:
                                tipo = "decimal";
                                break;
                        }

                        tknVar.Add(tkn_Var.ToString()); //Agrega el token de la variable a la lista de tokens de variables
                        nombreVar.Add(nombre_variable); //Agrega el nombre de la variable a la lista de nombres de variables
                        tipoVar.Add(tipo); //Agrega el tipo de la variable a la lista de tipos de variables
                        lineaVar.Add(ln[i]); //Agrega la linea de la variable a la lista de lineas de variables
                        contVar.Add(contenido_variable); //Agrega el contenido de la variable a la lista de contenidos de variables
                    }
                }
                //Todo lo relacionado con una variable ya declarada
                if (Convert.ToInt32(tokens[i]) == clsTokens.Variable) //Si el token es 100
                {
                    //Igualacion
                    if (Convert.ToInt32(tokens[i + 1]) == clsTokens.Igual) //Si el token siguiente es 103
                        asignarValor(getNombreVar(nombreTkn[i]), nombreTkn[i + 2]); //Asigna el valor a la variable
                    if (verificarExistencia(nombreTkn[i]) == false) //Si la variable no existe
                        errores.Add($"Error 502: {ln[i]} => La variable {nombreTkn[i]} no ha sido declardada en este contexto"); //Agrega el error a la lista de errores
                }
                //Asignacion / Igualacion
                if (Convert.ToInt32(tokens[i]) == clsTokens.Variable && Convert.ToInt32(tokens[i + 1]) == clsTokens.Igual) //Si el token es 100 y el token siguiente es 103
                {
                    contActual = this.nombreTkn[i]; //Almacena el nombre de la variable actual
                    contDespues = nombreTkn[i + 2]; //Almacena el nombre de la variable siguiente
                    if (getTipoVar(contActual) == "nada") //Si el tipo de la variable es "nada"
                    {
                        int tknss = Convert.ToInt32(tokens[i + 2]); //Almacena el token de la variable siguiente
                        string tipo = "entero"; //Almacena el tipo de la variable
                        switch (tknss) //Switch que devuelve el tipo de variable
                        {
                            case 101:
                                tipo = "entero"; //Si el token es 101, el tipo de la variable es "entero"
                                break;
                            case 126:
                                tipo = "cadena"; //Si el token es 126, el tipo de la variable es "cadena"
                                break;
                            case 102:
                                tipo = "decimal"; //Si el token es 102, el tipo de la variable es "decimal"
                                break;
                        }
                        for (int v = 0; v < nombreVar.Count; v++) //Recorre la lista de nombres de variables
                        {
                            if (nombreVar[v].ToString() == contActual) //Si el nombre de la variable en la posicion v es igual al nombre de la variable actual
                            {
                                tipoVar[v] = tipo; //Asigna el tipo de la variable
                                break;
                            }
                        }
                    }
                    if (nombreVar.Contains(contActual)) //Si la variable actual existe
                    {
                        if (getTipoVar(contActual) == "entero") //Si el tipo de la variable es "entero"
                        {
                            if (Convert.ToInt32(tokens[i + 2]) != clsTokens.NoEntero) //Si el token de la variable siguiente no es 101
                            {
                                if (tipo[i + 2] != "null") //Si el tipo de la variable siguiente no es "null"
                                    errores.Add($"Error 501: {ln[i]} => Se esta asignando un valor {tipo[i + 2]} a una variable tipo {getTipoVar(contActual)}"); //Agrega el error a la lista de errores
                            }
                        }
                        if (getTipoVar(contActual) == "decimal") //Si el tipo de la variable es "decimal"
                        {
                            if (Convert.ToInt32(tokens[i + 2]) != clsTokens.NoDecimal) //Si el token de la variable siguiente no es 102
                            {
                                if (tipo[i + 2] != "null") //Si el tipo de la variable siguiente no es "null"
                                    errores.Add($"Error 501: {ln[i]} => Se esta asignando un valor {tipo[i + 2]} a una variable tipo {getTipoVar(contActual)}"); //Agrega el error a la lista de errores
                            }
                        }
                        if (getTipoVar(contActual) == "cadena") //Si el tipo de la variable es "cadena"
                        {
                            if (Convert.ToInt32(tokens[i + 2]) != clsTokens.Cadena) //Si el token de la variable siguiente no es 126
                            {
                                if (tipo[i + 2] != "null") //Si el tipo de la variable siguiente no es "null"
                                    errores.Add($"Error 501: {ln[i]} => Se esta asignando un valor {tipo[i + 2]} a una variable tipo {getTipoVar(contActual)}"); //Agrega el error a la lista de errores
                            }
                        }
                    }
                    if (Convert.ToInt32(tokens[i + 2]) == clsTokens.NoEntero && Convert.ToInt32(tokens[i + 4]) == clsTokens.NoEntero) //Si el token de la variable siguiente es 101 y el token de la variable despues de la siguiente es 101
                    {
                        if (Convert.ToInt32(tokens[i + 3]) >= 104 && Convert.ToInt32(tokens[i + 3]) <= 107) //Si el token de la variable despues de la siguiente es un operador aritmetico
                        {
                            string var1 = nombreTkn[i + 2], var2 = nombreTkn[i + 4]; //Almacena el nombre de la variable siguiente y el nombre de la variable despues de la siguiente
                            int val1, val2; //Almacena el valor de la variable siguiente y el valor de la variable despues de la siguiente
                            if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Suma) //Si el token de la variable despues de la siguiente es 104
                            {
                                val1 = Convert.ToInt32(var1); //Almacena el valor de la variable siguiente
                                val2 = Convert.ToInt32(var2); //Almacena el valor de la variable despues de la siguiente
                                asignarValor(contActual, (Convert.ToInt32(val1) + Convert.ToInt32(val2)).ToString()); //Asigna el valor a la variable
                            }
                            if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Resta) //Si el token de la variable despues de la siguiente es 105
                            {
                                val1 = Convert.ToInt32(var1); //Almacena el valor de la variable siguiente
                                val2 = Convert.ToInt32(var2); //Almacena el valor de la variable despues de la siguiente
                                asignarValor(contActual, (Convert.ToInt32(val1) - Convert.ToInt32(val2)).ToString()); //Asigna el valor a la variable
                            }
                            if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Multiplicacion) //Si el token de la variable despues de la siguiente es 106
                            {
                                val1 = Convert.ToInt32(var1); //Almacena el valor de la variable siguiente
                                val2 = Convert.ToInt32(var2); //Almacena el valor de la variable despues de la siguiente
                                asignarValor(contActual, (Convert.ToInt32(val1) * Convert.ToInt32(val2)).ToString()); //Asigna el valor a la variable
                            }
                            if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Division) //Si el token de la variable despues de la siguiente es 107
                            {
                                val1 = Convert.ToInt32(var1); //Almacena el valor de la variable siguiente
                                val2 = Convert.ToInt32(var2); //Almacena el valor de la variable despues de la siguiente
                                if (val2 != 0) //Si el valor de la variable despues de la siguiente es diferente de 0
                                    asignarValor(contActual, (Convert.ToInt32(val1) / Convert.ToInt32(val2)).ToString()); //Asigna el valor a la variable
                                else //Si no
                                    errores.Add($"Error 504: Ln {ln[i]} No es posible realizar una division entre 0"); //Agrega el error a la lista de errores
                            }
                        }
                    }
                    //Tipos incompatibles OP Aritmeticos
                    if (Convert.ToInt32(tokens[i + 2]) == clsTokens.Variable && Convert.ToInt32(tokens[i + 4]) == clsTokens.Variable) //Si el token de la variable siguiente es 100 y el token de la variable despues de la siguiente es 100
                    {
                        if (Convert.ToInt32(tokens[i + 3]) >= 104 && Convert.ToInt32(tokens[i + 3]) <= 107) //Si el token de la variable despues de la siguiente es un operador aritmetico
                        {
                            string var1 = nombreTkn[i + 2], var2 = nombreTkn[i + 4]; //Almacena el nombre de la variable siguiente y el nombre de la variable despues de la siguiente
                            string val1, val2; //Almacena el valor de la variable siguiente y el valor de la variable despues de la siguiente
                            if (getTipoVar(var1) == getTipoVar(var2)) //Si el tipo de la variable siguiente es igual al tipo de la variable despues de la siguiente
                            {
                                if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Suma) //Si el token de la variable despues de la siguiente es 104                                
                                {
                                    switch (getTipoVar(var1)) //Switch que devuelve el tipo de variable
                                    {
                                        case "decimal": //Si el tipo de la variable es "decimal"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            //asignarValor(contActual, (Convert.ToDouble(val1) + Convert.ToDouble(val2)).ToString());
                                            break;
                                        case "entero": //Si el tipo de la variable es "entero"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            //asignarValor(contActual, (Convert.ToInt32(val1) + Convert.ToInt32(val2)).ToString());
                                            break;
                                        case "cadena": //Si el tipo de la variable es "cadena"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            //asignarValor(contActual, ($"'{removerComillas(val1)}{removerComillas(val2)}'"));
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Resta) //Si el token de la variable despues de la siguiente es 105
                                {
                                    switch (getTipoVar(var1)) //Switch que devuelve el tipo de variable
                                    {
                                        case "decimal": //Si el tipo de la variable es "decimal"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            //asignarValor(contActual, (Convert.ToDouble(val1) - Convert.ToDouble(val2)).ToString());
                                            break;
                                        case "entero": //Si el tipo de la variable es "entero"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            //asignarValor(contActual, (Convert.ToInt32(val1) - Convert.ToInt32(val2)).ToString());
                                            break;
                                        case "cadena": //Si el tipo de la variable es "cadena"
                                            errores.Add($"Error 503: Ln{ln[i]} Esta Operacion '{nombreTkn[i + 3]}' no es posible con datos tipo string"); //Agrega el error a la lista de errores
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Multiplicacion) //Si el token de la variable despues de la siguiente es 106
                                {
                                    switch (getTipoVar(var1)) //Switch que devuelve el tipo de variable
                                    {
                                        case "decimal": //Si el tipo de la variable es "decimal"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            asignarValor(contActual, (Convert.ToDouble(val1) * Convert.ToDouble(val2)).ToString());
                                            break;
                                        case "entero": //Si el tipo de la variable es "entero"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            asignarValor(contActual, (Convert.ToInt32(val1) * Convert.ToInt32(val2)).ToString());
                                            break;
                                        case "cadena": //Si el tipo de la variable es "cadena"
                                            errores.Add($"Error 503: Ln{ln[i]} Esta Operacion '{nombreTkn[i + 3]}' no es posible con datos tipo string"); //Agrega el error a la lista de errores
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(tokens[i + 3]) == clsTokens.Division) //Si el token de la variable despues de la siguiente es 107
                                {
                                    switch (getTipoVar(var1)) //Switch que devuelve el tipo de variable
                                    {
                                        case "decimal": //Si el tipo de la variable es "decimal"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            if (Convert.ToDouble(val2) == 0) //Si el valor de la variable despues de la siguiente es 0
                                                errores.Add($"Error 504: Ln {ln[i]} No es posible realizar una division entre 0"); //Agrega el error a la lista de errores
                                            break;
                                        case "entero": //Si el tipo de la variable es "entero"
                                            val1 = getValor(var1); //Almacena el valor de la variable siguiente
                                            val2 = getValor(var2); //Almacena el valor de la variable despues de la siguiente
                                            if (Convert.ToDouble(val2) == 0) //Si el valor de la variable despues de la siguiente es 0
                                                errores.Add($"Error 504: Ln {ln[i]} No es posible realizar una division entre 0"); //Agrega el error a la lista de errores
                                            break;
                                        case "cadena": //Si el tipo de la variable es "cadena"
                                            errores.Add($"Error 503: Ln{ln[i]} Esta Operacion '{nombreTkn[i + 3]}' no es posible con datos tipo string"); //Agrega el error a la lista de errores
                                            break;
                                    }
                                }
                            }
                            else //Si no
                                errores.Add($"Error 504: Ln {ln[i]} => No se puede realizar una operacion aritmetica entre un {getTipoVar(var1)} y un {getTipoVar(var2)}"); //Agrega el error a la lista de errores
                        }
                    }
                    if (Convert.ToInt32(tokens[i]) == clsTokens.Cond_IF) //Si el token es 205
                    {
                        if (Convert.ToInt32(tokens[i + 2]) == clsTokens.Variable) //Si el token de la variable siguiente es 100
                        {
                            if (!verificarExistencia(nombreTkn[i + 1])) //Si la variable no existe
                                errores.Add("Se esta Evaluando una variable que no existe dentro de un if"); //Agrega el error a la lista de errores
                        }
                    }
                }
                if (Convert.ToInt32(tokens[i]) == clsTokens.Write || Convert.ToInt32(tokens[i]) == clsTokens.Writeln) //Si el token es 203 o 204
                {
                    if (Convert.ToInt32(tokens[i + 2]) == clsTokens.Cadena) //Si el token de la variable siguiente es 126
                    {
                        string contenido_actual = nombreTkn[i + 2]; //Almacena el contenido de la variable siguiente
                        string cadena = removerComillas(contenido_actual); //Almacena el contenido de la variable siguiente sin comillas
                        string cadena_nueva = "'" + cadena + " $'"; //Almacena el contenido de la variable siguiente con comillas
                        tknVar.Add(126.ToString()); //Agrega el token de la variable a la lista de tokens de variables
                        nombreVar.Add("msg" + msg); //Agrega el nombre de la variable a la lista de nombres de variables
                        tipoVar.Add("cadena"); //Agrega el tipo de la variable a la lista de tipos de variables
                        contVar.Add(cadena_nueva); //Agrega el contenido de la variable a la lista de contenidos de variables
                        lineaVar.Add(ln[i]); //Agrega la linea de la variable a la lista de lineas de variables
                        msg++; //Aumenta el valor de la variable msg
                    }
                }
            }
            for (int i = 0; i < tknVar.Count; i++) //Recorre la lista de tokens de variables
            {
                tablaVars.Rows.Add(tknVar[i], nombreVar[i], tipoVar[i], contVar[i], lineaVar[i]); //Agrega una fila a la tabla de variables
            }
            for (int i = 0; i < errores.Count; i++) //Recorre la lista de errores
            {
                mssg += errores[i] + "\n"; //Agrega el error a la variable mssg
            }
        }
        int msg = 0; //Variable que almacena el valor de la variable msg
        public string getExpresion(int i) //Funcion que devuelve la expresion
        {
            List<string> op = new List<string>(); //Lista de operadores
            while (Convert.ToInt32(tokens[i]) != clsTokens.PuntoyComa) //Mientras el token sea diferente de 119
            {
                op.Add(nombreTkn[i]); //Agrega el nombre del token a la lista de operadores
                i++; //Aumenta el valor de i
            }
            return string.Join(" ", op); //Devuelve la expresion
        }
        public Tuple<string, string> CodigoLineas(List<string> contenido, List<string> linea) //Funcion que devuelve el codigo de las lineas
        {
            int ln = 1; //Variable que almacena el numero de linea
            string lineas = ""; //Variable que almacena el codigo de las lineas
            string tknsLn = ""; //Variable que almacena los tokens de las lineas
            for (int i = 0; i < linea.Count; i++) //Recorre la lista de lineas
            {
                if (ln == Convert.ToInt32(linea[i])) //Si el numero de linea es igual al numero de linea en la posicion i
                {
                    lineas += contenido[i] + " "; //Agrega el contenido de la linea a la variable lineas
                    tknsLn += tokens[i] + " "; //Agrega el token de la linea a la variable tknsLn
                }
                if (i != linea.Count - 1) //Si i es diferente de la ultima posicion de la lista de lineas
                {
                    if (ln != Convert.ToInt32(linea[i + 1])) //Si el numero de linea es diferente del numero de linea en la posicion i + 1
                    {
                        lineas += "\n"; //Agrega un salto de linea a la variable lineas
                        tknsLn += "\n"; //Agrega un salto de linea a la variable tknsLn
                        ln++; //Aumenta el valor de ln
                    }
                }
            }
            return Tuple.Create(tknsLn, lineas); //Devuelve el codigo de las lineas
        }

        public List<string> getErroresSem() => errores; //Funcion que devuelve la lista de errores

        public DataTable getDatatable() => tablaVars; //Funcion que devuelve la tabla de variables
        /// <summary>
        /// Tupla que contiene las listas con los resultados del analisis semantico
        /// </summary>
        /// <returns>Tupla que contiene las listas con los resultados del analisis semantico</returns>
        public Tuple<List<string>, List<string>, List<string>, List<string>, List<string>, List<string>> TuplaListas() => Tuple.Create(nombreVar, tokens, nombreTkn, ln, tknVar, contVar);
    }
}

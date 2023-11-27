using Editor_Zcript.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor_Zcript
{
    public partial class Editor_Main : Form
    {
        string defaultTitle = "Editor Zcript";
        //Dialogs
        OpenFileDialog OFD; SaveFileDialog SFD; FontDialog FD; ColorDialog CD;
        public Editor_Main()
        {
            InitializeComponent(); this.Text = defaultTitle; rtxt_Editor.ScrollBars = RichTextBoxScrollBars.Both; rtxt_Editor.WordWrap = false;
        }

        #region Eventos
        #region Lineas
        private void Editor_Main_Load(object sender, EventArgs e)
        {
            FD = new FontDialog(); CD = new ColorDialog();
            if (MessageBox.Show("¿Desea abrir un archivo existente?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                OpenFile();
            else
            {
                rtxt_Editor.Clear(); this.Text = defaultTitle;
                //rtxt_Editor.LoadFile(@"C:\Users\aaron\OneDrive\Escritorio\Prueba_Compi.txt", RichTextBoxStreamType.PlainText); rtxt_Editor.Visible = true; rtxt_Editor.Enabled = true;
            }
            LineasMostradas.Font = rtxt_Editor.Font;
            rtxt_Editor.Select();
            AddLines();
        }
        private void rtxt_Editor_TextChanged(object sender, EventArgs e)
        {
            if (rtxt_Editor.Text == "")
                AddLines();
        }
        private void Editor_Main_Resize(object sender, EventArgs e)
        {
            AddLines();
        }
        private void rtxt_Editor_SelectionChanged(object sender, EventArgs e)
        {
            Point pt = rtxt_Editor.GetPositionFromCharIndex(rtxt_Editor.SelectionStart);
            if (pt.X == 1)
                AddLines();
        }
        private void rtxt_Editor_VScroll(object sender, EventArgs e)
        {
            LineasMostradas.Text = "";
            AddLines();
            LineasMostradas.Invalidate();
        }
        private void rtxt_Editor_FontChanged(object sender, EventArgs e)
        {
            LineasMostradas.Font = rtxt_Editor.Font;
            rtxt_Editor.Select();
            AddLines();
        }
        private void LineasMostradas_MouseDown(object sender, MouseEventArgs e)
        {
            rtxt_Editor.Select();
            LineasMostradas.DeselectAll();
        }
        private void AddLines()
        {
            // create & set Point pt to (0,0)    
            Point pt = new Point(0, 0);
            // get First Index & First Line from richTextBox1    
            int First_Index = rtxt_Editor.GetCharIndexFromPosition(pt);
            int First_Line = rtxt_Editor.GetLineFromCharIndex(First_Index);
            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively    
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;
            // get Last Index & Last Line from richTextBox1    
            int Last_Index = rtxt_Editor.GetCharIndexFromPosition(pt);
            int Last_Line = rtxt_Editor.GetLineFromCharIndex(Last_Index);
            // set Center alignment to LineNumberTextBox    
            LineasMostradas.SelectionAlignment = HorizontalAlignment.Center;
            // set LineNumberTextBox text to null & width to getWidth() function value    
            LineasMostradas.Text = "";
            LineasMostradas.Width = anchura();
            // now add each line number to LineNumberTextBox upto last line    
            for (int i = First_Line; i <= Last_Line + 1; i++)
            {
                LineasMostradas.Text += i + 1 + "\n";
            }
        }
        private int anchura()
        {
            int linea = rtxt_Editor.Lines.Length;
            if (linea <= 99)
                return 20 + (int)rtxt_Editor.Font.Size;
            else if(linea <= 999)
                return 30 + (int)rtxt_Editor.Font.Size;
            return 50 + (int)rtxt_Editor.Font.Size;
        }
        #endregion

        #region Archivo
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtxt_Editor.Text))
            {
                if (MessageBox.Show("¿Desea guardar?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    SaveFile();
            }
            rtxt_Editor.Clear(); this.Text = defaultTitle; rtxt_Editor.Visible = true; rtxt_Editor.Enabled = true;
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e) => OpenFile();
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) => SaveFileAs();
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtxt_Editor.Text))
            {
                if (MessageBox.Show("¿Desea guardar?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    SaveFile();
            }
            rtxt_Editor.Clear(); rtxt_Editor.Enabled = false; rtxt_Editor.Visible = false;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtxt_Editor.Text))
            {
                if(MessageBox.Show("¿Desea guardar?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    SaveFile();
            }
            this.Close();
        }
        #endregion

        #region Edicion
        private void undoToolStripMenuItem_Click(object sender, EventArgs e) => rtxt_Editor.Undo();
        private void redoToolStripMenuItem_Click(object sender, EventArgs e) => rtxt_Editor.Redo();
        private void cutToolStripMenuItem_Click(object sender, EventArgs e) => rtxt_Editor.Cut();
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) => rtxt_Editor.Copy();
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) => rtxt_Editor.Paste();
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FD = new FontDialog();
            if(FD.ShowDialog() == DialogResult.OK)
                rtxt_Editor.Font= FD.Font;
        }
        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CD = new ColorDialog();
            if (CD.ShowDialog() == DialogResult.OK)
                rtxt_Editor.ForeColor = FD.Color;
        }
        #endregion

        #region Compilador
        private void lexicoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearDataGrid(dtgvLex); clearDataGrid(dtgvLexE);
            if (rtxt_Editor.Text != "")
            {
                DataTable LexResul = new DataTable(), LexErr = new DataTable();
                LexResul.Columns.Add("Token", typeof(int));
                LexResul.Columns.Add("Lexema", typeof(string));
                LexResul.Columns.Add("Linea", typeof(int));
                LexResul.Columns.Add("Estado", typeof(string));
                dtgvLex.DataSource = LexResul;
                LexErr.Columns.Add("Token", typeof(int));
                LexErr.Columns.Add("Lexema", typeof(string));
                LexErr.Columns.Add("Linea", typeof(int));
                LexErr.Columns.Add("Estado", typeof(string));
                dtgvLexE.DataSource = LexErr;
                Thread.Sleep(100);
                Cls_Lexico.verificar(rtxt_Editor.Text + '\n');
                foreach(var datos in Cls_Lexico.queue)
                {
                    if (datos.Item2 < 400)
                        LexResul.Rows.Add(datos.Item2, datos.Item1, datos.Item4, datos.Item3);
                    else
                        LexErr.Rows.Add(datos.Item2, datos.Item1, datos.Item4, datos.Item3);
                }
                //dtgvLex.DataSource = LexResul; dtgvLexE.DataSource = LexErr;
            }
            else
                MessageBox.Show("No hay codigo escrito.\r\nNo es posible ejecutar el analisis Léxico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void sintácticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtxt_Editor.Text != "")
            {
                clearDataGrid(dtgvSinEr);
                lexicoToolStripMenuItem_Click(sender, e); bool lexError = false;
                List<Tuple<int, int>> LexResul = new List<Tuple<int, int>>();
                foreach (var datos in Cls_Lexico.queue)
                {
                    if (datos.Item2 > 400)
                    {
                        lexError = true;
                        break;
                    }
                    LexResul.Add(new Tuple<int, int>(datos.Item2, datos.Item4));
                }
                if (!lexError)
                {
                    DataTable SinResul = new DataTable();
                    SinResul.Columns.Add("Token", typeof(int));
                    SinResul.Columns.Add("Error", typeof(string));
                    SinResul.Columns.Add("Linea", typeof(int));
                    dtgvSinEr.DataSource = SinResul;
                    Task t = new Task(() =>
                    {
                        Cls_Sintactico.Iniciar(LexResul);
                    }); t.Start(); Task.WaitAny(t);
                    Thread.Sleep(100);
                    foreach(var errores in Cls_Sintactico.OrdenErrores)
                    {
                        SinResul.Rows.Add(errores.Item1, errores.Item2, errores.Item3);
                    }
                }
                else
                    MessageBox.Show("Hay errorres Lexicos.\r\nNo es posible ejecutar el analisis Sintáctico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("No hay codigo escrito.\r\nNo es posible ejecutar el analisis Sintáctico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void semánticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lexicoToolStripMenuItem_Click(sender, e); bool error = false; string mssg = string.Empty;
            DataTable LexResul = new DataTable();
            LexResul.Columns.AddRange(new DataColumn[] { new DataColumn("Token", typeof(int)), new DataColumn("Palabra", typeof(string)), new DataColumn("Linea", typeof(int)) });
            if(dtgvSemEr.Rows.Count > 0)
                dtgvSemEr.Rows.Clear();
            foreach(var dato in Cls_Lexico.queue)
            {
                if(dato.Item2 > 400)
                {
                    error = true;
                    break;
                }
                LexResul.Rows.Add(dato.Item2, dato.Item1, dato.Item4); // Token, Palabra, Linea
            }
            if (!error)
            {
                Cls_Semantico semantico = new Cls_Semantico(LexResul);
                semantico.EvaluarVariables(ref mssg);

                dtgvSemEr.DataSource = semantico.getErroresSem();
                ensamblador();
            }
            else
                MessageBox.Show("Hay errores léxicos\nArregle esos errores antes de obtener el codigo objetivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ensamblador()
        {
            //Asm codigo = new Asm()
        }
        private void rtxt_Cod_DoubleClick(object sender, EventArgs e)
        {
            rtxt_Cod.Copy();
        }
        #endregion

        //Ayuda
        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor_AcercaDe mensaje = new Editor_AcercaDe(); mensaje.ShowDialog();
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Guarda el texto del rtxt en un archivo
        /// </summary>
        private void SaveFile()
        {
            try
            {
                if (!string.IsNullOrEmpty(rtxt_Editor.Text))
                {
                    if (this.Text == defaultTitle)
                    {
                        SFD = new SaveFileDialog(); SFD.Filter = "Text File(*.txt)|*.txt"; SFD.Title = "Guardar archivo";
                        if (SFD.ShowDialog() == DialogResult.OK)
                        {
                            rtxt_Editor.SaveFile(SFD.FileName, RichTextBoxStreamType.PlainText);
                            MessageBox.Show("Archivo guardado exitosamente", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                        File.WriteAllText(this.Text, rtxt_Editor.Text);
                }
                else
                {
                    MessageBox.Show("El archivo está vacío", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al intentar guardar el archivo\r\n{ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { SFD = null; }
        }
        /// <summary>
        /// Guarda el texto del rtxt como otro archivo
        /// </summary>
        private void SaveFileAs()
        {
            try
            {
                if (!string.IsNullOrEmpty(rtxt_Editor.Text))
                {
                    SFD = new SaveFileDialog(); SFD.Filter = "Text File(*.txt)|*.txt|All Files(*.*)|*.*";
                    if (SFD.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(SFD.FileName, rtxt_Editor.Text); this.Text = SFD.FileName;
                        MessageBox.Show("Archivo guardado exitosamente", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("El archivo está vacío", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al intentar guardar el archivo\r\n{ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { SFD = null; }
        }
        /// <summary>
        /// Abre el archivo seleccionado y lo imprime en el rtxt
        /// </summary>
        private void OpenFile()
        {
            try
            {
                OFD = new OpenFileDialog(); OFD.Filter = "Text Document(*.txt)|*.txt|All Files(*)|*.*"; OFD.Title = "Abrir Archivo";
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    rtxt_Editor.LoadFile(OFD.FileName, RichTextBoxStreamType.PlainText); rtxt_Editor.Visible = true; rtxt_Editor.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al intentar abrir el archivo\r\n{ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rtxt_Editor.Visible = false; rtxt_Editor.Enabled = false;
            }
            finally { OFD = null; }
        }
        private void clearDataGrid(DataGridView dtgv)
        {
            while(dtgv.Rows.Count > 0)
            {
                dtgv.Rows.RemoveAt(0);
            }
            while(dtgv.Columns.Count > 0)
            {
                dtgv.Columns.RemoveAt(0);
            }
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bimbo.DSD.UpdateAdmin
{
    /// <summary>
    /// Interaction logic for Administra.xaml
    /// </summary>
    public partial class Administra : Window
    {
        public List<Archivo> lArchivos = new List<Archivo>();
        public bool bArchivos = false;

        public Administra()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Menu wOwner = (Menu)this.Owner;

            System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
            myConn.ConnectionString = ((Menu)this.Owner).sConexion;
            string myQuery = "SELECT CODIGO_AGENCIA FROM HHc_Ruta Group By CODIGO_AGENCIA";
            OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
            myOdbcCommand.Connection = myConn;
            myConn.Open();
            OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);
            cmbAgencia.Items.Add("Selecciona una Agencia");
            while (myReader.Read())
            {
                cmbAgencia.Items.Add(myReader.GetString(0));
            }
            myConn.Close();
            cmbAgencia.SelectedIndex = 0;
        }

        private void FillCmbVersiones()
        {
            cmbVersion.Items.Clear();
            if (cmbAgencia.SelectedIndex != 0)
            {
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                string myQuery = "SELECT VERSION FROM HHc_ArchivosVersion Where CODIGO_AGENCIA = " + (string)cmbAgencia.SelectedValue + " Group By VERSION";
                OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                myOdbcCommand.Connection = myConn;
                myConn.Open();
                OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);
                cmbVersion.Items.Add("Selecciona una una version");
                while (myReader.Read())
                {
                    cmbVersion.Items.Add(myReader.GetString(0));
                }
                myConn.Close();
                cmbVersion.SelectedIndex = 0;
            }
        }

        private void FillCmbDispositivos()
        {
            cmbDispo.Items.Clear();
            if (cmbVersion.SelectedIndex != -1)
            {
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = ((Menu)this.Owner).sConexion;                
                string myQuery = "SELECT TIPO_DISPOSITIVO FROM HHc_ArchivosVersion Where CODIGO_AGENCIA = ? AND VERSION = ? GROUP BY TIPO_DISPOSITIVO";
                OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                myOdbcCommand.Connection = myConn;
                myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                myOdbcCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                myConn.Open();
                OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);
                cmbDispo.Items.Add("Selecciona un dispositivo");
                while (myReader.Read())
                {
                    cmbDispo.Items.Add(myReader.GetString(0));
                }
                myConn.Close();
                cmbDispo.SelectedIndex = 0;
            }
        }

        private void FillListArchivos()
        {
            lstArchivos.Items.Clear();            
            if ((cmbDispo.SelectedIndex != 0)&&(cmbDispo.SelectedIndex != -1))
            {
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                string myQuery = "SELECT ORDEN, NOMBRE, PATH_DESTINO, ACCION FROM HHc_ArchivosVersion Where CODIGO_AGENCIA = ? AND VERSION = ? AND  TIPO_DISPOSITIVO = ? ORDER BY ORDEN";
                OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                myOdbcCommand.Connection = myConn;
                myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                myOdbcCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                myOdbcCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = (string)cmbDispo.SelectedValue;
                myConn.Open();
                OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);                
                while (myReader.Read())
                {
                    Archivo aTmp = new Archivo();
                    aTmp.sShort = myReader.GetString(1);
                    aTmp.sDestino = myReader.GetString(2);
                    aTmp.option = myReader.GetInt32(3);                    
                    lstArchivos.Items.Add(aTmp);
                }
                myConn.Close();                
            }
        }

        private void cmbAgencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillCmbVersiones();
        }

        private void cmbVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillCmbDispositivos();
        }

        private void cmbDispo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillListArchivos();
            lstArchivos.SelectedIndex = 0;
            lstArchivos.Focus();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                if (ITCMessageBox.Show((Window)this, "Seguro que deseas eliminar el archivo?", "Cuidado!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int iTotal = lstArchivos.Items.Count;
                    int iSelec = lstArchivos.SelectedIndex + 1;
                    System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                    string sVer = (string)cmbVersion.SelectedValue;
                    string sAge = (string)cmbAgencia.SelectedValue;
                    string sDis = (string)cmbDispo.SelectedValue;
                    myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                    myConn.Open();
                    string myNonQuery = "DELETE FROM HHc_ArchivosVersion Where CODIGO_AGENCIA = ? AND VERSION = ? AND TIPO_DISPOSITIVO = ? AND ORDEN = ?";
                    OdbcCommand myCommand = new OdbcCommand(myNonQuery, myConn);
                    myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse(sAge);
                    myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = sVer;
                    myCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = sDis;
                    myCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = iSelec;
                    myCommand.ExecuteNonQuery();
                    lstArchivos.Items.RemoveAt(iSelec - 1);
                    for (int i = iSelec; i < iTotal; i++)
                    {
                        myNonQuery = "UPDATE HHc_ArchivosVersion SET ORDEN = ? WHERE CODIGO_AGENCIA = ? AND VERSION = ? AND TIPO_DISPOSITIVO = ? AND ORDEN = ?";
                        myCommand = new OdbcCommand(myNonQuery, myConn);
                        myCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = i;
                        myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse(sAge);
                        myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = sVer;
                        myCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = sDis;
                        myCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = i + 1;
                        myCommand.ExecuteNonQuery();
                    }
                    ITCMessageBox.Show(this, "Archivo eliminado", "Mensaje!");
                    myConn.Close();
                    lstArchivos.Focus();
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                ITCMessageBox.Show(this,"Error al intentar actualizar los archivos","Error!");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            int iTotal = lstArchivos.Items.Count;
            int iSelec = lstArchivos.SelectedIndex + 1;
            int iEsImpar = 0;

            Archivos view = new Archivos();
            view.Owner = this;
            view.bMain = true;
            view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            view.ShowDialog();
            
            if (!bArchivos)
            {
                ITCMessageBox.Show(this,"No agregaste Archivos","Error!");
                return;
            }
            int iTadd = lArchivos.Count;
            OdbcTransaction myTrans = null;

            this.IsEnabled = false;
            System.Threading.Thread t = new System.Threading.Thread(Sending);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start(getBase());

            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                string sVer = (string)cmbVersion.SelectedValue;
                string sAge = (string)cmbAgencia.SelectedValue;
                string sDis = (string)cmbDispo.SelectedValue;
                string sArchivo = string.Empty;
                myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                myConn.Open();
                string myNonQuery;
                OdbcCommand myCommand;
                myTrans = myConn.BeginTransaction();
                for (int i = iTotal; i >= iSelec; i--)
                {
                    myNonQuery = "UPDATE HHc_ArchivosVersion SET ORDEN = ? WHERE CODIGO_AGENCIA = ? AND VERSION = ? AND TIPO_DISPOSITIVO = ? AND ORDEN = ?";
                    myCommand = new OdbcCommand(myNonQuery, myConn);
                    myCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = i + iTadd;
                    myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse(sAge);
                    myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = sVer;
                    myCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = sDis;
                    myCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = i;
                    myCommand.Transaction = myTrans;
                    myCommand.ExecuteNonQuery();
                }
                int orden = iSelec;
                foreach (Archivo aFile in lArchivos)
                {
                    byte[] bFile = null;

                    System.IO.FileStream fLoad = null;
                    string sFname = System.IO.Path.GetFileName(aFile.sName);
                    if ((sFname != "") && (System.IO.Path.GetExtension(sFname) != ""))
                    {
                        fLoad = System.IO.File.Open(aFile.sName, FileMode.Open, FileAccess.Read);
                        bFile = new byte[fLoad.Length];
                        fLoad.Read(bFile, 0, (int)fLoad.Length);
                        fLoad.Close();

                        sArchivo = BitConverter.ToString(bFile);
                        sArchivo = sArchivo.Replace("-", "");
                        sArchivo = "0x" + sArchivo;

                        if ((bFile.Length % 2) == 1)
                        {
                            sArchivo += "00";
                            iEsImpar = 1;
                        }
                    }
                    else
                    {
                        sArchivo = "NULL";
                    }

                    

                    myNonQuery = "INSERT INTO HHc_ArchivosVersion VALUES (?, ?, ?, ?, ?, ?, ?, &, ?)";
                    myNonQuery = myNonQuery.Replace("&", sArchivo);
                    myCommand = new OdbcCommand(myNonQuery, myConn);
                    myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse(sAge);
                    myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = sVer;
                    myCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = (string)cmbDispo.SelectedValue;
                    myCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = orden;
                    myCommand.Parameters.Add("NOMBRE", OdbcType.VarChar).Value = aFile.sShort;
                    myCommand.Parameters.Add("PATH_DESTINO", OdbcType.VarChar).Value = aFile.sDestino;
                    myCommand.Parameters.Add("ACCION", OdbcType.Int).Value = aFile.option;
                    //myCommand.Parameters.Add("CONTENIDO", OdbcType.Binary).Value = bFile;
                    myCommand.Parameters.Add("ESIMPAR", OdbcType.Int).Value = iEsImpar;
                    myCommand.Transaction = myTrans;
                    myCommand.ExecuteNonQuery();
                    lstArchivos.Items.Insert(orden - 1, aFile);
                    orden++;
                }                
                myTrans.Commit();
                myConn.Close();
                t.Abort();
                ITCMessageBox.Show(this, "Archivos modificados", "Mensaje!");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                t.Abort();
                ITCMessageBox.Show(this, "Error al intentar modificar los archivos", "Error!");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            this.IsEnabled = true;
        }

        private void btnDescargar_Click(object sender, RoutedEventArgs e)
        {
            OdbcTransaction myTrans = null;
            try 
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                if (lstArchivos.SelectedIndex != -1)
                {
                    string selectedFolder = string.Empty;
                    FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
                    selectFolderDialog.ShowNewFolderButton = true;
                    selectFolderDialog.ShowDialog();
                    selectedFolder = selectFolderDialog.SelectedPath;
                    if (selectedFolder == string.Empty)
                    {
                        return;
                    }

                    System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                    myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                    myConn.Open();

                    myTrans = myConn.BeginTransaction();
                    string myNonQuery2 = "set textsize 2147483647";
                    OdbcCommand myCommand2 = new OdbcCommand(myNonQuery2, myConn);

                    string myQuery = "SELECT CONTENIDO FROM HHc_ArchivosVersion Where CODIGO_AGENCIA = ? AND VERSION = ? AND  TIPO_DISPOSITIVO = ? AND ORDEN = ?";
                    OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                    myOdbcCommand.Connection = myConn;
                    myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                    myOdbcCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                    myOdbcCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = (string)cmbDispo.SelectedValue;
                    myOdbcCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = lstArchivos.SelectedIndex + 1;
                    myOdbcCommand.Transaction = myTrans;                   
                    
                    myCommand2.Transaction = myTrans;
                    myCommand2.ExecuteNonQuery();

                    OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (myReader.Read())
                    {
                        byte[] bFile = null;
                        bFile = (byte[])myReader.GetValue(0);
                        Archivo tmp = (Archivo)lstArchivos.SelectedItem;
                        FileStream fileStream = new FileStream(selectedFolder + "\\" + tmp.sShort, FileMode.OpenOrCreate);
                        fileStream.Write(bFile, 0, bFile.GetLength(0));
                        fileStream.Close();
                    }
                    ITCMessageBox.Show(this, "Archivo descargado", "Mensaje!");
                    myConn.Close();
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                ITCMessageBox.Show(this, "Error al intentar descargar el archivo archivos", "Error!");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        private void Sending(object data)
        {
            Sending view = new Sending((string)data);
            view.ShowDialog();
        }

        private string getBase()
        {
            return base.Left.ToString() + "&" + base.Top.ToString() + "&" + base.RestoreBounds.Size.ToString();
        }
    }
}

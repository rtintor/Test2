using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.Odbc;

namespace Bimbo.DSD.UpdateAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class Agregar : Window
    {        
        public List<Archivo> lArchivos;
        public bool bArchivos = false;
        public bool bConexion = false;
        public string sConexion;
        int i = 0;        

        public Agregar()
        {
            InitializeComponent();
        }

        private string getBase()
        {
            return base.Left.ToString() + "&" + base.Top.ToString() + "&" + base.RestoreBounds.Size.ToString();
        }

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (txtAgencia.Text == "")
            {
                ITCMessageBox.Show((Window)this, "Necesitas Agregar una Agencia!", "Error!");
                txtAgencia.Focus();
            }
            else if(txtVersion.Text == "")
            {
                ITCMessageBox.Show((Window)this, "Necesitas Agregar una Version!", "Error!");
                txtVersion.Focus();
            }
            else if(bArchivos == false)
            {
                ITCMessageBox.Show((Window)this, "Necesitas Agregar los archivos de la version!", "Error!");
                btnAgregar.Focus();
            }            
            else if (cmbDispo.SelectedIndex == 0)
            {
                ITCMessageBox.Show((Window)this, "Necesitas agregar el tipo de dispositivo!", "Error!");
                cmbDispo.Focus();
            }
            else
            {                
                if (ITCMessageBox.Show((Window)this, "Segudo que deseas enviar la version?", "Cuidado!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.IsEnabled = false;
                    System.Threading.Thread t = new System.Threading.Thread(Sending);
                    t.SetApartmentState(System.Threading.ApartmentState.STA);
                    t.Start(getBase());
                    Enviar();
                    t.Abort();
                    if (i == 1)
                    {
                        ITCMessageBox.Show(this, "Envio Exitoso!!!", "Envio Exitoso!");
                    }
                    else
                    {
                        ITCMessageBox.Show(this, "Error al enviar los archivos!!!", "Error!");
                    }
                    this.IsEnabled = true;
                }                
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillDispositivos();            
            lArchivos = new List<Archivo>();            
            txtAgencia.Focus();
        }

        private System.Collections.SortedList GetSystemDataSourceNames()
        {
            System.Collections.SortedList dsnList = new System.Collections.SortedList();

            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBC.INI");
                    if (reg != null)
                    {
                        reg = reg.OpenSubKey("ODBC Data Sources");
                        if (reg != null)
                        {                           
                            foreach (string sName in reg.GetValueNames())
                            {                                                                                          
                                dsnList.Add(sName, sName);
                            }
                        }
                        try
                        {
                            reg.Close();
                        }
                        catch { /* ignore this exception if we couldn't close */ }
                    }
                }
            }

            return dsnList;
        }

        private System.Collections.SortedList GetUserDataSourceNames()
        {
            System.Collections.SortedList dsnList = new System.Collections.SortedList();

            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.CurrentUser).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBC.INI");
                    if (reg != null)
                    {
                        reg = reg.OpenSubKey("ODBC Data Sources");
                        if (reg != null)
                        {
                            foreach (string sName in reg.GetValueNames())
                            {
                                dsnList.Add(sName, sName);
                            }
                        }
                        try
                        {
                            reg.Close();
                        }
                        catch { /* ignore this exception if we couldn't close */ }
                    }
                }
            }

            return dsnList;
        }

        private System.Collections.SortedList GetAllDataSourceNames()
        {          
            System.Collections.SortedList dsnList = GetUserDataSourceNames();

            System.Collections.SortedList systemDsnList = GetSystemDataSourceNames();
            for (int i = 0; i < systemDsnList.Count; i++)
            {
                string sName = systemDsnList.GetKey(i) as string;
                string type = (string)systemDsnList.GetByIndex(i);
                try
                {                    
                    dsnList.Add(sName, type);
                }
                catch 
                { 
                    // An exception can be thrown if the key being added is a duplicate so 
                    // we just catch it here and have to ignore it.
                }
            }

            return dsnList;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            Archivos view = new Archivos();
            view.Owner = this;
            view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            view.ShowDialog();            
            FillFileList();
        }

        private string[] Dispositivos = { "Selecciona un dispositivo", "Intermec CN3A1A840G5E200", "Intermec 730B1E4004001000", "Motorola", "Samsung GT-B7300B", "Samsung GT-B6520L" };
        private string[] DispositivosCode = { "", "Intermec CN3A1A840G5E200", "Intermec 730B1E4004001000", "Motorola", "Samsung GT-B7300B", "Samsung GT-B6520L" };

        private void FillDispositivos()
        {
            foreach(string sDisp in Dispositivos)
            {                
                cmbDispo.Items.Add(sDisp);
            }
            cmbDispo.SelectedIndex = 0;
        }

        private void btnAgencia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = sConexion;                
                string myQuery = "SELECT CODIGO_AGENCIA FROM HHc_Ruta Group By CODIGO_AGENCIA";
                OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                myOdbcCommand.Connection = myConn;
                myConn.Open();
                OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (cmbAgencia.HasItems)
                {
                    cmbAgencia.Items.Clear();
                }
                cmbAgencia.Items.Add("Selecciona una Agencia");
                while (myReader.Read())
                {
                    cmbAgencia.Items.Add(myReader.GetString(0));
                }
                myConn.Close();
                cmbAgencia.SelectedIndex = 0;
                cmbAgencia.Visibility = System.Windows.Visibility.Visible;
                txtAgencia.Visibility = System.Windows.Visibility.Hidden;
                btnAgencia.IsEnabled = false;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message); 
            }
        }

        private void cmbAgencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtAgencia.Text = (string)cmbAgencia.SelectedValue;
        }

        private void FillFileList()
        {
            if (lArchivos.Count != 0)
            {                
                lstArchivos.Items.Clear();
                foreach (Archivo atemp in lArchivos)
                {
                    lstArchivos.Items.Add(atemp);
                }               
            }
        }

        private void Enviar()
        {            
            System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
            
            string sVer = (string)txtVersion.Text;
            string sAge = (string)txtAgencia.Text;
            string sDis = (string)cmbDispo.SelectedValue;

            int iEsImpar = 0;

            string sArchivo = string.Empty;
            
            // VALIDA QUE NO EXISTAN REGISTROS CON ESA AGENCIA, VERSION Y RUTA
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;            
            try
            {
                myConn.ConnectionString = sConexion;
                myConn.Open();
                OdbcTransaction myTrans = myConn.BeginTransaction();
                string myNonQuery2 = "set textsize 2147483647";
                OdbcCommand myCommand2 = new OdbcCommand(myNonQuery2, myConn);
                myCommand2.Transaction = myTrans;
                i = myCommand2.ExecuteNonQuery();

                string myQuery = "SELECT ORDEN FROM HHc_ArchivosVersion WHERE CODIGO_AGENCIA = ? AND VERSION = ? AND TIPO_DISPOSITIVO = ?";
                OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                myOdbcCommand.Connection = myConn;
                myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse(sAge);
                myOdbcCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = sVer;
                myOdbcCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = sDis;
                myOdbcCommand.Transaction = myTrans;
                OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (myReader.Read())
                {
                    string sMensaje = "ERROR!!! \nLa Agencia \"" + sAge + "\" \nLa Version \"" + sVer + "\"\nY el Dispositivo \"" + (string)cmbDispo.SelectedValue + "\" \nLA VVERSION YA EXISTE EN LA BD";
                    ITCMessageBox.Show((Window)this, sMensaje, "Error!");
                    return;
                }

                //INSERTA LA VERSION
                int orden = 1;
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

                    string myNonQuery = "INSERT INTO HHc_ArchivosVersion VALUES (?, ?, ?, ?, ?, ?, ?, &, ?)";
                    myNonQuery = myNonQuery.Replace("&", sArchivo);
                    OdbcCommand myCommand = new OdbcCommand(myNonQuery, myConn);
                    myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse(sAge);
                    myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = sVer;
                    myCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = sDis;
                    myCommand.Parameters.Add("ORDEN", OdbcType.Int).Value = orden;
                    myCommand.Parameters.Add("NOMBRE", OdbcType.VarChar).Value = aFile.sShort;
                    myCommand.Parameters.Add("PATH_DESTINO", OdbcType.VarChar).Value = aFile.sDestino;
                    myCommand.Parameters.Add("ACCION", OdbcType.Int).Value = aFile.option;
                    //myCommand.Parameters.Add("CONTENIDO", OdbcType.Binary).Value = bFile;
                    myCommand.Parameters.Add("ESIMPAR", OdbcType.Int).Value = iEsImpar;
                    myCommand.Transaction = myTrans;
                    i = myCommand.ExecuteNonQuery();
                    orden++;
                }
                myTrans.Commit();
                myConn.Close();
                i = 1;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            catch (Exception ex)
            {
                string m = ex.Message;
                i = 0;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }            
        }

        private void Sending(object data)
        {
            Sending view = new Sending((string)data);
            view.ShowDialog();
        }
    }


    public class Archivo
    { 
        public string sNameField;
        public string sShortField;
        public int optionField;
        public string sDestinoField;

        public string sName
        {
            get
            {
                return this.sNameField;
            }
            set
            {
                this.sNameField = value;
            }
        }

        public string sShort
        {
            get
            {
                return this.sShortField;
            }
            set
            {
                this.sShortField = value;
            }
        }

        public string sDestino
        {
            get
            {
                return this.sDestinoField;
            }
            set
            {
                this.sDestinoField = value;
            }
        }

        public int option
        {
            get
            {
                return this.optionField;
            }
            set
            {
                this.optionField = value;
            }
        }
    }
}

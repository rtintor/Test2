using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bimbo.DSD.UpdateAdmin
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Parametros : Window
    {
        public Parametros()
        {
            InitializeComponent();            
            txtHost.Focus();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if(txtHost.Text == "")
            {
                ITCMessageBox.Show(this, "Debes ingresar un Host", "Error!");
                txtHost.Focus();
            }
            else if (txtPort.Text == "")
            {
                ITCMessageBox.Show(this, "Debes ingresar un Puerto", "Error!");
                txtPort.Focus();
            }
            else if (txtUID.Text == "")
            {
                ITCMessageBox.Show(this, "Debes ingresar un Usuario", "Error!");
                txtUID.Focus();
            }            
            else
            {
                string sCon;


                sCon = "DRIVER={Adaptive Server Enterprise};";
                //sCon = "DRIVER={SQL ANYWHERE 11};";
                sCon += "server=" + txtHost.Text + ";";
                sCon += "port=" + txtPort.Text + ";";
                sCon += "uid=" + txtUID.Text + ";";
                if(txtPSW.Text != "")
                {
                    sCon += "pwd=" + txtPSW.Text + ";";
                }
                if (txtDB.Text != "")
                {
                    sCon += "db=" + txtDB.Text + ";";
                }
                //sCon += "DSN=\"\"";                

                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = sCon;
                try
                {
                    myConn.Open();
                    ((Menu)this.Owner).sConexion = sCon;
                    ((Menu)this.Owner).bConexion = true;
                    myConn.Close();
                    this.Close();
                }
                catch(Exception ex)
                {
                    ITCMessageBox.Show(this, "Error en los datos de conexión, no se pude realizar la conexion con los parametros seleccionados", "Error!");                    
                    txtHost.Focus();
                }                                
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((Menu)this.Owner).bConexion = false;
            GetSystemDriverList();
        }

        public static List<String> GetSystemDriverList()
        {
            List<string> names = new List<string>();
            // get system dsn's
            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBCINST.INI");
                    if (reg != null)
                    {

                        reg = reg.OpenSubKey("ODBC Drivers");
                        if (reg != null)
                        {
                            // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                            foreach (string sName in reg.GetValueNames())
                            {
                                names.Add(sName);
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

            return names;
        }
    }
}

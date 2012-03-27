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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bimbo.DSD.UpdateAdmin
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public string sConexion;
        public bool bConexion;        

        public Menu()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((sConexion == null) || (sConexion == ""))
            {
                ITCMessageBox.Show(this, "Debes configurar la conexion antes de continuar!", "Error!");
                cmbODBC.Focus();
            }
            else
            {
                Agregar view = new Agregar();
                view.Owner = this;
                view.sConexion = sConexion;
                view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                view.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sConexion == null)||(sConexion == ""))
            {
                ITCMessageBox.Show(this, "Debes configurar la conexion antes de continuar!", "Error!");
                cmbODBC.Focus();
            }
            else
            {
                Eliminar view = new Eliminar();
                view.Owner = this;
                view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                view.ShowDialog();
            }
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            if ((sConexion == null) || (sConexion == ""))
            {
                ITCMessageBox.Show(this, "Debes configurar la conexion antes de continuar!", "Error!");
                cmbODBC.Focus();
            }
            else 
            {
                Administra view = new Administra();
                view.Owner = this;
                view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                view.ShowDialog();
            }
        }

        private void btnConexion_Click(object sender, RoutedEventArgs e)
        {
            Parametros view = new Parametros();
            view.Owner = this;
            view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            view.ShowDialog();
        }

        private void cmbODBC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sConexion = "";
            if (cmbODBC.SelectedIndex == 1)
            {
                btnConexion.IsEnabled = true;
            }
            else
            {
                btnConexion.IsEnabled = false;
            }
            if (cmbODBC.SelectedIndex != 0)
            {
                if (cmbODBC.SelectedIndex != 1)             
                {                    
                    //sConexion = "DSN=" + (string)cmbODBC.SelectedValue;
                    Password view = new Password();
                    view.Owner = this;
                    view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                    view.ShowDialog();
                    string sPass = view.sPassword;
                    if (view.iOk == 1)
                    {
                        sConexion = "DSN=" + (string)cmbODBC.SelectedValue + "; PWD =" + sPass;
                        System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                        myConn.ConnectionString = sConexion;
                        try
                        {
                            myConn.Open();
                            myConn.Close();
                        }
                        catch (Exception ex)
                        {
                            ITCMessageBox.Show(this, "Error en los datos de conexión, no se pude realizar la conexion con los parametros seleccionados", "Error!");
                            cmbODBC.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        cmbODBC.SelectedIndex = 0;
                    }
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Collections.SortedList dsnList = GetAllDataSourceNames();
            cmbODBC.Items.Add("Agrega una conexión");
            cmbODBC.Items.Add("Definido por el usuario");
            for (int i = 0; i < dsnList.Count; i++)
            {
                string sName = dsnList.GetKey(i) as string;
                cmbODBC.Items.Add(sName);
            }
            cmbODBC.SelectedIndex = 0;
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

        private void btnRutas_Click(object sender, RoutedEventArgs e)
        {            
            if (sConexion == null)
            {
                ITCMessageBox.Show(this, "Debes configurar la conexion antes de continuar!", "Error!");
                cmbODBC.Focus();
            }
            else
            {
                Rutas view = new Rutas();
                view.Owner = this;
                view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                view.ShowDialog();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Odbc;
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
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Eliminar : Window
    {
        public Eliminar()
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

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
        
        private void FillCmbDispositivos()
        {            
            cmbDispo.Items.Clear();
            if (cmbVersion.SelectedIndex != -1)
            {
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                //string myQuery = "SELECT TIPO_DISPOSITIVO FROM HHc_Version Where CODIGO_AGENCIA = " + (string)cmbAgencia.SelectedValue + " Group By VERSION";
                string myQuery = "SELECT TIPO_DISPOSITIVO FROM HHc_ArchivosVersion WHERE CODIGO_AGENCIA = ? AND VERSION = ? GROUP BY TIPO_DISPOSITIVO";
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

        private void FillCmbVersiones()
        {            
            cmbVersion.Items.Clear();
            if (cmbAgencia.SelectedIndex != 0)
            {
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                string myQuery = "SELECT VERSION FROM HHc_ArchivosVersion Where CODIGO_AGENCIA = ? Group By VERSION";
                OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
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

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAgencia.SelectedIndex == 0)
            {
                ITCMessageBox.Show((Window)this, "Debes agregar una agencia!", "Error!");
            }
            else if(cmbVersion.SelectedIndex == 0)
            {
                ITCMessageBox.Show((Window)this, "Debes agregar una version!", "Error!");
            }           
            else if (cmbDispo.SelectedIndex == 0)
            {
                ITCMessageBox.Show((Window)this, "Debes agregar un dispositivo!", "Error!");
            }
            else
            {
                if (ITCMessageBox.Show((Window)this, "Seguro que deseas eliminar la version?", "Cuidado!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                    myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                    myConn.Open();              
                    //Elimina Version
                    string myNonQuery = "DELETE FROM HHc_ArchivosVersion Where CODIGO_AGENCIA = ? AND VERSION = ? AND TIPO_DISPOSITIVO = ?";
                    OdbcCommand myCommand = new OdbcCommand(myNonQuery, myConn);
                    myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                    myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                    myCommand.Parameters.Add("TIPO_DISPOSITIVO", OdbcType.VarChar).Value = (string)cmbDispo.SelectedValue;                                                        
                    myCommand.ExecuteNonQuery();
                    ITCMessageBox.Show(this, "Se elimino la version exitosamente", "Operacion correcta!"); 
                    //Verifica si existen alguna otra version con dispositivo
                    string myQuery = "SELECT TIPO_DISPOSITIVO FROM HHc_ArchivosVersion WHERE CODIGO_AGENCIA = ? AND VERSION = ? GROUP BY TIPO_DISPOSITIVO";
                    OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
                    myOdbcCommand.Connection = myConn;
                    myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                    myOdbcCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                    OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);                    
                    if (!myReader.Read())
                    {
                        //Elimina rutas con esa version y agencia
                        myNonQuery = "DELETE FROM HHc_Version Where CODIGO_AGENCIA = ? AND VERSION = ? ";
                        myCommand = new OdbcCommand(myNonQuery, myConn);
                        myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                        myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                        myCommand.ExecuteNonQuery();
                    }                                        
                    cmbAgencia.SelectedIndex = 0;                     
                    myConn.Close();
                }
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
    }
}
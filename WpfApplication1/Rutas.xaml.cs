using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
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
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Rutas : Window
    {               
        private List<string> lRutas = new List<string>();
        private List<string> lAgregados = new List<string>();

        public Rutas()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void FillListAgregados()
        {
            Menu wOwner = (Menu)this.Owner;

            System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
            myConn.ConnectionString = ((Menu)this.Owner).sConexion;
            string myQuery = "SELECT A.ID_RUTA FROM HHc_Version AS A, HHc_ArchivosVersion AS B Where A.CODIGO_AGENCIA = B.CODIGO_AGENCIA AND A.VERSION = B.VERSION AND B.CODIGO_AGENCIA = ? AND B.VERSION = ? GROUP BY A.ID_RUTA";
            OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
            myOdbcCommand.Connection = myConn;
            myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
            myOdbcCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;            
            myConn.Open();
            OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);
            lAgregados.Clear();
            while (myReader.Read())
            {
                lAgregados.Add(myReader.GetString(0));
            }
            myConn.Close();    
        }

        private void FillListRutas()
        {
            Menu wOwner = (Menu)this.Owner;

            System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
            myConn.ConnectionString = ((Menu)this.Owner).sConexion;
            string myQuery = "SELECT ID_RUTA FROM HHc_Ruta WHERE CODIGO_AGENCIA = ?";
            OdbcCommand myOdbcCommand = new OdbcCommand(myQuery);
            myOdbcCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
            myOdbcCommand.Connection = myConn;
            myConn.Open();
            OdbcDataReader myReader = myOdbcCommand.ExecuteReader(CommandBehavior.CloseConnection);            
            lRutas.Clear();
            while (myReader.Read())
            {
                lRutas.Add(myReader.GetString(0));
            }
            myConn.Close();            
        }
        
        private void FillLists()
        {
            int iDisp = cmbVersion.SelectedIndex;
            if ((iDisp != 0) && (iDisp != -1))
            {
                FillListAgregados();
                FillListRutas();
                foreach(string sTemp in lAgregados)
                {
                    if (lRutas.Contains(sTemp))
                    {
                        lRutas.Remove(sTemp);
                    }
                    ListBoxItem lTemp = new ListBoxItem();
                    lTemp.Content = sTemp;
                    lstAgregados.Items.Add(lTemp);
                }                
                lstAgregados.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
                foreach(string sTemp in lRutas)
                {
                    ListBoxItem lTemp = new ListBoxItem();
                    lTemp.Content = sTemp;
                    lstRutas.Items.Add(lTemp);
                }
                lstRutas.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
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

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            int iIndex = lstRutas.SelectedIndex;
            if (iIndex != -1)
            {
                ListBoxItem temp = new ListBoxItem();
                temp = (ListBoxItem)lstRutas.Items.GetItemAt(iIndex);
                lstRutas.Items.RemoveAt(iIndex);
                lstAgregados.Items.Add(temp);
                lstAgregados.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int iIndex = lstAgregados.SelectedIndex;
            if (iIndex != -1)
            {                
                ListBoxItem temp = new ListBoxItem();
                temp = (ListBoxItem)lstAgregados.Items.GetItemAt(iIndex);
                lstAgregados.Items.RemoveAt(iIndex);
                lstRutas.Items.Add(temp);
                lstRutas.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));                
            }
        }

        private void bntGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                System.Data.Odbc.OdbcConnection myConn = new System.Data.Odbc.OdbcConnection();
                myConn.ConnectionString = ((Menu)this.Owner).sConexion;
                myConn.Open();
                //Elimina
                string myNonQuery = "DELETE FROM HHc_Version Where CODIGO_AGENCIA = ? AND VERSION = ? ";
                OdbcCommand myCommand = new OdbcCommand(myNonQuery, myConn);
                myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                myCommand.ExecuteNonQuery();
                OdbcTransaction myTrans = myConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    //Insertar                                                
                    for (int i = 0; i < lstAgregados.Items.Count; i++)
                    {
                        myNonQuery = "INSERT INTO HHc_Version VALUES (?, ?, ?)";
                        myCommand = new OdbcCommand(myNonQuery, myConn);
                        myCommand.Transaction = myTrans;
                        myCommand.Parameters.Add("CODIGO_AGENCIA", OdbcType.Int).Value = Int32.Parse((string)cmbAgencia.SelectedValue);
                        myCommand.Parameters.Add("ID_RUTA", OdbcType.VarChar).Value = (string)((ListBoxItem)lstAgregados.Items.GetItemAt(i)).Content;
                        myCommand.Parameters.Add("VERSION", OdbcType.VarChar).Value = (string)cmbVersion.SelectedValue;
                        myCommand.ExecuteNonQuery();
                    }
                    myTrans.Commit();
                }
                catch (Exception ex)
                {
                    ITCMessageBox.Show((Window)this, ex.Message, "Error!");
                    myTrans.Rollback();
                }
                ITCMessageBox.Show(this, "Modificacion de rutas exitosa", "Mensaje!");
                myConn.Close();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            catch (Exception ex)
            {
                ITCMessageBox.Show(this, "Error al intentar ligar las rutas", "Error!");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            while(lstRutas.Items.Count != 0)
            {
                ListBoxItem temp = new ListBoxItem();
                temp = (ListBoxItem)lstRutas.Items.GetItemAt(0);
                lstRutas.Items.RemoveAt(0);
                lstAgregados.Items.Add(temp);
            }
            lstAgregados.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void btnNinguno_Click(object sender, RoutedEventArgs e)
        {
            while(lstAgregados.Items.Count != 0)
            {
                ListBoxItem temp = new ListBoxItem();
                temp = (ListBoxItem)lstAgregados.Items.GetItemAt(0);
                lstAgregados.Items.RemoveAt(0);
                lstRutas.Items.Add(temp);                
            }
            lstRutas.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void cmbAgencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstRutas.Items.Clear();            
            lstAgregados.Items.Clear();
            FillCmbVersiones();
        }

        private void cmbVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstRutas.Items.Clear();
            lstAgregados.Items.Clear();
            FillLists();
        }
    }
}

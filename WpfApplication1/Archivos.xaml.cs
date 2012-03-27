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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bimbo.DSD.UpdateAdmin
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Archivos : Window
    {
        private List<Archivo> lDirectorio = new List<Archivo>();
        private List<Archivo> lAgregados = new List<Archivo>();
        public bool bMain = false;

        public Archivos()
        {
            InitializeComponent();            
        }

        private void btnExplorar_Click(object sender, RoutedEventArgs e)
        {
            string selectedFolder = string.Empty;
            if (txtExplorar.Text == string.Empty)
            {                
                FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
                selectFolderDialog.ShowNewFolderButton = false;
                if (selectFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                selectedFolder = selectFolderDialog.SelectedPath;
            }
            else
            {
                selectedFolder = txtExplorar.Text;
            }
            if (selectedFolder != string.Empty)
            {
                txtExplorar.Text = selectedFolder;
            }
            FillUpdateArchivo(txtExplorar.Text);
            FillUpdateFileList();
        }

        private void FillUpdateArchivo(string path)
        {
            try
            {
                string[] sArchivos = Directory.GetFiles(path);

                foreach (string sName in sArchivos)
                {
                    Archivo aAdd = new Archivo();
                    aAdd.sName = sName;
                    aAdd.sShort = sName.Substring(path.Length + 1);
                    aAdd.option = -1;
                    aAdd.sDestino = "";
                    lDirectorio.Add(aAdd);
                }
            }
            catch 
            {
                txtExplorar.Text = string.Empty;
            }            
        }

        private void FillUpdateFileList()
        {
            lstArchivos.Items.Clear();
            foreach(Archivo aTemp in lDirectorio)
            {
                lstArchivos.Items.Add(aTemp);
            }
        }

        private void btnBajar_Click(object sender, RoutedEventArgs e)
        {
            int iIndex = lstArchivos.SelectedIndex;
            if ((iIndex != -1) && (iIndex != (lstArchivos.Items.Count - 1)))
            {
                string temp = (string)lstArchivos.Items.GetItemAt(iIndex);
                lstArchivos.Items.RemoveAt(iIndex); lstArchivos.Focus();
                lstArchivos.Items.Insert(iIndex + 1, temp);
                lstArchivos.Focus();
                lstArchivos.SelectedIndex = iIndex + 1;
                lstArchivos.ScrollIntoView(lstArchivos.Items.GetItemAt(iIndex + 1));
            }
            else
            {
                lstArchivos.Focus();
            }
        }

        private void btnSubir_Click(object sender, RoutedEventArgs e)
        {
            int iIndex = lstArchivos.SelectedIndex;
            if ((iIndex != -1) && (iIndex != 0))
            {
                string temp = (string)lstArchivos.Items.GetItemAt(iIndex);
                lstArchivos.Items.RemoveAt(iIndex);
                lstArchivos.Items.Insert(iIndex - 1, temp);
                lstArchivos.Focus();
                lstArchivos.SelectedIndex = iIndex - 1;
                lstArchivos.ScrollIntoView(lstArchivos.Items.GetItemAt(iIndex - 1));
            }
            else
            {
                lstArchivos.Focus();
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FillAcctionCmb()
        {
            cmbAccion.Items.Add("Selecciona una acción");
            cmbAccion.Items.Add("Agregar archivo");
            cmbAccion.Items.Add("Agregar e instalar archivo");
            cmbAccion.Items.Add("Borrar archivo");
            cmbAccion.Items.Add("Mueve directorio");
            cmbAccion.Items.Add("Elimina directorio");
            cmbAccion.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillAcctionCmb();
            if (!bMain)
            {
                Agregar wOwner = ((Agregar)this.Owner);
                if (wOwner.bArchivos)
                {
                    for (int i = 0; i < wOwner.lArchivos.Count; i++)
                    {
                        lstAgregados.Items.Add(wOwner.lArchivos[i]);
                    }
                }
            }   
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            int iAccion = cmbAccion.SelectedIndex;
            if (iAccion != 0)
            {
                if(txtDestino.Text == "")
                {
                    ITCMessageBox.Show((Window)this, "Debes pones la ruta destino!", "Error!");
                    txtDestino.Focus();
                }
                else
                {
                    if(lstArchivos.SelectedIndex == -1)
                    {
                        if (txtExplorar.Text == "")
                        {
                            ITCMessageBox.Show((Window)this, "Debes seleccionar un archivo o introducir una ruta!", "Error!");
                            txtDestino.Focus();
                        }
                        else
                        {                                                                                
                            Archivo aTemp = new Archivo();
                            aTemp.option = iAccion - 1;
                            aTemp.sDestino = txtDestino.Text;
                            aTemp.sName = txtExplorar.Text;
                            aTemp.sShort = txtExplorar.Text;                            
                            lstAgregados.Items.Add(aTemp);
                            cmbAccion.SelectedIndex = 0;
                        }                        
                    }
                    else
                    {
                        int iIndex = lstArchivos.SelectedIndex;

                        Archivo aTemp = (Archivo)lstArchivos.Items.GetItemAt(iIndex);
                        lstArchivos.Items.RemoveAt(iIndex);                        
                        aTemp.option = iAccion-1;
                        aTemp.sDestino = txtDestino.Text;                        
                        lstAgregados.Items.Add(aTemp);
                        cmbAccion.SelectedIndex = 0;
                    }
                }
            }
            else 
            {
                ITCMessageBox.Show((Window)this, "Debes seleccionar una accion!", "Error!");
                cmbAccion.Focus();
                cmbAccion.SelectedIndex = 0;
            }
        }

        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            int iIndex = lstAgregados.SelectedIndex;
            if (iIndex != -1)
            {
                lstAgregados.Items.RemoveAt(iIndex);
            }
        }

        private void btnSubir_Click_1(object sender, RoutedEventArgs e)
        {
            int iIndex = lstAgregados.SelectedIndex;
            if ((iIndex != -1) && (iIndex != 0))
            {
                Archivo temp = (Archivo)lstAgregados.Items.GetItemAt(iIndex);
                lstAgregados.Items.RemoveAt(iIndex);
                lstAgregados.Items.Insert(iIndex - 1, temp);               
                lstAgregados.SelectedIndex= iIndex -1;
                lstAgregados.Focus();                
            }

        }

        private void btnBajar_Click_1(object sender, RoutedEventArgs e)
        {
            int iIndex = lstAgregados.SelectedIndex;
            if ((iIndex != -1) && (iIndex != (lstAgregados.Items.Count-1)))
            {                
                Archivo temp = (Archivo)lstAgregados.Items.GetItemAt(iIndex);
                lstAgregados.Items.RemoveAt(iIndex);
                lstAgregados.Items.Insert(iIndex + 1, temp);
                lstAgregados.SelectedIndex = iIndex + 1;
                lstAgregados.Focus();
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (lstAgregados.HasItems)
            {
                if (!bMain)
                {
                    Agregar wOwner = ((Agregar)this.Owner);
                    if (wOwner.bArchivos)
                    {
                        wOwner.lArchivos.Clear();
                    }
                    ((Agregar)this.Owner).bArchivos = true;
                    for (int i = 0; i < lstAgregados.Items.Count; i++)
                    {
                        wOwner.lArchivos.Add((Archivo)(lstAgregados.Items.GetItemAt(i)));
                    }
                }
                else
                {
                    Administra wOwner = ((Administra)this.Owner);
                    wOwner.lArchivos.Clear();
                    ((Administra)this.Owner).bArchivos = true;
                    for (int i = 0; i < lstAgregados.Items.Count; i++)
                    {
                        wOwner.lArchivos.Add((Archivo)(lstAgregados.Items.GetItemAt(i)));
                    }
                }
                this.Close();
            }
        }
    }
}

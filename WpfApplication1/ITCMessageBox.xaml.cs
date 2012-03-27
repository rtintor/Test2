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
    /// Interaction logic for Window5.xaml
    /// </summary>   

    public partial class ITCMessageBox : Window
    {
        private string sMensaje;
        private string sTitulo;
        private MessageBoxButton iTipo;
        private static MessageBoxResult res;

        public ITCMessageBox()
        {
            InitializeComponent();
        }

        public static MessageBoxResult Show(Window wOwner, string sMes, string sTit, MessageBoxButton iTip = MessageBoxButton.OK)
        {
            ITCMessageBox view = new ITCMessageBox();
            view.Owner = wOwner;
            if (sMes.IndexOf('\n') == -1)
            {
                while ((sMes.Length - 35) > 0)
                {
                    string sTemp = sMes.Substring(0, 35);
                    int i = sTemp.LastIndexOf(' ');
                    if (i != -1)
                    {
                        sTemp = sMes.Substring(0, i + 1);
                        string sTemp2 = sMes.Substring(i + 1, sMes.Length - (i + 1));
                        view.sMensaje += sTemp + "\n";
                        sMes = sTemp2;
                    }
                    else
                    {
                        sTemp = sMes.Substring(0, 30);
                        string sTemp2 = sMes.Substring(30, sMes.Length - 30);
                        view.sMensaje += sTemp + "\n";
                        sMes = sTemp2;
                    }
                }
            }
            view.sMensaje += sMes;
            view.iTipo = iTip;
            view.sTitulo = sTit;
            view.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            view.ShowDialog();

            return res;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            blockMensaje.Text = sMensaje;
            this.Title = sTitulo;
            if (iTipo == MessageBoxButton.OK)
            {
                btnYes.Visibility = Visibility.Hidden;
                btnNo.Visibility = Visibility.Hidden;
                btnOk.Visibility = Visibility.Visible;
            }
            else if (iTipo == MessageBoxButton.YesNo)
            {
                btnYes.Visibility = Visibility.Visible;
                btnNo.Visibility = Visibility.Visible;
                btnOk.Visibility = Visibility.Hidden;
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            res = MessageBoxResult.Yes;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            res = MessageBoxResult.OK;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            res = MessageBoxResult.No;
            Close();
        }
    }
}

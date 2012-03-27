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
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class Password : Window
    {
        public string sPassword;
        public int iOk;

        public Password()
        {
            InitializeComponent();
            txtPass.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            sPassword = txtPass.Text;
            iOk = 1;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            sPassword = string.Empty;
            iOk = 0;
            this.Close();
        }
    }
}

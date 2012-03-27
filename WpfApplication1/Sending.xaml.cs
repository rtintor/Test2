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
    /// Interaction logic for Sending.xaml
    /// </summary>
    public partial class Sending : Window
    {
        string parametros;
        public Sending(string param)
        {
            InitializeComponent();
            parametros = param;
            string[] tmp = parametros.Split('&');
            double left = Double.Parse(tmp[0]);
            double top = Double.Parse(tmp[1]);
            string[] tmp2 = tmp[2].Split(',');
            double leftM = Double.Parse(tmp2[0]);
            double topM = Double.Parse(tmp2[1]);


            base.Left = left + ((leftM / 2.0) - 125);
            base.Top = top + ((topM / 2.0) - 100);
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Duration duration = new Duration(TimeSpan.FromSeconds(5));
            DoubleAnimation doubleanimation = new DoubleAnimation(100.0, duration);
            doubleanimation.RepeatBehavior = RepeatBehavior.Forever;
            pBar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }
    }
}

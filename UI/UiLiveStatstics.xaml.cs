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

namespace VANET_SIM.UI
{
    /// <summary>
    /// Interaction logic for UiLiveStatstics.xaml
    /// </summary>
    public partial class UiLiveStatstics : Window
    {
      

        public UiLiveStatstics()
        {
            InitializeComponent();

            this.Left = SystemParameters.FullPrimaryScreenWidth-this.Width;
            this.Top = SystemParameters.FullPrimaryScreenHeight-this.Height;
           // this.Height = SystemParameters.FullPrimaryScreenHeight;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
           
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}

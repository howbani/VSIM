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

namespace VSIM.UI
{
    /// <summary>
    /// Interaction logic for UiLiveStatstics.xaml
    /// </summary>
    public partial class UiLiveStatstics : Window
    {
      
        public bool IsCloseUable
        {
            get;set;
        }
        public UiLiveStatstics()
        {
            InitializeComponent();

            this.Left = SystemParameters.FullPrimaryScreenWidth-this.Width;
            this.Top = 60; // SystemParameters.FullPrimaryScreenHeight-this.Height;
                           // this.Height = SystemParameters.FullPrimaryScreenHeight;

            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (IsCloseUable)
                {
                    Close();
                }
                else
                {
                    e.Cancel = true;
                    Hide();
                }
            }
            catch
            {

            }
        }
    }
}

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
using VSIM.Operations;
using VSIM.Properties;

namespace VSIM.UI
{
    /// <summary>
    /// Interaction logic for UiNetGen.xaml
    /// </summary>
    public partial class UiNetGen : Window
    {
        MainWindow _mainWindow; 
        public UiNetGen(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
             
            com_laneCount.Items.Add("2"); com_laneCount.Items.Add("4"); com_laneCount.Items.Add("6"); 
            for (int i = 1; i <= 20; i++)
            {
                com_rows.Items.Add(i);
                com_cols.Items.Add(i);
            }
            
            for (int i = 1; i <= 20; i++)
            {
                double j = i * 100;
                com_HorizontalLength.Items.Add(j);
                com_VerticalLength.Items.Add(j);
            }


            com_laneCount.Text = Settings.Default.LaneCount_top_gen.ToString();
            com_rows.Text = Settings.Default.Rows_net_top.ToString();
            com_cols.Text = Settings.Default.Cols_net_top.ToString();
            com_HorizontalLength.Text = Settings.Default.HorizontalLength.ToString();
            com_VerticalLength.Text = Settings.Default.VerticalLength.ToString();
        }

        private void Btn_gen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindow.Clear();

                int rows, cols, laneCount;
                double hLen, vLen;
                rows = Convert.ToInt16(com_rows.Text);
                cols = Convert.ToInt16(com_cols.Text);
                laneCount = Convert.ToInt16(com_laneCount.Text);
                hLen = Convert.ToDouble(com_HorizontalLength.Text);
                vLen = Convert.ToDouble(com_VerticalLength.Text);

                NetworkGenrator.ConstructGird(_mainWindow, rows, cols, hLen, vLen, laneCount);

                this.Hide();


                Settings.Default.LaneCount_top_gen = laneCount;
                Settings.Default.Cols_net_top = cols;
                Settings.Default.Rows_net_top = rows;
                Settings.Default.HorizontalLength = hLen;
                Settings.Default.VerticalLength = vLen;
                Settings.Default.NetTopName = "";
                Settings.Default.Save();

                PublicStatistics.LiveStatstics.lbl_segment_length.Content = "Horizontal:" + hLen + "m " + "Vertical:" + vLen;
                PublicStatistics.LiveStatstics.lbl_net_grid_top.Content = "Rows:" + rows + "x" + "Cols:" + cols;


                _mainWindow.canvas_vanet.Height = 100 + (rows - 1) * vLen;
                _mainWindow.canvas_vanet.Width = 100 + (cols - 1) * hLen;
            }
            catch
            {
               
            }
        }
    }
}

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
    /// Interaction logic for UiShowResults.xaml
    /// </summary>
    public partial class UiShowResults : Window
    {
        public UiShowResults(IEnumerable<object> List)
        {
            InitializeComponent();

            dg_results.ItemsSource = List;
        }
    }
}

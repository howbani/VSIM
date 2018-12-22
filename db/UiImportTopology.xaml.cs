using System.Windows;

namespace VANET_SIM.db
{
    /// <summary>
    /// Interaction logic for UiImportTopology.xaml
    /// </summary>
    public partial class UiImportTopology : Window
    {

        public UiImportTopology(MainWindow MainWindow)
        {
            InitializeComponent();

           foreach( NetwokImport net in NetworkTopolgy.ImportNetworkNames(this))
           {
                net._MainWindow = MainWindow;
                stk_netwoks.Children.Add(net);
           }
        }
    }
}

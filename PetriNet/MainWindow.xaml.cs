using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetriNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Close the Window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //todo Replace this with some proper xaml
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TheDockPanel.DataContext is SystemModel sysModel)
            {
                if (e.AddedItems.Count > 0)
                    if (e.AddedItems[0] is Displayable disp)
                        sysModel.SelectedItem = disp;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (TheDockPanel.DataContext as SystemModel).TheListView = lstItems;
        }
    }

}

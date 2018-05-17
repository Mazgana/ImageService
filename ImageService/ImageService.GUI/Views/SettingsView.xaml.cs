using ImageService.GUI.Model;
using ImageService.GUI.ViewModels;
using System;
using System.Collections.Generic;
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

namespace ImageService.GUI.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private SettingsViewModel vm;

        public SettingsView()
        {
            vm = new SettingsViewModel(new SettingsModel());
            this.DataContext = vm;
            InitializeComponent();
        }        private void ItemSelected(object sender, RoutedEventArgs e)
        {
            Remove.IsEnabled = true;
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            string handlerToRemove = handlers.SelectedItem.ToString();
            bool res = vm.removeHandler(handlerToRemove);
            if (res)
            {
                handlers.Items.Remove(handlers.SelectedItem);
            }
        }
    }
}

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
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Morphological_image_analyzer
{
    /// <summary>
    /// Logika interakcji dla klasy ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        public static string myValue = String.Empty;
        public ModalWindow()
        {
            InitializeComponent();
        }

        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {
            myValue = txtSomeBox.Text;
            this.Close();
        }

        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

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
        public static string size = String.Empty;
        public ModalWindow()
        {
            InitializeComponent();
        }

        private void setKernelSize_Click(object sender, RoutedEventArgs e)
        {
            size = kernelSize.Text;

            stackPanel.Children.Clear();

            for(int i = 1; i <= Int32.Parse(size); i++)
            {
                StackPanel stackPanelHorizontal = new StackPanel();
                stackPanelHorizontal.Name = "panel_" + i.ToString();
                stackPanelHorizontal.Orientation = Orientation.Horizontal;
                stackPanelHorizontal.HorizontalAlignment = HorizontalAlignment.Center;

                stackPanel.Children.Add(stackPanelHorizontal);
                stackPanel.RegisterName(stackPanelHorizontal.Name, stackPanelHorizontal);

                for (int j = 1; j <= Int32.Parse(size); j++)
                {
                    createNewTextBox(stackPanelHorizontal, i, j);
                }
            }


        }

        private void confirmKernelMatrix_Click(object sender, RoutedEventArgs e)
        {

        }



        private void numberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        protected void createNewTextBox(StackPanel panel, int rowId, int columnId)
        {
            TextBox txtNumber = new TextBox();
            txtNumber.Name = "box_" + rowId.ToString() + "_" + columnId.ToString();
            txtNumber.Text = "1";
            txtNumber.Width = 20;
            panel.Children.Add(txtNumber);
            panel.RegisterName(txtNumber.Name, txtNumber);
        }

    }
}

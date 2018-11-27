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
        private static readonly char[] delimiters = { '_' };

        public static byte[,] kernel;
        public ModalWindow()
        {
            InitializeComponent();
        }

        private void setKernelSize_Click(object sender, RoutedEventArgs e)
        {
            string size = kernelSize.Text;

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

            int sizeOfKernel = stackPanel.Children.Count;

            kernel = new byte[sizeOfKernel, sizeOfKernel];

            foreach (UIElement element in stackPanel.Children)
            {
                StackPanel horizontalPanel = (StackPanel) element;
                string horizontalPanelName = horizontalPanel.Name;
                string[] splitStringPanelName = horizontalPanelName.Split(delimiters);
                int id_x = Int32.Parse(splitStringPanelName[1]) - 1;

                foreach (UIElement elem in horizontalPanel.Children)
                {
                    TextBox box = (TextBox) elem;
                    string textBoxName = box.Name;
                    string[] splitStringTextBoxName = textBoxName.Split(delimiters);
                    int id_y = Int32.Parse(splitStringTextBoxName[2]) - 1;

                    kernel[id_x, id_y] = Byte.Parse(box.Text);
                }

            }

            this.Close();
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

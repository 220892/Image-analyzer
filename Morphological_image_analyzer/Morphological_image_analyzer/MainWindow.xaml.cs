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

namespace Morphological_image_analyzer
{
    /// <summary>
    /// Logic of interaction for class MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rnd = new Random();

        static readonly int minSize = 15;
        static readonly int sizeOfWindow = 260;

        public MainWindow()
        {
            InitializeComponent();
        }

        void addSquere_Click(object sender, RoutedEventArgs e)
        {
            int width = rnd.Next(minSize, sizeOfWindow);
            int height = rnd.Next(minSize, sizeOfWindow);
            int startPointLeft = rnd.Next(sizeOfWindow - width);
            int startPointTop = rnd.Next(sizeOfWindow - height);

            Rectangle rect = new Rectangle() { Width = width, Height = height, Fill = Brushes.Transparent, StrokeThickness = 5, Stroke = Brushes.Black };
            this.analizedCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, startPointLeft);
            Canvas.SetTop(rect, startPointTop);
        }

        void addEllipse_Click(object sender, RoutedEventArgs e)
        {
            int width = rnd.Next(minSize, sizeOfWindow);
            int height = rnd.Next(minSize, sizeOfWindow);
            int startPointLeft = rnd.Next(sizeOfWindow - width);
            int startPointTop = rnd.Next(sizeOfWindow - height);

            Ellipse ellipse = new Ellipse() { Width = width, Height = height, Fill = Brushes.Transparent, StrokeThickness = 5, Stroke = Brushes.Black };
            this.analizedCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, startPointLeft);
            Canvas.SetTop(ellipse, startPointTop);
        }

        void addLine_Click(object sender, RoutedEventArgs e)
        {
            int x1 = rnd.Next(minSize, sizeOfWindow);
            int y1 = rnd.Next(minSize, sizeOfWindow);
            int x2 = rnd.Next(minSize, sizeOfWindow);
            int y2 = rnd.Next(minSize, sizeOfWindow);

            Line line = new Line() { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, StrokeThickness = 5, Stroke = Brushes.Black };
            this.analizedCanvas.Children.Add(line);
        }
    }
}

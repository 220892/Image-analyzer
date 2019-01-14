using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Win32;

namespace Morphological_image_analyzer
{
    /// <summary>
    /// Logic of interaction for class MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // injection of morphological operation performers
        static readonly IMorphologicalCalculator dilationCalculator = new DilationCalculator();
        static readonly IMorphologicalCalculator erosionCalculator = new ErosionCalculator();
        static readonly IMorphologicalCalculator dilationOfErosionCalculator = new DilationOfErosionCalculator();
        static readonly IMorphologicalCalculator erosionOfDilationCalculator = new ErosionOfDilationCalculator();

        static readonly int minSize = 15; // minimum size of auto-generated element
        static readonly int sizeOfWindow = 260; // size of window with analized image
        static readonly int divide = 10;

        static readonly Random rnd = new Random(); // random numbers generator

        CanvasBitmapSupport canvasBitmapSupport = new CanvasBitmapSupport();

        private byte[,] kernel = { {0, 1, 0 }, {1, 1, 1}, {0, 1, 0} };

        public MainWindow()
        {
            InitializeComponent();

            // creating directory for temporary image files if it doeas not exists
            canvasBitmapSupport.initialize();
        }

        void addSquere_Click(object sender, RoutedEventArgs e)
        {
            int width = rnd.Next(minSize, sizeOfWindow/divide);
            int height = rnd.Next(minSize, sizeOfWindow / divide);
            int startPointLeft = rnd.Next(sizeOfWindow - width);
            int startPointTop = rnd.Next(sizeOfWindow - height);

            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle() { Width = width, Height = height, Fill = System.Windows.Media.Brushes.Transparent, StrokeThickness = 5, Stroke = System.Windows.Media.Brushes.Black };
            this.analizedCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, startPointLeft);
            Canvas.SetTop(rect, startPointTop);
        }

        void addEllipse_Click(object sender, RoutedEventArgs e)
        {
            int width = rnd.Next(minSize, sizeOfWindow / divide);
            int height = rnd.Next(minSize, sizeOfWindow / divide);
            int startPointLeft = rnd.Next(sizeOfWindow - width);
            int startPointTop = rnd.Next(sizeOfWindow - height);

            Ellipse ellipse = new Ellipse() { Width = width, Height = height, Fill = System.Windows.Media.Brushes.Transparent, StrokeThickness = 5, Stroke = System.Windows.Media.Brushes.Black };
            this.analizedCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, startPointLeft);
            Canvas.SetTop(ellipse, startPointTop);
        }

        void addLine_Click(object sender, RoutedEventArgs e)
        {
            int x1 = rnd.Next(minSize, sizeOfWindow - 5);
            int y1 = rnd.Next(minSize, sizeOfWindow - 5);

            System.Windows.Shapes.Rectangle rect;

            if (x1 % 2 == 0)
            {
                int x2 = rnd.Next(x1, sizeOfWindow - 5);
                rect = new System.Windows.Shapes.Rectangle() { Width = 5, Height = x2 - x1, Fill = System.Windows.Media.Brushes.Transparent, StrokeThickness = 5, Stroke = System.Windows.Media.Brushes.Black };
            } else
            {
                int y2 = rnd.Next(y1, sizeOfWindow - 5);
                rect = new System.Windows.Shapes.Rectangle() { Width = y2 - y1, Height = 5, Fill = System.Windows.Media.Brushes.Transparent, StrokeThickness = 5, Stroke = System.Windows.Media.Brushes.Black };
            }

            this.analizedCanvas.Children.Add(rect);
            Canvas.SetTop(rect, x1);
            Canvas.SetLeft(rect, y1);
        }

        private void clearAnalized_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Background = System.Windows.Media.Brushes.White;
            analizedCanvas.Children.Clear();
            analizedCanvas.Children.Add(analizedBorder);
        }

        private void setAsOriginal_Click(object sender, RoutedEventArgs e)
        {
            originalCanvas.Children.Clear();
            originalCanvas.Children.Add(originalBorder);

            System.Windows.Media.Brush brush = analizedCanvas.Background;
            originalCanvas.Background = brush;

            foreach (UIElement element in analizedCanvas.Children)
            {
                string saved = XamlWriter.Save(element);
                StringReader sReader = new StringReader(saved);
                XmlReader xReader = XmlReader.Create(sReader);
                UIElement newElement = (UIElement)XamlReader.Load(xReader);
                originalCanvas.Children.Add(newElement);
            }
        }

        void performDilation_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = canvasBitmapSupport.convertCanvasToBitmap(analizedCanvas);

            Bitmap bitmapPerformed = dilationCalculator.performMorphologicalOperation(bitmapConverted, kernel);

            BitmapSource bitmapToPut = canvasBitmapSupport.convertBitmapToBitmapImage(bitmapPerformed);

            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }


        void performErosion_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = canvasBitmapSupport.convertCanvasToBitmap(analizedCanvas);

            Bitmap bitmapPerformed = erosionCalculator.performMorphologicalOperation(bitmapConverted, kernel);

            BitmapSource bitmapToPut = canvasBitmapSupport.convertBitmapToBitmapImage(bitmapPerformed);

            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }

        void performDilationOfErosion_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = canvasBitmapSupport.convertCanvasToBitmap(analizedCanvas);

            Bitmap bitmapPerformed = dilationOfErosionCalculator.performMorphologicalOperation(bitmapConverted, kernel);

            BitmapSource bitmapToPut = canvasBitmapSupport.convertBitmapToBitmapImage(bitmapPerformed);

            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }

        void performErosionOfDilation_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = canvasBitmapSupport.convertCanvasToBitmap(analizedCanvas);

            Bitmap bitmapPerformed = erosionOfDilationCalculator.performMorphologicalOperation(bitmapConverted, kernel);

            BitmapSource bitmapToPut = canvasBitmapSupport.convertBitmapToBitmapImage(bitmapPerformed);

            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }

        void performEdgesWithErosion_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = canvasBitmapSupport.convertCanvasToBitmap(analizedCanvas);

            bitmapConverted = canvasBitmapSupport.convertToGray(bitmapConverted);

            Bitmap bitmapPerformed = erosionCalculator.performMorphologicalOperation(bitmapConverted, kernel);

            bitmapPerformed = canvasBitmapSupport.bitmapDifference(bitmapPerformed, bitmapConverted);

            BitmapSource bitmapToPut = canvasBitmapSupport.convertBitmapToBitmapImage(bitmapPerformed);

            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }

        void performEdgesWithDilation_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = canvasBitmapSupport.convertCanvasToBitmap(analizedCanvas);

            bitmapConverted = canvasBitmapSupport.convertToGray(bitmapConverted);

            Bitmap bitmapPerformed = dilationCalculator.performMorphologicalOperation(bitmapConverted, kernel);

            bitmapPerformed = canvasBitmapSupport.bitmapDifference(bitmapConverted, bitmapPerformed);

            BitmapSource bitmapToPut = canvasBitmapSupport.convertBitmapToBitmapImage(bitmapPerformed);

            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }

        void configureKernel_Click(object sender, RoutedEventArgs e)
        {
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.ShowDialog();

            kernel = ModalWindow.kernel;
        }

        void load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Wybierz obraz";
            op.Filter = "Wspierane formaty|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(op.FileName));
                analizedCanvas.Background = brush;
            }

        }

        void analizeImage_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = canvasBitmapSupport.convertCanvasToBitmap(analizedCanvas);

            analizedCanvas.Children.Add(analizedBorder);

            ModalWindowAnaliseResult modalWindow = new ModalWindowAnaliseResult(bitmapConverted);
            modalWindow.ShowDialog();
        }

    }
}

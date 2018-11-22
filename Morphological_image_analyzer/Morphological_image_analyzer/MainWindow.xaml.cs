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

        static readonly Random rnd = new Random(); // random numbers generator

        CanvasBitmapSupport canvasBitmapSupport = new CanvasBitmapSupport();

        private byte[,] kernel
        {
            get
            {
                return new byte[,]
                {
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 }
                };
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // creating directory for temporary image files if it doeas not exists
            canvasBitmapSupport.initialize();
        }

        void addSquere_Click(object sender, RoutedEventArgs e)
        {
            int width = rnd.Next(minSize, sizeOfWindow);
            int height = rnd.Next(minSize, sizeOfWindow);
            int startPointLeft = rnd.Next(sizeOfWindow - width);
            int startPointTop = rnd.Next(sizeOfWindow - height);

            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle() { Width = width, Height = height, Fill = System.Windows.Media.Brushes.Transparent, StrokeThickness = 5, Stroke = System.Windows.Media.Brushes.Black };
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

            Ellipse ellipse = new Ellipse() { Width = width, Height = height, Fill = System.Windows.Media.Brushes.Transparent, StrokeThickness = 5, Stroke = System.Windows.Media.Brushes.Black };
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

            Line line = new Line() { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, StrokeThickness = 5, Stroke = System.Windows.Media.Brushes.Black };
            this.analizedCanvas.Children.Add(line);
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

        void configureKernel_Click(object sender, RoutedEventArgs e)
        {
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.ShowDialog();

            //string valueFromModalTextBox = ModalWindow.myValue;
        }

    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;

namespace Morphological_image_analyzer
{
    /// <summary>
    /// Logic of interaction for class MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly int minSize = 15;
        static readonly int sizeOfWindow = 260;

        DilationCalculator dilationCalculator = new DilationCalculator();

        Random rnd = new Random();

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

        void performDilation_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap writeableBitmapFromCanvas = SaveAsWriteableBitmap(analizedCanvas);
            Bitmap bitmapFromCanvas = BitmapFromWriteableBitmap(writeableBitmapFromCanvas);

            dilationCalculator.setImage(bitmapFromCanvas);
            dilationCalculator.performMorphologicalOperation();
            Bitmap bitmapToCanvas = dilationCalculator.getImage();

            System.Windows.Media.Imaging.BitmapSource bitmapSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmapToCanvas.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                System.Windows.Media.Imaging.WriteableBitmap writeableBitmap = new System.Windows.Media.Imaging.WriteableBitmap(bitmapSource);

            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapSource;
            analizedCanvas.Background = brush;
        }

        public WriteableBitmap SaveAsWriteableBitmap(Canvas surface)
        {
            if (surface == null) return null;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            System.Windows.Size size = new System.Windows.Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
              (int)size.Width,
              (int)size.Height,
              96d,
              96d,
              PixelFormats.Pbgra32);
            renderBitmap.Render(surface);


            //Restore previously saved layout
            surface.LayoutTransform = transform;

            //create and return a new WriteableBitmap using the RenderTargetBitmap
            return new WriteableBitmap(renderBitmap);

        }

        private System.Drawing.Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            System.Drawing.Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                bmp = new System.Drawing.Bitmap(outStream);
            }
            return bmp;
        }

        private void clearAnalized_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Clear();
            analizedCanvas.Children.Add(analizedBorder);
        }

        private void setAsOriginal_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}

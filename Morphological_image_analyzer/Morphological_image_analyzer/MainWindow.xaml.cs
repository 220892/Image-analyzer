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
        static readonly int minSize = 15; // minimum size of auto-generated element
        static readonly int sizeOfWindow = 260; // size of window with analized image

        static int imageId = 1; // initial image id
        static String catalogName = @"c:\Test\"; // path to catalog with temporary image files

        static readonly Random rnd = new Random(); // random numbers generator

        // injection of morphological operation performers
        static readonly DilationCalculator dilationCalculator = new DilationCalculator();
        static readonly ErosionCalculator erosionCalculator = new ErosionCalculator();

        public MainWindow()
        {
            InitializeComponent();

            // creating directory for temporary image files if it doeas not exists
            bool exists = System.IO.Directory.Exists(catalogName);

            if (!exists)
                System.IO.Directory.CreateDirectory(catalogName);
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

            Bitmap bitmapConverted = convertCanvasToBitmap(analizedCanvas);

            Bitmap bitmapPerformed = dilationCalculator.performMorphologicalOperation(bitmapConverted, 3);

            BitmapSource bitmapToPut = Bitmap2BitmapImage(bitmapPerformed);


            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }


        void performErosion_Click(object sender, RoutedEventArgs e)
        {
            analizedCanvas.Children.Remove(analizedBorder);

            Bitmap bitmapConverted = convertCanvasToBitmap(analizedCanvas);

            Bitmap bitmapPerformed = erosionCalculator.performMorphologicalOperation(bitmapConverted);

            BitmapSource bitmapToPut = Bitmap2BitmapImage(bitmapPerformed);


            analizedCanvas.Children.Clear();
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bitmapToPut;
            analizedCanvas.Background = brush;
            analizedCanvas.Children.Add(analizedBorder);
        }

        void performDilationOfErosion_Click(object sender, RoutedEventArgs e)
        {

        }

        void performErosionOfDilation_Click(object sender, RoutedEventArgs e)
        {

        }


        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        private Bitmap convertCanvasToBitmap(Canvas canvas)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            double dpi = 96d;


            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);


            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();

                System.IO.File.WriteAllBytes(catalogName + @"image" + imageId + @".png", ms.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(catalogName + @"image" + imageId + @".png");
            bitmap.EndInit();

            imageId = imageId + 1;


            return BitmapImage2Bitmap(bitmap);
        }

    }
}

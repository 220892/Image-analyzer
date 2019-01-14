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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Morphological_image_analyzer
{
    /// <summary>
    /// Logika interakcji dla klasy ModalWindowAnaliseResult.xaml
    /// </summary>
    public partial class ModalWindowAnaliseResult : Window
    {

        ImageAnalyzer imageAnalyzer = new ImageAnalyzer();

        Bitmap bitmap;

        public ModalWindowAnaliseResult(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            InitializeComponent();
        }

        public void startAnalize_Click(object sender, RoutedEventArgs e)
        {
            int allObjects = imageAnalyzer.countObjects(bitmap);
            int onePixelObjects = imageAnalyzer.countObjectsWithOnePixelLine(bitmap);
            int closedObjects = imageAnalyzer.countClosedObjects(bitmap);
            showResultsOfAnalize(allObjects, onePixelObjects, closedObjects);
        }

        private void showResultsOfAnalize(int allObjects, int onePixelObjects, int closedObjects)
        {
            countOfAllObjectsValue.Content = allObjects;
            countOfObjectsWithOnePointLineValue.Content = onePixelObjects;
            countOfClosedObjectsValue.Content = closedObjects;
        }
    }
}

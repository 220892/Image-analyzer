using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Morphological_image_analyzer
{
    class CanvasBitmapSupport
    {
        static int imageId = 1; // initial image id
        static String catalogName = @"c:\Test\"; // path to catalog with temporary image files

        public void initialize()
        {
            // creating directory for temporary image files if it doeas not exists
            bool exists = System.IO.Directory.Exists(catalogName);

            if (!exists)
                System.IO.Directory.CreateDirectory(catalogName);
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

        public BitmapSource convertBitmapToBitmapImage(Bitmap bitmap)
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

        public Bitmap convertCanvasToBitmap(Canvas canvas)
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

        public Bitmap bitmapDifference(Bitmap bitmap1, Bitmap bitmap2)
        {
            int width1 = bitmap1.Width;
            int height1 = bitmap1.Height;

            int width2 = bitmap2.Width;
            int height2 = bitmap2.Height;

            if (width1 != width2 || height1 != height2)
            {
                throw new ArgumentException("Obrazy muszą mieć dokładnie takie same wymiary!");
            }

            // Bitmap 1
            System.Drawing.Rectangle canvas1 = new System.Drawing.Rectangle(0, 0, width1, height1);
            BitmapData srcData1 = bitmap1.LockBits(canvas1, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytes1 = srcData1.Stride * srcData1.Height;
            byte[] pixelBuffer1 = new byte[bytes1];

            Marshal.Copy(srcData1.Scan0, pixelBuffer1, 0, bytes1);
            bitmap1.UnlockBits(srcData1);


            // Bitmap 2
            System.Drawing.Rectangle canvas2 = new System.Drawing.Rectangle(0, 0, width1, height1);
            BitmapData srcData2 = bitmap2.LockBits(canvas2, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytes2 = srcData2.Stride * srcData2.Height;
            byte[] pixelBuffer2 = new byte[bytes2];

            Marshal.Copy(srcData2.Scan0, pixelBuffer2, 0, bytes2);
            bitmap2.UnlockBits(srcData2);

            byte[] resultBuffer = new byte[bytes1];


            int byteOffset = 0;
            int value = 0;

            for (int y = 0; y < height1; y++)
            {
                for (int x = 0; x < width1; x++)
                {
                    byteOffset = y * srcData1.Stride + x * 4;

                    value = pixelBuffer1[byteOffset] - pixelBuffer2[byteOffset];
                    if (value >= 0)
                    {
                        resultBuffer[byteOffset] = (byte)value;
                    }
                    else
                    {
                        resultBuffer[byteOffset] = 0;
                    }

                    value = pixelBuffer1[byteOffset + 1] - pixelBuffer2[byteOffset + 1];
                    if (value >= 0)
                    {
                        resultBuffer[byteOffset + 1] = (byte)value;
                    }
                    else
                    {
                        resultBuffer[byteOffset + 1] = 0;
                    }

                    value = pixelBuffer1[byteOffset + 2] - pixelBuffer2[byteOffset + 2];
                    if (value >= 0)
                    {
                        resultBuffer[byteOffset + 2] = (byte)value;
                    }
                    else
                    {
                        resultBuffer[byteOffset + 2] = 0;
                    }

                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap result = new Bitmap(width1, height1);
            BitmapData resultData = result.LockBits(canvas1, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes1);
            result.UnlockBits(resultData);
            return result;
        }

        public Bitmap convertToGray(Bitmap bitmap1)
        {
            int width1 = bitmap1.Width;
            int height1 = bitmap1.Height;

            // Bitmap 1
            System.Drawing.Rectangle canvas1 = new System.Drawing.Rectangle(0, 0, width1, height1);
            BitmapData srcData1 = bitmap1.LockBits(canvas1, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytes1 = srcData1.Stride * srcData1.Height;
            byte[] pixelBuffer1 = new byte[bytes1];

            Marshal.Copy(srcData1.Scan0, pixelBuffer1, 0, bytes1);
            bitmap1.UnlockBits(srcData1);

            byte[] resultBuffer = new byte[bytes1];


            int byteOffset = 0;
            int value = 0;

            for (int y = 0; y < height1; y++)
            {
                for (int x = 0; x < width1; x++)
                {
                    byteOffset = y * srcData1.Stride + x * 4;

                    value = pixelBuffer1[byteOffset];
                    resultBuffer[byteOffset] = (byte)value;
                    resultBuffer[byteOffset + 1] = (byte)value;
                    resultBuffer[byteOffset + 2] = (byte)value;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap result = new Bitmap(width1, height1);
            BitmapData resultData = result.LockBits(canvas1, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes1);
            result.UnlockBits(resultData);
            return result;
        }


    }
}

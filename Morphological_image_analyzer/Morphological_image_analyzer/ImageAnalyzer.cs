using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Morphological_image_analyzer
{
    class ImageAnalyzer
    {
        FloodFillService floodFillService = new FloodFillService();

        public int countObjects(Bitmap bitmap)
        {
            int count = 0;
            Bitmap image = new Bitmap(bitmap);
            floodFillService.FloodFill(image, new Point(1, 1), Color.Black, Color.White);

            while (true)
            {
                Point point = findFirstBlackPixel(image);
                if (point.X != -1 || point.Y != -1)
                {
                    count++;
                    floodFillService.FloodFill(image, point, Color.Black, Color.White);
                }
                else
                {
                    return count;
                }
            }

        }

        public int countObjectsWithOnePixelLine(Bitmap bitmap)
        {
            int count = 0;
            Bitmap image = new Bitmap(bitmap);
            floodFillService.FloodFill(image, new Point(1, 1), Color.Black, Color.White);

            while (true)
            {
                Point point = findPixelOfOnePixelLine(image);
                if (point.X != -1 || point.Y != -1)
                {
                    count++;
                    floodFillService.FloodFill(image, point, Color.Black, Color.White);
                }
                else
                {
                    return count;
                }
            }

        }

        private Point findFirstBlackPixel(Bitmap srcImage)
        {
            int width = srcImage.Width;
            int height = srcImage.Height;

            Rectangle canvas = new Rectangle(0, 0, width, height);
            BitmapData srcData = srcImage.LockBits(canvas, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int bytes = srcData.Stride * srcData.Height;
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, bytes);
            srcImage.UnlockBits(srcData);

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (pixelBuffer[calculateByteOffset(x, y, srcData.Stride)] == 0)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(-1, -1);
        }

        private Point findPixelOfOnePixelLine(Bitmap srcImage)
        {
            int width = srcImage.Width;
            int height = srcImage.Height;

            Rectangle canvas = new Rectangle(0, 0, width, height);
            BitmapData srcData = srcImage.LockBits(canvas, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int bytes = srcData.Stride * srcData.Height;
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, bytes);
            srcImage.UnlockBits(srcData);

            for (int y = 2; y < height - 1; y++)
            {
                for (int x = 2; x < width - 2; x++)
                {
                    if ((pixelBuffer[calculateByteOffset(x - 1, y, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x, y,srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x + 1, y, srcData.Stride)] != 0)
                        || (pixelBuffer[calculateByteOffset(x, y - 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x, y, srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x, y + 1, srcData.Stride)] != 0))
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(-1, -1);
        }

        private int calculateByteOffset(int x, int y, int stride)
        {
            return y * stride + x * 4;
        }

    }
}

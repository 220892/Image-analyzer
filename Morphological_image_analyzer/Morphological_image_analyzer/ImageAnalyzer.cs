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
            Bitmap image = new Bitmap(performNormalization(bitmap));
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
            Bitmap image = new Bitmap(performNormalization(bitmap));
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

        public int countClosedObjects(Bitmap bitmap)
        {
            int count = 0;
            Bitmap image = new Bitmap(performNormalization(bitmap));
            floodFillService.FloodFill(image, new Point(1, 1), Color.Black, Color.White);

            Point searchStartPoint = new Point(1, 1);

            while (true)
            {
                Tuple<Point, Point, Point> points = findPixelsToClosedObjectsSearch(image, searchStartPoint);

                if (points.Item3.X != -1 || points.Item3.Y != -1)
                {
                    Bitmap imageCopy = new Bitmap(image);

                    floodFillService.FloodFill(imageCopy, points.Item1, Color.White, Color.Red);

                    Color startPixelColor = imageCopy.GetPixel(points.Item1.X, points.Item1.Y);
                    Color endPixelColor = imageCopy.GetPixel(points.Item3.X, points.Item3.Y);

                    if (startPixelColor.A != Color.Red.A || startPixelColor.R != Color.Red.R || startPixelColor.G != Color.Red.G || startPixelColor.B != Color.Red.B)
                    {
                        throw new ArgumentException("Niepoprawny kolor w punkcie rozpoczęcia analizy!");
                    }

                    if (startPixelColor.A != endPixelColor.A || startPixelColor.R != endPixelColor.R || startPixelColor.G != endPixelColor.G || startPixelColor.B != endPixelColor.B)
                    {
                        count++;
                        floodFillService.FloodFill(image, points.Item2, Color.Black, Color.White);
                    }

                    searchStartPoint = points.Item3;
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

            for (int y = 2; y < height - 2; y++)
            {
                for (int x = 2; x < width - 2; x++)
                {
                    if ((pixelBuffer[calculateByteOffset(x - 1, y, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x, y, srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x + 1, y, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x - 1, y - 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x, y - 1, srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x + 1, y - 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x - 1, y + 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x, y + 1, srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x + 1, y + 1, srcData.Stride)] != 0)
                        || (pixelBuffer[calculateByteOffset(x - 1, y - 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x - 1, y, srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x - 1, y + 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x, y - 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x, y, srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x, y + 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x + 1, y - 1, srcData.Stride)] != 0
                        && pixelBuffer[calculateByteOffset(x + 1, y, srcData.Stride)] == 0
                        && pixelBuffer[calculateByteOffset(x + 1, y + 1, srcData.Stride)] != 0))
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(-1, -1);
        }

        private Tuple<Point, Point, Point> findPixelsToClosedObjectsSearch(Bitmap srcImage, Point searchStartPoint)
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

            Point startPoint = new Point(-1, -1);
            Point middlePoint = new Point(-1, -1);
            Point endPoint = new Point(-1, -1);

            int restartIndex = searchStartPoint.X;

            for (int y = searchStartPoint.Y; y < height - 1; y++)
            {
                for (int x = restartIndex; x < width - 1; x++)
                {
                    if (startPoint.X == -1 && startPoint.Y == -1 &&
                        pixelBuffer[calculateByteOffset(x, y, srcData.Stride)] != 0)
                    {
                        startPoint = new Point(x, y);
                    }
                    else if (middlePoint.X == -1 && middlePoint.Y == -1 &&
                      pixelBuffer[calculateByteOffset(x, y, srcData.Stride)] == 0)
                    {
                        middlePoint = new Point(x, y);
                    }
                    else if (endPoint.X == -1 && endPoint.Y == -1 && middlePoint.X != -1 && middlePoint.Y != -1 &&
                      pixelBuffer[calculateByteOffset(x, y, srcData.Stride)] != 0)
                    {
                        endPoint = new Point(x, y);
                        return Tuple.Create(startPoint, middlePoint, endPoint);
                    }
                    restartIndex = 1;
                }
            }
            return Tuple.Create(new Point(-1, -1), new Point(-1, -1), new Point(-1, -1));
        }

        private int calculateByteOffset(int x, int y, int stride)
        {
            return y * stride + x * 4;
        }

        private Bitmap performNormalization(Bitmap srcImage)
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

            int byteOffset;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byteOffset = calculateByteOffset(x, y, srcData.Stride);
                    byte value = pixelBuffer[byteOffset];

                    if (value > 127)
                    {
                        value = 255;
                    }
                    else
                    {
                        value = 0;
                    }

                    resultBuffer[byteOffset] = value;
                    resultBuffer[byteOffset + 1] = value;
                    resultBuffer[byteOffset + 2] = value;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }


            Bitmap result = new Bitmap(width, height);
            BitmapData resultData = result.LockBits(canvas, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            result.UnlockBits(resultData);
            return result;
        }
    }
}

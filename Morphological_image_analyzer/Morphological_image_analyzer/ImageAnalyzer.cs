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

            while (true)
            {
                Point point = findPixel(bitmap);
                if (point.X != -1 || point.Y != -1)
                {
                    count++;
                    bitmap = floodFillService.FloodFill(bitmap, point, Color.Black, Color.White);
                }
                else
                {
                    return count;
                }
            }

        }

        private Point findPixel(Bitmap srcImage)
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

            int byteOffset = 0;

            for (int y = 5; y < height - 5; y++)
            {
                for (int x = 5; x < width - 5; x++)
                {
                    byteOffset = y * srcData.Stride + x * 4;
                    if (pixelBuffer[byteOffset] == 0)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(-1, -1);
        }

    }
}

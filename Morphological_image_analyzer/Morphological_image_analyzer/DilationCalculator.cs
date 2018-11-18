using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Morphological_image_analyzer
{
    public class DilationCalculator
    {

        private byte[,] shape
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

        public Bitmap performMorphologicalOperation(Bitmap srcImg)
        {
            //Create image dimension variables for convenience
            int width = srcImg.Width;
            int height = srcImg.Height;

            //Lock bits to system memory for fast processing
            Rectangle canvas = new Rectangle(0, 0, width, height);
            BitmapData srcData = srcImg.LockBits(canvas, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;
            int bytes = stride * srcData.Height;

            //Create byte arrays that will hold all pixel data, one for processing, one for output
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            //Write pixel data to array meant for processing
            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, bytes);
            srcImg.UnlockBits(srcData);

            int kernelSize = 3;
            int kernelDim = kernelSize;

            //This is the offset of center pixel from border of the kernel
            int kernelOffset = (kernelDim - 1) / 2;
            int calcOffset = 0;
            int byteOffset = 0;
            for (int y = kernelOffset; y < height - kernelOffset; y++)
            {
                for (int x = kernelOffset; x < width - kernelOffset; x++)
                {
                    byte value = 255;
                    byteOffset = y * stride + x * 4;

                    //Apply dilation
                    for (int ykernel = -kernelOffset; ykernel <= kernelOffset; ykernel++)
                    {
                        for (int xkernel = -kernelOffset; xkernel <= kernelOffset; xkernel++)
                        {
                            if (shape[ykernel + kernelOffset, xkernel + kernelOffset] == 1)
                            {
                                calcOffset = byteOffset + ykernel * stride + xkernel * 4;
                                value = Math.Min(value, pixelBuffer[calcOffset]);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    //Write processed data into the second array
                    resultBuffer[byteOffset] = value;
                    resultBuffer[byteOffset + 1] = value;
                    resultBuffer[byteOffset + 2] = value;
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            //Create output bitmap of this function
            Bitmap rsltImg = new Bitmap(width, height);
            BitmapData rsltData = rsltImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            //Write processed data into bitmap form
            Marshal.Copy(resultBuffer, 0, rsltData.Scan0, bytes);
            rsltImg.UnlockBits(rsltData);
            return rsltImg;
        }
    }
}


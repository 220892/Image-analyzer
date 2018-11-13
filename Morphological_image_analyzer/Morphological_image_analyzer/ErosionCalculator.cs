using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class ErosionCalculator
{

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

    public Bitmap performMorphologicalOperation(Bitmap srcImage)
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


        int kernelSize = 3;
        int kernelOffset = (kernelSize - 1) / 2;
        int calcOffset = 0;
        int byteOffset = 0;

        for (int y = kernelOffset; y < height - kernelOffset; y++)
        {
            for (int x = kernelOffset; x < width - kernelOffset; x++)
            {
                byte value = 255;
                byteOffset = y * srcData.Stride + x * 4;
                for (int ykernel = -kernelOffset; ykernel <= kernelOffset; ykernel++)
                {
                    for (int xkernel = -kernelOffset; xkernel <= kernelOffset; xkernel++)
                    {
                        if (kernel[ykernel + kernelOffset, xkernel + kernelOffset] == 1)
                        {
                            calcOffset = byteOffset + ykernel * srcData.Stride + xkernel * 4;
                            value = Math.Min(value, pixelBuffer[calcOffset]);
                        }
                        else
                        {
                            continue;
                        }
                    }
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

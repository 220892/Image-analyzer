using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Morphological_image_analyzer
{
    public class DilationCalculator
    {

        private Bitmap bmpimg;

        public void setImage(Bitmap bmp)
        {
            bmpimg = (Bitmap)bmp.Clone();
        }

        public Bitmap getImage()
        {
            return (Bitmap)bmpimg.Clone();
        }

        public void performMorphologicalOperation()
        {

            byte[,] sele = new byte[3, 3];

                sele[0, 0] = 1;
                sele[0, 1] = 1;
                sele[0, 2] = 1;
                sele[1, 0] = 1;
                sele[1, 1] = 1;
                sele[1, 2] = 1;
                sele[2, 0] = 1;
                sele[2, 1] = 1;
                sele[2, 2] = 1;


            Bitmap tempbmp = (Bitmap)this.bmpimg.Clone();
            BitmapData data2 = tempbmp.LockBits(new Rectangle(0, 0, tempbmp.Width, tempbmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData data = bmpimg.LockBits(new Rectangle(0, 0, bmpimg.Width, bmpimg.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte[,] sElement = sele;

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* tptr = (byte*)data2.Scan0;

                ptr += data.Stride + 3;
                tptr += data.Stride + 3;

                int remain = data.Stride - data.Width * 3;

                for (int i = 1; i < data.Height - 1; i++)
                {
                    for (int j = 1; j < data.Width - 1; j++)
                    {

                        if (ptr[0] == 255)
                        {
                            byte* temp = tptr - data.Stride - 3;

                            for (int k = 0; k < 3; k++)
                            {
                                for (int l = 0; l < 3; l++)
                                {
                                    temp[data.Stride * k + l * 3] = temp[data.Stride * k + l * 3 + 1] = temp[data.Stride * k + l * 3 + 2] = (byte)(sElement[k, l] * 255);
                                }
                            }
                        }

                        ptr += 3;
                        tptr += 3;
                    }
                    ptr += remain + 6;
                    tptr += remain + 6;
                }
            }

            bmpimg.UnlockBits(data);
            tempbmp.UnlockBits(data2);

            bmpimg = (Bitmap)tempbmp.Clone();

        }
    }
}


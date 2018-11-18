using System;
using System.Drawing;


namespace Morphological_image_analyzer
{
    public abstract class AbstractMorphologicalCalculator : IMorphologicalCalculator
    {
        public abstract Bitmap performMorphologicalOperation(Bitmap srcImg, byte[,] kernel);

        protected void validateKernel(byte[,] kernel)
        {
            if (kernel.GetLength(0) != kernel.GetLength(1))
            {
                throw new ArgumentException("Podano niepoprawny kernel!");
            }
        }
    }
}

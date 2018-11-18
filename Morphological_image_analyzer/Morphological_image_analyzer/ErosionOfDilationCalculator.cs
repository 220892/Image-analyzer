using System.Drawing;

namespace Morphological_image_analyzer
{
    class ErosionOfDilationCalculator : AbstractMorphologicalCalculator
    {
        // injection of morphological operation performers
        static readonly DilationCalculator dilationCalculator = new DilationCalculator();
        static readonly ErosionCalculator erosionCalculator = new ErosionCalculator();

        public override Bitmap performMorphologicalOperation(Bitmap srcImg, byte[,] kernel)
        {
            validateKernel(kernel);

            srcImg = dilationCalculator.performMorphologicalOperation(srcImg, kernel);
            srcImg = erosionCalculator.performMorphologicalOperation(srcImg, kernel);

            return srcImg;
        }
    }
}

using System.Drawing;

namespace Morphological_image_analyzer
{
    class ErosionOfDilationCalculator : IMorphologicalCalculator
    {
        // injection of morphological operation performers
        static readonly DilationCalculator dilationCalculator = new DilationCalculator();
        static readonly ErosionCalculator erosionCalculator = new ErosionCalculator();

        public Bitmap performMorphologicalOperation(Bitmap srcImg, byte[,] kernel)
        {
            srcImg = dilationCalculator.performMorphologicalOperation(srcImg, kernel);
            srcImg = erosionCalculator.performMorphologicalOperation(srcImg, kernel);

            return srcImg;
        }
    }
}

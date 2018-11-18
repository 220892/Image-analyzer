using System.Drawing;

namespace Morphological_image_analyzer
{
    class ErosionOfDilationCalculator
    {
        // injection of morphological operation performers
        static readonly DilationCalculator dilationCalculator = new DilationCalculator();
        static readonly ErosionCalculator erosionCalculator = new ErosionCalculator();

        public Bitmap performMorphologicalOperation(Bitmap srcImg)
        {
            srcImg = dilationCalculator.performMorphologicalOperation(srcImg);
            srcImg = erosionCalculator.performMorphologicalOperation(srcImg);

            return srcImg;
        }
    }
}

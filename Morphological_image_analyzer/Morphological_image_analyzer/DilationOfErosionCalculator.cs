using System.Drawing;


namespace Morphological_image_analyzer
{
    class DilationOfErosionCalculator : IMorphologicalCalculator
    {
        // injection of morphological operation performers
        static readonly DilationCalculator dilationCalculator = new DilationCalculator();
        static readonly ErosionCalculator erosionCalculator = new ErosionCalculator();

        public Bitmap performMorphologicalOperation(Bitmap srcImg)
        {
            srcImg = erosionCalculator.performMorphologicalOperation(srcImg);
            srcImg = dilationCalculator.performMorphologicalOperation(srcImg);

            return srcImg;
        }
    }
}

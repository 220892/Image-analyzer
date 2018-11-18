using System.Drawing;

namespace Morphological_image_analyzer
{
    interface IMorphologicalCalculator
    {
        Bitmap performMorphologicalOperation(Bitmap srcImg);
    }
}

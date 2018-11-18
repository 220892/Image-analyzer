using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Morphological_image_analyzer
{
    public class DilationCalculator : AbstractDilationAndErosionCalculator
    {
        protected override byte getInitialValueForAlgorithm()
        {
            return 255;
        }

        protected override byte getValueFromCoreOperation(byte val1, byte val2)
        {
            return Math.Min(val1, val2);
        }
    }
}


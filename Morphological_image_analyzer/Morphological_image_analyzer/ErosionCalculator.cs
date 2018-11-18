using System;

namespace Morphological_image_analyzer
{
    public class ErosionCalculator : AbstractDilationAndErosionCalculator
    {
        protected override byte getInitialValueForAlgorithm()
        {
            return 0;
        }

        protected override byte getValueFromCoreOperation(byte val1, byte val2)
        {
            return Math.Max(val1, val2);
        }
    }
}

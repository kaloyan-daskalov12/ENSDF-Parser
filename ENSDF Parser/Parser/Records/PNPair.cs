using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class PNPair
    {
        public ParentRecord Parent;
        public NormalizationRecord Normalization;
        public ProductionNormalizationRecord ProductionNormalization;

        public override string ToString()
        {
            if (Parent is null)
                return Normalization?.ToString() ?? string.Empty;

            if (Normalization is null)
                return Parent.ToString();

            return Parent.ToString()
                + Normalization.ToString();
        }
    }
}

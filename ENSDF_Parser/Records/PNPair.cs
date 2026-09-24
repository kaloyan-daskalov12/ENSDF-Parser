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
            var records = new List<Record>();

            if (Parent != null)
                records.Add(Parent);

            if (Normalization != null)
                records.Add(Normalization);

            if (ProductionNormalization != null)
                records.Add(ProductionNormalization);

            return string.Join(
                Environment.NewLine,
                records.Select(r => r.ToString()));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ENSDF_Parser.MetaData
{
    public struct RecordType
    {
        public char C1;
        public char C2;
        public char C3;
        public char C4;

        public override string ToString()
        {
            return $"{C1}{C2}{C3}{C4}";
        }
    }
}

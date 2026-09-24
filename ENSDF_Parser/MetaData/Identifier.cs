using System;
using System.Collections.Generic;
using System.Text;

namespace ENSDF_Parser.MetaData
{
    public struct Identifier
    {
        public byte Isotrope;
        public string Name;

        public override string ToString()
        {
            return $"{Isotrope}{Name}";
        }
    }
}

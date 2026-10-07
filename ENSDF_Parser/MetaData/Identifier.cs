using System;
using System.Collections.Generic;
using System.Text;

namespace ENSDF_Parser.MetaData
{
    public struct Identifier
    {
        public string Isotrope;
        public string Name;

        public override string ToString()
        {
            string isotrope = Isotrope ?? "";
            string name = Name ?? "";

            if (isotrope.Length > 3)
                throw new InvalidOperationException(
                    $"Mass identifier '{isotrope}' exceeds ENSDF columns 1-3.");

            if (name.Length > 2)
                throw new InvalidOperationException(
                    $"Element identifier '{name}' exceeds ENSDF columns 4-5.");

            return isotrope.PadLeft(3) + name.PadRight(2);
        }
    }
}

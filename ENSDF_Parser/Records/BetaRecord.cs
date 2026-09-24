using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class BetaRecord : Record
    {
        public BetaRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype)
        {
            E = line.Substring(9, 10);
            DE = line.Substring(19, 2);
            IB = line.Substring(21, 8);
            DIB = line.Substring(29, 2);
            LOGFT = line.Substring(41, 8);
            DFT = line.Substring(49, 6);
            FLAG = line.Substring(76, 1);
            UN = line.Substring(77, 2);
            Q = line.Substring(79, 1);
        }

        private string e;
        public string E
        {
            get => e;
            set => e = value.Trim();
        }

        private string de;
        public string DE
        {
            get => de;
            set => de = value.Trim();
        }

        private string ib;
        public string IB
        {
            get => ib;
            set => ib = value.Trim();
        }

        private string dib;
        public string DIB
        {
            get => dib;
            set => dib = value.Trim();
        }

        private string logft;
        public string LOGFT
        {
            get => logft;
            set => logft = value.Trim();
        }

        private string dft;
        public string DFT
        {
            get => dft;
            set => dft = value.Trim();
        }

        private string flag;
        public string FLAG
        {
            get => flag;
            set => flag = value.Trim();
        }

        private string un;
        public string UN
        {
            get => un;
            set => un = value.Trim();
        }

        private string q;
        public string Q
        {
            get => q;
            set => q = value.Trim();
        }

        public override string ToString()
        {
            return base.ToString()
                + FormatToNChars(E, 10)
                + FormatToNChars(DE, 2)
                + FormatToNChars(IB, 8)
                + FormatToNChars(DIB, 2)
                + new string(' ', 10)
                + FormatToNChars(LOGFT, 8)
                + FormatToNChars(DFT, 6)
                + new string(' ', 21)
                + FormatToNChars(FLAG, 1)
                + FormatToNChars(UN, 2)
                + FormatToNChars(Q, 1);
        }
    }
}

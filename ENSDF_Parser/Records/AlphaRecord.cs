using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class AlphaRecord : Record
    {
        public AlphaRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype, line)
        {
            E = line.Substring(9, 10);
            DE = line.Substring(19, 2);
            IA = line.Substring(21, 8);
            DIA = line.Substring(29, 2);
            HF = line.Substring(31, 8);
            DHF = line.Substring(39, 2);
            FLAG = line.Substring(76, 1);
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

        private string ia;
        public string IA
        {
            get => ia;
            set => ia = value.Trim();
        }

        private string dia;
        public string DIA
        {
            get => dia;
            set => dia = value.Trim();
        }

        private string hf;
        public string HF
        {
            get => hf;
            set => hf = value.Trim();
        }

        private string dhf;
        public string DHF
        {
            get => dhf;
            set => dhf = value.Trim();
        }

        private string flag;
        public string FLAG
        {
            get => flag;
            set => flag = value.Trim();
        }

        private string q;
        public string Q
        {
            get => q;
            set => q = value.Trim();
        }

        public override string ToString()
        {
            return $"{Id}{RType}"
                + FormatToNChars(E, 10)
                + FormatToNChars(DE, 2)
                + FormatToNChars(IA, 8)
                + FormatToNChars(DIA, 2)
                + FormatToNChars(HF, 8)
                + FormatToNChars(DHF, 2)
                + new string(' ', 35)
                + FormatToNChars(FLAG, 1)
                + new string(' ', 2)
                + FormatToNChars(Q, 1);
        }
    }
}

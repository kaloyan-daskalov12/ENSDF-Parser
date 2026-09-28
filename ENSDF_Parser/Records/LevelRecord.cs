using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class LevelRecord : Record
    {
        public LevelRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype, line)
        {
            E = line.Substring(9, 10);
            DE = line.Substring(19, 2);
            J = line.Substring(21, 18);
            T = line.Substring(39, 10);
            DT = line.Substring(49, 6);
            L = line.Substring(55, 9);
            S = line.Substring(64, 10);
            DS = line.Substring(74, 2);
            FLAG = line.Substring(76, 1);
            MS = line.Substring(77, 2);
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

        private string j;
        public string J
        {
            get => j;
            set => j = value.Trim();
        }

        private string t;
        public string T
        {
            get => t;
            set => t = value.Trim();
        }

        private string dt;
        public string DT
        {
            get => dt;
            set => dt = value.Trim();
        }

        private string l;
        public string L
        {
            get => l;
            set => l = value.Trim();
        }

        private string s;
        public string S
        {
            get => s;
            set => s = value.Trim();
        }

        private string ds;
        public string DS
        {
            get => ds;
            set => ds = value.Trim();
        }

        private string flag;
        public string FLAG
        {
            get => flag;
            set => flag = value.Trim();
        }

        private string ms;
        public string MS
        {
            get => ms;
            set => ms = value.Trim();
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
                + FormatToNChars(J, 18)
                + FormatToNChars(T, 10)
                + FormatToNChars(DT, 6)
                + FormatToNChars(L, 9)
                + FormatToNChars(S, 10)
                + FormatToNChars(DS, 2)
                + FormatToNChars(FLAG, 1)
                + FormatToNChars(MS, 2)
                + FormatToNChars(Q, 1);
        }
    }
}

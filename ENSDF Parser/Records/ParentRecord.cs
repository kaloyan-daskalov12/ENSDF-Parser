using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class ParentRecord : Record
    {
        public ParentRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype)
        {
            E = line.Substring(9, 10);
            DE = line.Substring(19, 2);
            J = line.Substring(21, 18);
            T = line.Substring(39, 10);
            DT = line.Substring(49, 6);
            QP = line.Substring(64, 10);
            DQP = line.Substring(74, 2);
            ION = line.Substring(76, 4);
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

        private string qp;
        public string QP
        {
            get => qp;
            set => qp = value.Trim();
        }

        private string dqp;
        public string DQP
        {
            get => dqp;
            set => dqp = value.Trim();
        }

        private string ion;
        public string ION
        {
            get => ion;
            set => ion = value.Trim();
        }

        public override string ToString()
        {
            return base.ToString()
                + FormatToNChars(E, 10)
                + FormatToNChars(DE, 2)
                + FormatToNChars(J, 18)
                + FormatToNChars(T, 10)
                + FormatToNChars(DT, 6)
                + new string(' ', 9)
                + FormatToNChars(QP, 10)
                + FormatToNChars(DQP, 2)
                + FormatToNChars(ION, 4);
        }
    }
}

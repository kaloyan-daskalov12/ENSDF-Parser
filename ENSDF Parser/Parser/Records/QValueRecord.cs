using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class QValueRecord : Record
    {
        public QValueRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype)
        {
            Q = line.Substring(9, 10);
            DQ = line.Substring(19, 2);
            SN = line.Substring(21, 8);
            DSN = line.Substring(29, 2);
            SP = line.Substring(31, 8);
            DSP = line.Substring(39, 2);
            QA = line.Substring(41, 8);
            DQA = line.Substring(49, 6);
            QREF = line.Substring(55, 25);
        }

        private string q;
        public string Q
        {
            get => q;
            set => q = value.Trim();
        }

        private string dq;
        public string DQ
        {
            get => dq;
            set => dq = value.Trim();
        }

        private string sn;
        public string SN
        {
            get => sn;
            set => sn = value.Trim();
        }

        private string dsn;
        public string DSN
        {
            get => dsn;
            set => dsn = value.Trim();
        }

        private string sp;
        public string SP
        {
            get => sp;
            set => sp = value.Trim();
        }

        private string dsp;
        public string DSP
        {
            get => dsp;
            set => dsp = value.Trim();
        }

        private string qa;
        public string QA
        {
            get => qa;
            set => qa = value.Trim();
        }

        private string dqa;
        public string DQA
        {
            get => dqa;
            set => dqa = value.Trim();
        }

        private string qref;
        public string QREF
        {
            get => qref;
            set => qref = value.Trim();
        }

        public override string ToString()
        {
            return base.ToString()
                + FormatToNChars(Q, 10)
                + FormatToNChars(DQ, 2)
                + FormatToNChars(SN, 8)
                + FormatToNChars(DSN, 2)
                + FormatToNChars(SP, 8)
                + FormatToNChars(DSP, 2)
                + FormatToNChars(QA, 8)
                + FormatToNChars(DQA, 6)
                + FormatToNChars(QREF, 25);
        }
    }
}

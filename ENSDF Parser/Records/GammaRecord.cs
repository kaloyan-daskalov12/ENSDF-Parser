using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class GammaRecord : Record
    {
        public GammaRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype)
        {
            E = line.Substring(9, 10);
            DE = line.Substring(19, 2);
            RI = line.Substring(21, 8);
            DRI = line.Substring(29, 2);
            M = line.Substring(31, 10);
            MR = line.Substring(41, 8);
            DMR = line.Substring(49, 6);
            CC = line.Substring(55, 7);
            DCC = line.Substring(62, 2);
            TI = line.Substring(64, 10);
            DTI = line.Substring(74, 2);
            FLAG = line.Substring(76, 1);
            COIN = line.Substring(77, 1);
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

        private string ri;
        public string RI
        {
            get => ri;
            set => ri = value.Trim();
        }

        private string dri;
        public string DRI
        {
            get => dri;
            set => dri = value.Trim();
        }

        private string m;
        public string M
        {
            get => m;
            set => m = value.Trim();
        }

        private string mr;
        public string MR
        {
            get => mr;
            set => mr = value.Trim();
        }

        private string dmr;
        public string DMR
        {
            get => dmr;
            set => dmr = value.Trim();
        }

        private string cc;
        public string CC
        {
            get => cc;
            set => cc = value.Trim();
        }

        private string dcc;
        public string DCC
        {
            get => dcc;
            set => dcc = value.Trim();
        }

        private string ti;
        public string TI
        {
            get => ti;
            set => ti = value.Trim();
        }

        private string dti;
        public string DTI
        {
            get => dti;
            set => dti = value.Trim();
        }

        private string flag;
        public string FLAG
        {
            get => flag;
            set => flag = value.Trim();
        }

        private string coin;
        public string COIN
        {
            get => coin;
            set => coin = value.Trim();
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
                + FormatToNChars(RI, 8)
                + FormatToNChars(DRI, 2)
                + FormatToNChars(M, 10)
                + FormatToNChars(MR, 8)
                + FormatToNChars(DMR, 6)
                + FormatToNChars(CC, 7)
                + FormatToNChars(DCC, 2)
                + FormatToNChars(TI, 10)
                + FormatToNChars(DTI, 2)
                + FormatToNChars(FLAG, 1)
                + FormatToNChars(COIN, 1)
                + new string(' ', 1)
                + FormatToNChars(Q, 1);
        }
    }
}

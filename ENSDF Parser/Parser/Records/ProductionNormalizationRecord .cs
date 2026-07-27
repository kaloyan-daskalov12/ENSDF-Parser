using ENSDF_Parser.MetaData;
using ENSDF_Parser.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSDF_Parser.Records
{
    public class ProductionNormalizationRecord : Record
    {
        public ProductionNormalizationRecord(
            Identifier id,
            RecordType rtype,
            string line)
            : base(id, rtype)
        {
            NRBR = line.Substring(9, 10);
            DNRBR = line.Substring(19, 2);
            NTBR = line.Substring(21, 8);
            DNTBR = line.Substring(29, 2);

            NBBR = line.Substring(41, 8);
            DNBBR = line.Substring(49, 6);
            NP = line.Substring(55, 7);
            DNP = line.Substring(62, 2);

            COM = line.Substring(76, 1);
            OPT = line.Substring(77, 1);
        }

        private string nrbr;
        public string NRBR
        {
            get => nrbr;
            set => nrbr = value.Trim();
        }

        private string dnrbr;
        public string DNRBR
        {
            get => dnrbr;
            set => dnrbr = value.Trim();
        }

        private string ntbr;
        public string NTBR
        {
            get => ntbr;
            set => ntbr = value.Trim();
        }

        private string dntbr;
        public string DNTBR
        {
            get => dntbr;
            set => dntbr = value.Trim();
        }

        private string nbbr;
        public string NBBR
        {
            get => nbbr;
            set => nbbr = value.Trim();
        }

        private string dnbbr;
        public string DNBBR
        {
            get => dnbbr;
            set => dnbbr = value.Trim();
        }

        private string np;
        public string NP
        {
            get => np;
            set => np = value.Trim();
        }

        private string dnp;
        public string DNP
        {
            get => dnp;
            set => dnp = value.Trim();
        }

        private string com;
        public string COM
        {
            get => com;
            set => com = value.Trim();
        }

        private string opt;
        public string OPT
        {
            get => opt;
            set => opt = value.Trim();
        }

        public override string ToString()
        {
            return base.ToString()
                + FormatToNChars(NRBR, 10)
                + FormatToNChars(DNRBR, 2)
                + FormatToNChars(NTBR, 8)
                + FormatToNChars(DNTBR, 2)
                + new string(' ', 10)
                + FormatToNChars(NBBR, 8)
                + FormatToNChars(DNBBR, 6)
                + FormatToNChars(NP, 7)
                + FormatToNChars(DNP, 2)
                + new string(' ', 12)
                + FormatToNChars(COM, 1)
                + FormatToNChars(OPT, 1)
                + new string(' ', 2);
        }
    }
}

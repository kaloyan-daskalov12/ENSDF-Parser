using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class NormalizationRecord : Record
    {
        public NormalizationRecord(
            Identifier id,
            RecordType rtype,
            string line)
            : base(id, rtype)
        {
            NR = line.Substring(9, 10);
            DNR = line.Substring(19, 2);
            NT = line.Substring(21, 8);
            DNT = line.Substring(29, 2);
            BR = line.Substring(31, 8);
            DBR = line.Substring(39, 2);
            NB = line.Substring(41, 8);
            DNB = line.Substring(49, 6);
            NP = line.Substring(55, 7);
            DNP = line.Substring(62, 2);
        }

        private string nr;
        public string NR
        {
            get => nr;
            set => nr = value.Trim();
        }

        private string dnr;
        public string DNR
        {
            get => dnr;
            set => dnr = value.Trim();
        }

        private string nt;
        public string NT
        {
            get => nt;
            set => nt = value.Trim();
        }

        private string dnt;
        public string DNT
        {
            get => dnt;
            set => dnt = value.Trim();
        }

        private string br;
        public string BR
        {
            get => br;
            set => br = value.Trim();
        }

        private string dbr;
        public string DBR
        {
            get => dbr;
            set => dbr = value.Trim();
        }

        private string nb;
        public string NB
        {
            get => nb;
            set => nb = value.Trim();
        }

        private string dnb;
        public string DNB
        {
            get => dnb;
            set => dnb = value.Trim();
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

        public override string ToString()
        {
            return base.ToString()
                + FormatToNChars(NR, 10)
                + FormatToNChars(DNR, 2)
                + FormatToNChars(NT, 8)
                + FormatToNChars(DNT, 2)
                + FormatToNChars(BR, 8)
                + FormatToNChars(DBR, 2)
                + FormatToNChars(NB, 8)
                + FormatToNChars(DNB, 6)
                + FormatToNChars(NP, 7)
                + FormatToNChars(DNP, 2)
                + new string(' ', 16);
        }
    }
}

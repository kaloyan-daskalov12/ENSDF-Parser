using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class IdentificationRecord : Record
    {
        public IdentificationRecord(
            Identifier id,
            RecordType rtype,
            string line)
            : base(id, rtype, line)
        {
            DSID = line.Substring(9, 30);
            DSREF = line.Substring(39, 26);
            PUB = line.Substring(65, 9);
            DATE = line.Substring(74, 6);
        }

        private string dsid;
        public string DSID
        {
            get => dsid;
            set => dsid = value.Trim();
        }

        private string dsref;
        public string DSREF
        {
            get => dsref;
            set => dsref = value.Trim();
        }

        private string pub;
        public string PUB
        {
            get => pub;
            set => pub = value.Trim();
        }

        private string date;
        public string DATE
        {
            get => date;
            set => date = value.Trim();
        }

        public override string ToString()
        {
            return $"{Id}{RType}"
                + FormatToNChars(DSID, 30)
                + FormatToNChars(DSREF, 26)
                + FormatToNChars(PUB, 9)
                + FormatToNChars(DATE, 6);
        }
    }
}

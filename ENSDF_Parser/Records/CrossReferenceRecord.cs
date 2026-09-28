using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class CrossReferenceRecord : Record
    {
        public CrossReferenceRecord(
            Identifier id,
            RecordType rtype,
            string line)
            : base(id, rtype, line)
        {
            DSSYM = line.Substring(8, 1);
            DSID = line.Substring(9, 30);
        }

        private string dssym;
        public string DSSYM
        {
            get => dssym;
            set => dssym = value.Trim();
        }

        private string dsid;
        public string DSID
        {
            get => dsid;
            set => dsid = value.Trim();
        }

        public override string ToString()
        {
            return $"{Id}{RType}".Substring(0, 8)
                + FormatToNChars(DSSYM, 1)
                + FormatToNChars(DSID, 30)
                + new string(' ', 41);
        }
    }
}

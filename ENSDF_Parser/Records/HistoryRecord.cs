using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class HistoryRecord : Record
    {
        public HistoryRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype, line)
        {
            HTEXT = line.Substring(9, 71);
        }

        private string htext;
        public string HTEXT
        {
            get => htext;
            set => htext = value.Trim();
        }

        public override string ToString()
        {
            return $"{Id}{RType}"
                + FormatToNChars(HTEXT, 71);
        }
    }
}

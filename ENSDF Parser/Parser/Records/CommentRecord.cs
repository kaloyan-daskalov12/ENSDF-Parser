using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class CommentRecord : Record
    {
        public CommentRecord(Identifier id, RecordType rtype, string line)
            : base(id, rtype)
        {
            CTEXT = line.Substring(9, 71);
        }

        private string ctext;
        public string CTEXT
        {
            get => ctext;
            set => ctext = value.Trim();
        }

        public override string ToString()
        {
            return base.ToString()
                + FormatToNChars(CTEXT, 71);
        }
    }
}

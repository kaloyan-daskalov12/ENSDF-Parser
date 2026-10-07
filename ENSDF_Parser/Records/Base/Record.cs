using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class Record
    {
        public Record(Identifier id, RecordType rtype, string line)
        {
            Id = id;
            RType = rtype;
            Comments = new List<CommentRecord>();
            OriginalText = line;
            if (line.Length < 80) throw new Exception($"Incorrect input from the line: \"{line}\" -> Length < 80");
        }

        public Identifier Id { get; }
        public RecordType RType { get; }
        public List<CommentRecord> Comments { get; }

        string OriginalText;

        public override string ToString()
        {
            return OriginalText;
        }

        protected string CompileComments()
        {
            string result = "";
            foreach(var c in Comments) result += $"{c}\n";
            return result;
        }

        public static string FormatToNChars(string input, int length)
        {
            if (input == null) input = "";

            if (input.Length > length) return input.Substring(0, length);

            return input.PadRight(length, ' '); 
        }
    }
}

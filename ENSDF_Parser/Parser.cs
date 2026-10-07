using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;
using ENSDF_Parser.Records;

namespace ENSDF_Parser
{
    public class Parser
    {
        public static List<DataSet> Parse(List<string> lines, string file)
        {
            List<DataSet> sets = new List<DataSet>();
            List<Record> records = new List<Record>();

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (line == "" || line.Replace(" ", "") == "")
                {
                    if (records.Count > 0)
                    {
                        sets.Add(new DataSet(records[0].Id, records, file));
                        records = new List<Record>();
                    }

                    continue;
                }

                if (line.Length < 80)
                {
                    throw new ArgumentException(
                        $"Invalid ENSDF record in '{file}' at line {i + 1}: " +
                        $"expected at least 80 characters, but got {line.Length}.");
                }

                records.Add(
                    RecognizePattern(
                        ParseId(line.Substring(0, 5)),
                        ParseRecType(line.Substring(5, 4)),
                        line));
            }

            if (records.Count > 0)
            {
                sets.Add(new DataSet(records[0].Id, records, file));
            }

            return sets;
        }

        static Identifier ParseId(string nucid)
        {
            return new Identifier()
            {
                Isotrope = nucid[..3].Trim(),
                Name = nucid[3..5].Trim()
            };
        }

        static RecordType ParseRecType(string rtype)
        {
            return new RecordType { C1 = rtype[0], C2 = rtype[1], C3 = rtype[2], C4 = rtype[3] };
        }

        static Record RecognizePattern(Identifier id, RecordType rtype, string line)
        {
            // END record: all 80 columns blank
            if (string.IsNullOrWhiteSpace(line)) return new Record(id, rtype, line);

            // Comment record:
            // col 7 = C/D/T/c/t
            if (rtype.C2 == 'C' || rtype.C2 == 'D' || rtype.C2 == 'T' ||rtype.C2 == 'c' || rtype.C2 == 't')
            {
                return new CommentRecord(id, rtype, line);
            }

            // Continuation record:
            // col 6 = any printable/alphanumeric char except blank or '1'
            // col 7 = blank
            // col 8 = L/B/E/G/H
            // col 9 = blank
            if (rtype.C1 != ' ' && rtype.C1 != '1' && rtype.C2 == ' ' && rtype.C4 == ' ' && (rtype.C3 == 'L' || rtype.C3 == 'B' || rtype.C3 == 'E' || rtype.C3 == 'G' || rtype.C3 == 'H'))
            {
                return new CommentRecord(id, rtype, line);
            }

            // Identification record:
            // cols 6-9 are blank
            if ((rtype.C1 == ' ' && rtype.C2 == ' ' && rtype.C3 == ' ' && rtype.C4 == ' ') || (char.IsDigit(rtype.C1) && rtype.C2 == ' ' && rtype.C3 == ' ' && rtype.C4 == ' '))
            {
                return new IdentificationRecord(id, rtype, line);
            }

            // Production Normalization record:
            // col 7 = P, col 8 = N
            // ProductionNormalizationRecord class
            if (rtype.C2 == 'P' && rtype.C3 == 'N')
            {
                return new ProductionNormalizationRecord(id, rtype, line);
            }

            // Particle or delayed-particle record:
            // col 8 = D for delayed particle
            // col 8 = blank for prompt particle
            // col 9 = particle symbol, e.g. N, P, A
            if ((rtype.C3 == 'D' || rtype.C3 == ' ') && (rtype.C4 == 'N' || rtype.C4 == 'P' || rtype.C4 == 'A' || rtype.C4 == 'D' || rtype.C4 == 'T'))
            {
                return new DelayedParticleRecord(id, rtype, line);
            }

            switch (rtype.C3)
            {
                case 'L': return new LevelRecord(id, rtype, line);
                case 'G': return new GammaRecord(id, rtype, line);
                case 'B': return new BetaRecord(id, rtype, line);
                case 'E': return new ECRecord(id, rtype, line);
                case 'A': return new AlphaRecord(id, rtype, line);
                case 'P': return new ParentRecord(id, rtype, line);
                case 'N': return new NormalizationRecord(id, rtype, line);
                case 'Q': return new QValueRecord(id, rtype, line);
                case 'X': return new CrossReferenceRecord(id, rtype, line);
                case 'H': return new HistoryRecord(id, rtype, line);

                default:
                    return new Record(id, rtype, line);
            }
        }
    }
}

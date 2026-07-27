using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;
using ENSDF_Parser.Records;

namespace ENSDF_Parser
{
    public class DataSet
    {
        public DataSet(Identifier id, List<Record> records)
        {
            Id = id;
            OrderedRecords = records;

            DistributeRecords(records);
        }

        void DistributeRecords(List<Record> records)
        {
            Level currentLevel = null;
            PNPair currentNormalizationPair = null;

            foreach (var r in records)
            {
                switch (r)
                {
                    case CommentRecord c:
                        switch (c.RType.C3)
                        {
                            case 'L':
                                if (currentLevel != null) currentLevel.LevelRecord.Comments.Add(c);
                                break;
                            case 'G':
                                if (currentLevel != null) { if (currentLevel.Data.GammaRecords.Count != 0) currentLevel.Data.GammaRecords[^1].Comments.Add(c); }
                                else
                                {
                                    if (UnplacedRecords.GammaRecords.Count != 0) UnplacedRecords.GammaRecords[^1].Comments.Add(c);
                                    else Header.CommentRecords.Add(c);
                                }
                                break;
                            case 'B':
                                if (currentLevel != null) { if (currentLevel.Data.BettaRecords.Count != 0) currentLevel.Data.BettaRecords[^1].Comments.Add(c); }
                                else
                                {
                                    if (UnplacedRecords.BettaRecords.Count != 0) UnplacedRecords.BettaRecords[^1].Comments.Add(c);
                                    else Header.CommentRecords.Add(c);
                                }
                                break;
                            case 'E':
                                if (currentLevel != null) { if (currentLevel.Data.ECRecords.Count != 0) currentLevel.Data.ECRecords[^1].Comments.Add(c); }
                                else
                                {
                                    if (UnplacedRecords.ECRecords.Count != 0) UnplacedRecords.ECRecords[^1].Comments.Add(c);
                                    else Header.CommentRecords.Add(c);
                                }
                                break;
                            case 'A':
                                if (currentLevel != null) { if (currentLevel.Data.AlphaRecords.Count != 0) currentLevel.Data.AlphaRecords[^1].Comments.Add(c); }
                                else
                                {
                                    if (UnplacedRecords.AlphaRecords.Count != 0) UnplacedRecords.AlphaRecords[^1].Comments.Add(c);
                                    else Header.CommentRecords.Add(c);
                                }
                                break;
                            case 'D':
                                if (currentLevel != null) { if (currentLevel.Data.DelayedParticleRecords.Count != 0) currentLevel.Data.DelayedParticleRecords[^1].Comments.Add(c); }
                                else
                                {
                                    if (UnplacedRecords.DelayedParticleRecords.Count != 0) UnplacedRecords.DelayedParticleRecords[^1].Comments.Add(c);
                                    else Header.CommentRecords.Add(c);
                                }
                                break;
                            case 'P':
                                if (Header.PN_Pairs.Count != 0) Header.PN_Pairs[^1].Parent.Comments.Add(c);
                                else Header.CommentRecords.Add(c);
                                break;
                            case 'N':
                                if (Header.PN_Pairs.Count != 0) Header.PN_Pairs[^1].Normalization.Comments.Add(c);
                                else Header.CommentRecords.Add(c);
                                break;
                            case 'Q':
                                if (Header.QValueRecords.Count != 0) Header.QValueRecords[^1].Comments.Add(c);
                                else Header.CommentRecords.Add(c);
                                break;
                            default:
                                Header.CommentRecords.Add(c);
                                break;
                        }
                        break;
                    case IdentificationRecord i:
                        Header.IdentificationRecords.Add(i);
                        break;
                    case LevelRecord l:
                        if (currentLevel != null) Levels.Add(currentLevel);
                        currentLevel = new Level() { LevelRecord = l };
                        break;
                    case GammaRecord g:
                        if (currentLevel != null) currentLevel.Data.GammaRecords.Add(g);
                        else UnplacedRecords.GammaRecords.Add(g);
                        break;
                    case BetaRecord b:
                        if (currentLevel != null) currentLevel.Data.BettaRecords.Add(b);
                        else UnplacedRecords.BettaRecords.Add(b);
                        break;
                    case ECRecord e:
                        if (currentLevel != null) currentLevel.Data.ECRecords.Add(e);
                        else UnplacedRecords.ECRecords.Add(e);
                        break;
                    case AlphaRecord a:
                        if (currentLevel != null) currentLevel.Data.AlphaRecords.Add(a);
                        else UnplacedRecords.AlphaRecords.Add(a);
                        break;
                    case DelayedParticleRecord d:
                        if (currentLevel != null) currentLevel.Data.DelayedParticleRecords.Add(d);
                        else UnplacedRecords.DelayedParticleRecords.Add(d);
                        break;
                    case ParentRecord p:
                        {
                            var pair = new PNPair
                            {
                                Parent = p
                            };

                            Header.PN_Pairs.Add(pair);
                            break;
                        }

                    case NormalizationRecord n:
                        {
                            currentNormalizationPair = null;

                            // A blank designator associates the N record with
                            // the most recently encountered P record.
                            if (n.RType.C4 == ' ')
                            {
                                if (Header.PN_Pairs.Count != 0)
                                {
                                    currentNormalizationPair = Header.PN_Pairs[^1];
                                    currentNormalizationPair.Normalization = n;
                                }
                                else
                                {
                                    Header.UnpairedNormalizationRecords.Add(n);
                                }
                                break;
                            }

                            // With multiple parents, column 9 identifies
                            // the corresponding P record.
                            foreach (var pair in Header.PN_Pairs)
                            {
                                if (pair.Parent?.RType.C4 == n.RType.C4)
                                {
                                    pair.Normalization = n;
                                    currentNormalizationPair = pair;
                                    break;
                                }
                            }

                            break;
                        }

                    case ProductionNormalizationRecord pn:
                        {
                            // According to the ENSDF format, PN immediately follows
                            // the N record to which it belongs.
                            if (currentNormalizationPair != null)
                            {
                                currentNormalizationPair.ProductionNormalization = pn;
                            }
                            else
                            {
                                // This needs a separate collection because an N/PN record
                                // can also appear in a non-decay data set without a P record.
                                Header.UnpairedProductionNormalizationRecords.Add(pn);
                            }

                            break;
                        }
                    case QValueRecord q:
                        Header.QValueRecords.Add(q);
                        break;
                    case CrossReferenceRecord x:
                        Header.CrossReferenceRecords.Add(x);
                        break;
                    case HistoryRecord h:
                        Header.HistoryRecords.Add(h);
                        break;
                }
            }
        }

        public string GetOverview()
        {
            return $"Element: {Id.Name} #{Id.Isotrope}; Recorded Levels: {Levels.Count}";
        }

        public Identifier Id;
        public Header Header = new();
        public RadiationData UnplacedRecords = new();
        public List<Level> Levels = new();

        List<Record> OrderedRecords;
        public override string ToString()
        {
            return string.Join("\n", OrderedRecords) + $"\n{new string(' ', 85)}";
        }
    }
}

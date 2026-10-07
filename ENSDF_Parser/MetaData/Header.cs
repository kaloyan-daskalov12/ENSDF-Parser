using ENSDF_Parser.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENSDF_Parser.MetaData
{
    public class Header
    {
        public List<IdentificationRecord> IdentificationRecords { get; } = new();
        public List<HistoryRecord> HistoryRecords { get; } = new();
        public List<CrossReferenceRecord> CrossReferenceRecords { get; } = new();
        public List<QValueRecord> QValueRecords { get; } = new();
        public List<PNPair> PN_Pairs { get; } = new();
        public List<CommentRecord> CommentRecords { get; } = new();
        public List<ProductionNormalizationRecord> UnpairedProductionNormalizationRecords { get; } = new();
        public List<NormalizationRecord> UnpairedNormalizationRecords { get; } = new();
    }
}

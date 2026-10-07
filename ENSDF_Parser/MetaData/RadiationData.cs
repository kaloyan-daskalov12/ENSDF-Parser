using ENSDF_Parser.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENSDF_Parser.MetaData
{
    public class RadiationData
    {
        public List<GammaRecord> GammaRecords { get; } = new();
        public List<BetaRecord> BettaRecords { get; } = new();
        public List<ECRecord> ECRecords { get; } = new();
        public List<AlphaRecord> AlphaRecords { get; } = new();
        public List<DelayedParticleRecord> DelayedParticleRecords { get; } = new();
    }
}

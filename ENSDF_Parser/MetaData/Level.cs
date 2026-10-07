using ENSDF_Parser.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENSDF_Parser.MetaData
{
    public class Level
    {
        public LevelRecord LevelRecord { get; set; }
        public RadiationData Data { get; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser;
using ENSDF_Parser.Records;
using ENSDF_Parser.MetaData;

namespace UserInterface.IO
{
    internal class Session
    {
        public Session()
        {
            Datasets = new();
            GammaCommentsValues = new();
        }

        public List<DataSet> Datasets;
        public DataSet? CurrentDataset;
        public Record? CurrentRecord;
        public Dictionary<DataSet, (List<string> FoundErrors, Dictionary<Level, Dictionary<GammaRecord, List<Value>>> Values)> GammaCommentsValues;
    }
}

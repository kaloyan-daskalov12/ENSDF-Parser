using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSDF_Parser
{
    struct Identifier
    {
        public byte Isotrope;
        public string Name;
    }

    struct RecordType
    {
        public char C1;
        public char C2;
        public char C3;
        public char C4;
    }

    struct Value
    {
        public double Val;
        public double DVal;
    }

    class Record
    {
        public Record(Identifier id, RecordType rtype)
        {
            Id = id;
            RType = rtype;
        }

        public Identifier Id;
        public RecordType RType;
    }

    class CommentRecord : Record
    {
        public CommentRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            CTEXT = line.Substring(9, 71).Replace(" ", "");
        }

        public string CTEXT;
    }

    class ContinuationRecord : Record
    {
        public ContinuationRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            DATA = line.Substring(9, 71).Replace(" ", "");
        }

        public string DATA;
    }

    class IdentificationRecord : Record
    {
        public IdentificationRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            DSID = line.Substring(9, 30).Replace(" ", "");
            DSREF = line.Substring(39, 26).Replace(" ", "");
            PUB = line.Substring(65, 9).Replace(" ", "");
            DATE = line.Substring(74, 6).Replace(" ", "");
        }

        public string DSID;
        public string DSREF;
        public string PUB;
        public string DATE;
    }

    class LevelRecord : Record
    {
        public LevelRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            E = line.Substring(9, 10).Replace(" ", "");
            DE = line.Substring(19, 2).Replace(" ", "");
            J = line.Substring(21, 18).Replace(" ", "");
            T = line.Substring(39, 10).Replace(" ", "");
            DT = line.Substring(49, 6).Replace(" ", "");
            L = line.Substring(55, 9).Replace(" ", "");
            S = line.Substring(64, 10).Replace(" ", "");
            DS = line.Substring(74, 2).Replace(" ", "");
            FLAG = line.Substring(76, 1).Replace(" ", "");
            MS = line.Substring(77, 2).Replace(" ", "");
            Q = line.Substring(79, 1).Replace(" ", "");
        }

        public string E;
        public string DE;
        public string J;
        public string T;
        public string DT;
        public string L;
        public string S;
        public string DS;
        public string FLAG;
        public string MS;
        public string Q;
    }

    class GammaRecord : Record
    {
        public GammaRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            E = line.Substring(9, 10).Replace(" ", "");
            DE = line.Substring(19, 2).Replace(" ", "");
            RI = line.Substring(21, 8).Replace(" ", "");
            DRI = line.Substring(29, 2).Replace(" ", "");
            M = line.Substring(31, 10).Replace(" ", "");
            MR = line.Substring(41, 8).Replace(" ", "");
            DMR = line.Substring(49, 6).Replace(" ", "");
            CC = line.Substring(55, 7).Replace(" ", "");
            DCC = line.Substring(62, 2).Replace(" ", "");
            TI = line.Substring(64, 10).Replace(" ", "");
            DTI = line.Substring(74, 2).Replace(" ", "");
            FLAG = line.Substring(76, 1).Replace(" ", "");
            COIN = line.Substring(77, 1).Replace(" ", "");
            Q = line.Substring(79, 1).Replace(" ", "");
        }

        public string E;
        public string DE;
        public string RI;
        public string DRI;
        public string M;
        public string MR;
        public string DMR;
        public string CC;
        public string DCC;
        public string TI;
        public string DTI;
        public string FLAG;
        public string COIN;
        public string Q;
    }

    class BetaRecord : Record
    {
        public BetaRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            E = line.Substring(9, 10).Replace(" ", "");
            DE = line.Substring(19, 2).Replace(" ", "");
            IB = line.Substring(21, 8).Replace(" ", "");
            DIB = line.Substring(29, 2).Replace(" ", "");
            LOGFT = line.Substring(41, 8).Replace(" ", "");
            DFT = line.Substring(49, 6).Replace(" ", "");
            FLAG = line.Substring(76, 1).Replace(" ", "");
            UN = line.Substring(77, 2).Replace(" ", "");
            Q = line.Substring(79, 1).Replace(" ", "");
        }

        public string E;
        public string DE;
        public string IB;
        public string DIB;
        public string LOGFT;
        public string DFT;
        public string FLAG;
        public string UN;
        public string Q;
    }

    class ECRecord : Record
    {
        public ECRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            E = line.Substring(9, 10).Replace(" ", "");
            DE = line.Substring(19, 2).Replace(" ", "");
            IB = line.Substring(21, 8).Replace(" ", "");
            DIB = line.Substring(29, 2).Replace(" ", "");
            IE = line.Substring(31, 8).Replace(" ", "");
            DIE = line.Substring(39, 2).Replace(" ", "");
            LOGFT = line.Substring(41, 8).Replace(" ", "");
            DFT = line.Substring(49, 6).Replace(" ", "");
            TI = line.Substring(64, 10).Replace(" ", "");
            DTI = line.Substring(74, 2).Replace(" ", "");
            FLAG = line.Substring(76, 1).Replace(" ", "");
            UN = line.Substring(77, 2).Replace(" ", "");
            Q = line.Substring(79, 1).Replace(" ", "");
        }

        public string E;
        public string DE;
        public string IB;
        public string DIB;
        public string IE;
        public string DIE;
        public string LOGFT;
        public string DFT;
        public string TI;
        public string DTI;
        public string FLAG;
        public string UN;
        public string Q;
    }

    class AlphaRecord : Record
    {
        public AlphaRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            E = line.Substring(9, 10).Replace(" ", "");
            DE = line.Substring(19, 2).Replace(" ", "");
            IA = line.Substring(21, 8).Replace(" ", "");
            DIA = line.Substring(29, 2).Replace(" ", "");
            HF = line.Substring(31, 8).Replace(" ", "");
            DHF = line.Substring(39, 2).Replace(" ", "");
            FLAG = line.Substring(76, 1).Replace(" ", "");
            Q = line.Substring(79, 1).Replace(" ", "");
        }

        public string E;
        public string DE;
        public string IA;
        public string DIA;
        public string HF;
        public string DHF;
        public string FLAG;
        public string Q;
    }

    class DelayedParticleRecord : Record
    {
        public DelayedParticleRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            Particle = line.Substring(8, 1).Replace(" ", "");

            E = line.Substring(9, 10).Replace(" ", "");
            DE = line.Substring(19, 2).Replace(" ", "");
            IP = line.Substring(21, 8).Replace(" ", "");
            DIP = line.Substring(29, 2).Replace(" ", "");
            EI = line.Substring(31, 8).Replace(" ", "");
            T = line.Substring(39, 10).Replace(" ", "");
            DT = line.Substring(49, 6).Replace(" ", "");
            L = line.Substring(55, 9).Replace(" ", "");
            FLAG = line.Substring(76, 1).Replace(" ", "");
            COIN = line.Substring(77, 1).Replace(" ", "");
            Q = line.Substring(79, 1).Replace(" ", "");
        }

        public string Particle;

        public string E;
        public string DE;
        public string IP;
        public string DIP;
        public string EI;
        public string T;
        public string DT;
        public string L;
        public string FLAG;
        public string COIN;
        public string Q;
    }

    class ParentRecord : Record
    {
        public ParentRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            E = line.Substring(9, 10).Replace(" ", "");
            DE = line.Substring(19, 2).Replace(" ", "");
            J = line.Substring(21, 18).Replace(" ", "");
            T = line.Substring(39, 10).Replace(" ", "");
            DT = line.Substring(49, 6).Replace(" ", "");
            QP = line.Substring(64, 10).Replace(" ", "");
            DQP = line.Substring(74, 2).Replace(" ", "");
            ION = line.Substring(76, 4).Replace(" ", "");
        }

        public string E;
        public string DE;
        public string J;
        public string T;
        public string DT;
        public string QP;
        public string DQP;
        public string ION;
    }

    class NormalizationRecord : Record
    {
        public NormalizationRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            NR = line.Substring(9, 10).Replace(" ", "");
            DNR = line.Substring(19, 2).Replace(" ", "");
            NT = line.Substring(21, 8).Replace(" ", "");
            DNT = line.Substring(29, 2).Replace(" ", "");
            BR = line.Substring(31, 8).Replace(" ", "");
            DBR = line.Substring(39, 2).Replace(" ", "");
            NB = line.Substring(41, 8).Replace(" ", "");
            DNB = line.Substring(49, 6).Replace(" ", "");
            NP = line.Substring(55, 7).Replace(" ", "");
            DNP = line.Substring(62, 2).Replace(" ", "");
        }

        public string NR;
        public string DNR;
        public string NT;
        public string DNT;
        public string BR;
        public string DBR;
        public string NB;
        public string DNB;
        public string NP;
        public string DNP;
    }

    class QValueRecord : Record
    {
        public QValueRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            Q = line.Substring(9, 10).Replace(" ", "");
            DQ = line.Substring(19, 2).Replace(" ", "");
            SN = line.Substring(21, 8).Replace(" ", "");
            DSN = line.Substring(29, 2).Replace(" ", "");
            SP = line.Substring(31, 8).Replace(" ", "");
            DSP = line.Substring(39, 2).Replace(" ", "");
            QA = line.Substring(41, 8).Replace(" ", "");
            DQA = line.Substring(49, 6).Replace(" ", "");
            QREF = line.Substring(55, 25).Replace(" ", "");
        }

        public string Q;
        public string DQ;
        public string SN;
        public string DSN;
        public string SP;
        public string DSP;
        public string QA;
        public string DQA;
        public string QREF;
    }

    class CrossReferenceRecord : Record
    {
        public CrossReferenceRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            DSSYM = line.Substring(8, 1).Replace(" ", "");
            DSID = line.Substring(9, 30).Replace(" ", "");
        }

        public string DSSYM;
        public string DSID;
    }

    class HistoryRecord : Record
    {
        public HistoryRecord(Identifier id, RecordType rtype, string line) : base(id, rtype)
        {
            HTEXT = line.Substring(9, 71).Replace(" ", "");
        }

        public string HTEXT;
    }
}
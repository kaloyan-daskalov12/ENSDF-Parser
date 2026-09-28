using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;

namespace ENSDF_Parser.Records
{
    public class DelayedParticleRecord : Record
    {
        public DelayedParticleRecord(
            Identifier id,
            RecordType rtype,
            string line)
            : base(id, rtype, line)
        {
            Particle = line.Substring(8, 1);
            E = line.Substring(9, 10);
            DE = line.Substring(19, 2);
            IP = line.Substring(21, 8);
            DIP = line.Substring(29, 2);
            EI = line.Substring(31, 8);
            T = line.Substring(39, 10);
            DT = line.Substring(49, 6);
            L = line.Substring(55, 9);
            FLAG = line.Substring(76, 1);
            COIN = line.Substring(77, 1);
            Q = line.Substring(79, 1);
        }

        private string particle;
        public string Particle
        {
            get => particle;
            set => particle = value.Trim();
        }

        private string e;
        public string E
        {
            get => e;
            set => e = value.Trim();
        }

        private string de;
        public string DE
        {
            get => de;
            set => de = value.Trim();
        }

        private string ip;
        public string IP
        {
            get => ip;
            set => ip = value.Trim();
        }

        private string dip;
        public string DIP
        {
            get => dip;
            set => dip = value.Trim();
        }

        private string ei;
        public string EI
        {
            get => ei;
            set => ei = value.Trim();
        }

        private string t;
        public string T
        {
            get => t;
            set => t = value.Trim();
        }

        private string dt;
        public string DT
        {
            get => dt;
            set => dt = value.Trim();
        }

        private string l;
        public string L
        {
            get => l;
            set => l = value.Trim();
        }

        private string flag;
        public string FLAG
        {
            get => flag;
            set => flag = value.Trim();
        }

        private string coin;
        public string COIN
        {
            get => coin;
            set => coin = value.Trim();
        }

        private string q;
        public string Q
        {
            get => q;
            set => q = value.Trim();
        }

        public override string ToString()
        {
            return $"{Id}{RType}".Substring(0, 8)
                + FormatToNChars(Particle, 1)
                + FormatToNChars(E, 10)
                + FormatToNChars(DE, 2)
                + FormatToNChars(IP, 8)
                + FormatToNChars(DIP, 2)
                + FormatToNChars(EI, 8)
                + FormatToNChars(T, 10)
                + FormatToNChars(DT, 6)
                + FormatToNChars(L, 9)
                + new string(' ', 12)
                + FormatToNChars(FLAG, 1)
                + FormatToNChars(COIN, 1)
                + new string(' ', 1)
                + FormatToNChars(Q, 1);
        }
    }
}

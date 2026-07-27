using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSDF_Parser.MetaData;
using ENSDF_Parser.Records;

namespace ENSDF_Parser
{
    public class ValueExtractor
    {
        /// <summary>
        /// Extracts the commented gamma values
        /// </summary>
        /// <param name="set">The observed set</param>
        /*public ValueExtractor(DataSet set)
        {
            Values = new();
            FoundErrors = new();
            bool isEnergyComment = false;

            foreach (var l in set.Levels)
            {
                foreach (var g in l.Data.GammaRecords)
                {
                    foreach (var c in g.Comments)
                    {
                        if (c.RType.C2 == 'c')
                        {
                            if (c.CTEXT.Contains("E$") || isEnergyComment)
                            {
                                if (c.CTEXT.Contains("$") && !c.CTEXT.Contains("E$"))
                                {
                                    isEnergyComment = false;
                                    continue;
                                }
                                isEnergyComment = true;

                                if (c.CTEXT.Contains("Other")) isEnergyComment = false;
                                string[] raw_data = c.CTEXT.Split("Other", StringSplitOptions.RemoveEmptyEntries);
                                if (raw_data.Length == 1 && isEnergyComment == false) continue;
                                string[] data = raw_data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (data[i] == "E$") continue;
                                    if (data[i].Contains(",")) continue;
                                    double energy;
                                    if (double.TryParse(data[i], out energy))
                                    {
                                        Value val = new Value();
                                        val.SetValue(data[i]);
                                        if (i + 1 < data.Length)
                                        {
                                            double dVal;
                                            if (data[i + 1].Contains("I"))
                                            {
                                                val.DVal = double.Parse(data[i + 1].Replace("{I", "").Replace("}", ""));
                                                i++;
                                            }
                                            else if (double.TryParse(data[i + 1], out dVal))
                                            {
                                                FoundErrors.Add($"Missing {{I...}} for Level: {l.LevelRecord.E} and Gamma: {g.E}!");
                                                val.DVal = dVal;
                                                i++;
                                            }
                                        }
                                        if (!Values.ContainsKey(l)) Values.Add(l, new Dictionary<GammaRecord, List<Value>>());
                                        if (!Values[l].ContainsKey(g)) Values[l].Add(g, new List<Value>());
                                        Values[l][g].Add(val);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }*/

        public static (List<string> FoundErrors, Dictionary<Level, Dictionary<GammaRecord, List<Value>>> Values) ParseFromComments(DataSet set, string initial, List<string> breakers)
        {
            List<string> FoundErrors = new();
            Dictionary<Level, Dictionary<GammaRecord, List<Value>>> Values = new();
            bool isEnergyComment = false;

            foreach (var l in set.Levels)
            {
                foreach (var g in l.Data.GammaRecords)
                {
                    foreach (var c in g.Comments)
                    {
                        if (c.RType.C2 == 'c')
                        {
                            if (c.CTEXT.Contains($"{initial}$") || isEnergyComment)
                            {
                                if (c.CTEXT.Contains("$") && !c.CTEXT.Contains($"{initial}$"))
                                {
                                    isEnergyComment = false;
                                    continue;
                                }
                                isEnergyComment = true;

                                if (breakers.Any(b => c.CTEXT.Contains(b))) isEnergyComment = false;
                                string[] data = c.CTEXT.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (!isEnergyComment && breakers.Any(b => data[i].Contains(b))) break;
                                    if (data[i] == $"{initial}$") continue;
                                    if (data[i].Contains(",")) continue;
                                    double energy;
                                    if (double.TryParse(data[i], out energy))
                                    {
                                        Value val = new Value();
                                        val.SetValue(data[i]);
                                        if (i + 1 < data.Length)
                                        {
                                            double dVal;
                                            if (data[i + 1].Contains("I"))
                                            {
                                                val.DVal = double.Parse(data[i + 1].Replace("{I", "").Replace("}", ""));
                                                i++;
                                            }
                                            else if (double.TryParse(data[i + 1], out dVal))
                                            {
                                                FoundErrors.Add($"Missing {{I...}} for Level: {l.LevelRecord.E} and Gamma: {g.E}!");
                                                val.DVal = dVal;
                                                i++;
                                            }
                                        }
                                        if (!Values.ContainsKey(l)) Values.Add(l, new Dictionary<GammaRecord, List<Value>>());
                                        if (!Values[l].ContainsKey(g)) Values[l].Add(g, new List<Value>());
                                        Values[l][g].Add(val);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return (FoundErrors, Values);
        }

        public static void RemoveInvalidEntries(Dictionary<Level, Dictionary<GammaRecord, List<Value>>> Values, List<string> FoundErrors)
        {
            foreach (var l in Values)
            {
                foreach (var g in l.Value)
                {
                    for (int i = 0; i < g.Value.Count; i++)
                    {
                        var v = g.Value[i];
                        if (v.DVal == 0)
                        {
                            FoundErrors.Add($"Value with missing or zero uncertainty for Level:{l.Key.LevelRecord.E} and Gamma:{g.Key.E}: {v.Val}±{v.DVal};");
                            g.Value.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }     
    }
}

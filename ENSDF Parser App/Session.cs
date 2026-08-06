using ENSDF_Parser;
using ENSDF_Parser.Extractor;
using ENSDF_Parser.MetaData;
using ENSDF_Parser.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ENSDF_Parser_App
{
    internal class Session
    {
        public Session()
        {
            Dataset = new();
            GammaCommentsValues = new();
            ResetSelection();
            SelectionStack = new Stack<Selection>();
        }

        public List<DataSet> Dataset;
        public Dictionary<DataSet, (List<string> FoundErrors, Dictionary<Level, Dictionary<GammaRecord, List<Value>>> Values)> GammaCommentsValues;

        Selection RootSelection {  get; set; }
        Selection CurrentSelection { get { return RootSelection.GetLast(); } }
        Stack<Selection> SelectionStack { get; }
        public void ResetSelection()
        {
            RootSelection = new Selection("root", Dataset);
        }
        public void MakeTemporarySelection()
        {
            SelectionStack.Push(RootSelection);
            ResetSelection();
        }
        public void CloneCurrentSelection()
        {
            SelectionStack.Push(RootSelection);
            RootSelection = RootSelection.Clone();
        }
        public void RestoreSelection()
        {
            if (SelectionStack.Count == 0) return;
            else RootSelection = SelectionStack.Pop();
        }
        public string GetCurrentSelection()
        {
            return RootSelection.ToString();
        }

        void ApplyPath(List<string> path)
        {
            foreach (string s in path)
            {
                CurrentSelection.SelectSubitem(s);
            }
        }

        public string Dir()
        {
            return string.Join("\n", CurrentSelection.GetProperties());
        }

        public object GetValue(string prop)
        {
            List<string> path = Tools.ParsePath(prop);
            if (path.Count == 0) throw new Exception($"You have to specify a property to read!");
            object? value = null;
            if (path[0].Contains("dataset"))
            {
                MakeTemporarySelection();
                ApplyPath(path.Take(path.Count - 1).ToList());
                value = CurrentSelection.GetValue(path[^1]);
                RestoreSelection();
            }
            else
            {
                CloneCurrentSelection();
                ApplyPath(path.Take(path.Count - 1).ToList());
                value = CurrentSelection.GetValue(path[^1]);
                RestoreSelection();
            }
            if (value == null) throw new Exception("The specified property has \"null\" value!");
            return value;
        }

        public void SetValue(string prop, object value)
        {
            List<string> path = Tools.ParsePath(prop);
            if (path.Count == 0) throw new Exception($"You have to specify a property to read!");
            if (path[0].Contains("dataset"))
            {
                MakeTemporarySelection();
                ApplyPath(path.Take(path.Count - 1).ToList());
                CurrentSelection.SetValue(path[^1], value);
                RestoreSelection();
            }
            else
            {
                CloneCurrentSelection();
                ApplyPath(path.Take(path.Count - 1).ToList());
                CurrentSelection.SetValue(path[^1], value);
                RestoreSelection();
            }
        }

        public void Select(string prop)
        {
            if (prop == "..")
            {
                if (!RootSelection.RemoveLast()) ResetSelection();
                return;
            }
            List<string> path = Tools.ParsePath(prop);
            if (path.Count == 0) throw new Exception($"You have to specify an object to select!");
            if (path[0].Contains("dataset"))
            {
                ResetSelection();
                ApplyPath(path);
            }
            else
            {
                ApplyPath(path);
            }
        }

        public void AddDataset(List<DataSet> dataset)
        {
            Dataset.AddRange(dataset);
        }

        public void RemoveDataset(List<int> indices)
        {
            indices.Sort();
            indices.Reverse();
            foreach(var i in indices)
            {
                if (i < 0 || i >= Dataset.Count) throw new Exception($"The selected index \"{i}\" is out of range!");
                Dataset.RemoveAt(i);
            }
        }

        public void ParseGammaComments(List<int> dataset, string identifier, List<string> breakers, bool removeInvalids)
        {
            foreach (var i in dataset)
            {
                if (i < 0 || i >= Dataset.Count) throw new Exception($"The selected index \"{i}\" is out of range!");

                var extractor = ValueExtractor.ParseFromComments(Dataset[i], identifier, breakers);
                if (removeInvalids) ValueExtractor.RemoveInvalidEntries(extractor.Values, extractor.FoundErrors);
                GammaCommentsValues.Add(Dataset[i], extractor);
            }
        }

        public void LoadFileENS(string filename)
        {
            List<string> lines = File.ReadAllLines(filename).ToList();
            List<DataSet> sets = Parser.Parse(lines, Path.GetFileNameWithoutExtension(filename));
            AddDataset(sets);
        }

        public void SaveDatasets(List<int> indices, string filepath)
        {
            string content = "";
            foreach (var i in indices)
            {
                if (i < 0 || i >= Dataset.Count) throw new Exception($"The selected index \"{i}\" is out of range!");

                content += Dataset[i].ToString();
            }
            File.WriteAllText(filepath, content);
        }

        public void SaveParsedValues(string directoryName, string template)
        {
            string valuePath = Path.Combine(directoryName, "Values");
            string logPath = Path.Combine(directoryName, "Logs");

            Directory.CreateDirectory(valuePath);
            Directory.CreateDirectory(logPath);

            foreach(var v in GammaCommentsValues)
            {
                foreach(var l in v.Value.Values)
                {
                    foreach(var g in l.Value)
                    {
                        string path = Path.Combine(valuePath, $"Dataset-{v.Key.File}", $"Level-{l.Key.LevelRecord.E}");
                        Directory.CreateDirectory(path);
                        File.WriteAllText(Path.Combine(path, $"Gamma-{g.Key.E}.txt"), Tools.ValuesToString(g.Value, template));
                    }
                }
                File.WriteAllText(Path.Combine(logPath, $"Dataset-{v.Key.File}.txt"), string.Join('\n', v.Value.FoundErrors));
            }
        }
    }
}

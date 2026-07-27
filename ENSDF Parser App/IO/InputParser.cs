using ENSDF_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace UserInterface.IO
{
    /*
     Return codes specify the command
    -1) parse file <file_path> /loads the file parsed
    -2) parse gcv * r|k <field_identifier> <list: breakers> /parse the values from gamma comments
    -2) parse gcv datasets[<dataset numbers>] /r\/k <field_identifier> <list: breakers> /parse the values from gamma comments
    3) show datasets /shows the parsed datasets in the current buffer
    4) show datasets[<list: number>] /gives info for the specified dataset
    5) show <field> /shows the field of the selected item
    5*) show <object>.<field>
    6) show gcv /shows the parsed values from gamma comments as collections
    7) show gcv[<number>] *<format>(use "val" for the value and "dval" for the uncertainty
    8) select dataset[<number>] /selects the current dataset to work with
    9) select <object> /selects the current working item
    10) modify <field> <new_value> /modifies the field of the selected item *all values are strings*
    10*) modify <object>.<field> <new_value>
    11) save datasets <file_path>
    12) save datasets[<list: number>] <file_path>
    13) save gcv <directory> *<format>
    13) save gcv[<list: number>] <directory> *<format>

    Error codes:
    1 - Wrong command/syntax
    2 - Arguments are required, but arent set correctly
    3 - Arguments are set correctly, but are out of range
    */
    internal class InputParser
    {
        public static byte Execute(string command, Session session)
        {
            Queue<string> parsedCommand = new Queue<string>(command.Split(" ", StringSplitOptions.RemoveEmptyEntries));
            string type = parsedCommand.Dequeue();
            if (!Command.ContainsKey(type)) return 1;
            return Command[type](parsedCommand, session);
        }

        static Dictionary<string, Func<Queue<string>, Session, byte>> Command = new()
        {
            { "parse", (Queue<string> args, Session session) => Parse(args, session) },
            { "show", (Queue<string> args, Session session) => Show(args, session) },
            { "select", (Queue<string> args, Session session) => Select(args, session) },
            { "modify", (Queue<string> args, Session session) => Modify(args, session) },
            { "save", (Queue<string> args, Session session) => Save(args, session) },
            { "list", (Queue<string> args, Session session) => List(args, session) },
        };

        private static byte List(Queue<string> args, Session session)
        {
            throw new NotImplementedException();
        }

        static Dictionary<string, Func<Queue<string>, Session, byte>> ParseCommand = new()
        {
            { "file", (Queue<string> args, Session session) => ParseFile(args, session) },
            { "gcv", (Queue<string> args, Session session) => ParseGammaComment(args, session) }
        };
        static byte Parse(Queue<string> args, Session session)
        {
            string type = args.Dequeue();
            if (!ParseCommand.ContainsKey(type)) return 1;
            return ParseCommand[type](args, session);
        }
        static byte ParseFile(Queue<string> args, Session session)
        {
            string file_path = args.Dequeue().Replace("\"", "");
            List<string> lines = File.ReadAllLines(file_path).ToList();
            List<DataSet> sets = Parser.Parse(lines);
            session.Datasets.AddRange(sets);
            return 0;
        }
        static byte ParseGammaComment(Queue<string> args, Session session)
        {
            string datasetsIDs = args.Dequeue();
            bool removeInvalids = args.Dequeue() == "r";
            string identifier = args.Dequeue();
            var br = ParseIndexes(args.Dequeue());
            List<string> breakers = (br.Success) ? br.Indexes : new List<string>();
            List<DataSet> sets;
            if (datasetsIDs == "*") sets = session.Datasets;
            else
            {
                sets = new List<DataSet>();
                var parsed_datasetsIDs = ParseIndexes(datasetsIDs);
                if (parsed_datasetsIDs.KeyWord != "datasets") return 1;
                if (parsed_datasetsIDs.Success == false || parsed_datasetsIDs.Indexes.Count == 0) return 2;
                List<int> indexes = parsed_datasetsIDs.Indexes.Select(int.Parse).ToList();
                foreach (var i in indexes)
                {
                    if (i < 0 || i >= session.Datasets.Count) return 3;
                    sets.Add(session.Datasets[i]);
                }
            }
            for (int i = 0; i < sets.Count; i++)
            {
                var extractor = ValueExtractor.ParseFromComments(sets[i], identifier, breakers);
                if (removeInvalids) ValueExtractor.RemoveInvalidEntries(extractor.Values, extractor.FoundErrors);
                session.GammaCommentsValues.Add(sets[i], extractor);
            }
            return 0;
        }

        static Dictionary<string, Func<Queue<string>, Session, byte>> ShowCommand = new()
        {
            { "datasets", (Queue<string> args, Session session) => ShowDatasets(args, session) },
            { "gcv", (Queue<string> args, Session session) => ShowGCV(args, session) }
        };

        private static byte ShowGCV(Queue<string> args, Session session)
        {
            throw new NotImplementedException();
        }

        private static byte ShowDatasets(Queue<string> args, Session session)
        {
            
            throw new NotImplementedException();
        }

        static byte Show(Queue<string> args, Session session)
        {
            string obj = args.Peek();
        
            if (obj.Contains("datasets")) return ShowCommand["datasets"](args, session);
            if (obj.Contains("gcv")) return ShowCommand["gcv"](args, session);

            string[] chain = obj.Split('.');
            if(chain.Length == 0) if (session.CurrentRecord == null) if (session.CurrentDataset != null) Console.WriteLine(JsonSerializer.Serialize(session.CurrentDataset, new JsonSerializerOptions() { IncludeFields = true, WriteIndented = true }));
            else Console.WriteLine("No DataSet is selected!");
            else Console.WriteLine(JsonSerializer.Serialize(session.CurrentRecord, new JsonSerializerOptions() { IncludeFields = true, WriteIndented = true }));
            

            return 0;
        }


        static byte Select(Queue<string> args, Session session)
        {

            return 0;
        }

        static byte Modify(Queue<string> args, Session session)
        {

            return 0;
        }

        static Dictionary<string, Func<Queue<string>, Session, byte>> SaveCommand = new()
        {
            { "datasets", (Queue<string> args, Session session) => SaveDatasets(args, session) },
            { "gcv", (Queue<string> args, Session session) => SaveGCV(args, session) }
        };

        private static byte SaveGCV(Queue<string> args, Session session)
        {
            throw new NotImplementedException();
        }

        private static byte SaveDatasets(Queue<string> args, Session session)
        {
            throw new NotImplementedException();
        }

        static byte Save(Queue<string> args, Session session)
        {

            return 0;
        }

        static (string? KeyWord, List<string>? Indexes, bool Success) ParseIndexes(string input)
        {
            if (input == "" || !input.Contains('[') || !input.Contains(']')) return (input, null, false);
            int i = 0;
            while (i < input.Length && input[i] != '[') i++;
            string keyword = input.Substring(0, i);
            int j = i + 1;
            while (j < input.Length && input[j] != ']') j++;
            string indexes_str = input.Substring(i + 1, j - i - 1);
            List<string> indexes = indexes_str.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            return (keyword, indexes, true);
        }
    }
}

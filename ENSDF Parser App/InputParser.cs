using ENSDF_Parser;
using ENSDF_Parser.Extractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ENSDF_Parser_App
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
            Queue<string> parsedCommand = Tools.TokenizeInput(command);
            string type = parsedCommand.Dequeue();
            if (!Command.ContainsKey(type)) throw new Exception($"The command \"{type}\" doesn't exist!");
            return Command[type](parsedCommand, session);
        }

        static Dictionary<string, Func<Queue<string>, Session, byte>> Command = new()
        {
            { "parse", (Queue<string> args, Session session) => Parse(args, session) },
            { "get", (Queue<string> args, Session session) => Get(args, session) },
            { "select", (Queue<string> args, Session session) => Select(args, session) },
            { "set", (Queue<string> args, Session session) => Set(args, session) },
            { "save", (Queue<string> args, Session session) => Save(args, session) },
            { "dir", (Queue<string> args, Session session) => Dir(args, session) },
        };

        private static byte Dir(Queue<string> args, Session session)
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
            if (!ParseCommand.ContainsKey(type)) throw new Exception($"The argument \"{type}\" is incorrect! Possible arguments are \"file\" and \"gcv\"!");
            return ParseCommand[type](args, session);
        }
        static byte ParseFile(Queue<string> args, Session session)
        {
            string file_path = args.Dequeue().Replace("\"", "");
            session.LoadFileENS(file_path);
            return 0;
        }
        static byte ParseGammaComment(Queue<string> args, Session session)
        {
            if (args.Count < 3) throw new Exception($"Invalid command!");
            List<string>? datasetsIDs_str = Tools.ParseSquareBrackets(args.Dequeue()).Args;
            if (datasetsIDs_str == null) throw new Exception($"The id numbers of the datasets to parse aren't set correctly!");
            List<int> datasetsIDs = datasetsIDs_str.Select(int.Parse).ToList();
            string identifier = args.Dequeue();
            var breakers = Tools.ParseSquareBrackets(args.Dequeue()).Args;
            bool removeInvalids = false;
            if (args.Count > 0 && args.Dequeue() == "r") removeInvalids = true; 
            session.ParseGammaComments(datasetsIDs, identifier, breakers, removeInvalids);
            return 0;
        }

        static byte Get(Queue<string> args, Session session)
        {
            string path = args.Dequeue();
            Console.WriteLine(session.GetValue(path));

            return 0;
        }


        static byte Select(Queue<string> args, Session session)
        {
            string path = args.Dequeue();
            session.Select(path);

            return 0;
        }

        static byte Set(Queue<string> args, Session session)
        {
            string path = args.Dequeue();
            string value = args.Dequeue();

            session.SetValue(path, value);

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

        
    }
}

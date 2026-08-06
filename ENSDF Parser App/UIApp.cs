using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSDF_Parser_App
{
    internal class UIApp
    {
        public UIApp()
        {
            MainSession = new Session();
        }

        Session MainSession { get; }
        
        public void RunConsoleSession()
        {
            while (true)
            {
                Console.Write($"<{MainSession.GetCurrentSelection()}>>");
                try
                {
                    string command = Console.ReadLine();
                    if (command == "") continue;
                    if (command == "exit") break;
                    InputParser.Execute(command, MainSession);
                }
                catch (Exception ex)
                {
                    MainSession.RestoreSelection();
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

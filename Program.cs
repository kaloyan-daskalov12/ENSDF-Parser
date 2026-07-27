namespace ENSDF_Parser {
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            List<string> lines = new List<string>
            {
            "106PD  G 511.8605  31 1000      E2                                           C  ",
            "106PD cB E         3050 {I20} (1960Se05), 3050 (1962Am03), 3040 (1966JoZZ)      ",
            "106PD2 L G=0.290 47                                                             "
            };

            List<Record> records = Parser.Parse(lines);
            foreach(var r in records)
            {
                Console.WriteLine(r.GetType());
            }
            GammaRecord g = (GammaRecord)records[0];
            Console.WriteLine(g.E);
            Console.WriteLine(g.DE);
            Console.WriteLine(g.Id.Isotrope);
            Console.WriteLine(g.Id.Name);

            CommentRecord r1 = (CommentRecord)records[1];
            Console.WriteLine(r1.CTEXT);
        }
    }
}
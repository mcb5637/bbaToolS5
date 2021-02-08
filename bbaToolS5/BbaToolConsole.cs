using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class BbaToolConsole
    {
        internal static void Main(string[] args)
        {
            foreach (string f in args)
            {
                if (File.Exists(f))
                {
                    Console.WriteLine($"unpacking file {f}");
                    SimpleUnpack(f);
                }
                else if (Directory.Exists(f))
                {
                    Console.WriteLine($"packing file {f}");
                    SimplePack(f);
                }
                else
                {
                    Console.WriteLine($"does not exist {f}");
                }
                Console.WriteLine("done");
            }
            Console.ReadKey();
        }

        private static void SimpleUnpack(string inp)
        {
            try
            {
                string outp = Path.ChangeExtension(inp, null);
                BbaArchive a = new BbaArchive();
                a.ReadBba(inp, null, ProgressReport);
                a.WriteToFolder(outp, ProgressReport);
                a.Clear();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SimplePack(string inp)
        {
            try
            {
                string outp = Path.ChangeExtension(inp, ".bba");
                BbaArchive a = new BbaArchive();
                a.ReadFromFolder(inp, ProgressReport);
                if (a.GetFileByName("maps\\externalmap\\info.xml") != null)
                    outp = Path.ChangeExtension(inp, ".s5x");
                a.WriteToBba(outp, ProgressReport);
                a.Clear();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ProgressReport(ProgressStatus x)
        {
            Console.WriteLine(x.ToString());
        }
    }
}

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
            bool errexit = false;
            List<string> files = new List<string>();

            foreach (string f in args)
            {
                if ("-err".Equals(f))
                    errexit = true;
                else
                    files.Add(f);
            }

            try
            {
                string output;
                bool toArchive;
                bool checkForS5x = false;
                if (files.Count == 0)
                {
                    Console.WriteLine("usage: drop a file/folder onto the executable to unpack/pack it.\n\nadvanced usage: commandline option -err disables waiting for input, exits with 1 on errors. all other parameters are processed as files/folders, the last one is the output, the others are loaded in the given order. if only one file is present, ouput is automatically determined.");
                    Console.ReadKey();
                    return;
                }
                else if (files.Count > 1)
                {
                    output = files.Last();
                    files.RemoveAt(files.Count - 1);
                    toArchive = ".s5x".Equals(Path.GetExtension(output)) || ".bba".Equals(Path.GetExtension(output));
                }
                else
                {
                    if (File.Exists(files[0]))
                    {
                        output = Path.ChangeExtension(files[0], null);
                        toArchive = false;
                    }
                    else if (Directory.Exists(files[0]))
                    {
                        output = Path.ChangeExtension(files[0], ".bba");
                        toArchive = true;
                        checkForS5x = true;
                    }
                    else
                    {
                        throw new IOException($"{files[0]} does not exist");
                    }
                }
                BbaArchive a = new BbaArchive();
                foreach (string f in files)
                {
                    if (File.Exists(f))
                    {
                        Console.WriteLine($"loading archive {f}");
                        a.ReadBba(f, null, ProgressReport);
                    }
                    else if (Directory.Exists(f))
                    {
                        Console.WriteLine($"loading folder {f}");
                        a.ReadFromFolder(f, ProgressReport);
                    }
                    else
                    {
                        Console.WriteLine($"does not exist {f}");
                    }
                }
                if (toArchive)
                {
                    if (checkForS5x && a.GetFileByName("maps\\externalmap\\info.xml") != null)
                        output = Path.ChangeExtension(output, ".s5x");
                    Console.WriteLine($"writing to archive {output}");
                    a.WriteToBba(output, ProgressReport);
                }
                else
                {
                    Console.WriteLine($"writing to folder {output}");
                    a.WriteToFolder(output, ProgressReport);
                }
                a.Clear();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                if (errexit)
                    Environment.Exit(1);
            }

            Console.WriteLine("done. press any key to close this window.");
            if (!errexit)
                Console.ReadKey();
        }

        private static void ProgressReport(ProgressStatus x)
        {
            Console.WriteLine(x.ToString());
        }
    }
}

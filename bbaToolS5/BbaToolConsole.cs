﻿using bbaLib;
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
            bool ignorehidden = false;
            bool autoCompression = false;
            bool searchDups = false;
            List<string> files = [];
            bool? over = null;

            foreach (string f in args)
            {
                if ("-err".Equals(f))
                    errexit = true;
                else if ("-ignorehidden".Equals(f))
                    ignorehidden = true;
                else if ("-autoCompression".Equals(f))
                    autoCompression = true;
                else if ("-searchDuplicates".Equals(f))
                    searchDups = true;
                else if ("-override".Equals(f))
                    over = true;
                else if ("-add".Equals(f))
                    over = false;
                else
                    files.Add(f.TrimEnd('\\', '/'));
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
                    output = Path.GetFullPath(files.Last());
                    files.RemoveAt(files.Count - 1);
                    toArchive = ".s5x".Equals(Path.GetExtension(output)) || ".bba".Equals(Path.GetExtension(output));
                }
                else
                {
                    string p = Path.GetFullPath(files[0]);
                    if (File.Exists(p))
                    {
                        output = Path.ChangeExtension(p, null);
                        toArchive = false;
                    }
                    else if (Directory.Exists(p))
                    {
                        output = Path.ChangeExtension(p, ".bba");
                        toArchive = true;
                        checkForS5x = true;
                    }
                    else
                    {
                        throw new IOException($"{p} does not exist");
                    }
                }
                using BbaArchive a = new();
                foreach (string fi in files)
                {
                    string f = Path.GetFullPath(fi);
                    if (File.Exists(f))
                    {
                        Console.WriteLine($"loading archive {f}");
                        a.ReadBba(f, null, ProgressReport);
                    }
                    else if (Directory.Exists(f))
                    {
                        Console.WriteLine($"loading folder {f}");
                        a.ReadFromFolder(f, ProgressReport, ignorehidden);
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
                    if (searchDups)
                        a.SearchAndLinkDuplicates();
                    Console.WriteLine($"writing to archive {output}");
                    a.WriteToBba(output, ProgressReport, autoCompression, OverrideConfirm);
                }
                else
                {
                    Console.WriteLine($"writing to folder {output}");
                    a.WriteToFolder(output, ProgressReport, OverrideConfirm);
                }
            }
            catch (Exception e)
            {
                Console.Write($"Error processing:");
                foreach (string s in args)
                    Console.Write("\"" + s + "\" ");
                Console.WriteLine();
                Console.WriteLine(e);
                if (errexit)
                    Environment.Exit(1);
            }

            if (errexit)
                Environment.Exit(0);
            Console.WriteLine("done. press any key to close this window.");
            Console.ReadKey();


            bool? OverrideConfirm(string p)
            {
                if (over != null)
                    return over;
                Console.WriteLine($"override {p}? (Y)es, (N)o (add to existing, replacing if already exists), Cancel (anything else)");
                string? l = Console.ReadLine();
                if (l == null)
                    return null;
                else if (l.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else if (l.Equals("n", StringComparison.CurrentCultureIgnoreCase))
                    return false;
                else
                    return null;
            }

        }

        private static void ProgressReport(ProgressStatus x)
        {
            Console.WriteLine(x.ToString());
        }
    }
}

using bbaLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace bbaToolS5
{
    internal static class BbaToolConsole
    {
        internal abstract class Operation
        {
            internal abstract void Run(BbaArchive a, ref RunParams p);
        }

        internal class ReadFrom : Operation
        {
            internal required string ToRead;
            
            internal override void Run(BbaArchive a, ref RunParams p)
            {
                string f = Path.GetFullPath(ToRead);
                if (File.Exists(f))
                {
                    Console.WriteLine($"loading archive {f}");
                    a.ReadBba(f, null, ProgressReport);
                }
                else if (Directory.Exists(f))
                {
                    Console.WriteLine($"loading folder {f}");
                    a.ReadFromFolder(f, ProgressReport, p.IgnoreHidden);
                }
                else
                {
                    Console.WriteLine($"does not exist {f}");
                }
            }
        }

        internal class WriteTo : Operation
        {
            internal required string OutFile;
            internal required bool CheckForS5S, ToArchive;
            
            internal override void Run(BbaArchive a, ref RunParams p)
            {
                if (ToArchive)
                {
                    if (CheckForS5S && a.GetFileByName(BbaArchive.InfoXML) != null)
                        OutFile = Path.ChangeExtension(OutFile, ".s5x");
                    if (p.SearchDuplicates)
                        a.SearchAndLinkDuplicates();
                    Console.WriteLine($"writing to archive {OutFile}");
                    a.WriteToBba(OutFile, ProgressReport, p.AutoCompression, p.Override);
                }
                else
                {
                    Console.WriteLine($"writing to folder {OutFile}");
                    a.WriteToFolder(OutFile, ProgressReport, p.Override);
                }
            }
        }

        internal class SetGUID : Operation
        {
            internal override void Run(BbaArchive a, ref RunParams p)
            {
                var i = a.MapInfo;
                if (i == null)
                {
                    Console.WriteLine("warning: could not find mapinfo.xml to change GUID");
                    return;
                }
                i.GUID = Guid.NewGuid().ToString();
                Console.WriteLine($"GUID is now {i.GUID}");
                a.MapInfo = i;
            }
        }
        
        internal class SetVersion : Operation
        {
            internal required string ModName;
            internal required string Version;
            internal override void Run(BbaArchive a, ref RunParams p)
            {
                var i = a.GetModPackInfo(ModName);
                if (i == null)
                {
                    Console.WriteLine("warning: could not find modpack.xml to change Version");
                    return;
                }

                i.Version = Version;
                Console.WriteLine($"Version is now {Version}");
                a.SetModPackInfo(ModName, i);
            }
        }

        internal class MakeMapVersionInfo : Operation
        {
            internal required string OutFile;
            
            internal override void Run(BbaArchive a, ref RunParams p)
            {
                var i = a.MapInfo;
                if (i == null)
                {
                    Console.WriteLine("warning: could not find mapinfo.xml to read GUID");
                    return;
                }
                Console.WriteLine($"write GUID {i.GUID} to {OutFile}");
                File.WriteAllText(OutFile, i.GUID);
            }
        }

        internal class MakeModPackVersionInfo : Operation
        {
            internal required string OutFile, ModName;
            
            internal override void Run(BbaArchive a, ref RunParams p)
            {
                var i = a.GetModPackInfo(ModName);
                if (i == null)
                {
                    Console.WriteLine("warning: could not find modpack.xml to read Version");
                    return;
                }
                Console.WriteLine($"write version {i.Version} to {OutFile}");
                File.WriteAllText(OutFile, i.Version);
            }
        }

        internal struct RunParams
        {
            internal bool IgnoreHidden;
            internal bool AutoCompression;
            internal bool SearchDuplicates;
            internal Func<string, bool?> Override;
        }
        
        internal static void Main(string[] args)
        {
            bool? over = null;
            RunParams par = new RunParams
            {
                IgnoreHidden = false,
                AutoCompression = false,
                SearchDuplicates = false,
            };
            bool errexit = false;
            List<Operation> ops = [];

            foreach (string f in args)
            {
                if ("-err".Equals(f))
                    errexit = true;
                else if ("-ignorehidden".Equals(f))
                    par.IgnoreHidden = true;
                else if ("-autoCompression".Equals(f))
                    par.AutoCompression = true;
                else if ("-searchDuplicates".Equals(f))
                    par.SearchDuplicates = true;
                else if ("-override".Equals(f))
                    over = true;
                else if ("-add".Equals(f))
                    over = false;
                else if (f.StartsWith("-write:"))
                {
                    string p = f.Remove(0, 7).TrimEnd('\\', '/');
                    ops.Add(new WriteTo{ CheckForS5S = false, OutFile = p, ToArchive = PathIsArchive(p) });
                }
                else if (f == "-randomGUID")
                {
                    ops.Add(new SetGUID());
                }
                else if (f.StartsWith("-version:"))
                {
                    string[] p = f.Remove(0, 9).Split(':');
                    ops.Add(new SetVersion()
                    {
                        ModName = p[0],
                        Version = p[1],
                    });
                }
                else if (f.StartsWith("-mapVersionInfo:"))
                {
                    string p = f.Remove(0, 16);
                    ops.Add(new MakeMapVersionInfo{OutFile = p});
                }
                else if (f.StartsWith("-modVersionInfo:"))
                {
                    string[] p = f.Remove(0, 16).Split(':');
                    ops.Add(new MakeModPackVersionInfo{OutFile = p[1], ModName = p[0]});
                }
                else
                    ops.Add(new ReadFrom{ ToRead = f.TrimEnd('\\', '/') });
            }

            try
            {
                string output;
                bool toArchive;
                // ReSharper disable once InconsistentNaming
                bool checkForS5x = false;
                if (ops.Count == 0)
                {
                    Console.WriteLine("usage: drop a file/folder onto the executable to unpack/pack it.\n\nadvanced usage: commandline option -err disables waiting for input, exits with 1 on errors. all other parameters are processed as files/folders, the last one is the output, the others are loaded in the given order. if only one file is present, ouput is automatically determined.");
                    Console.ReadKey();
                    return;
                }
                else if (ops.Count > 1)
                {
                    if (ops.Last() is ReadFrom r)
                    {
                        output = Path.GetFullPath(r.ToRead);
                        ops.RemoveAt(ops.Count - 1);
                        toArchive = PathIsArchive(output);
                        ops.Add(new WriteTo
                        {
                            OutFile = output,
                            CheckForS5S = false,
                            ToArchive = toArchive
                        });
                    }
                    else if (ops.Last() is not WriteTo)
                    {
                        Console.WriteLine("warning: last command is not a write");
                    }
                }
                else
                {
                    string p = Path.GetFullPath(((ReadFrom) ops[0]).ToRead);
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
                    ops.Add(new WriteTo
                    {
                        OutFile = output,
                        CheckForS5S = checkForS5x,
                        ToArchive = toArchive
                    });
                }

                par.Override = OverrideConfirm;
                using BbaArchive a = new();
                foreach (var v in ops)
                {
                    v.Run(a, ref par);
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
                return;
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

            static bool PathIsArchive(string p)
            {
                p = Path.GetExtension(p);
                return ".s5x".Equals(p) || ".bba".Equals(p);
            }
        }

        private static void ProgressReport(ProgressStatus x)
        {
            Console.WriteLine(x.ToString());
        }
    }
}

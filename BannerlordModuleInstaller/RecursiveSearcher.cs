using System;
using System.IO;
using System.Collections.Generic;

namespace BannerlordModuleInstaller
{
    public class RecursiveSearcher
    {
        public List<SourceFile> Files = new List<SourceFile>();

        public RecursiveSearcher(string sDir)
        {
            GetFilesRecursive(sDir);

#if DEBUG
            //using (StreamWriter sw = new StreamWriter("recursiveSearcherDump.txt"))
            //{

            //    sw.WriteLine(Files.Count + " files found\n");
            //    for(int i = 0; i < Files.Count; i++)
            //    {
            //        sw.WriteLine(Files[i].Name);
            //    }
            //}
#endif
        }

        private void GetFilesRecursive(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    GetFilesRecursive(d);
                }
                foreach (string file in Directory.GetFiles(sDir))
                {
                    //This is where you would manipulate each file found, e.g.:
                    DoAction(file);
                }
            }
            catch (System.Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private void DoAction(string filepath)
        {
            Files.Add(new SourceFile(filepath));
        }
    }
}
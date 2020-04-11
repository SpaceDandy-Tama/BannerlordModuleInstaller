using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace BannerlordModuleInstaller
{
    static class Program
    {
        public static string MagicGuid = "f81a318a-d971-45ba-a2c5-55a02aa56f2e";
        public static byte[] MagicGuidBytes => Encoding.UTF8.GetBytes(MagicGuid);

        public static bool ByteArrayCompare(ref byte[] a1, ref byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ConsoleHelper.Initialize();
            }

            if (args != null && args.Length > 2)
            {
                if (args[0] == "-packModule")
                {
                    if (File.Exists(args[1]))
                    {
                        byte[] moduleInfoBytes = PackModuleInfo(args);

                        if (Directory.Exists(args[2]))
                        {
                            byte[] moduleDataBytes = PackModule(args[2]);

                            MemoryStream inStream = new MemoryStream(moduleDataBytes);
                            MemoryStream outStream = new MemoryStream();
                            SevenZip.Compression.LZMA.SevenZipHelper.Compress(inStream, outStream);
                            moduleDataBytes = outStream.ToArray();

                            inStream.Dispose();

                            string installerName = args[2] + "Installer.exe";
                            File.Copy(Application.ExecutablePath, Path.Combine(Application.StartupPath, installerName), true);
                            using (FileStream fs = new FileStream(installerName, FileMode.Append))
                            {
                                using (BinaryWriter bw = new BinaryWriter(fs))
                                {
                                    bw.Write(moduleInfoBytes);
                                    bw.Write(moduleDataBytes);
                                    //tailer = header but at the end -_-
                                    bw.Write(moduleDataBytes.Length);
                                    bw.Write(moduleInfoBytes.Length);
                                    bw.Write(MagicGuidBytes);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Directory");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid ModuleInfo.txt file");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Argument");
                }

                Console.ReadKey();
                Environment.Exit(0);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static private byte[] PackModule(string arg)
        {
            RecursiveSearcher rs = new RecursiveSearcher(arg);

            MemoryStream tempMemoryStream = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(tempMemoryStream))
            {
                //header
                int emptySpaces = sizeof(int) * 4; //Empty Space for Future and Backwards Compatibility
                int endOfHeader = emptySpaces + ((1 + rs.Files.Count) * sizeof(int));
                bw.BaseStream.Position = emptySpaces;
                bw.Write(rs.Files.Count);
                bw.BaseStream.Position = endOfHeader;
                //files
                for (int i = 0; i < rs.Files.Count; i++)
                {
                    int headerPos = emptySpaces + (sizeof(int) * (i + 1));
                    rs.Files[i].PackToStream(bw, headerPos);
                }
            }

            return tempMemoryStream.ToArray();
        }

        static public bool UnpackModule()
        {
            byte[] compressedModuleDataFiles = null;
            using (FileStream fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] magicGuidBytes = Program.MagicGuidBytes;
                    br.BaseStream.Position = br.BaseStream.Length - ((sizeof(int) * 2) + magicGuidBytes.Length);
                    int moduleDataBytesLength = br.ReadInt32();
                    int offsetPosition = (int)br.BaseStream.Length - ((sizeof(int) * 2) + magicGuidBytes.Length + moduleDataBytesLength);

                    br.BaseStream.Position = offsetPosition;
                    compressedModuleDataFiles = br.ReadBytes(moduleDataBytesLength);
                }
            }

            if (compressedModuleDataFiles == null)
                return false;

            MemoryStream inStream = new MemoryStream(compressedModuleDataFiles);
            FileStream outStream = new FileStream("tempData", FileMode.Create);
            SevenZip.Compression.LZMA.SevenZipHelper.Decompress(inStream, outStream);
            inStream.Dispose();
            compressedModuleDataFiles = null;

            using (BinaryReader br = new BinaryReader(outStream))
            {
                int emptySpaces = sizeof(int) * 4; //Empty Space for Future and Backwards Compatibility
                br.BaseStream.Position = emptySpaces;
                int fileCount = br.ReadInt32();
                int endOfHeader = emptySpaces + ((1 + fileCount) * sizeof(int));

                for (int i = 0; i < fileCount; i++)
                {
                    int headerPos = emptySpaces + (sizeof(int) * (i + 1));
                    SourceFile.FromPackedStream(br, headerPos, endOfHeader);
                }
            }

            outStream.Dispose();
            File.Delete("tempData");
            return true;
        }

        static private byte[] PackModuleInfo(string[] args)
        {
            ModuleInfo moduleInfo = ModuleInfo.FromModuleInfoFile(args[1], args[2]);

            MemoryStream moduleInfoMemoryStream = new MemoryStream();
            moduleInfo.WriteToStream(moduleInfoMemoryStream);

#if DEBUG
            //FileStream testStream = new FileStream("moduleInfoDump", FileMode.Create);
            //moduleInfo.WriteToStream(testStream);
            //testStream.Dispose();
#endif

            return moduleInfoMemoryStream.ToArray();
        }

    }
}
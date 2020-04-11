using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace BannerlordModuleInstaller
{
    public class ModuleInfo
    {
        static public readonly string NameBeginsWith = "name=";
        static public readonly string VersionBeginsWith = "version=";
        static public readonly string AuthorBeginsWith = "author=";
        static public readonly string AuthorLinkBeginsWith = "authorLink=";
        static public readonly string AuthorIsWebLinkBeginsWith = "authorIsWebLink=";
        static public readonly string ImagePathBeginsWith = "imagePath=";
        static public readonly string IconPathBeginsWith = "iconPath=";

        public byte[] NameBytes => Encoding.UTF8.GetBytes(Name);
        public byte[] FolderNameBytes => Encoding.UTF8.GetBytes(FolderName);
        public byte[] VersionBytes => Encoding.UTF8.GetBytes(Version);
        public byte[] AuthorBytes => Encoding.UTF8.GetBytes(Author);
        public byte[] AuthorLinkBytes => Encoding.UTF8.GetBytes(AuthorLink);
        public byte AuthorIsWebLinkByte => (byte)(AuthorIsWebLink ? 1 : 0);
        public byte[] ImageBytes => File.Exists(ImagePath) ? File.ReadAllBytes(ImagePath) : null;
        public byte[] IconBytes => File.Exists(IconPath) ? File.ReadAllBytes(IconPath) : null;

        public string Name;
        public string FolderName;
        public string Version;
        public string Author;
        public string AuthorLink;
        public bool AuthorIsWebLink;
        public string ImagePath;
        public string IconPath;

        public MemoryStream ImageStream; //This needs to stay open so long as Image Object Exists
        public Image Image;
        public Icon Icon;

        static public ModuleInfo FromModuleInfoFile(string filePath, string folderName = "")
        {
            ModuleInfo moduleInfo = new ModuleInfo();

            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.StartsWith(ModuleInfo.NameBeginsWith))
                {
                    moduleInfo.Name = line.Remove(0, ModuleInfo.NameBeginsWith.Length);
                    Console.WriteLine(moduleInfo.Name);

                    moduleInfo.FolderName = folderName == "" ? moduleInfo.Name.RemoveWhitespace() : folderName;
                    Console.WriteLine(moduleInfo.FolderName);
                }
                else if (line.StartsWith(ModuleInfo.VersionBeginsWith))
                {
                    moduleInfo.Version = line.Remove(0, ModuleInfo.VersionBeginsWith.Length);
                    Console.WriteLine(moduleInfo.Version);
                }
                else if (line.StartsWith(ModuleInfo.AuthorBeginsWith))
                {
                    moduleInfo.Author = line.Remove(0, ModuleInfo.AuthorBeginsWith.Length);
                    Console.WriteLine(moduleInfo.Author);
                }
                else if (line.StartsWith(ModuleInfo.AuthorLinkBeginsWith))
                {
                    moduleInfo.AuthorLink = line.Remove(0, ModuleInfo.AuthorLinkBeginsWith.Length);
                    Console.WriteLine(moduleInfo.AuthorLink);
                }
                else if (line.StartsWith(ModuleInfo.AuthorIsWebLinkBeginsWith))
                {
                    string booleanString = line.Remove(0, ModuleInfo.AuthorIsWebLinkBeginsWith.Length);

                    try
                    {
                        moduleInfo.AuthorIsWebLink = System.Convert.ToInt32(booleanString) > 0 ? true : false;
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine("Can't convert to Integer: " + booleanString);
                        Console.WriteLine(e.Message);
                        moduleInfo.AuthorIsWebLink = false;
                    }

                    Console.WriteLine(moduleInfo.AuthorIsWebLink);
                }
                else if (line.StartsWith(ModuleInfo.ImagePathBeginsWith))
                {
                    moduleInfo.ImagePath = line.Remove(0, ModuleInfo.ImagePathBeginsWith.Length);
                    Console.WriteLine(moduleInfo.ImagePath);
                }
                else if (line.StartsWith(ModuleInfo.IconPathBeginsWith))
                {
                    moduleInfo.IconPath = line.Remove(0, ModuleInfo.IconPathBeginsWith.Length);
                    Console.WriteLine(moduleInfo.IconPath);
                }
            }

            return moduleInfo;
        }

        public static ModuleInfo FromPackedStream(Stream stream)
        {
            ModuleInfo moduleInfo = new ModuleInfo();

            using (BinaryReader br = new BinaryReader(stream))
            {
                byte[] magicGuidBytes = Program.MagicGuidBytes;
                br.BaseStream.Position = br.BaseStream.Length - ((sizeof(int) * 2) + magicGuidBytes.Length);
                int moduleDataBytesLength = br.ReadInt32();
                int moduleInfoBytesLength = br.ReadInt32();
                int offsetPosition = (int)br.BaseStream.Length - ((sizeof(int) * 2) + magicGuidBytes.Length + moduleDataBytesLength + moduleInfoBytesLength);
                
                br.BaseStream.Position = offsetPosition;

                int[] headerPositions = new int[12];

                for (int i = 0; i < headerPositions.Length; i++)
                {
                    headerPositions[i] = offsetPosition + br.ReadInt32();
                    Console.WriteLine(headerPositions[i]);
                }

                br.BaseStream.Position = headerPositions[4];
                moduleInfo.Name = Encoding.UTF8.GetString(br.ReadBytes(headerPositions[5] - headerPositions[4]));

                br.BaseStream.Position = headerPositions[5];
                moduleInfo.FolderName = Encoding.UTF8.GetString(br.ReadBytes(headerPositions[6] - headerPositions[5]));

                br.BaseStream.Position = headerPositions[6];
                moduleInfo.Version = Encoding.UTF8.GetString(br.ReadBytes(headerPositions[7] - headerPositions[6]));

                br.BaseStream.Position = headerPositions[7];
                moduleInfo.Author = Encoding.UTF8.GetString(br.ReadBytes(headerPositions[8] - headerPositions[7]));

                br.BaseStream.Position = headerPositions[8];
                moduleInfo.AuthorLink = Encoding.UTF8.GetString(br.ReadBytes(headerPositions[9] - headerPositions[8]));

                br.BaseStream.Position = headerPositions[9];
                moduleInfo.AuthorIsWebLink = br.ReadByte() > 0 ? true : false;

                br.BaseStream.Position = headerPositions[10];
                moduleInfo.ImageStream = new MemoryStream(br.ReadBytes(headerPositions[11] - headerPositions[10]));
                moduleInfo.Image = Image.FromStream(moduleInfo.ImageStream);

                br.BaseStream.Position = headerPositions[11];
                MemoryStream iconStream = new MemoryStream(br.ReadBytes((int)(br.BaseStream.Length - ((sizeof(int) * 2) + magicGuidBytes.Length + moduleDataBytesLength)) - headerPositions[11]));
                moduleInfo.Icon = new Icon(iconStream);
                iconStream.Dispose();
            }

            return moduleInfo;
        }

        public void WriteToStream(Stream stream, int offset = 0)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                int emptySpaces = sizeof(int) * 4; //Empty Space for Future and Backwards Compatibility
                int headerSize = emptySpaces + (sizeof(int) * 8);

                bw.BaseStream.Position = sizeof(int) * 4;
                bw.Write(headerSize);
                bw.BaseStream.Position = headerSize;
                bw.Write(NameBytes);

                int position = (int)bw.BaseStream.Position;
                bw.BaseStream.Position = sizeof(int) * 5;
                bw.Write(position);
                bw.BaseStream.Position = position;
                bw.Write(FolderNameBytes);

                position = (int)bw.BaseStream.Position;
                bw.BaseStream.Position = sizeof(int) * 6;
                bw.Write(position);
                bw.BaseStream.Position = position;
                bw.Write(VersionBytes);

                position = (int)bw.BaseStream.Position;
                bw.BaseStream.Position = sizeof(int) * 7;
                bw.Write(position);
                bw.BaseStream.Position = position;
                bw.Write(AuthorBytes);

                position = (int)bw.BaseStream.Position;
                bw.BaseStream.Position = sizeof(int) * 8;
                bw.Write(position);
                bw.BaseStream.Position = position;
                bw.Write(AuthorLinkBytes);

                position = (int)bw.BaseStream.Position;
                bw.BaseStream.Position = sizeof(int) * 9;
                bw.Write(position);
                bw.BaseStream.Position = position;
                bw.Write(AuthorIsWebLinkByte);

                position = (int)bw.BaseStream.Position;
                bw.BaseStream.Position = sizeof(int) * 10;
                bw.Write(position);
                bw.BaseStream.Position = position;
                bw.Write(ImageBytes);

                position = (int)bw.BaseStream.Position;
                bw.BaseStream.Position = sizeof(int) * 11;
                bw.Write(position);
                bw.BaseStream.Position = position;
                bw.Write(IconBytes);
            }
        }
    }
}
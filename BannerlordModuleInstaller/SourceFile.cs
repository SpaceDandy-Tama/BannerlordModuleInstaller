using System;
using System.IO;
using System.Text;

namespace BannerlordModuleInstaller
{
    public class SourceFile
    {
        public string Name;
        public byte[] Data;

        public byte[] NameBytes => Encoding.UTF8.GetBytes(Name);
        public string NameOnly => Path.GetFileName(Name);
        public string DirOnly => Path.GetDirectoryName(Name);

        public SourceFile(string name)
        {
            Name = name;
            Data = File.ReadAllBytes(name);
        }

        public static void FromPackedStream(BinaryReader br, int headerPos, int endOfheader)
        {
            string name = null;
            byte[] data = null;

            br.BaseStream.Position = headerPos;
            int dataPos = br.ReadInt32();

            br.BaseStream.Position = dataPos;
            int nameByteLength = br.ReadInt32();
            name = Encoding.UTF8.GetString(br.ReadBytes(nameByteLength));

            int fileDataPos = (int)br.BaseStream.Position;

            if (headerPos + sizeof(int) < endOfheader - 1)
            {
                br.BaseStream.Position = headerPos + sizeof(int);
                int fileDataLength = br.ReadInt32() - fileDataPos;
                br.BaseStream.Position = fileDataPos;
                data = br.ReadBytes(fileDataLength);
            }
            else
            {
                int fileDataLength = (int)br.BaseStream.Length - fileDataPos;
                data = br.ReadBytes(fileDataLength);
            }

            if (name != null || name.Length > 0 || data != null)
            {
                name = Path.Combine(Form1.InstallationDir, name);
                string dirOnly = Path.GetDirectoryName(name);
                //string nameOnly = Path.GetFileName(name);

                if (!Directory.Exists(dirOnly))
                {
                    Directory.CreateDirectory(dirOnly);
                }

                using (FileStream fs = new FileStream(name, FileMode.Create))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
        }

        public void PackToStream(BinaryWriter bw, int headerPos)
        {
            int dataPos = (int)bw.BaseStream.Position;

            bw.BaseStream.Position = headerPos;
            bw.Write(dataPos);
            bw.BaseStream.Position = dataPos;

            byte[] nameBytes = NameBytes;
            bw.Write(nameBytes.Length);
            bw.Write(nameBytes);
            bw.Write(Data);
        }
    }
}
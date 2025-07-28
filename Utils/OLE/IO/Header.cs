using System;
using Utils;

namespace Utils.OLE.IO{
    public class Header{
        public byte[] Version   { get; set; } = new byte[4]; // バージョン情報
        public byte[] CLSID     { get; set; } = new byte[16]; // CLSID

        public Header(){
            Version = ((byte)0x00).Repeat(4);   // 初期化: 0x00を4回繰り返す
            CLSID   = ((byte)0x00).Repeat(16);  // 初期化:
        }
        public Header(byte[] version, byte[] clsid){
            if (version == null || version.Length != 4) throw new ArgumentException("Version must be 4 bytes long.");
            if (clsid == null || clsid.Length != 16) throw new ArgumentException("CLSID must be 16 bytes long.");
            Version = version;
            CLSID   = clsid;
        }
        public Header(uint version, Guid clsid){
            Version = BitConverter.GetBytes(version);
            CLSID   = clsid.ToByteArray();
        }


        public byte[] ToBytes(){
            byte[] headerBytes = new byte[20]; // 4 (Version) + 16 (CLSID)
            Buffer.BlockCopy(Version, 0, headerBytes, 0, Version.Length);
            Buffer.BlockCopy(CLSID, 0, headerBytes, Version.Length, CLSID.Length);
            return headerBytes;
        }
    }
}
using System;
using System.Text;

namespace Utils.DataType.Bitmap.Offset{
    public class Header{
        private const int HeaderSize = 14; // BMPヘッダーのサイズ
        private const string HeaderSignature = "BM"; // BMPヘッダーのシグネチャ
        public int FileSize { get; private set; }
        public int Offset { get; private set; }

        private byte[] FileSizeByte => BitConverter.GetBytes(FileSize);
        private byte[] OffsetByte => BitConverter.GetBytes(Offset);

        public Header(){
            FileSize = 0;
            Offset = HeaderSize;
        }
        public byte[] ToBytes(){
            byte[] header = new byte[HeaderSize];
            // ヘッダーのシグネチャ
            Encoding.ASCII.GetBytes(HeaderSignature, 0, HeaderSignature.Length, header, 0);
            // ファイルサイズ（4バイト）
            Buffer.BlockCopy(FileSizeByte, 0, header, 2, FileSizeByte.Length);
            // 予約領域（2バイト） - 0で初期化
            // オフセット（4バイト）
            Buffer.BlockCopy(OffsetByte, 0, header, 10, OffsetByte.Length);

            return header;
        }
    }
}
using System;

namespace Utils.DataType.DIB.Offset{
    public class BITMAPCOREHEADER{
        public const int   HeaderSize     = 12;           // ヘッダーのサイズ
        public const short Planes         = 1;            // プレーン数
        public const short BitCount       = 24;           // ビット数

        public int   Width            { get; private set; }                    // 画像の幅
        public int   Height           { get; private set; }                    // 画像の高さ

        public BITMAPCOREHEADER(int width, int height){
            Width = width;
            Height = height;
        }

        public byte[] ToBytes(){
            byte[] header = new byte[HeaderSize];
            BitConverter.GetBytes(Width)   .CopyTo(header, 0);
            BitConverter.GetBytes(Height)  .CopyTo(header, 4);
            BitConverter.GetBytes(Planes)  .CopyTo(header, 8);
            BitConverter.GetBytes(BitCount).CopyTo(header, 10);
            return header;
        }
    }
}
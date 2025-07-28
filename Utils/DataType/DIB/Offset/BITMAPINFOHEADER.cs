using System;

namespace Utils.DataType.DIB.Offset{
    public class BITMAPINFOHEADER{
        public const int   HeaderSize     = 40;           // ヘッダーのサイズ
        public const short Planes         = 1;            // プレーン数
        public const short BitCount       = 24;           // ビット数

        public int   Size             { get; private set; } = HeaderSize;      // 構造体サイズ
        public int   Width            { get; private set; }                    // 画像の幅
        public int   Height           { get; private set; }                    // 画像の高さ
        public short PlanesValue      { get; private set; } = Planes;          // プレーン数
        public short BitCountValue    { get; private set; } = BitCount;        // ビット数
        public int   Compression      { get; private set; } = 0;               // 圧縮方式（BI_RGB）
        public int   SizeImage        { get; private set; } = 0;               // 画像データサイズ
        public int   XPelsPerMeter    { get; private set; } = 2835;            // 水平解像度
        public int   YPelsPerMeter    { get; private set; } = 2835;            // 垂直解像度
        public int   ClrUsed          { get; private set; } = 0;               // 使用色数
        public int   ClrImportant     { get; private set; } = 0;               // 重要色数

        public BITMAPINFOHEADER(int width, int height){
            Width = width;
            Height = height;
        }

        public byte[] ToBytes(){
            byte[] header = new byte[HeaderSize];
            int offset = 0;
            BitConverter.GetBytes(Size)         .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Width)        .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Height)       .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(PlanesValue)  .CopyTo(header, offset); offset += 2;
            BitConverter.GetBytes(BitCountValue).CopyTo(header, offset); offset += 2;
            BitConverter.GetBytes(Compression)  .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(SizeImage)    .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(XPelsPerMeter).CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(YPelsPerMeter).CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(ClrUsed)      .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(ClrImportant) .CopyTo(header, offset); offset += 4;
            return header;
        }
    }
}
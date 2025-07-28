using System;

namespace Utils.DataType.DIB.Offset{
    public class BITMAPV4HEADER{
        public const int   HeaderSize     = 108;          // ヘッダーのサイズ
        public const short Planes         = 1;            // プレーン数
        public const short BitCount       = 32;           // ビット数

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
        public uint  RedMask          { get; private set; } = 0x00FF0000;      // 赤マスク
        public uint  GreenMask        { get; private set; } = 0x0000FF00;      // 緑マスク
        public uint  BlueMask         { get; private set; } = 0x000000FF;      // 青マスク
        public uint  AlphaMask        { get; private set; } = 0xFF000000;      // アルファマスク
        public uint  CSType           { get; private set; } = 0x73524742;      // 色空間型（LCS_sRGB）
        public int   Endpoints1       { get; private set; } = 0;               // 色空間エンドポイント（Xr）
        public int   Endpoints2       { get; private set; } = 0;               // 色空間エンドポイント（Xg）
        public int   Endpoints3       { get; private set; } = 0;               // 色空間エンドポイント（Xb）
        public int   Endpoints4       { get; private set; } = 0;               // 色空間エンドポイント（Yr）
        public int   Endpoints5       { get; private set; } = 0;               // 色空間エンドポイント（Yg）
        public int   Endpoints6       { get; private set; } = 0;               // 色空間エンドポイント（Yb）
        public int   Endpoints7       { get; private set; } = 0;               // 色空間エンドポイント（Zr）
        public int   Endpoints8       { get; private set; } = 0;               // 色空間エンドポイント（Zg）
        public int   Endpoints9       { get; private set; } = 0;               // 色空間エンドポイント（Zb）
        public uint  GammaRed         { get; private set; } = 0;               // ガンマ値（赤）
        public uint  GammaGreen       { get; private set; } = 0;               // ガンマ値（緑）
        public uint  GammaBlue        { get; private set; } = 0;               // ガンマ値（青）

        public BITMAPV4HEADER(int width, int height){
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
            BitConverter.GetBytes(RedMask)      .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(GreenMask)    .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(BlueMask)     .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(AlphaMask)    .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(CSType)       .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints1)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints2)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints3)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints4)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints5)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints6)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints7)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints8)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(Endpoints9)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(GammaRed)     .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(GammaGreen)   .CopyTo(header, offset); offset += 4;
            BitConverter.GetBytes(GammaBlue)    .CopyTo(header, offset); offset += 4;
            return header;
        }
    }
}
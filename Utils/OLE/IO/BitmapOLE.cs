using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System;

namespace Utils.OLE.IO{
    public class BitmapOLE{
        public byte[] DibData { get; private set; }

        public BitmapOLE(Bitmap bitmap){
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap), "Bitmap cannot be null.");
            DibData = ConvertBitmapToDib(bitmap);
        }
        private byte[] ConvertBitmapToDib(Bitmap bitmap){
            using(MemoryStream ms = new MemoryStream()){
                // BitmapをDIB形式で保存
                bitmap.Save(ms, ImageFormat.Bmp);
                byte[] bmpData = ms.ToArray();

                // BMPヘッダーを除去してDIBデータのみを取得
                if (bmpData.Length < 54) throw new InvalidDataException("Invalid BMP data.");
                int dibSize = bmpData.Length - 54; // 54バイトはBMPヘッダーのサイズ
                byte[] dibData = new byte[dibSize];
                Buffer.BlockCopy(bmpData, 54, dibData, 0, dibSize);
                return dibData;
            }
        }

        public byte[] ToBytes(){
            if (DibData == null || DibData.Length == 0) throw new InvalidOperationException("DibData is empty or null.");
            return DibData;
        }
    }
}
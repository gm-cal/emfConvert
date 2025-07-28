namespace Utils.DataType.Bitmap.Offset{
    public class DibInfo{
        public int HeaderSize { get; private set; } // ヘッダーのサイズ
        public int Width { get; private set; } // 画像の幅
        public int Height { get; private set; } // 画像の高さ
        public short Planes { get; private set; } // プレーン数
        public short BitCount { get; private set; } // ビット数
        public int Compression { get; private set; } // 圧縮方式
        public int ImageSize { get; private set; } // 画像データのサイズ
        public int XPixelsPerMeter { get; private set; } // 水平方向の解像度
        public int YPixelsPerMeter { get; private set; } // 垂直方向の解像度
        public int ColorsUsed { get; private set; } // 使用される色数
        public int ImportantColors { get; private set; } // 重要な色数

        public DibInfo(){
            HeaderSize = 40;
            Width = 0;
            Height = 0;
            Planes = 1;
            BitCount = 24;
            Compression = 0;
            ImageSize = 0;
            XPixelsPerMeter = 2835;
            YPixelsPerMeter = 2835;
            ColorsUsed = 0;
            ImportantColors = 0;
        }
    }
}
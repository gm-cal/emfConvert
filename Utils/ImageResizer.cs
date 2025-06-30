using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Utils{
    public static class ImageResizer{
        // クリップボードの画像を指定倍率でリサイズし、再度クリップボードへ格納する
        public static bool ResizeClipboardImage(double percent, out string errorMessage){
            errorMessage = string.Empty;
            if (!Clipboard.ContainsImage()){
                errorMessage = "クリップボードに画像がありません。";
                return false;
            }
            if (percent <= 0){
                errorMessage = "有効な倍率(%)を入力してください。";
                return false;
            }
            BitmapSource srcBmp = GetClipboardImage();
            (int width, int height) = CalculateNewSize(srcBmp, percent);
            if (width <= 0 || height <= 0){
                errorMessage = "倍率が小さすぎます。";
                return false;
            }
            Bitmap bmp = BitmapSourceToBitmap(srcBmp);
            Bitmap resizedBmp = ResizeBitmap(bmp, width, height);
            BitmapSource resultSource = BitmapToBitmapSource(resizedBmp);
            SetClipboardImage(resultSource);
            bmp.Dispose();
            resizedBmp.Dispose();
            return true;
        }

        // クリップボードから画像を取得する
        private static BitmapSource GetClipboardImage(){
            return Clipboard.GetImage();
        }

        // リサイズ後の幅と高さを計算する
        private static (int width, int height) CalculateNewSize(BitmapSource srcBmp, double percent){
            int width = (int)(srcBmp.Width * percent / 100.0);
            int height = (int)(srcBmp.Height * percent / 100.0);
            return (width, height);
        }

        // BitmapSourceをBitmapに変換する
        private static Bitmap BitmapSourceToBitmap(BitmapSource source){
            using (MemoryStream ms = new MemoryStream()){
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(ms);
                using (Bitmap bmp = new Bitmap(ms)){
                    return new Bitmap(bmp);
                }
            }
        }

        // Bitmapを指定サイズでリサイズする
        private static Bitmap ResizeBitmap(Bitmap bmp, int width, int height){
            Bitmap resizedBmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedBmp)){
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(bmp, 0, 0, width, height);
            }
            return resizedBmp;
        }

        // BitmapをBitmapSourceに変換する
        private static BitmapSource BitmapToBitmapSource(Bitmap bmp){
            using (MemoryStream msOut = new MemoryStream()){
                bmp.Save(msOut, ImageFormat.Png);
                msOut.Seek(0, SeekOrigin.Begin);
                PngBitmapDecoder pngBitmap = new PngBitmapDecoder(msOut, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return pngBitmap.Frames[0];
            }
        }

        // 画像をクリップボードにセットする
        private static void SetClipboardImage(BitmapSource image){
            Clipboard.SetImage(image);
        }
    }
}

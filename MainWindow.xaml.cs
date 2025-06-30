using System.Windows;
using System;
using Services;
using System.Runtime.InteropServices; // 追加
using System.Windows.Interop; // 追加

public partial class MainWindow : Window{
    // Win32 API 定義
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int HOTKEY_ID_CONVERT = 9000;
    private const int HOTKEY_ID_RESIZE = 9001;
    private const uint MOD_CONTROL = 0x2;
    private const uint MOD_SHIFT = 0x4;
    private const uint VK_E = 0x45;
    private const uint VK_R = 0x52;

    public MainWindow(){
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e){
        var helper = new WindowInteropHelper(this);
        RegisterHotKey(helper.Handle, HOTKEY_ID_CONVERT, MOD_CONTROL | MOD_SHIFT, VK_E); // Ctrl+Shift+E
        RegisterHotKey(helper.Handle, HOTKEY_ID_RESIZE, MOD_CONTROL | MOD_SHIFT, VK_R);  // Ctrl+Shift+R
        HwndSource source = HwndSource.FromHwnd(helper.Handle);
        source.AddHook(HwndHook);
    }

    private void MainWindow_Closed(object sender, EventArgs e){
        var helper = new WindowInteropHelper(this);
        UnregisterHotKey(helper.Handle, HOTKEY_ID_CONVERT);
        UnregisterHotKey(helper.Handle, HOTKEY_ID_RESIZE);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled){
        const int WM_HOTKEY = 0x0312;
        if (msg == WM_HOTKEY){
            if (wParam.ToInt32() == HOTKEY_ID_CONVERT){
                Dispatcher.Invoke(() => OnConvertClick(this, null));
                handled = true;
            } else if (wParam.ToInt32() == HOTKEY_ID_RESIZE){
                Dispatcher.Invoke(() => OnResizeImageClick(this, null));
                handled = true;
            }
        }
        return IntPtr.Zero;
    }

    private void OnConvertClick(object sender, RoutedEventArgs e){
        EmfGenerator generator = new EmfGenerator();
        generator.ConvertClipboardHtmlTableToEmf();
    }

    private void OnDebugClick(object sender, RoutedEventArgs e){
        if (Clipboard.ContainsText(TextDataFormat.Html)){
            string html = Clipboard.GetText(TextDataFormat.Html);
            // カスタムウィンドウでHTMLを表示（スクロール・サイズ指定）
            Window htmlWindow = new Window{
                Title = "Clipboard HTML",
                Width = 800,
                Height = 600,
                Content = new System.Windows.Controls.ScrollViewer{
                    Content = new System.Windows.Controls.TextBox{
                        Text = html,
                        IsReadOnly = true,
                        AcceptsReturn = true,
                        AcceptsTab = true,
                        VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                        TextWrapping = System.Windows.TextWrapping.NoWrap
                    }
                }
            };
            htmlWindow.ShowDialog();
        } else {
            System.Windows.MessageBox.Show("クリップボードにHTMLデータがありません", "Clipboard HTML");
        }
    }

    private void OnResizeImageClick(object sender, RoutedEventArgs e){
        if (!Clipboard.ContainsImage()){
            MessageBox.Show("クリップボードに画像がありません。", "画像リサイズ");
            return;
        }
        // 倍率取得
        if (!double.TryParse(ResizeScaleTextBox.Text, out double percent) || percent <= 0){
            MessageBox.Show("有効な倍率(%)を入力してください。", "画像リサイズ");
            return;
        }
        var srcBmp = Clipboard.GetImage();
        int width = (int)(srcBmp.Width * percent / 100.0);
        int height = (int)(srcBmp.Height * percent / 100.0);
        if (width <= 0 || height <= 0){
            MessageBox.Show("倍率が小さすぎます。", "画像リサイズ");
            return;
        }
        // BitmapSource → System.Drawing.Bitmap
        using (var ms = new System.IO.MemoryStream()){
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(srcBmp));
            encoder.Save(ms);
            using (var bmp = new System.Drawing.Bitmap(ms)){
                using (var resizedBmp = new System.Drawing.Bitmap(width, height)){
                    using (var g = System.Drawing.Graphics.FromImage(resizedBmp)){
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.DrawImage(bmp, 0, 0, width, height);
                    }
                    // PNG形式でクリップボードへ（画素情報保持）
                    using (var msOut = new System.IO.MemoryStream()){
                        resizedBmp.Save(msOut, System.Drawing.Imaging.ImageFormat.Png);
                        msOut.Seek(0, System.IO.SeekOrigin.Begin);
                        var pngBitmap = new System.Windows.Media.Imaging.PngBitmapDecoder(msOut, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
                        Clipboard.SetImage(pngBitmap.Frames[0]);
                    }
                }
            }
        }
        MessageBox.Show("画像を" + percent + "%でリサイズしクリップボードへ戻しました。", "画像リサイズ");
    }
}
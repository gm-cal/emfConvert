using System.Windows;
using System;
using System.IO;
using Services;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using Utils;

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

    private readonly EmfGenerator _emfGenerator;

    public MainWindow(){
        InitializeComponent();
        // DIで各機能を注入
        Utils.HTML.Parser parser = new Utils.HTML.Parser();
        Utils.HTML.Layout.TableLayoutCalculator tableLayoutCalculator = new Utils.HTML.Layout.TableLayoutCalculator();
        _emfGenerator = new EmfGenerator(parser, tableLayoutCalculator);
        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e){
        WindowInteropHelper helper = new WindowInteropHelper(this);
        RegisterHotKey(helper.Handle, HOTKEY_ID_CONVERT, MOD_CONTROL | MOD_SHIFT, VK_E); // Ctrl+Shift+E
        RegisterHotKey(helper.Handle, HOTKEY_ID_RESIZE, MOD_CONTROL | MOD_SHIFT, VK_R);  // Ctrl+Shift+R
        HwndSource source = HwndSource.FromHwnd(helper.Handle);
        source.AddHook(HwndHook);
    }

    private void MainWindow_Closed(object sender, EventArgs e){
        WindowInteropHelper helper = new WindowInteropHelper(this);
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
        if (Clipboard.ContainsText(TextDataFormat.Html)){
            _emfGenerator.ConvertClipboardHtmlTableToEmf();
            return;
        }

        if (Clipboard.ContainsText()){
            string text = Clipboard.GetText();
            if (SvgClipboardHelper.IsSvgText(text)){
                SvgClipboardHelper.SetSvgFormat(text);
                MessageBox.Show("SVGフォーマットをクリップボードに生成しました。", "SVG" );
                return;
            }
        }

        MessageBox.Show("クリップボードにHTMLまたはSVGデータがありません。", "エラー");
    }

    private void OnViewClipboardClick(object sender, RoutedEventArgs e){
        if (Clipboard.ContainsText(TextDataFormat.Html)){
            string html = Clipboard.GetText(TextDataFormat.Html);
            ShowTextWindow(html, "Clipboard HTML");
            return;
        }

        if (Clipboard.ContainsText()){
            string text = Clipboard.GetText();
            ShowTextWindow(text, "Clipboard Text");
            return;
        }

        MessageBox.Show("クリップボードに表示可能なデータがありません", "Clipboard");
    }

    private void ShowTextWindow(string text, string title){
        Window window = new Window{
            Title = title,
            Width = 800,
            Height = 600,
            Content = new ScrollViewer{
                Content = new TextBox{
                    Text = text,
                    IsReadOnly = true,
                    AcceptsReturn = true,
                    AcceptsTab = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    TextWrapping = TextWrapping.NoWrap
                }
            }
        };
        window.ShowDialog();
    }

    private void OnResizeImageClick(object sender, RoutedEventArgs e){
        double percent;
        if (!double.TryParse(ResizeScaleTextBox.Text, out percent) || percent <= 0){
            MessageBox.Show("有効な倍率(%)を入力してください。", "画像リサイズ");
            return;
        }
        string errorMessage;
        bool result = Utils.ImageResizer.ResizeClipboardImage(percent, out errorMessage);
        if (!result)
        {
            MessageBox.Show(errorMessage, "画像リサイズ");
        }
        // 成功時のメッセージは不要ならコメントアウトのまま
        // else
        // {
        //     MessageBox.Show($"画像を{percent}%でリサイズしクリップボードへ戻しました。", "画像リサイズ");
        // }
    }
}

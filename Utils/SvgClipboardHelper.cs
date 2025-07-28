using System;
using System.Windows;

namespace Utils{
    public static class SvgClipboardHelper{
        // SVG文字列かどうか判定
        public static bool IsSvgText(string text){
            if(string.IsNullOrWhiteSpace(text)) return false;
            string trimmed = text.TrimStart();
            if(trimmed.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)){
                int index = trimmed.IndexOf("<svg", StringComparison.OrdinalIgnoreCase);
                return index >= 0;
            }
            return trimmed.StartsWith("<svg", StringComparison.OrdinalIgnoreCase);
        }

        // SVGフォーマットをクリップボードに設定
        public static void SetSvgFormat(string svgText){
            DataObject obj = new DataObject();
            obj.SetData(DataFormats.UnicodeText, svgText);
            obj.SetData(DataFormats.Text, svgText);
            obj.SetData("image/svg+xml", svgText);
            Clipboard.SetDataObject(obj, true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Utils;
using Utils.HTML;
using Utils.HTML.IO;
using Utils.HTML.Layout;

namespace Services{
    public class EmfGenerator{
        public void ConvertClipboardHtmlTableToEmf(){
            if (!Clipboard.ContainsText(TextDataFormat.Html)){
                MessageBox.Show("HTML形式のクリップボードデータが見つかりません。", "エラー");
                return;
            }

            string html = Clipboard.GetText(TextDataFormat.Html);
            Parser parser = new Parser();
            List<List<Cell>> table = parser.ParseHtmlTable(html);

            if (table.Count == 0){
                MessageBox.Show("テーブルが見つかりません。", "エラー");
                return;
            }

            Font font = new Font("MS Gothic", 9);
            float padding = 8.0f;

            SizeF tableSize = TableLayoutCalculator.ComputeCellSizes(table, font, padding);

            Rectangle frame = new Rectangle(0, 0, (int)Math.Ceiling(tableSize.Width), (int)Math.Ceiling(tableSize.Height));
            using (MemoryStream stream = new MemoryStream())
            using (Graphics referenceGraphics = Graphics.FromHwnd(IntPtr.Zero)){
                IntPtr hdc = referenceGraphics.GetHdc();
                Metafile metafile = new Metafile(stream, hdc, frame, MetafileFrameUnit.Pixel);
                referenceGraphics.ReleaseHdc(hdc);

                using (Graphics graphics = Graphics.FromImage(metafile)){
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    // 表全体を描画（外枠オブジェクト）
                    RectangleF tableRect = new RectangleF(0, 0, tableSize.Width, tableSize.Height);
                    graphics.FillRectangle(Brushes.White, tableRect); // 背景白（または他色）
                    Pen outerPen = new Pen(Color.Gray, 1.5f) { DashStyle = DashStyle.Solid };
                    graphics.DrawRectangle(outerPen, tableRect.X, tableRect.Y, tableRect.Width, tableRect.Height);

                    // ...既存コード続き
                    float y = 0.0f;
                    for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                        List<Cell> row = table[rowIndex];
                        float x = 0.0f;

                        for (int colIndex = 0; colIndex < row.Count; colIndex++){
                            Cell cell = row[colIndex];
                            if (cell.IsDrawn) continue;

                            float width = 0.0f;
                            for (int i = 0; i < cell.ColSpan && colIndex + i < row.Count; i++){
                                width += row[colIndex + i].Width;
                            }

                            float height = 0.0f;
                            for (int i = 0; i < cell.RowSpan && rowIndex + i < table.Count; i++){
                                height += table[rowIndex + i][colIndex].Height;
                            }

                            RectangleF rect = new RectangleF(x, y, width, height);

                            // セル描画処理内（cell に対する処理の中）
                            Pen pen = new Pen(Color.Black, 1.0f);
                            Brush backgroundBrush = new SolidBrush(cell.BackgroundColor);
                            graphics.FillRectangle(backgroundBrush, rect); // 背景
                            graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height); // 枠線
                            graphics.DrawString(cell.Text, font, Brushes.Black, rect.X + padding, rect.Y + 4); // テキスト

                            x += width;
                        }

                        y += row[0].Height;
                    }
                }

                ClipboardMetafile.PutEnhMetafileOnClipboard(metafile);
            }
        }
    }
}

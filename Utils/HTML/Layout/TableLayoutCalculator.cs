using System.Collections.Generic;
using System.Drawing;
using Utils.HTML.IO;

namespace Utils.HTML.Layout{
    public static class TableLayoutCalculator{
        public static SizeF ComputeCellSizes(List<List<Cell>> table, Font font, float padding){
            int columnCount = 0;
            foreach (List<Cell> row in table){
                if (row.Count > columnCount){
                    columnCount = row.Count;
                }
            }

            float[] maxColumnWidths = new float[columnCount];
            float[] rowHeights = new float[table.Count];

            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap)){
                for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                    List<Cell> row = table[rowIndex];
                    float maxHeight = 0.0f;

                    for (int colIndex = 0; colIndex < row.Count; colIndex++){
                        Cell cell = row[colIndex];
                        if (cell.IsDrawn) continue;

                        SizeF size = graphics.MeasureString(cell.Text, font);
                        float width = size.Width + padding * 2;
                        float height = size.Height + padding;

                        if (width > maxColumnWidths[colIndex]){
                            maxColumnWidths[colIndex] = width;
                        }

                        if (height > maxHeight){
                            maxHeight = height;
                        }

                        // 一旦仮に保存
                        cell.Width = width;
                        cell.Height = height;
                    }

                    rowHeights[rowIndex] = maxHeight;
                }

                // 各セルに列の最大幅を設定
                for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                    List<Cell> row = table[rowIndex];
                    for (int colIndex = 0; colIndex < row.Count; colIndex++){
                        Cell cell = row[colIndex];
                        if (!cell.IsDrawn){
                            cell.Width = maxColumnWidths[colIndex];
                        }
                    }
                }
            }

            float totalWidth = 0.0f;
            foreach (float w in maxColumnWidths){
                totalWidth += w;
            }

            float totalHeight = 0.0f;
            foreach (float h in rowHeights){
                totalHeight += h;
            }

            return new SizeF(totalWidth, totalHeight);
        }
    }
}

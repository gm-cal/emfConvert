using System.Collections.Generic;
using System.Drawing;
using Utils.HTML.IO;

namespace Utils.HTML.Layout{
    public static class TableLayoutCalculator{
        public static SizeF ComputeCellSizes(List<List<Cell>> table, Font font, float padding){
            float totalHeight = 0.0f;
            float totalWidth = 0.0f;
            int maxCols = 0;

            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap)){
                for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                    List<Cell> row = table[rowIndex];
                    float rowHeight = 0.0f;
                    float rowWidth = 0.0f;

                    for (int colIndex = 0; colIndex < row.Count; colIndex++){
                        Cell cell = row[colIndex];
                        if (cell.IsDrawn) continue;

                        SizeF size = graphics.MeasureString(cell.Text, font);
                        float width = size.Width + padding * 2;
                        float height = size.Height + padding;

                        cell.Width = width;
                        cell.Height = height;

                        rowWidth += width;
                        if (height > rowHeight){
                            rowHeight = height;
                        }
                    }

                    totalHeight += rowHeight;
                    if (rowWidth > totalWidth){
                        totalWidth = rowWidth;
                    }

                    if (row.Count > maxCols){
                        maxCols = row.Count;
                    }
                }
            }

            return new SizeF(totalWidth, totalHeight);
        }
    }
}

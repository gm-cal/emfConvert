using System.Collections.Generic;
using Utils.HTML.IO;

namespace Utils.HTML.Interfaces{
    public interface IParser{
        List<List<Cell>> ParseHtmlTable(string html);
    }

}
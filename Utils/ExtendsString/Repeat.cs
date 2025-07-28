namespace Utils{
    public static partial class ExtendsString{
        // 指定された文字列を指定された回数だけ繰り返す。
        public static string Repeat(this string str, int count){
            return new string(char.Parse(str), count);
        }
    }
}
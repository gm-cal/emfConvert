using System;

namespace Utils{
    public static partial class ExtendsByte{
        // 指定されたバイト配列を指定された回数だけ繰り返す。
        public static byte[] Repeat(this byte[] bytes, int count){
            if (bytes == null || bytes.Length == 0 || count <= 0) return new byte[0];
            byte[] result = new byte[bytes.Length * count];
            for (int i = 0; i < count; i++){
                Buffer.BlockCopy(bytes, 0, result, i * bytes.Length, bytes.Length);
            }
            return result;
        }
        public static byte[] Repeat(this byte b, int count){
            if (count <= 0) return new byte[0];
            byte[] result = new byte[count];
            for (int i = 0; i < count; i++){
                result[i] = b;
            }
            return result;
        }
    }
}
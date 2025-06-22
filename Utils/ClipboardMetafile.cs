using System;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Utils{
    public static class ClipboardMetafile{
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("gdi32.dll")]
        private static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, string lpszFile);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hemf);

        public const uint CF_ENHMETAFILE = 14;

        public static void PutEnhMetafileOnClipboard(Metafile metafile){
            IntPtr hEmf = metafile.GetHenhmetafile();
            IntPtr hEmfCopy = CopyEnhMetaFile(hEmf, null);

            if (hEmfCopy == IntPtr.Zero){
                return;
            }

            if (OpenClipboard(IntPtr.Zero)){
                EmptyClipboard();
                SetClipboardData(CF_ENHMETAFILE, hEmfCopy);
                CloseClipboard();
            }

            DeleteEnhMetaFile(hEmf);
        }
    }
}

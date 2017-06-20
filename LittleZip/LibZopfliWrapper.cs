using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace System.IO.Compression
{
    /// <summary>
    /// Zopfli format options
    /// </summary>
    public enum ZopfliFormat
    {
        /// <summary>Compress in GZIP format</summary>
        ZOPFLI_FORMAT_GZIP,
        /// <summary>Compress in ZLIB format</summary>
        ZOPFLI_FORMAT_ZLIB,
        /// <summary>Compress in DEFLATE format</summary>
        ZOPFLI_FORMAT_DEFLATE
    };

    /// <summary>
    /// Zopfli Options
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ZopfliOptions
    {
        /// <summary>Whether to print output</summary>
        public Int32 verbose;
        // Whether to print more detailed output
        /// <summary></summary>
        public Int32 verbose_more;
        /// <summary>
        /// Maximum amount of times to rerun forward and backward pass to optimize LZ77 compression cost.
        /// Good values: 10, 15 for small files, 5 for files over several MB in size or it will be too slow.
        /// </summary>
        public Int32 numiterations;
        /// <summary>
        /// If true, splits the data in multiple deflate blocks with optimal choice for the block boundaries.
        /// Block splitting gives better compression. Default: true (1).
        /// </summary>
        public Int32 blocksplitting;
        /// <summary>
        /// If true, chooses the optimal block split points only after doing the iterative
        /// LZ77 compression. If false, chooses the block split points first, then does
        /// iterative LZ77 on each individual block. Depending on the file, either first
        /// or last gives the best compression. Default: false (0).
        /// </summary>
        public Int32 blocksplittinglast;
        /// <summary>
        /// Maximum amount of blocks to split into (0 for unlimited, but this can give
        /// extreme results that hurt compression on some files). Default value: 15.</summary>
        public Int32 blocksplittingmax;

        /// <summary>Initializes options used throughout the program with default values.</summary>
        public ZopfliOptions()
        {
            verbose = 0;
            verbose_more = 0;
            numiterations = 5;
            blocksplitting = 1;
            blocksplittinglast = 0;
            blocksplittingmax = 15;
        }
    }

    class LibZopfli
    {
        /// <summary>convert byte array to compressed byte array</summary>
        /// <param name="data_in">Uncompressed data array</param>
        /// <param name="type">Format type, DEFLATE, GZIP, ZLIB</param>
        /// <param name="options">Compression options</param>
        /// <returns>Compressed data array</returns>
        public static void Deflate(byte[] inBuffer, out byte[] outBuffer, out uint compresedSize, out uint crc32, ZopfliOptions options = null)
        {
            IntPtr ptrInBuffer = IntPtr.Zero;
            IntPtr ptrOutBuffer = IntPtr.Zero;

            try
            {
                //If not options, set default.
                if (options == null)
                    options = new ZopfliOptions();

                //get CRC32
                ptrInBuffer = Marshal.AllocHGlobal(inBuffer.Length);
                Marshal.Copy(inBuffer, 0, ptrInBuffer, inBuffer.Length);
                crc32 = UnsafeNativeMethods.LibdeflateCrc32(0, ptrInBuffer, inBuffer.Length);

                // Get image data length
                UIntPtr data_size = (UIntPtr)inBuffer.Length;

                // Compress the data via native methods
                int result_size = UnsafeNativeMethods.ZopfliCompress(ref options, ZopfliFormat.ZOPFLI_FORMAT_DEFLATE, inBuffer, inBuffer.Length, out ptrOutBuffer);
                compresedSize = (uint)result_size;
                // Copy the result to array managed memory
                outBuffer = new byte[result_size];
                Marshal.Copy(ptrOutBuffer, outBuffer, 0, result_size);
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LibZopfli.Deflate"); }
            finally
            {
                // Free unmanaged memory
                Marshal.FreeHGlobal(ptrInBuffer);
                Marshal.FreeHGlobal(ptrOutBuffer);
            }
        }
    }

    [SuppressUnmanagedCodeSecurityAttribute]
    internal sealed partial class UnsafeNativeMethods
    {
        public static int ZopfliCompress(ref ZopfliOptions options, ZopfliFormat output_type, byte[] data, int data_size, out IntPtr data_out)
        {
            UIntPtr data_out_size = UIntPtr.Zero;
            data_out = IntPtr.Zero;

            switch (IntPtr.Size)
            {
                case 4:
                    ZopfliCompress_x86(ref options, output_type, data, data_size, ref data_out, ref data_out_size);
                    break;
                case 8:
                    ZopfliCompress_x64(ref options, output_type, data, data_size, ref data_out, ref data_out_size);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
            return (int)data_out_size;
        }
        [DllImport("zopfli_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ZopfliCompress")]
        private static extern void ZopfliCompress_x86(ref ZopfliOptions options, ZopfliFormat output_type, byte[] data, int data_size, ref IntPtr data_out, ref UIntPtr data_out_size);
        [DllImport("zopfli_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ZopfliCompress")]
        private static extern void ZopfliCompress_x64(ref ZopfliOptions options, ZopfliFormat output_type, byte[] data, int data_size, ref IntPtr data_out, ref UIntPtr data_out_size);
    }
}


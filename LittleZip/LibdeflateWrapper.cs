/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Wrapper for Libdeflate in C#. (GPL) Jose M. Piñeiro
///////////////////////////////////////////////////////////////////////////////////////////////////////////// 
/// Compress Functions:
/// Deflate(byte[] inBuffer, out byte[] outBuffer, out uint compresedSize, out uint crc32)
///      Deflate inBuffer and returns result in outBuffer. Gets compresed size  and CRC32 of inBuffer data
/// 
/// 
/// Another functions:
/// string GetVersion() - Get the library version
/// GetInfo(byte[] rawWebP, out int width, out int height, out bool has_alpha, out bool has_animation, out string format) - Get information of WEBP data
/// float[] PictureDistortion(Bitmap source, Bitmap reference, int metric_type) - Get PSNR, SSIM or LSIM distortion metric between two pictures
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Runtime.InteropServices;
using System.Security;


namespace LibdeflateWrapper
{
    class Libdeflate
    {
        public static void Deflate(byte[] inBuffer, out byte[] outBuffer, out uint compresedSize, out uint crc32)
        {
            try
            {
                IntPtr ptrInBuffer = Marshal.AllocHGlobal(inBuffer.Length);
                Marshal.Copy(inBuffer, 0, ptrInBuffer, inBuffer.Length);

                crc32 = UnsafeNativeMethods.LibdeflateCrc32(0, ptrInBuffer, inBuffer.Length);

                IntPtr compressor = UnsafeNativeMethods.LibdeflateAllocCompressor(12);
                if (compressor== null)
                    throw new Exception("Out of memory");

                int maxCompresedSize = UnsafeNativeMethods.LibdeflateDeflateCompressBound(compressor, inBuffer.Length);
                outBuffer = new byte[(int)(maxCompresedSize)];
                IntPtr ptrOutBuffer = Marshal.AllocHGlobal(maxCompresedSize);

                compresedSize = (uint)UnsafeNativeMethods.LibdeflateDeflateCompress(compressor, ptrInBuffer, inBuffer.Length, ptrOutBuffer, (int)maxCompresedSize);
                UnsafeNativeMethods.LibdeflateFreeCompressor(compressor);
                outBuffer = new byte[compresedSize];
                Marshal.Copy(ptrOutBuffer, outBuffer, 0, (int)compresedSize);
                Marshal.FreeHGlobal(ptrInBuffer);
                Marshal.FreeHGlobal(ptrOutBuffer);
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn Libdeflate.Deflate"); }
        }
    }

    [SuppressUnmanagedCodeSecurityAttribute]
    internal sealed partial class UnsafeNativeMethods
    {
        /* ========================================================================== */
        /*                             Compression                                    */
        /* ========================================================================== */

        /// <summary>
        /// Allocates a new compressor that supports DEFLATE, zlib, and gzip compression.
        ///
        /// Note: for compression, the sliding window size is defined at compilation time
        /// to 32768, the largest size permissible in the DEFLATE format.  It cannot be
        /// changed at runtime.
        /// 
        /// A single compressor is not safe to use by multiple threads concurrently.
        /// However, different threads may use different compressors concurrently.
        /// </summary>
        /// <param name="compression_level">the compression level on a zlib-like scale but with a higher maximum value (1 = fastest, 6 = medium/default, 9 = slow, 12 = slowest)</param>
        /// <returns>Pointer to the new compressor, or NULL if out of memory.</returns>
        public static IntPtr LibdeflateAllocCompressor(int compression_level)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return libdeflate_alloc_compressor_x86(compression_level);
                case 8:
                    return libdeflate_alloc_compressor_x64(compression_level);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
        private static extern IntPtr libdeflate_alloc_compressor_x86(int compression_level);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_alloc_compressor")]
        private static extern IntPtr libdeflate_alloc_compressor_x64(int compression_level);

        /// <summary>Performs raw DEFLATE in the ZLIB format compression on a buffer of data.</summary>
        /// <param name="compressor">Pointer to the compressor</param>
        /// <param name="inData">Data to compress</param>
        /// <param name="in_nbytes">Length of data to compress</param>
        /// <param name="outBuffer">Data compresed</param>
        /// <param name="out_nbytes_avail">Leght of buffer for data compresed</param>
        /// <returns>Compressed size in bytes, or 0 if the data could not be compressed to 'out_nbytes_avail' bytes or fewer.</returns>
        public static long LibdeflateZlibCompress(IntPtr compressor, IntPtr inBuffer, int in_nbytes, IntPtr outBuffer, int out_nbytes_avail)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (long)(libdeflate_zlib_compress_x86(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail));
                case 8:
                    return (long)(libdeflate_zlib_compress_x64(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail));
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_zlib_compress")]
        private static extern UIntPtr libdeflate_zlib_compress_x86(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_zlib_compress")]
        private static extern UIntPtr libdeflate_zlib_compress_x64(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);

        /// <summary>Performs raw DEFLATE compression on a buffer of data.</summary>
        /// <param name="compressor">Pointer to the compressor</param>
        /// <param name="inData">Data to compress</param>
        /// <param name="in_nbytes">Length of data to compress</param>
        /// <param name="outData">Data compresed</param>
        /// <param name="out_nbytes_avail">Leght of buffer for data compresed</param>
        /// <returns>Compressed size in bytes, or 0 if the data could not be compressed to 'out_nbytes_avail' bytes or fewer.</returns>
        public static long LibdeflateDeflateCompress(IntPtr compressor, IntPtr inBuffer, int in_nbytes, IntPtr outBuffer, int out_nbytes_avail)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (long)(libdeflate_deflate_compress_x86(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail));
                case 8:
                    return (long)(libdeflate_deflate_compress_x64(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail));
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress")]
        private static extern UIntPtr libdeflate_deflate_compress_x86(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress")]
        private static extern UIntPtr libdeflate_deflate_compress_x64(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);

        /// <summary>Performs raw GZIP compression on a buffer of data.</summary>
        /// <param name="compressor">Pointer to the compressor</param>
        /// <param name="inData">Data to compress</param>
        /// <param name="in_nbytes">Length of data to compress</param>
        /// <param name="outData">Data compresed</param>
        /// <param name="out_nbytes_avail">Leght of buffer for data compresed</param>
        /// <returns>Compressed size in bytes, or 0 if the data could not be compressed to 'out_nbytes_avail' bytes or fewer.</returns>
        public static long libdeflateGzipCompress(IntPtr compressor, IntPtr inBuffer, int in_nbytes, IntPtr outBuffer, int out_nbytes_avail)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (long)(libdeflate_gzip_compress_x86(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail));
                case 8:
                    return (long)(libdeflate_gzip_compress_x64(compressor, inBuffer, (UIntPtr)in_nbytes, outBuffer, (UIntPtr)out_nbytes_avail));
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_gzip_compress")]
        private static extern UIntPtr libdeflate_gzip_compress_x86(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_gzip_compress")]
        private static extern UIntPtr libdeflate_gzip_compress_x64(IntPtr compressor, IntPtr inBuffer, UIntPtr in_nbytes, IntPtr outBuffer, UIntPtr out_nbytes_avail);

        /// <summary>Get the worst-case upper bound on the number of bytes of compressed data that may be produced
        /// by compressing any buffer of length less than or equal to 'in_nbytes'.
        /// Mathematically, this bound will necessarily be a number greater than or equal to 'in_nbytes'.
        /// It may be an overestimate of the true upper bound.  
        /// As a special case, 'compressor' may be NULL.  This causes the bound to be taken across *any*
        /// libdeflate_compressor that could ever be allocated with this build of the library, with any options.
        /// 
        /// With block-based compression, it is usually preferable to separately store the uncompressed size of each
        /// block and to store any blocks that did not compress to less than their original size uncompressed.  In that
        /// scenario, there is no need to know the worst-case compressed size, since the maximum number of bytes of
        /// compressed data that may be used would always be one less than the input length.  You can just pass a
        /// buffer of that size to libdeflate_deflate_compress() and store the data uncompressed if libdeflate_deflate_compress()
        /// returns 0, indicating that the compressed data did not fit into the provided output buffer.</summary>
        /// <param name="compressor">Pointer to the compressor</param>
        /// <param name="in_nbytes">Length of data to compress</param>
        /// <returns>Worst-case upper bound on the number of bytes of compressed data that may be produced by compressing any buffer of length less than or equal to 'in_nbytes'.</returns>
        public static int LibdeflateZlibCompressBound(IntPtr compressor, int in_nbytes)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (int)libdeflate_zlib_compress_bound_x86(compressor, (UIntPtr)in_nbytes);
                case 8:
                    return (int)libdeflate_zlib_compress_bound_x64(compressor, (UIntPtr)in_nbytes);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_zlib_compress_bound")]
        private static extern UIntPtr libdeflate_zlib_compress_bound_x86(IntPtr compressor, UIntPtr in_nbytes);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_zlib_compress_bound")]
        private static extern UIntPtr libdeflate_zlib_compress_bound_x64(IntPtr compressor, UIntPtr in_nbytes);

        /// <summary>Returns a worst-case upper bound on the number of bytes of compressed data that may be produced
        /// by compressing any buffer of length less than or equal to 'in_nbytes'.
        /// Mathematically, this bound will necessarily be a number greater than or equal to 'in_nbytes'.
        /// It may be an overestimate of the true upper bound.  
        /// As a special case, 'compressor' may be NULL.  This causes the bound to be taken across *any*
        /// libdeflate_compressor that could ever be allocated with this build of the library, with any options.
        /// </summary>
        /// <param name="compressor">Pointer to the compressor</param>
        /// <param name="in_nbytes">Length of data to compress</param>
        /// <returns>Worst-case upper bound on the number of bytes of compressed data that may be produced by compressing any buffer of length less than or equal to 'in_nbytes'.</returns>
        public static int LibdeflateDeflateCompressBound(IntPtr compressor, int in_nbytes)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (int)libdeflate_deflate_compress_bound_x86(compressor, (UIntPtr)in_nbytes);
                case 8:
                    return (int)libdeflate_deflate_compress_bound_x64(compressor, (UIntPtr)in_nbytes);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_x86(IntPtr compressor, UIntPtr in_nbytes);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_deflate_compress_bound")]
        private static extern UIntPtr libdeflate_deflate_compress_bound_x64(IntPtr compressor, UIntPtr in_nbytes);

        /// <summary>Returns a worst-case upper bound on the number of bytes of compressed data that may be produced
        /// by compressing any buffer of length less than or equal to 'in_nbytes'.
        /// Mathematically, this bound will necessarily be a number greater than or equal to 'in_nbytes'.
        /// It may be an overestimate of the true upper bound.  
        /// As a special case, 'compressor' may be NULL.  This causes the bound to be taken across *any*
        /// libdeflate_compressor that could ever be allocated with this build of the library, with any options.
        /// 
        /// With block-based compression, it is usually preferable to separately store the uncompressed size of each
        /// block and to store any blocks that did not compress to less than their original size uncompressed.  In that
        /// scenario, there is no need to know the worst-case compressed size, since the maximum number of bytes of
        /// compressed data that may be used would always be one less than the input length.  You can just pass a
        /// buffer of that size to libdeflate_deflate_compress() and store the data uncompressed if libdeflate_deflate_compress()
        /// returns 0, indicating that the compressed data did not fit into the provided output buffer.</summary>
        /// <param name="compressor">Pointer to the compressor</param>
        /// <param name="in_nbytes">Length of data to compress</param>
        /// <returns>Worst-case upper bound on the number of bytes of compressed data that may be produced by compressing any buffer of length less than or equal to 'in_nbytes'.</returns>
        public static int LibdeflateGzipCompressBound(IntPtr compressor, int in_nbytes)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return (int)libdeflate_gzip_compress_bound_x86(compressor, (UIntPtr)in_nbytes);
                case 8:
                    return (int)libdeflate_gzip_compress_bound_x64(compressor, (UIntPtr)in_nbytes);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_gzip_compress_bound")]
        private static extern UIntPtr libdeflate_gzip_compress_bound_x86(IntPtr compressor, UIntPtr in_nbytes);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_gzip_compress_bound")]
        private static extern UIntPtr libdeflate_gzip_compress_bound_x64(IntPtr compressor, UIntPtr in_nbytes);

        /// <summary>Frees a compressor that was allocated with libdeflate_alloc_compressor()</summary>
        /// <param name="compressor">Pointer to the compressor</param>
        public static void LibdeflateFreeCompressor(IntPtr compressor)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    libdeflate_free_compressor_x86(compressor);
                    break;
                case 8:
                    libdeflate_free_compressor_x64(compressor);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_compressor")]
        private static extern void libdeflate_free_compressor_x86(IntPtr compressor);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_free_compressor")]
        private static extern void libdeflate_free_compressor_x64(IntPtr compressor);


        /* ========================================================================== */
        /*                                Checksums                                   */
        /* ========================================================================== */

        /// <summary>Updates a running CRC-32 checksum</summary>
        /// <param name="crc">Inial value of checksum. When starting a new checksum will be 0</param>
        /// <param name="buffer">Data to checksum</param>
        /// <param name="len">Length of data</param>
        /// <returns>The updated checksum</returns>
        public static UInt32 LibdeflateCrc32(UInt32 crc, IntPtr inBuffer, int len)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return libdeflate_crc32_x86(crc, inBuffer, (UIntPtr)len);
                case 8:
                    return libdeflate_crc32_x64(crc, inBuffer, (UIntPtr)len);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
        private static extern UInt32 libdeflate_crc32_x86(UInt32 crc, IntPtr inBuffer, UIntPtr len);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_crc32")]
        private static extern UInt32 libdeflate_crc32_x64(UInt32 crc, IntPtr inBuffer, UIntPtr len);

        /// <summary>updates a running Adler-32 checksum</summary>
        /// <param name="crc">Inial value of checksum. When starting a new checksum will be 1</param>
        /// <param name="buffer">Data to checksum</param>
        /// <param name="len">Length of data</param>
        /// <returns>The updated checksum</returns>
        public static UInt32 LibdeflateAdler32(UInt32 crc, IntPtr inBuffer, int len)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return libdeflate_adler32_x86(crc, inBuffer, (UIntPtr)len);
                case 8:
                    return libdeflate_adler32_x64(crc, inBuffer, (UIntPtr)len);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libdeflate_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_adler32")]
        private static extern UInt32 libdeflate_adler32_x86(UInt32 crc, IntPtr inBuffer, UIntPtr len);
        [DllImport("libdeflate_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "libdeflate_adler32")]
        private static extern UInt32 libdeflate_adler32_x64(UInt32 crc, IntPtr inBuffer, UIntPtr len);
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// LittleZip C#. (GPL) Jose M. Piñeiro
/// Version: 1.0.0.1 (Jun 18, 2017)
/// 
/// This file is Test for libdeflate wrapper
/// LittleZip can:
/// - Compress several files in a very little zip
/// - Compress in very little time
/// - Use very little code
/// - Very little learning for use
/// 
/// LittleZip can not:
/// - Create a large zip ( > 2.147.483.647 bytes)
/// - Store a large file ( > 2.147.483.647 bytes)
/// - Use little memory ( need two times the compresed file )
/// - Decompress one ZIP file. Use another zip program.
/// 
/// Use code from: http://github.com/jaime-olivares/zipstorer
/// Use library from: https://github.com/ebiggers/libdeflate
///////////////////////////////////////////////////////////////////////////////////////////////////////////// 
/// Compress Functions:
/// LittleZip Create(string _filename, string _comment)
/// - Create a new ZIP file. Optionally you can put a general comment.
/// 
/// LittleZip Create(Stream _stream, string _comment)
/// - Create a new ZIP stream. Optionally you can put a general comment.
/// 
/// LittleZip Open(string _filename, FileAccess _access)
/// - Open an existing ZIP file for append files
/// 
/// LittleZip Open(Stream _stream)
/// - Open an existing storage from stream
/// 
/// AddFile(string pathFilename, string filenameInZip, string comment)
/// - Add full contents of a file into the Zip storage. Optionally you can put a file comment.
/// 
/// AddBuffer(byte[] inBuffer, string filenameInZip, DateTime modifyTime, string comment = "")
/// - Add full contents of a array into the Zip storage. Optionally you can put a file comment.
/// 
/// Close()
/// -  Updates central directory and close the Zip storage. DO NOT FORGET THIS STEP OR THE ZIP WILL NOT BE VALID.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#if NET45
using System.Threading.Tasks;
#endif

namespace System.IO.Compression
{
#if NETSTANDARD
    /// <summary>
    /// Extension method for covering missing Close() method in .Net Standard
    /// </summary>
    public static class StreamExtension
    {
        public static void Close(this Stream stream)
        {
            stream.Dispose(); 
            GC.SuppressFinalize(stream);
        }
    }
#endif

    /// <summary>
    /// Unique class for compression/decompression file. Represents a Zip file.
    /// </summary>
    public class LittleZip : IDisposable
    {
        #region Private structs
        /// <summary>
        /// Compression method enumeration
        /// </summary>
        private enum Compression : ushort
        {
            /// <summary>Uncompressed storage</summary> 
            Store = 0,
            /// <summary>Deflate compression method</summary>
            Deflate = 8
        }
        #endregion

        /// <summary>
        /// Represents an entry in Zip file directory
        /// </summary>
        private struct ZipFileEntry
        {
            /// <summary>Compression method</summary>
            public Compression Method;
            /// <summary>Full path and filename as stored in Zip</summary>
            public string FilenameInZip;
            /// <summary>Original file size</summary>
            public uint FileSize;
            /// <summary>Compressed file size</summary>
            public uint CompressedSize;
            /// <summary>Offset of header information inside Zip storage</summary>
            public uint HeaderOffset;
            /// <summary>Offset of file inside Zip storage</summary>
            public uint FileOffset;
            /// <summary>Size of header information</summary>
            public uint HeaderSize;
            /// <summary>32-bit checksum of entire file</summary>
            public uint Crc32;
            /// <summary>Last modification time of file</summary>
            public DateTime ModifyTime;
            /// <summary>User comment for file</summary>
            public string Comment;
            /// <summary>True if UTF8 encoding for filename and comments, false if default (CP 437)</summary>
            public bool EncodeUTF8;

            /// <summary>Overriden method</summary>
            /// <returns>Filename in Zip</returns>
            public override string ToString()
            {
                return this.FilenameInZip;
            }
        }

        #region Public fields
        /// <summary>True if UTF8 encoding for filename and comments, false if default (CP 437)</summary>
        public bool EncodeUTF8 = false;
        #endregion

        #region Private fields
        // List of files to store
        private List<ZipFileEntry> Files = new List<ZipFileEntry>();
        // Filename of storage file
        private string FileName;
        // Stream object of storage file
        private Stream ZipFileStream;
        // General comment
        private string Comment = "";
        // Central dir image
        private byte[] CentralDirImage = null;
        // Existing files in zip
        private ushort ExistingFiles = 0;
        // Inform of archive blocking the zip file. Null if not blocked.
        private string blocked = null;
        // Default filename encoder
        private static Encoding DefaultEncoding = Encoding.GetEncoding(437);
        #endregion

        #region Public methods
        /// <summary>
        /// Create a new ZIP file
        /// </summary>
        /// <param name="pathFilename">Full path of Zip file to create</param>
        /// <param name="zipComment">General comment for Zip file</param>
        /// <returns>LittleZip object</returns>
        public static LittleZip Create(string pathFilename, string zipComment = "")
        {
            try
            {
                Stream stream = new FileStream(pathFilename, FileMode.Create, FileAccess.ReadWrite);

                LittleZip zip = Create(stream, zipComment);
                zip.Comment = zipComment;
                zip.FileName = pathFilename;

                return zip;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.Create"); }
        }

        /// <summary>
        /// Create a new zip storage in a stream
        /// </summary>
        /// <param name="zipStream">Stream Zip to create</param>
        /// <param name="_comment">General comment for Zip file</param>
        /// <returns>LittleZip object</returns>
        public static LittleZip Create(Stream zipStream, string _comment = "")
        {
            try
            {
                LittleZip zip = new LittleZip();
                zip.Comment = _comment;
                zip.ZipFileStream = zipStream;

                return zip;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.Create"); }
        }

        /// <summary>
        /// Open an existing ZIP file
        /// </summary>
        /// <param name="zipFilename">Full path of Zip file to open</param>
        /// <param name="_access">File access mode as used in FileStream constructor</param>
        /// <returns>LittleZip object</returns>
        public static LittleZip Open(string zipFilename)
        {
            try
            {
                Stream stream = (Stream)new FileStream(zipFilename, FileMode.Open);

                LittleZip zip = Open(stream);
                zip.FileName = zipFilename;

                return zip;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.Open"); }
        }

        /// <summary>
        /// Open an existing storage from stream
        /// </summary>
        /// <param name="zipStream">Already opened stream with zip contents</param>
        /// <param name="_access">File access mode for stream operations</param>
        /// <returns>LittleZip object</returns>
        public static LittleZip Open(Stream zipStream)
        {
            try
            {
                if (!zipStream.CanSeek)
                    throw new InvalidOperationException("Stream cannot seek");

                LittleZip zip = new LittleZip();
                zip.ZipFileStream = zipStream;
                zip.ReadFileInfo();

                return zip;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.Open"); }
        }

        /// <summary>
        /// Add full contents of a file into the Zip storage
        /// </summary>
        /// <param name="_pathname">Full path of file to add to Zip storage</param>
        /// <param name="filenameInZip">Filename and path as desired in Zip directory</param>
        /// <param name="fileComment">Comment for stored file</param>   
        public void AddFile(string pathFilename, string filenameInZip, string fileComment = "")
        {
            try
            {
                //Check the maximun file size
                if (pathFilename.Length > Int32.MaxValue - 56)
                    throw new Exception("File is too large to be processed by this program. Maximum size " + (Int32.MaxValue - 56));

                //Read the imput file
                byte[] inBuffer = File.ReadAllBytes(pathFilename);
                DateTime modifyTime = File.GetLastWriteTime(pathFilename);

                //Add inBuffer to Zip
                AddBuffer(inBuffer, filenameInZip, modifyTime, fileComment);
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.AddFile"); }
        }

        /// <summary>
        /// Add full contents of a array into the Zip storage
        /// </summary>
        /// <param name="inBuffer">Data to store in Zip</param>
        /// <param name="filenameInZip">Filename and path as desired in Zip directory</param>>
        /// <param name="modifyTime">Modify time for stored file</param>>
        /// <param name="fileComment">Comment for stored file</param>   
        public void AddBuffer(byte[] inBuffer, string filenameInZip, DateTime modifyTime, string fileComment = "")
        {
            try
            {
                byte[] outBuffer;

                // Prepare the ZipFileEntry
                ZipFileEntry zfe = new ZipFileEntry();
                zfe.FileSize = (uint)inBuffer.Length;
                zfe.EncodeUTF8 = this.EncodeUTF8;
                zfe.FilenameInZip = NormalizedFilename(filenameInZip);
                zfe.Comment = fileComment;
                zfe.ModifyTime = modifyTime;
                zfe.Method = Compression.Deflate;

                // Deflate the Source and get ZipFileEntry data
                Libdeflate.Deflate(inBuffer, out outBuffer, out zfe.CompressedSize, out zfe.Crc32);
                //LibZopfli.Deflate(inBuffer, out outBuffer, out zfe.CompressedSize, out zfe.Crc32);

                // If not reduced the size, use the original data.
                if (zfe.FileSize <= zfe.CompressedSize)
                {
                    zfe.Method = Compression.Store;
                    zfe.CompressedSize = zfe.FileSize;
                }

                //Wait for idle ZipFile stream
                while (blocked != filenameInZip)
                {
                    if (blocked == null)
                        blocked = filenameInZip;
                    Thread.Sleep(5);
                    Application.DoEvents();
                }

                // Write local header
                zfe.HeaderOffset = (uint)this.ZipFileStream.Position;  // offset within file of the start of this local record
                WriteLocalHeader(ref zfe);

                // Write deflate data (or original data if can´t deflate) to zip
                if (zfe.Method == Compression.Deflate)
                {
                    this.ZipFileStream.Write(outBuffer, 0, outBuffer.Length);
                }
                else
                {
                    this.ZipFileStream.Write(inBuffer, 0, inBuffer.Length);
                }

                //Add file in the Zip Directory struct
                Files.Add(zfe);

                //unblock zip file
                blocked = null;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.AddBuffer"); }
        }

        /// <summary>
        /// Updates central directory and close the Zip storage
        /// </summary>
        /// <remarks>This is a required step, unless automatic dispose is used</remarks>
        public void Close()
        {
            try
            {
                uint centralOffset = (uint)this.ZipFileStream.Position;
                uint centralSize = 0;

                if (this.CentralDirImage != null)
                    this.ZipFileStream.Write(CentralDirImage, 0, CentralDirImage.Length);

                for (int i = 0; i < Files.Count; i++)
                {
                    long pos = this.ZipFileStream.Position;
                    this.WriteCentralDirRecord(Files[i]);
                    centralSize += (uint)(this.ZipFileStream.Position - pos);
                }

                if (this.CentralDirImage != null)
                    this.WriteEndRecord(centralSize + (uint)CentralDirImage.Length, centralOffset);
                else
                    this.WriteEndRecord(centralSize, centralOffset);

                if (this.ZipFileStream != null)
                {
                    this.ZipFileStream.Flush();
                    this.ZipFileStream.Dispose();
                    this.ZipFileStream = null;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.Close"); }
        }
        #endregion

        #region Private methods
        /* Local file header:
            local file header signature     4 bytes  (0x504b0304)
            version needed to extract       2 bytes  (20 in this implementation)
            general purpose bit flag        2 bytes  (Only implement encodin: 437 page or UTF8)  
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes  (Ever 0)

            filename (variable size)
            extra field (variable size). This implementation not use extra field for minimize ZIP size.
        */
        private void WriteLocalHeader(ref ZipFileEntry _zfe)
        {
            try
            {
                //Encode filename
                Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
                byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);

                //Write local header
                this.ZipFileStream.Write(new byte[] { 0x50, 0x4b, 0x03, 0x04, 0x14, 0x00 }, 0, 6);              // local file header signature + version needed to extract
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 0x0800 : 0)), 0, 2);  // filename encoding 
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);                     // zipping method
                this.ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);      // zipping date and time
                this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);                              // CRC32
                this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);                     // Compressed size
                this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);                           // Uncompressed size
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2);          // filename length
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                               // extra length = 0
                this.ZipFileStream.Write(encodedFilename, 0, encodedFilename.Length);

                // Add header size to the Zip Directory struct
                _zfe.HeaderSize = (uint)(30 + encodedFilename.Length);
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.WriteLocalHeader"); }
        }

        /* Central directory's File header:
            central file header signature   4 bytes  (0x504b0102)
            version made by                 2 bytes  
            version needed to extract       2 bytes  (Version 20 for maximun compatibility)
            general purpose bit flag        2 bytes
            compression method              2 bytes
            last mod file time              2 bytes
            last mod file date              2 bytes
            crc-32                          4 bytes
            compressed size                 4 bytes
            uncompressed size               4 bytes
            filename length                 2 bytes
            extra field length              2 bytes
            file comment length             2 bytes
            disk number start               2 bytes
            internal file attributes        2 bytes
            external file attributes        4 bytes
            relative offset of local header 4 bytes

            filename (variable size)
            extra field (variable size)
            file comment (variable size)
        */
        private void WriteCentralDirRecord(ZipFileEntry _zfe)
        {
            try
            {
                Encoding encoder = _zfe.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
                byte[] encodedFilename = encoder.GetBytes(_zfe.FilenameInZip);
                byte[] encodedComment = encoder.GetBytes(_zfe.Comment);

                this.ZipFileStream.Write(new byte[] { 0x50, 0x4b, 0x01, 0x02, 0x17, 0x0B, 0x14, 0x00 }, 0, 8);  // central file header signature + version made by + version needed to extract
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 0x0800 : 0)), 0, 2);  // filename and comment encoding 
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);                     // zipping method
                this.ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);      // zipping date and time
                this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);                              // file CRC
                this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);                     // compressed file size
                this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);                           // uncompressed file size
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedFilename.Length), 0, 2);          // length of Filename in zip
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                               // extra length = 0
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                               // disk=0
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                               // file type: binary
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                               // Internal file attributes
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)0x8100), 0, 2);                          // External file attributes (normal/readable)
                this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);                       // Offset of header
                this.ZipFileStream.Write(encodedFilename, 0, encodedFilename.Length);                           // Filename in zip
                this.ZipFileStream.Write(encodedComment, 0, encodedComment.Length);                             // Comment of file
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.WriteCentralDirRecord"); }
        }

        /* End of central dir record:
            end of central dir signature    4 bytes  (0x06054b50)
            number of this disk             2 bytes
            number of the disk with the
            start of the central directory  2 bytes
            total number of entries in
            the central dir on this disk    2 bytes
            total number of entries in
            the central dir                 2 bytes
            size of the central directory   4 bytes
            offset of start of central
            directory with respect to
            the starting disk number        4 bytes
            zipfile comment length          2 bytes
            zipfile comment (variable size)
        */
        private void WriteEndRecord(uint _size, uint _offset)
        {
            try
            {
                Encoding encoder = this.EncodeUTF8 ? Encoding.UTF8 : DefaultEncoding;
                byte[] encodedComment = encoder.GetBytes(this.Comment);

                this.ZipFileStream.Write(new byte[] { 80, 75, 5, 6, 0, 0, 0, 0 }, 0, 8);
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count + ExistingFiles), 0, 2);
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count + ExistingFiles), 0, 2);
                this.ZipFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
                this.ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
                this.ZipFileStream.Write(BitConverter.GetBytes((ushort)encodedComment.Length), 0, 2);
                this.ZipFileStream.Write(encodedComment, 0, encodedComment.Length);
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.WriteEndRecord"); }
        }

        /* DOS Date and time:
            MS-DOS date. The date is a packed value with the following format. Bits Description 
                0-4 Day of the month (131) 
                5-8 Month (1 = January, 2 = February, and so on) 
                9-15 Year offset from 1980 (add 1980 to get actual year) 
            MS-DOS time. The time is a packed value with the following format. Bits Description 
                0-4 Second divided by 2 
                5-10 Minute (059) 
                11-15 Hour (023 on a 24-hour clock) 
        */
        private uint DateTimeToDosTime(DateTime _dt)
        {
            return (uint)(
                (_dt.Second / 2) | (_dt.Minute << 5) | (_dt.Hour << 11) |
                (_dt.Day << 16) | (_dt.Month << 21) | ((_dt.Year - 1980) << 25));
        }

        // Replaces backslashes with slashes to store in zip header. Remove unit letter
        private string NormalizedFilename(string _filename)
        {
            try
            {
                string filename = _filename.Replace('\\', '/');

                int pos = filename.IndexOf(':');
                if (pos >= 0)
                    filename = filename.Remove(0, pos + 1);

                return filename.Trim('/');
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.NormalizedFilename"); }
        }

        /// <summary>
        /// Reads the end-of-central-directory record
        /// </summary>
        /// <returns></returns>
        private void ReadFileInfo()
        {
            if (this.ZipFileStream.Length < 22)
                throw new System.IO.InvalidDataException("Invalid ZIP file");

            try
            {
                this.ZipFileStream.Seek(-17, SeekOrigin.End);
                BinaryReader br = new BinaryReader(this.ZipFileStream);
                do
                {
                    this.ZipFileStream.Seek(-5, SeekOrigin.Current);
                    UInt32 sig = br.ReadUInt32();
                    if (sig == 0x06054b50)
                    {
                        this.ZipFileStream.Seek(6, SeekOrigin.Current);

                        UInt16 entries = br.ReadUInt16();
                        Int32 centralSize = br.ReadInt32();
                        UInt32 centralDirOffset = br.ReadUInt32();
                        UInt16 commentSize = br.ReadUInt16();

                        // check if comment field is the very last data in file
                        if (this.ZipFileStream.Position + commentSize != this.ZipFileStream.Length)
                            throw new System.IO.InvalidDataException("Invalid ZIP file");

                        // Copy entire central directory to a memory buffer
                        this.ExistingFiles = entries;
                        this.CentralDirImage = new byte[centralSize];
                        this.ZipFileStream.Seek(centralDirOffset, SeekOrigin.Begin);
                        this.ZipFileStream.Read(this.CentralDirImage, 0, centralSize);

                        // Leave the pointer at the begining of central dir, to append new files
                        this.ZipFileStream.Seek(centralDirOffset, SeekOrigin.Begin);
                        return;
                    }
                } while (this.ZipFileStream.Position > 0);
                throw new System.IO.InvalidDataException("Invalid ZIP file");
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn LittleZip.ReadFileInfo"); }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Closes the Zip file stream
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
        #endregion
    }

    class Libdeflate
    {
        public static void Deflate(byte[] inBuffer, out byte[] outBuffer, out uint compresedSize, out uint crc32)
        {
            IntPtr ptrInBuffer = IntPtr.Zero;
            IntPtr ptrOutBuffer = IntPtr.Zero;
            try
            {
                ptrInBuffer = Marshal.AllocHGlobal(inBuffer.Length);
                Marshal.Copy(inBuffer, 0, ptrInBuffer, inBuffer.Length);

                crc32 = UnsafeNativeMethods.LibdeflateCrc32(0, ptrInBuffer, inBuffer.Length);

                IntPtr compressor = UnsafeNativeMethods.LibdeflateAllocCompressor(12);
                if (compressor == null)
                    throw new Exception("Out of memory");

                int maxCompresedSize = UnsafeNativeMethods.LibdeflateDeflateCompressBound(compressor, inBuffer.Length);
                outBuffer = new byte[(int)(maxCompresedSize)];
                ptrOutBuffer = Marshal.AllocHGlobal(maxCompresedSize);

                compresedSize = (uint)UnsafeNativeMethods.LibdeflateDeflateCompress(compressor, ptrInBuffer, inBuffer.Length, ptrOutBuffer, (int)maxCompresedSize);
                UnsafeNativeMethods.LibdeflateFreeCompressor(compressor);
                outBuffer = new byte[compresedSize];
                Marshal.Copy(ptrOutBuffer, outBuffer, 0, (int)compresedSize);
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn Libdeflate.Deflate"); }
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


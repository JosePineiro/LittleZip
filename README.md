# LittleZip. A libdeflate and Zopfli wrapper
Wrapper for libdeflate  and Zopfli in C#. Implement ZIP archive create an append.

Exposes libdeflate and Zopfli compress and CRC32 API functions.

The wapper is in safe managed code in one class. No need external dll except libdeflate_x86.dll, libdeflate_x64.dll (included v1.08) . For use Zopfli need zopfli_x86.dll and zopfli_x64.dll. The wrapper work in 32, 64 bit or ANY (auto switch to the appropriate library).

The code is full commented and include simple example for using the wrapper.
## Compress file functions
Open an existing ZIP file for append files or, if test.zip not exit, create a new ZIP file. Optionally you can put a general comment.
```C#
LittleZip zip = LittleZip("test.zip", "Zip comment");
```

Add full contents of a file into the Zip storage. Optionally you can put a file comment
```C#
zip.AddFile("c:\\directory\\file1.txt", "file1.txt", "This is the comment for file1")
```

Add full contents of one array into the Zip storage. Optionally you can put a file comment.
```C#
byte[] buffer = File.ReadAllBytes("c:\\directory\\file1.txt");
zip.AddFile(buffer, "file1.txt", "This is the comment for file1")
```

Close zip file. This function is automatic call when dispose LittleZip
```C#
zip.Close()
```

## Additional stream functions
Open an existing ZIP stream for append files or, if stream is void, create a new ZIP file in stream. Optionally you can put a general comment.
```C#
LittleZip zip = LittleZip(stream, "Zip comment");
```
## Important note
Compression levels above 12, use ZOPLI library. It is very slow.

## Purpose
LittleZip can:
- Compress several files in a very little zip
- Compress in very little time
- Use very little code
- Very little learning for use

LittleZip can not:
- Create a large zip (> 2.147.483.647 bytes)
- Store a large file (> 2.147.483.647 bytes)
- Use little memory (need two times the compressed file)
- Decompress one ZIP file, erase files in zip, update files in zip, test files in zip. For this purpose, use https://github.com/JosePineiro/LittleUnZip

## Credit
- https://github.com/ebiggers/libdeflate
- https://github.com/drivehappy/libzopfli-sharp

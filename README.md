# LittleZip. A libdeflate-wrapper
Wrapper for libdeflate in C#. Implement ZIP archive create an apend.

Exposes libdeflate compress and CRC32 API functions.

The wapper is in safe managed code in one class. No need external dll except libdeflate_x86.dll and libdeflate_x64.dll (included v0.8). The wrapper work in 32, 64 bit or ANY (auto swith to the apropiate library).

The code is full comented and include simple example for using the wrapper.
## Compress file functions:
Open an existing ZIP file for append files or, if test.zip not exit, create a new ZIP file. Optionally you can put a general comment.
```C#
LittleZip zip = LittleZip("test.zip", "Zip comment");
```

Add full contents of a file into the Zip storage. Optionally you can put a file comment.
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

## Aditional stream functions:
Open an existing ZIP stream for append files or, if stream is void, create a new ZIP file in stream. Optionally you can put a general comment.
```C#
LittleZip zip = LittleZip(stream, "Zip comment");
```

## Use
LittleZip can:
- Compress several files in a very little zip
- Compress in very little time
- Use very little code
- Very little learning for use

LittleZip can not:
- Create a large zip (> 2.147.483.647 bytes)
- Store a large file (> 2.147.483.647 bytes)
- Use little memory (need two times the compresed file)
- Decompress one ZIP file, erase files in zip, update files in zip, test files in zip. For this purpose, use http://github.com/jaime-olivares/zipstorer

## Thanks
- https://github.com/ebiggers/libdeflate

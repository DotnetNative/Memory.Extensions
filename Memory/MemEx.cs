using System.Runtime.InteropServices;
using System;

namespace Memory;
public unsafe static class MemEx
{
    public static T* New<T>() where T : unmanaged => NewAlloc<T>();
    public static T* New<T>(T val) where T : unmanaged => NewAlloc(&val);

    public static T* NewAlloc<T>(T* from) where T : unmanaged
    {
        T* to = (T*)Marshal.AllocCoTaskMem(sizeof(T));
        *to = *from;
        return to;
    }

    public static T* NewAlloc<T>() where T : unmanaged => (T*)Marshal.AllocCoTaskMem(sizeof(T));

    public static byte* Alloc(int count) => (byte*)Marshal.AllocCoTaskMem(count);
    public static T* Alloc<T>(int count) where T : unmanaged => (T*)Marshal.AllocCoTaskMem(count * sizeof(T));

    public static T* AllocFrom<T>(T[] arr) where T : unmanaged
    {
        T* ptr = Alloc<T>(arr.Length);
        Copy(ptr, arr);
        return ptr;
    }

    public static void Free(void* ptr) => Marshal.FreeCoTaskMem((nint)ptr);
    public static void Free(params void*[] ptrs)
    {
        foreach (void* ptr in ptrs)
            Marshal.FreeCoTaskMem((nint)ptr);
    }
    public static void Free(nint addr) => Marshal.FreeCoTaskMem(addr);

    public static T* NULL<T>() where T : unmanaged => (T*)0;

    public static GCHandle Pin<T>(T[] arr) where T : unmanaged => GCHandle.Alloc(arr, GCHandleType.Pinned);

    public static void Copy(void* to, void* from, int len)
    {
        Buffer.MemoryCopy(from, to, len, len);
    }

    public static void Copy<T>(void* to, T[] from, int len) where T : unmanaged
    {
        fixed (void* fromPtr = from)
            Buffer.MemoryCopy(fromPtr, to, len, len);
    }

    public static void Copy<T>(void* to, T[] from) where T : unmanaged
    {
        int length = sizeof(T) * from.Length;
        fixed (void* fromPtr = from)
            Buffer.MemoryCopy(fromPtr, to, length, length);
    }

    public static void Copy<T, T2>(T[] to, T2[] from, int len) where T : unmanaged where T2 : unmanaged
    {
        fixed (void* fromPtr = from)
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(fromPtr, toPtr, len, len);
    }

    public static void Copy<T, T2>(T[] to, T2[] from) where T : unmanaged where T2 : unmanaged
    {
        int length = sizeof(T2) * from.Length;
        fixed (void* fromPtr = from)
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(fromPtr, toPtr, length, length);
    }

    public static void Copy<T>(T[] to, void* from, int len) where T : unmanaged
    {
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(from, toPtr, len, len);
    }

    public static void Copy<T>(T[] to, void* from) where T : unmanaged
    {
        int length = sizeof(T) * to.Length;
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(from, toPtr, length, length);
    }

    public static byte[] Read(void* ptr, int len)
    {
        byte[] bytes = new byte[len];
        fixed (byte* bytesPtr = bytes)
            Copy(bytesPtr, ptr, len);
        return bytes;
    }

    public static void WriteStruct<T>(T str, void* ptr) where T : unmanaged => Copy(&str, ptr, sizeof(T));
    public static void WriteStruct<T>(T str, byte[] arr) where T : unmanaged
    {
        fixed (byte* ptr = arr)
            WriteStruct(str, ptr);
    }

    public static byte[] ReadStruct<T>(T str) where T : unmanaged => Read(&str, sizeof(T));
}
using System.Runtime.InteropServices;

using MethImpl = System.Runtime.CompilerServices.MethodImplAttribute;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Memory;
public unsafe static class MemEx
{
    [MethImpl(AggressiveInlining)]
    public static T* New<T>() where T : unmanaged => NewAlloc<T>();
    [MethImpl(AggressiveInlining)]
    public static T* New<T>(T val) where T : unmanaged => NewAlloc(&val);

    [MethImpl(AggressiveInlining)]
    public static T* NewAlloc<T>(T* from) where T : unmanaged
    {
        var to = (T*)Marshal.AllocCoTaskMem(sizeof(T));
        *to = *from;
        return to;
    }

    [MethImpl(AggressiveInlining)]
    public static T* NewAlloc<T>() where T : unmanaged => (T*)Marshal.AllocCoTaskMem(sizeof(T));

    [MethImpl(AggressiveInlining)]
    public static byte* Alloc(int count) => (byte*)Marshal.AllocCoTaskMem(count);
    [MethImpl(AggressiveInlining)]
    public static T* Alloc<T>(int count) where T : unmanaged => (T*)Marshal.AllocCoTaskMem(count * sizeof(T));

    [MethImpl(AggressiveInlining)]
    public static T* AllocFrom<T>(T[] arr) where T : unmanaged
    {
        var ptr = Alloc<T>(arr.Length);
        Copy(ptr, arr);
        return ptr;
    }

    [MethImpl(AggressiveInlining)]
    public static void Free(void* ptr) => Marshal.FreeCoTaskMem((nint)ptr);
    [MethImpl(AggressiveInlining)]
    public static void Free(params void*[] ptrs)
    {
        foreach (var ptr in ptrs)
            Marshal.FreeCoTaskMem((nint)ptr);
    }
    [MethImpl(AggressiveInlining)]
    public static void Free(nint addr) => Marshal.FreeCoTaskMem(addr);

    [MethImpl(AggressiveInlining)]
    public static T* NULL<T>() where T : unmanaged => (T*)0;

    [MethImpl(AggressiveInlining)]
    public static GCHandle Pin<T>(T[] arr) where T : unmanaged => GCHandle.Alloc(arr, GCHandleType.Pinned);

    [MethImpl(AggressiveInlining)]
    public static void Copy(void* to, void* from, int len) => Buffer.MemoryCopy(from, to, len, len);

    [MethImpl(AggressiveInlining)]
    public static void Copy<T>(void* to, T[] from) where T : unmanaged
    {
        int length = sizeof(T) * from.Length;
        fixed (void* fromPtr = from)
            Buffer.MemoryCopy(fromPtr, to, length, length);
    }

    [MethImpl(AggressiveInlining)]
    public static void Copy<T, T2>(T[] to, T2[] from, int len) where T : unmanaged where T2 : unmanaged
    {
        fixed (void* fromPtr = from)
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(fromPtr, toPtr, len, len);
    }

    [MethImpl(AggressiveInlining)]
    public static void Copy<T, T2>(T[] to, T2[] from) where T : unmanaged where T2 : unmanaged
    {
        int length = sizeof(T2) * from.Length;
        fixed (void* fromPtr = from)
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(fromPtr, toPtr, length, length);
    }

    [MethImpl(AggressiveInlining)]
    public static void Copy<T>(T[] to, void* from, int len) where T : unmanaged
    {
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(from, toPtr, len, len);
    }

    [MethImpl(AggressiveInlining)]
    public static void Copy<T>(T[] to, void* from) where T : unmanaged
    {
        int length = sizeof(T) * to.Length;
        fixed (void* toPtr = to)
            Buffer.MemoryCopy(from, toPtr, length, length);
    }

    [MethImpl(AggressiveInlining)]
    public static byte[] Read(void* ptr, int len)
    {
        var bytes = new byte[len];
        Copy(bytes, ptr);
        return bytes;
    }

    [MethImpl(AggressiveInlining)]
    public static T[] Read<T>(void* ptr, int len) where T : unmanaged
    {
        var bytes = new T[len];
        Copy(bytes, ptr);
        return bytes;
    }

    [MethImpl(AggressiveInlining)]
    public static void WriteStruct<T>(T str, void* ptr) where T : unmanaged => Copy(&str, ptr, sizeof(T));
    [MethImpl(AggressiveInlining)]
    public static void WriteStruct<T>(T str, byte[] arr) where T : unmanaged
    {
        fixed (byte* ptr = arr)
            WriteStruct(str, ptr);
    }

    [MethImpl(AggressiveInlining)]
    public static byte[] ReadStruct<T>(T str) where T : unmanaged => Read<byte>(&str, sizeof(T));

    [MethImpl(AggressiveInlining)]
    public static bool Compare(byte* ptr, byte* with, int len)
    {
        for (int i = 0; i < len; i++)
            if (ptr[i] != with[i])
                return false;

        return true;
    }

    [MethImpl(AggressiveInlining)]
    public static bool Compare(void* ptr, void* with, int len) => Compare((byte*)ptr, (byte*)with, len);

    [MethImpl(AggressiveInlining)]
    public static bool Compare(void* ptr, byte[] with, int len)
    {
        fixed (void* withPtr = with)
            return Compare(ptr, withPtr, len);
    }

    [MethImpl(AggressiveInlining)]
    public static bool Compare(byte[] arr, byte[] with, int len)
    {
        fixed (void* withPtr = with)
        fixed (void* ptr = arr)
            return Compare(ptr, withPtr, len);
    }

    [MethImpl(AggressiveInlining)]
    public static bool Compare(byte[] arr, byte* with, int len)
    {
        fixed (void* ptr = arr)
            return Compare(ptr, with, len);
    }
}
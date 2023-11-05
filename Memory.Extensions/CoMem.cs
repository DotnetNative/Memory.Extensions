using System.Runtime.InteropServices;

namespace Memory;
public sealed unsafe class CoMem : IDisposable
{
    public CoMem(string str, CoStrType charSet = CoStrType.Utf8)
        => Alloc(str, charSet);

    void Alloc(string str, CoStrType charSet)
    {
        if (charSet == CoStrType.Utf8)
            Ptr = (byte*)Marshal.StringToCoTaskMemUTF8(str);
        else if (charSet == CoStrType.Ansi)
            Ptr = (byte*)Marshal.StringToCoTaskMemAnsi(str);
        else if (charSet == CoStrType.Auto)
            Ptr = (byte*)Marshal.StringToCoTaskMemAuto(str);
        else Ptr = (byte*)Marshal.StringToCoTaskMemUni(str);
    }

    public byte* Ptr;
    public char* CharPtr => (char*)Ptr;

    public static explicit operator void*(CoMem co) => co.Ptr;
    public static explicit operator byte*(CoMem co) => co.Ptr;
    public static explicit operator nint(CoMem co) => (nint)co.Ptr;

    #region Dispose
    public void MarkAsDisposed() => isDisposed = true;

    bool isDisposed;
    public void Dispose()
    {
        if (isDisposed)
            return;
        isDisposed = true;

        Free(Ptr);
        GC.SuppressFinalize(this);
    }

    ~CoMem() => Dispose();
    #endregion
}

internal sealed unsafe class CoMem<T> : IDisposable where T : unmanaged
{
    public CoMem(T obj) => Ptr = NewAlloc(obj);

    public CoMem(T[] arr) => Ptr = AllocFrom(arr);

    public T* Ptr;

    public static explicit operator void*(CoMem<T> co) => co.Ptr;
    public static explicit operator T*(CoMem<T> co) => co.Ptr;
    public static explicit operator nint(CoMem<T> co) => (nint)co.Ptr;

    #region Dispose
    bool isDisposed;
    public void Dispose()
    {
        if (isDisposed)
            return;
        isDisposed = true;

        Free(Ptr);
        GC.SuppressFinalize(this);
    }

    ~CoMem() => Dispose();
    #endregion
}
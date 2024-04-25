namespace Memory;
public sealed unsafe class CoMem : IDisposable
{
    public CoMem(string str, CoStrType charSet = CoStrType.Utf8)
        => Alloc(str, charSet);

    void Alloc(string str, CoStrType charSet)
    {
        Ptr = (byte*)(charSet switch
        {
            CoStrType.Utf8 => Marshal.StringToCoTaskMemUTF8(str),
            CoStrType.Ansi => Marshal.StringToCoTaskMemAnsi(str),
            CoStrType.Auto => Marshal.StringToCoTaskMemAuto(str),
            CoStrType.Uni => Marshal.StringToCoTaskMemUni(str)
        });
    }

    public byte* Ptr;
    public char* CharPtr => (char*)Ptr;

    public static implicit operator void*(CoMem co) => co.Ptr;
    public static implicit operator byte*(CoMem co) => co.Ptr;
    public static implicit operator nint(CoMem co) => (nint)co.Ptr;
    public static implicit operator pointer(CoMem co) => co.Ptr;

    #region IDisposable
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

public sealed unsafe class CoMem<T> : IDisposable where T : unmanaged
{
    public CoMem(T obj) => Ptr = NewAlloc(obj);
    public CoMem(T[] arr) => Ptr = AllocFrom(arr);

    public T* Ptr;

    public static implicit operator void*(CoMem<T> co) => co.Ptr;
    public static implicit operator T*(CoMem<T> co) => co.Ptr;
    public static implicit operator nint(CoMem<T> co) => (nint)co.Ptr;
    public static implicit operator pointer(CoMem<T> co) => co.Ptr;

    #region IDisposable
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
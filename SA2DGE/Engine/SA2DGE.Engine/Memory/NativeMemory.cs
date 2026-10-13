using System.Runtime.InteropServices;

namespace SA2DGE.Engine.Memory;

public static unsafe class NativeMemory
{
    public static nint Allocate(
        nuint byteCount)
    {
        if (byteCount == 0)
        {
            return nint.Zero;
        }

        return (nint)System.Runtime.InteropServices.NativeMemory.Alloc(
            byteCount);
    }

    public static nint AllocateZeroed(
        nuint byteCount)
    {
        if (byteCount == 0)
        {
            return nint.Zero;
        }

        return (nint)System.Runtime.InteropServices.NativeMemory.AllocZeroed(
            byteCount);
    }

    public static nint Reallocate(
        nint pointer,
        nuint byteCount)
    {
        if (byteCount == 0)
        {
            Free(pointer);
            return nint.Zero;
        }

        return (nint)System.Runtime.InteropServices.NativeMemory.Realloc(
            (void*)pointer,
            byteCount);
    }

    public static void Free(
        nint pointer)
    {
        if (pointer == nint.Zero)
        {
            return;
        }

        System.Runtime.InteropServices.NativeMemory.Free(
            (void*)pointer);
    }

    public static void Copy(
        nint source,
        nint destination,
        nuint byteCount)
    {
        if (byteCount == 0)
        {
            return;
        }

        if (source == nint.Zero)
        {
            throw new ArgumentNullException(
                nameof(source));
        }

        if (destination == nint.Zero)
        {
            throw new ArgumentNullException(
                nameof(destination));
        }

        Buffer.MemoryCopy(
            (void*)source,
            (void*)destination,
            byteCount,
            byteCount);
    }

    public static void Clear(
        nint pointer,
        nuint byteCount)
    {
        if (byteCount == 0)
        {
            return;
        }

        if (pointer == nint.Zero)
        {
            throw new ArgumentNullException(
                nameof(pointer));
        }

        System.Runtime.InteropServices.NativeMemory.Clear(
            (void*)pointer,
            byteCount);
    }

    public static Span<T> AsSpan<T>(
        nint pointer,
        int elementCount)
        where T : unmanaged
    {
        if (elementCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(elementCount));
        }

        if (elementCount == 0)
        {
            return Span<T>.Empty;
        }

        if (pointer == nint.Zero)
        {
            throw new ArgumentNullException(
                nameof(pointer));
        }

        return new Span<T>(
            (void*)pointer,
            elementCount);
    }

    public static ref T AsRef<T>(
        nint pointer)
        where T : unmanaged
    {
        if (pointer == nint.Zero)
        {
            throw new ArgumentNullException(
                nameof(pointer));
        }
        
        return ref *(T*)pointer;
    }

    public static nint GetFunctionPointer(
        Delegate callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        return Marshal.GetFunctionPointerForDelegate(
            callback);
    }

    public static void Write<T>(
        nint destination,
        ReadOnlySpan<T> source)
        where T : unmanaged
    {
        if (source.IsEmpty)
        {
            return;
        }

        if (destination == nint.Zero)
        {
            throw new ArgumentNullException(
                nameof(destination));
        }

        Span<T> destinationSpan =
            AsSpan<T>(
                destination,
                source.Length);

        source.CopyTo(destinationSpan);
    }

    public static void Read<T>(
        nint source,
        Span<T> destination)
        where T : unmanaged
    {
        if (destination.IsEmpty)
        {
            return;
        }

        if (source == nint.Zero)
        {
            throw new ArgumentNullException(
                nameof(source));
        }

        Span<T> sourceSpan =
            AsSpan<T>(
                source,
                destination.Length);

        sourceSpan.CopyTo(destination);
    }
}
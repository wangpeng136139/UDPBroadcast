using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 小端
/// </summary>
public static class SpanByteExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt(this in ReadOnlySequence<byte> byteSequence)
    {
        unsafe
        {
            Span<byte> span = stackalloc byte[4];
            byteSequence.Slice(0, 4).CopyTo(span);
            return span.ReadInt();
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadShort(this in ReadOnlySequence<byte> byteSequence)
    {
        unsafe
        {
            Span<byte> span = stackalloc byte[2];
            byteSequence.Slice(0, 2).CopyTo(span);
            return span.ReadShort();
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ReadBool(this in ReadOnlySequence<byte> byteSequence)
    {
        unsafe
        {
            Span<byte> span = stackalloc byte[1];
            byteSequence.Slice(0, 1).CopyTo(span);
            return span[0] > 0;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="num"></param>
    /// <param name="span"></param>
    /// <returns>offset</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteTo(this int num, Span<byte> span)
    {
        BinaryPrimitives.WriteInt32BigEndian(span, num);
        return 4;
    }

    /// <summary>
    /// 写入一个int
    /// </summary>
    /// <param name="span"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write(this Span<byte> span, int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        return 4;
    }


    /// <summary>
    /// 写入一个short
    /// </summary>
    /// <param name="span"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Write(this Span<byte> span, short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        return 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteTo(this ushort num, Span<byte> span)
    {
        BinaryPrimitives.WriteUInt16BigEndian(span, num);
        return 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteTo(this short num, Span<byte> span)
    {
        BinaryPrimitives.WriteInt16BigEndian(span, num);
        return 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteTo(this long num, Span<byte> span)
    {
        BinaryPrimitives.WriteInt64BigEndian(span, num);
        return 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt(this ReadOnlySpan<byte> span)
        => BinaryPrimitives.ReadInt32BigEndian(span);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUshort(this ReadOnlySpan<byte> span)
        => BinaryPrimitives.ReadUInt16BigEndian(span);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadShort(this ReadOnlySpan<byte> span)
        => BinaryPrimitives.ReadInt16BigEndian(span);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadLong(this ReadOnlySpan<byte> span)
        => BinaryPrimitives.ReadInt64BigEndian(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt(this Span<byte> span)
        => BinaryPrimitives.ReadInt32BigEndian(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUshort(this Span<byte> span)
        => BinaryPrimitives.ReadUInt16BigEndian(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadShort(this Span<byte> span)
        => BinaryPrimitives.ReadInt16BigEndian(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadLong(this Span<byte> span)
        => BinaryPrimitives.ReadInt64BigEndian(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt(this Memory<byte> span)
        => BinaryPrimitives.ReadInt32BigEndian(span.Span);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUshort(this Memory<byte> span)
        => BinaryPrimitives.ReadUInt16BigEndian(span.Span);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadShort(this Memory<byte> span)
        => BinaryPrimitives.ReadInt16BigEndian(span.Span);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadLong(this Memory<byte> span)
        => BinaryPrimitives.ReadInt64BigEndian(span.Span);


    /// <summary>
    /// todo 优化alloc
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid ReadGuid(this ReadOnlySpan<byte> span)
    {
        if (span.Length < 16)
        {
            return default;
        }

        byte[] temp = new byte[16];
        span.Slice(0, 16).CopyTo(temp);
        return new Guid(temp);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid ReadGuid(this Span<byte> span)
    {
        return ReadGuid((ReadOnlySpan<byte>)span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteTo(this in Guid guid, Span<byte> target)
    {
        var temp = guid.ToByteArray();
        temp.AsSpan().CopyTo(target);
        return 16;
    }
}


/// <summary>
/// 线程安全ID生成器
/// </summary>
/// <typeparam name="T"></typeparam>
internal static class InterlockedID<T>
{
    static int id = 0;
    static readonly object locker = new object();
    public static int NewID(int min = 0)
    {
        lock (locker)
        {
            if (id < min)
            {
                id = min;
                return id;
            }

            return id++;
        }
    }
}

namespace Megumin.Remote
{
    /// <summary>
    /// 主动还是被动
    /// </summary>
    public enum ActiveOrPassive
    {
        /// <summary>
        /// 主动的
        /// </summary>
        Active,
        /// <summary>
        /// 被动的
        /// </summary>
        Passive,
    }

    /// <summary>
    /// 记录器
    /// </summary>
    [Obsolete("为每个实例赋值一个logger没有必要,能区分赋值,就能拿到调用的实例")]
    public interface IMeguminRemoteLogger
    {
        void Log(string error);
    }

    /// <summary>
    /// 事实上 无论UID是Int,long,还是string,都无法满足全部需求。当你需要其他类型是，请修改源码。
    /// </summary>
    public interface IRemoteUID<T>
    {
        /// <summary>
        /// 预留给用户使用的ID，（用户自己赋值ID，自己管理引用，框架不做处理）
        /// </summary>
        T UID { get; set; }
    }

    public class Utility
    {
    }

}

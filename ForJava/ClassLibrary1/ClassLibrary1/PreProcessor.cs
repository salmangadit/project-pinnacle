using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class UnsupportedImageFormatException : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
    /// </summary>
    public UnsupportedImageFormatException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public UnsupportedImageFormatException(string message) :
        base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="paramName">Name of the invalid parameter.</param>
    /// 
    public UnsupportedImageFormatException(string message, string paramName) :
        base(message, paramName) { }
}
/// <summary>
/// Invalid image properties exception.
/// </summary>
/// 
/// <remarks><para>The invalid image properties exception is thrown in the case when
/// user provides an image with certain properties, which are treated as invalid by
/// particular image processing routine. Another case when this exception is
/// thrown is the case when user tries to access some properties of an image (or
/// of a recently processed image by some routine), which are not valid for that image.</para>
/// </remarks>
/// 
public class InvalidImagePropertiesException : ArgumentException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
    /// </summary>
    public InvalidImagePropertiesException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public InvalidImagePropertiesException(string message) :
        base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="paramName">Name of the invalid parameter.</param>
    /// 
    public InvalidImagePropertiesException(string message, string paramName) :
        base(message, paramName) { }
}
// Some common exception messages
internal static class ExceptionMessage
{
    public const string ColorHistogramException = "Cannot access color histogram since the last processed image was grayscale.";
    public const string GrayHistogramException = "Cannot access gray histogram since the last processed image was color.";
}

[Serializable]
public struct Range
{
    private float min, max;

    /// <summary>
    /// Minimum value of the range.
    /// </summary>
    /// 
    /// <remarks><para>The property represents minimum value (left side limit) or the range -
    /// [<b>min</b>, max].</para></remarks>
    /// 
    public float Min
    {
        get { return min; }
        set { min = value; }
    }

    /// <summary>
    /// Maximum value of the range.
    /// </summary>
    /// 
    /// <remarks><para>The property represents maximum value (right side limit) or the range -
    /// [min, <b>max</b>].</para></remarks>
    /// 
    public float Max
    {
        get { return max; }
        set { max = value; }
    }

    /// <summary>
    /// Length of the range (deffirence between maximum and minimum values).
    /// </summary>
    public float Length
    {
        get { return max - min; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> structure.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// 
    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// Check if the specified value is inside of the range.
    /// </summary>
    /// 
    /// <param name="x">Value to check.</param>
    /// 
    /// <returns><b>True</b> if the specified value is inside of the range or
    /// <b>false</b> otherwise.</returns>
    /// 
    public bool IsInside(float x)
    {
        return ((x >= min) && (x <= max));
    }

    /// <summary>
    /// Check if the specified range is inside of the range.
    /// </summary>
    /// 
    /// <param name="range">Range to check.</param>
    /// 
    /// <returns><b>True</b> if the specified range is inside of the range or
    /// <b>false</b> otherwise.</returns>
    /// 
    public bool IsInside(Range range)
    {
        return ((IsInside(range.min)) && (IsInside(range.max)));
    }

    /// <summary>
    /// Check if the specified range overlaps with the range.
    /// </summary>
    /// 
    /// <param name="range">Range to check for overlapping.</param>
    /// 
    /// <returns><b>True</b> if the specified range overlaps with the range or
    /// <b>false</b> otherwise.</returns>
    /// 
    public bool IsOverlapping(Range range)
    {
        return ((IsInside(range.min)) || (IsInside(range.max)) ||
                 (range.IsInside(min)) || (range.IsInside(max)));
    }

    /// <summary>
    /// Convert the signle precision range to integer range.
    /// </summary>
    /// 
    /// <param name="provideInnerRange">Specifies if inner integer range must be returned or outer range.</param>
    /// 
    /// <returns>Returns integer version of the range.</returns>
    /// 
    /// <remarks>If <paramref name="provideInnerRange"/> is set to <see langword="true"/>, then the
    /// returned integer range will always fit inside of the current single precision range.
    /// If it is set to <see langword="false"/>, then current single precision range will always
    /// fit into the returned integer range.</remarks>
    ///
    public IntRange ToIntRange(bool provideInnerRange)
    {
        int iMin, iMax;

        if (provideInnerRange)
        {
            iMin = (int)Math.Ceiling(min);
            iMax = (int)Math.Floor(max);
        }
        else
        {
            iMin = (int)Math.Floor(min);
            iMax = (int)Math.Ceiling(max);
        }

        return new IntRange(iMin, iMax);
    }

    /// <summary>
    /// Equality operator - checks if two ranges have equal min/max values.
    /// </summary>
    /// 
    /// <param name="range1">First range to check.</param>
    /// <param name="range2">Second range to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if min/max values of specified
    /// ranges are equal.</returns>
    ///
    public static bool operator ==(Range range1, Range range2)
    {
        return ((range1.min == range2.min) && (range1.max == range2.max));
    }

    /// <summary>
    /// Inequality operator - checks if two ranges have different min/max values.
    /// </summary>
    /// 
    /// <param name="range1">First range to check.</param>
    /// <param name="range2">Second range to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if min/max values of specified
    /// ranges are not equal.</returns>
    ///
    public static bool operator !=(Range range1, Range range2)
    {
        return ((range1.min != range2.min) || (range1.max != range2.max));

    }

    /// <summary>
    /// Check if this instance of <see cref="Range"/> equal to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another range to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object obj)
    {
        return (obj is Range) ? (this == (Range)obj) : false;
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode()
    {
        return min.GetHashCode() + max.GetHashCode();
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains min/max values of the range in readable form.</returns>
    ///
    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", min, max);
    }
}

[Serializable]
public struct IntRange
{
    private int min, max;

    /// <summary>
    /// Minimum value of the range.
    /// </summary>
    /// 
    /// <remarks><para>The property represents minimum value (left side limit) or the range -
    /// [<b>min</b>, max].</para></remarks>
    /// 
    public int Min
    {
        get { return min; }
        set { min = value; }
    }

    /// <summary>
    /// Maximum value of the range.
    /// </summary>
    /// 
    /// <remarks><para>The property represents maximum value (right side limit) or the range -
    /// [min, <b>max</b>].</para></remarks>
    /// 
    public int Max
    {
        get { return max; }
        set { max = value; }
    }

    /// <summary>
    /// Length of the range (deffirence between maximum and minimum values).
    /// </summary>
    public int Length
    {
        get { return max - min; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntRange"/> structure.
    /// </summary>
    /// 
    /// <param name="min">Minimum value of the range.</param>
    /// <param name="max">Maximum value of the range.</param>
    /// 
    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// Check if the specified value is inside of the range.
    /// </summary>
    /// 
    /// <param name="x">Value to check.</param>
    /// 
    /// <returns><b>True</b> if the specified value is inside of the range or
    /// <b>false</b> otherwise.</returns>
    /// 
    public bool IsInside(int x)
    {
        return ((x >= min) && (x <= max));
    }

    /// <summary>
    /// Check if the specified range is inside of the range.
    /// </summary>
    /// 
    /// <param name="range">Range to check.</param>
    /// 
    /// <returns><b>True</b> if the specified range is inside of the range or
    /// <b>false</b> otherwise.</returns>
    /// 
    public bool IsInside(IntRange range)
    {
        return ((IsInside(range.min)) && (IsInside(range.max)));
    }

    /// <summary>
    /// Check if the specified range overlaps with the range.
    /// </summary>
    /// 
    /// <param name="range">Range to check for overlapping.</param>
    /// 
    /// <returns><b>True</b> if the specified range overlaps with the range or
    /// <b>false</b> otherwise.</returns>
    /// 
    public bool IsOverlapping(IntRange range)
    {
        return ((IsInside(range.min)) || (IsInside(range.max)) ||
                 (range.IsInside(min)) || (range.IsInside(max)));
    }

    /// <summary>
    /// Implicit conversion to <see cref="Range"/>.
    /// </summary>
    /// 
    /// <param name="range">Integer range to convert to single precision range.</param>
    /// 
    /// <returns>Returns new single precision range which min/max values are implicitly converted
    /// to floats from min/max values of the specified integer range.</returns>
    /// 
    public static implicit operator Range(IntRange range)
    {
        return new Range(range.Min, range.Max);
    }

    /// <summary>
    /// Equality operator - checks if two ranges have equal min/max values.
    /// </summary>
    /// 
    /// <param name="range1">First range to check.</param>
    /// <param name="range2">Second range to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if min/max values of specified
    /// ranges are equal.</returns>
    ///
    public static bool operator ==(IntRange range1, IntRange range2)
    {
        return ((range1.min == range2.min) && (range1.max == range2.max));
    }

    /// <summary>
    /// Inequality operator - checks if two ranges have different min/max values.
    /// </summary>
    /// 
    /// <param name="range1">First range to check.</param>
    /// <param name="range2">Second range to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if min/max values of specified
    /// ranges are not equal.</returns>
    ///
    public static bool operator !=(IntRange range1, IntRange range2)
    {
        return ((range1.min != range2.min) || (range1.max != range2.max));

    }

    /// <summary>
    /// Check if this instance of <see cref="Range"/> equal to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another range to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object obj)
    {
        return (obj is IntRange) ? (this == (IntRange)obj) : false;
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode()
    {
        return min.GetHashCode() + max.GetHashCode();
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains min/max values of the range in readable form.</returns>
    ///
    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", min, max);
    }
}

public static class SystemTools
{
    /// <summary>
    /// Copy block of unmanaged memory.
    /// </summary>
    /// 
    /// <param name="dst">Destination pointer.</param>
    /// <param name="src">Source pointer.</param>
    /// <param name="count">Memory block's length to copy.</param>
    /// 
    /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
    /// 
    /// <remarks><para>This function is required because of the fact that .NET does
    /// not provide any way to copy unmanaged blocks, but provides only methods to
    /// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
    ///
    public static IntPtr CopyUnmanagedMemory(IntPtr dst, IntPtr src, int count)
    {
        unsafe
        {
            CopyUnmanagedMemory((byte*)dst.ToPointer(), (byte*)src.ToPointer(), count);
        }
        return dst;
    }

    /// <summary>
    /// Copy block of unmanaged memory.
    /// </summary>
    /// 
    /// <param name="dst">Destination pointer.</param>
    /// <param name="src">Source pointer.</param>
    /// <param name="count">Memory block's length to copy.</param>
    /// 
    /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
    /// 
    /// <remarks><para>This function is required because of the fact that .NET does
    /// not provide any way to copy unmanaged blocks, but provides only methods to
    /// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
    /// 
    public static unsafe byte* CopyUnmanagedMemory(byte* dst, byte* src, int count)
    {
#if !MONO
        return memcpy(dst, src, count);
#else
            int countUint = count >> 2;
            int countByte = count & 3;

            uint* dstUint = (uint*) dst;
            uint* srcUint = (uint*) src;

            while ( countUint-- != 0 )
            {
                *dstUint++ = *srcUint++;
            }

            byte* dstByte = (byte*) dstUint;
            byte* srcByte = (byte*) srcUint;

            while ( countByte-- != 0 )
            {
                *dstByte++ = *srcByte++;
            }
            return dst;
#endif
    }

    /// <summary>
    /// Fill memory region with specified value.
    /// </summary>
    /// 
    /// <param name="dst">Destination pointer.</param>
    /// <param name="filler">Filler byte's value.</param>
    /// <param name="count">Memory block's length to fill.</param>
    /// 
    /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
    /// 
    public static IntPtr SetUnmanagedMemory(IntPtr dst, int filler, int count)
    {
        unsafe
        {
            SetUnmanagedMemory((byte*)dst.ToPointer(), filler, count);
        }
        return dst;
    }

    /// <summary>
    /// Fill memory region with specified value.
    /// </summary>
    /// 
    /// <param name="dst">Destination pointer.</param>
    /// <param name="filler">Filler byte's value.</param>
    /// <param name="count">Memory block's length to fill.</param>
    /// 
    /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
    /// 
    public static unsafe byte* SetUnmanagedMemory(byte* dst, int filler, int count)
    {
#if !MONO
        return memset(dst, filler, count);
#else
            int countUint = count >> 2;
            int countByte = count & 3;

            byte fillerByte = (byte) filler;
            uint fiilerUint = (uint) filler | ( (uint) filler << 8 ) |
                                              ( (uint) filler << 16 );// |
                                              //( (uint) filler << 24 );

            uint* dstUint = (uint*) dst;

            while ( countUint-- != 0 )
            {
                *dstUint++ = fiilerUint;
            }

            byte* dstByte = (byte*) dstUint;

            while ( countByte-- != 0 )
            {
                *dstByte++ = fillerByte;
            }
            return dst;
#endif
    }


#if !MONO
    // Win32 memory copy function
    [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
    private static unsafe extern byte* memcpy(
        byte* dst,
        byte* src,
        int count);
    // Win32 memory set function
    [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
    private static unsafe extern byte* memset(
        byte* dst,
        int filler,
        int count);
#endif
}

[Serializable]
public struct Point
{
    /// <summary> 
    /// X coordinate.
    /// </summary> 
    /// 
    public float X;

    /// <summary> 
    /// Y coordinate.
    /// </summary> 
    /// 
    public float Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// 
    public Point(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Calculate Euclidean distance between two points.
    /// </summary>
    /// 
    /// <param name="anotherPoint">Point to calculate distance to.</param>
    /// 
    /// <returns>Returns Euclidean distance between this point and
    /// <paramref name="anotherPoint"/> points.</returns>
    /// 
    public float DistanceTo(Point anotherPoint)
    {
        float dx = X - anotherPoint.X;
        float dy = Y - anotherPoint.Y;

        return (float)System.Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static Point operator +(Point point1, Point point2)
    {
        return new Point(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static Point Add(Point point1, Point point2)
    {
        return new Point(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static Point operator -(Point point1, Point point2)
    {
        return new Point(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static Point Subtract(Point point1, Point point2)
    {
        return new Point(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static Point operator +(Point point, float valueToAdd)
    {
        return new Point(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static Point Add(Point point, float valueToAdd)
    {
        return new Point(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static Point operator -(Point point, float valueToSubtract)
    {
        return new Point(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static Point Subtract(Point point, float valueToSubtract)
    {
        return new Point(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static Point operator *(Point point, float factor)
    {
        return new Point(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static Point Multiply(Point point, float factor)
    {
        return new Point(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static Point operator /(Point point, float factor)
    {
        return new Point(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static Point Divide(Point point, float factor)
    {
        return new Point(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Equality operator - checks if two points have equal coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are equal.</returns>
    ///
    public static bool operator ==(Point point1, Point point2)
    {
        return ((point1.X == point2.X) && (point1.Y == point2.Y));
    }

    /// <summary>
    /// Inequality operator - checks if two points have different coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are not equal.</returns>
    ///
    public static bool operator !=(Point point1, Point point2)
    {
        return ((point1.X != point2.X) || (point1.Y != point2.Y));
    }

    /// <summary>
    /// Check if this instance of <see cref="Point"/> equal to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another point to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object obj)
    {
        return (obj is Point) ? (this == (Point)obj) : false;
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    /// <summary>
    /// Explicit conversion to <see cref="IntPoint"/>.
    /// </summary>
    /// 
    /// <param name="point">Single precision point to convert to integer point.</param>
    /// 
    /// <returns>Returns new integer point which coordinates are explicitly converted
    /// to integers from coordinates of the specified single precision point by
    /// casting float values to integers value.</returns>
    /// 
    public static explicit operator IntPoint(Point point)
    {
        return new IntPoint((int)point.X, (int)point.Y);
    }

    /// <summary>
    /// Implicit conversion to <see cref="DoublePoint"/>.
    /// </summary>
    /// 
    /// <param name="point">Single precision point to convert to double precision point.</param>
    /// 
    /// <returns>Returns new double precision point which coordinates are implicitly converted
    /// to doubles from coordinates of the specified single precision point.</returns>
    /// 
    public static implicit operator DoublePoint(Point point)
    {
        return new DoublePoint(point.X, point.Y);
    }

    /// <summary>
    /// Rounds the single precision point.
    /// </summary>
    /// 
    /// <returns>Returns new integer point, which coordinates equal to whole numbers
    /// nearest to the corresponding coordinates of the single precision point.</returns>
    /// 
    public IntPoint Round()
    {
        return new IntPoint((int)Math.Round(X), (int)Math.Round(Y));
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains values of the point in readable form.</returns>
    ///
    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", X, Y);
    }

    /// <summary>
    /// Calculate Euclidean norm of the vector comprised of the point's 
    /// coordinates - distance from (0, 0) in other words.
    /// </summary>
    /// 
    /// <returns>Returns point's distance from (0, 0) point.</returns>
    /// 
    public float EuclideanNorm()
    {
        return (float)System.Math.Sqrt(X * X + Y * Y);
    }
}

[Serializable]
public struct DoublePoint
{
    /// <summary> 
    /// X coordinate.
    /// </summary> 
    /// 
    public double X;

    /// <summary> 
    /// Y coordinate.
    /// </summary> 
    /// 
    public double Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoublePoint"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// 
    public DoublePoint(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Calculate Euclidean distance between two points.
    /// </summary>
    /// 
    /// <param name="anotherPoint">Point to calculate distance to.</param>
    /// 
    /// <returns>Returns Euclidean distance between this point and
    /// <paramref name="anotherPoint"/> points.</returns>
    /// 
    public double DistanceTo(DoublePoint anotherPoint)
    {
        double dx = X - anotherPoint.X;
        double dy = Y - anotherPoint.Y;

        return System.Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static DoublePoint operator +(DoublePoint point1, DoublePoint point2)
    {
        return new DoublePoint(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static DoublePoint Add(DoublePoint point1, DoublePoint point2)
    {
        return new DoublePoint(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static DoublePoint operator -(DoublePoint point1, DoublePoint point2)
    {
        return new DoublePoint(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static DoublePoint Subtract(DoublePoint point1, DoublePoint point2)
    {
        return new DoublePoint(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static DoublePoint operator +(DoublePoint point, double valueToAdd)
    {
        return new DoublePoint(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static DoublePoint Add(DoublePoint point, double valueToAdd)
    {
        return new DoublePoint(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static DoublePoint operator -(DoublePoint point, double valueToSubtract)
    {
        return new DoublePoint(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static DoublePoint Subtract(DoublePoint point, double valueToSubtract)
    {
        return new DoublePoint(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static DoublePoint operator *(DoublePoint point, double factor)
    {
        return new DoublePoint(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static DoublePoint Multiply(DoublePoint point, double factor)
    {
        return new DoublePoint(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static DoublePoint operator /(DoublePoint point, double factor)
    {
        return new DoublePoint(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static DoublePoint Divide(DoublePoint point, double factor)
    {
        return new DoublePoint(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Equality operator - checks if two points have equal coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are equal.</returns>
    ///
    public static bool operator ==(DoublePoint point1, DoublePoint point2)
    {
        return ((point1.X == point2.X) && (point1.Y == point2.Y));
    }

    /// <summary>
    /// Inequality operator - checks if two points have different coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are not equal.</returns>
    ///
    public static bool operator !=(DoublePoint point1, DoublePoint point2)
    {
        return ((point1.X != point2.X) || (point1.Y != point2.Y));
    }

    /// <summary>
    /// Check if this instance of <see cref="DoublePoint"/> equal to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another point to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object obj)
    {
        return (obj is DoublePoint) ? (this == (DoublePoint)obj) : false;
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    /// <summary>
    /// Explicit conversion to <see cref="IntPoint"/>.
    /// </summary>
    /// 
    /// <param name="point">Double precision point to convert to integer point.</param>
    /// 
    /// <returns>Returns new integer point which coordinates are explicitly converted
    /// to integers from coordinates of the specified double precision point by
    /// casting double values to integers value.</returns>
    /// 
    public static explicit operator IntPoint(DoublePoint point)
    {
        return new IntPoint((int)point.X, (int)point.Y);
    }

    /// <summary>
    /// Explicit conversion to <see cref="Point"/>.
    /// </summary>
    /// 
    /// <param name="point">Double precision point to convert to single precision point.</param>
    /// 
    /// <returns>Returns new single precision point which coordinates are explicitly converted
    /// to floats from coordinates of the specified double precision point by
    /// casting double values to float value.</returns>
    /// 
    public static explicit operator Point(DoublePoint point)
    {
        return new Point((float)point.X, (float)point.Y);
    }

    /// <summary>
    /// Rounds the double precision point.
    /// </summary>
    /// 
    /// <returns>Returns new integer point, which coordinates equal to whole numbers
    /// nearest to the corresponding coordinates of the double precision point.</returns>
    /// 
    public IntPoint Round()
    {
        return new IntPoint((int)Math.Round(X), (int)Math.Round(Y));
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains values of the point in readable form.</returns>
    ///
    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", X, Y);
    }

    /// <summary>
    /// Calculate Euclidean norm of the vector comprised of the point's 
    /// coordinates - distance from (0, 0) in other words.
    /// </summary>
    /// 
    /// <returns>Returns point's distance from (0, 0) point.</returns>
    /// 
    public double EuclideanNorm()
    {
        return System.Math.Sqrt(X * X + Y * Y);
    }
}

[Serializable]
public struct IntPoint
{
    /// <summary> 
    /// X coordinate.
    /// </summary> 
    /// 
    public int X;

    /// <summary> 
    /// Y coordinate.
    /// </summary> 
    /// 
    public int Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntPoint"/> structure.
    /// </summary>
    /// 
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    /// 
    public IntPoint(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Calculate Euclidean distance between two points.
    /// </summary>
    /// 
    /// <param name="anotherPoint">Point to calculate distance to.</param>
    /// 
    /// <returns>Returns Euclidean distance between this point and
    /// <paramref name="anotherPoint"/> points.</returns>
    /// 
    public float DistanceTo(IntPoint anotherPoint)
    {
        int dx = X - anotherPoint.X;
        int dy = Y - anotherPoint.Y;

        return (float)System.Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static IntPoint operator +(IntPoint point1, IntPoint point2)
    {
        return new IntPoint(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Addition operator - adds values of two points.
    /// </summary>
    /// 
    /// <param name="point1">First point for addition.</param>
    /// <param name="point2">Second point for addition.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to sum of corresponding
    /// coordinates of specified points.</returns>
    /// 
    public static IntPoint Add(IntPoint point1, IntPoint point2)
    {
        return new IntPoint(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static IntPoint operator -(IntPoint point1, IntPoint point2)
    {
        return new IntPoint(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Subtraction operator - subtracts values of two points.
    /// </summary>
    /// 
    /// <param name="point1">Point to subtract from.</param>
    /// <param name="point2">Point to subtract.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to difference of corresponding
    /// coordinates of specified points.</returns>
    ///
    public static IntPoint Subtract(IntPoint point1, IntPoint point2)
    {
        return new IntPoint(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static IntPoint operator +(IntPoint point, int valueToAdd)
    {
        return new IntPoint(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Addition operator - adds scalar to the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to increase coordinates of.</param>
    /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point increased by specified value.</returns>
    /// 
    public static IntPoint Add(IntPoint point, int valueToAdd)
    {
        return new IntPoint(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static IntPoint operator -(IntPoint point, int valueToSubtract)
    {
        return new IntPoint(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Subtraction operator - subtracts scalar from the specified point.
    /// </summary>
    /// 
    /// <param name="point">Point to decrease coordinates of.</param>
    /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point decreased by specified value.</returns>
    /// 
    public static IntPoint Subtract(IntPoint point, int valueToSubtract)
    {
        return new IntPoint(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static IntPoint operator *(IntPoint point, int factor)
    {
        return new IntPoint(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to multiply coordinates of.</param>
    /// <param name="factor">Multiplication factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point multiplied by specified value.</returns>
    ///
    public static IntPoint Multiply(IntPoint point, int factor)
    {
        return new IntPoint(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static IntPoint operator /(IntPoint point, int factor)
    {
        return new IntPoint(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Division operator - divides coordinates of the specified point by scalar value.
    /// </summary>
    /// 
    /// <param name="point">Point to divide coordinates of.</param>
    /// <param name="factor">Division factor.</param>
    /// 
    /// <returns>Returns new point which coordinates equal to coordinates of
    /// the specified point divided by specified value.</returns>
    /// 
    public static IntPoint Divide(IntPoint point, int factor)
    {
        return new IntPoint(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Equality operator - checks if two points have equal coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are equal.</returns>
    ///
    public static bool operator ==(IntPoint point1, IntPoint point2)
    {
        return ((point1.X == point2.X) && (point1.Y == point2.Y));
    }

    /// <summary>
    /// Inequality operator - checks if two points have different coordinates.
    /// </summary>
    /// 
    /// <param name="point1">First point to check.</param>
    /// <param name="point2">Second point to check.</param>
    /// 
    /// <returns>Returns <see langword="true"/> if coordinates of specified
    /// points are not equal.</returns>
    ///
    public static bool operator !=(IntPoint point1, IntPoint point2)
    {
        return ((point1.X != point2.X) || (point1.Y != point2.Y));
    }

    /// <summary>
    /// Check if this instance of <see cref="IntPoint"/> equal to the specified one.
    /// </summary>
    /// 
    /// <param name="obj">Another point to check equalty to.</param>
    /// 
    /// <returns>Return <see langword="true"/> if objects are equal.</returns>
    /// 
    public override bool Equals(object obj)
    {
        return (obj is IntPoint) ? (this == (IntPoint)obj) : false;
    }

    /// <summary>
    /// Get hash code for this instance.
    /// </summary>
    /// 
    /// <returns>Returns the hash code for this instance.</returns>
    /// 
    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    /// <summary>
    /// Implicit conversion to <see cref="Point"/>.
    /// </summary>
    /// 
    /// <param name="point">Integer point to convert to single precision point.</param>
    /// 
    /// <returns>Returns new single precision point which coordinates are implicitly converted
    /// to floats from coordinates of the specified integer point.</returns>
    /// 
    public static implicit operator Point(IntPoint point)
    {
        return new Point(point.X, point.Y);
    }

    /// <summary>
    /// Implicit conversion to <see cref="DoublePoint"/>.
    /// </summary>
    /// 
    /// <param name="point">Integer point to convert to double precision point.</param>
    /// 
    /// <returns>Returns new double precision point which coordinates are implicitly converted
    /// to doubles from coordinates of the specified integer point.</returns>
    /// 
    public static implicit operator DoublePoint(IntPoint point)
    {
        return new DoublePoint(point.X, point.Y);
    }

    /// <summary>
    /// Get string representation of the class.
    /// </summary>
    /// 
    /// <returns>Returns string, which contains values of the point in readable form.</returns>
    ///
    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", X, Y);
    }

    /// <summary>
    /// Calculate Euclidean norm of the vector comprised of the point's 
    /// coordinates - distance from (0, 0) in other words.
    /// </summary>
    /// 
    /// <returns>Returns point's distance from (0, 0) point.</returns>
    /// 
    public float EuclideanNorm()
    {
        return (float)System.Math.Sqrt(X * X + Y * Y);
    }
} 

public class RGB
{
    /// <summary>
    /// Index of red component.
    /// </summary>
    public const short R = 2;

    /// <summary>
    /// Index of green component.
    /// </summary>
    public const short G = 1;

    /// <summary>
    /// Index of blue component.
    /// </summary>
    public const short B = 0;

    /// <summary>
    /// Index of alpha component for ARGB images.
    /// </summary>
    public const short A = 3;

    /// <summary>
    /// Red component.
    /// </summary>
    public byte Red;

    /// <summary>
    /// Green component.
    /// </summary>
    public byte Green;

    /// <summary>
    /// Blue component.
    /// </summary>
    public byte Blue;

    /// <summary>
    /// Alpha component.
    /// </summary>
    public byte Alpha;

    /// <summary>
    /// <see cref="System.Drawing.Color">Color</see> value of the class.
    /// </summary>
    public System.Drawing.Color Color
    {
        get { return Color.FromArgb(Alpha, Red, Green, Blue); }
        set
        {
            Red = value.R;
            Green = value.G;
            Blue = value.B;
            Alpha = value.A;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RGB"/> class.
    /// </summary>
    public RGB()
    {
        Red = 0;
        Green = 0;
        Blue = 0;
        Alpha = 255;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RGB"/> class.
    /// </summary>
    /// 
    /// <param name="red">Red component.</param>
    /// <param name="green">Green component.</param>
    /// <param name="blue">Blue component.</param>
    /// 
    public RGB(byte red, byte green, byte blue)
    {
        this.Red = red;
        this.Green = green;
        this.Blue = blue;
        this.Alpha = 255;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RGB"/> class.
    /// </summary>
    /// 
    /// <param name="red">Red component.</param>
    /// <param name="green">Green component.</param>
    /// <param name="blue">Blue component.</param>
    /// <param name="alpha">Alpha component.</param>
    /// 
    public RGB(byte red, byte green, byte blue, byte alpha)
    {
        this.Red = red;
        this.Green = green;
        this.Blue = blue;
        this.Alpha = alpha;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RGB"/> class.
    /// </summary>
    /// 
    /// <param name="color">Initialize from specified <see cref="System.Drawing.Color">color.</see></param>
    /// 
    public RGB(System.Drawing.Color color)
    {
        this.Red = color.R;
        this.Green = color.G;
        this.Blue = color.B;
        this.Alpha = color.A;
    }
}

/// <summary>
/// HSL components.
/// </summary>
/// 
/// <remarks>The class encapsulates <b>HSL</b> color components.</remarks>
/// 
public class HSL
{
    /// <summary>
    /// Hue component.
    /// </summary>
    /// 
    /// <remarks>Hue is measured in the range of [0, 359].</remarks>
    /// 
    public int Hue;

    /// <summary>
    /// Saturation component.
    /// </summary>
    /// 
    /// <remarks>Saturation is measured in the range of [0, 1].</remarks>
    /// 
    public float Saturation;

    /// <summary>
    /// Luminance value.
    /// </summary>
    /// 
    /// <remarks>Luminance is measured in the range of [0, 1].</remarks>
    /// 
    public float Luminance;

    /// <summary>
    /// Initializes a new instance of the <see cref="HSL"/> class.
    /// </summary>
    public HSL() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HSL"/> class.
    /// </summary>
    /// 
    /// <param name="hue">Hue component.</param>
    /// <param name="saturation">Saturation component.</param>
    /// <param name="luminance">Luminance component.</param>
    /// 
    public HSL(int hue, float saturation, float luminance)
    {
        this.Hue = hue;
        this.Saturation = saturation;
        this.Luminance = luminance;
    }

    /// <summary>
    /// Convert from RGB to HSL color space.
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// <param name="hsl">Destination color in <b>HSL</b> color space.</param>
    /// 
    /// <remarks><para>See <a href="http://en.wikipedia.org/wiki/HSI_color_space#Conversion_from_RGB_to_HSL_or_HSV">HSL and HSV Wiki</a>
    /// for information about the algorithm to convert from RGB to HSL.</para></remarks>
    /// 
    public static void FromRGB(RGB rgb, HSL hsl)
    {
        float r = (rgb.Red / 255.0f);
        float g = (rgb.Green / 255.0f);
        float b = (rgb.Blue / 255.0f);

        float min = Math.Min(Math.Min(r, g), b);
        float max = Math.Max(Math.Max(r, g), b);
        float delta = max - min;

        // get luminance value
        hsl.Luminance = (max + min) / 2;

        if (delta == 0)
        {
            // gray color
            hsl.Hue = 0;
            hsl.Saturation = 0.0f;
        }
        else
        {
            // get saturation value
            hsl.Saturation = (hsl.Luminance <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

            // get hue value
            float hue;

            if (r == max)
            {
                hue = ((g - b) / 6) / delta;
            }
            else if (g == max)
            {
                hue = (1.0f / 3) + ((b - r) / 6) / delta;
            }
            else
            {
                hue = (2.0f / 3) + ((r - g) / 6) / delta;
            }

            // correct hue if needed
            if (hue < 0)
                hue += 1;
            if (hue > 1)
                hue -= 1;

            hsl.Hue = (int)(hue * 360);
        }
    }

    /// <summary>
    /// Convert from RGB to HSL color space.
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// 
    /// <returns>Returns <see cref="HSL"/> instance, which represents converted color value.</returns>
    /// 
    public static HSL FromRGB(RGB rgb)
    {
        HSL hsl = new HSL();
        FromRGB(rgb, hsl);
        return hsl;
    }

    /// <summary>
    /// Convert from HSL to RGB color space.
    /// </summary>
    /// 
    /// <param name="hsl">Source color in <b>HSL</b> color space.</param>
    /// <param name="rgb">Destination color in <b>RGB</b> color space.</param>
    /// 
    public static void ToRGB(HSL hsl, RGB rgb)
    {
        if (hsl.Saturation == 0)
        {
            // gray values
            rgb.Red = rgb.Green = rgb.Blue = (byte)(hsl.Luminance * 255);
        }
        else
        {
            float v1, v2;
            float hue = (float)hsl.Hue / 360;

            v2 = (hsl.Luminance < 0.5) ?
                (hsl.Luminance * (1 + hsl.Saturation)) :
                ((hsl.Luminance + hsl.Saturation) - (hsl.Luminance * hsl.Saturation));
            v1 = 2 * hsl.Luminance - v2;

            rgb.Red = (byte)(255 * Hue_2_RGB(v1, v2, hue + (1.0f / 3)));
            rgb.Green = (byte)(255 * Hue_2_RGB(v1, v2, hue));
            rgb.Blue = (byte)(255 * Hue_2_RGB(v1, v2, hue - (1.0f / 3)));
            rgb.Alpha = 255;
        }
    }

    /// <summary>
    /// Convert the color to <b>RGB</b> color space.
    /// </summary>
    /// 
    /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
    /// 
    public RGB ToRGB()
    {
        RGB rgb = new RGB();
        ToRGB(this, rgb);
        return rgb;
    }

    #region Private members
    // HSL to RGB helper routine
    private static float Hue_2_RGB(float v1, float v2, float vH)
    {
        if (vH < 0)
            vH += 1;
        if (vH > 1)
            vH -= 1;
        if ((6 * vH) < 1)
            return (v1 + (v2 - v1) * 6 * vH);
        if ((2 * vH) < 1)
            return v2;
        if ((3 * vH) < 2)
            return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);
        return v1;
    }
    #endregion
}

/// <summary>
/// YCbCr components.
/// </summary>
/// 
/// <remarks>The class encapsulates <b>YCbCr</b> color components.</remarks>
/// 
public class YCbCr
{
    /// <summary>
    /// Index of <b>Y</b> component.
    /// </summary>
    public const short YIndex = 0;

    /// <summary>
    /// Index of <b>Cb</b> component.
    /// </summary>
    public const short CbIndex = 1;

    /// <summary>
    /// Index of <b>Cr</b> component.
    /// </summary>
    public const short CrIndex = 2;

    /// <summary>
    /// <b>Y</b> component.
    /// </summary>
    public float Y;

    /// <summary>
    /// <b>Cb</b> component.
    /// </summary>
    public float Cb;

    /// <summary>
    /// <b>Cr</b> component.
    /// </summary>
    public float Cr;

    /// <summary>
    /// Initializes a new instance of the <see cref="YCbCr"/> class.
    /// </summary>
    public YCbCr() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="YCbCr"/> class.
    /// </summary>
    /// 
    /// <param name="y"><b>Y</b> component.</param>
    /// <param name="cb"><b>Cb</b> component.</param>
    /// <param name="cr"><b>Cr</b> component.</param>
    /// 
    public YCbCr(float y, float cb, float cr)
    {
        this.Y = Math.Max(0.0f, Math.Min(1.0f, y));
        this.Cb = Math.Max(-0.5f, Math.Min(0.5f, cb));
        this.Cr = Math.Max(-0.5f, Math.Min(0.5f, cr));
    }

    /// <summary>
    /// Convert from RGB to YCbCr color space (Rec 601-1 specification). 
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// <param name="ycbcr">Destination color in <b>YCbCr</b> color space.</param>
    /// 
    public static void FromRGB(RGB rgb, YCbCr ycbcr)
    {
        float r = (float)rgb.Red / 255;
        float g = (float)rgb.Green / 255;
        float b = (float)rgb.Blue / 255;

        ycbcr.Y = (float)(0.2989 * r + 0.5866 * g + 0.1145 * b);
        ycbcr.Cb = (float)(-0.1687 * r - 0.3313 * g + 0.5000 * b);
        ycbcr.Cr = (float)(0.5000 * r - 0.4184 * g - 0.0816 * b);
    }

    /// <summary>
    /// Convert from RGB to YCbCr color space (Rec 601-1 specification).
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// 
    /// <returns>Returns <see cref="YCbCr"/> instance, which represents converted color value.</returns>
    /// 
    public static YCbCr FromRGB(RGB rgb)
    {
        YCbCr ycbcr = new YCbCr();
        FromRGB(rgb, ycbcr);
        return ycbcr;
    }

    /// <summary>
    /// Convert from YCbCr to RGB color space.
    /// </summary>
    /// 
    /// <param name="ycbcr">Source color in <b>YCbCr</b> color space.</param>
    /// <param name="rgb">Destination color in <b>RGB</b> color spacs.</param>
    /// 
    public static void ToRGB(YCbCr ycbcr, RGB rgb)
    {
        // don't warry about zeros. compiler will remove them
        float r = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 0.0000 * ycbcr.Cb + 1.4022 * ycbcr.Cr)));
        float g = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y - 0.3456 * ycbcr.Cb - 0.7145 * ycbcr.Cr)));
        float b = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 1.7710 * ycbcr.Cb + 0.0000 * ycbcr.Cr)));

        rgb.Red = (byte)(r * 255);
        rgb.Green = (byte)(g * 255);
        rgb.Blue = (byte)(b * 255);
        rgb.Alpha = 255;
    }

    /// <summary>
    /// Convert the color to <b>RGB</b> color space.
    /// </summary>
    /// 
    /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
    /// 
    public RGB ToRGB()
    {
        RGB rgb = new RGB();
        ToRGB(this, rgb);
        return rgb;
    }
}

public class LevelsLinear : BaseInPlacePartialFilter
{
    private IntRange inRed = new IntRange(0, 255);
    private IntRange inGreen = new IntRange(0, 255);
    private IntRange inBlue = new IntRange(0, 255);

    private IntRange outRed = new IntRange(0, 255);
    private IntRange outGreen = new IntRange(0, 255);
    private IntRange outBlue = new IntRange(0, 255);

    private byte[] mapRed = new byte[256];
    private byte[] mapGreen = new byte[256];
    private byte[] mapBlue = new byte[256];

    // private format translation dictionary
    private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

    /// <summary>
    /// Format translations dictionary.
    /// </summary>
    public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
    {
        get { return formatTranslations; }
    }

    #region Public Propertis

    /// <summary>
    /// Red component's input range.
    /// </summary>
    public IntRange InRed
    {
        get { return inRed; }
        set
        {
            inRed = value;
            CalculateMap(inRed, outRed, mapRed);
        }
    }

    /// <summary>
    /// Green component's input range.
    /// </summary>
    public IntRange InGreen
    {
        get { return inGreen; }
        set
        {
            inGreen = value;
            CalculateMap(inGreen, outGreen, mapGreen);
        }
    }

    /// <summary>
    /// Blue component's input range.
    /// </summary>
    public IntRange InBlue
    {
        get { return inBlue; }
        set
        {
            inBlue = value;
            CalculateMap(inBlue, outBlue, mapBlue);
        }
    }

    /// <summary>
    /// Gray component's input range.
    /// </summary>
    public IntRange InGray
    {
        get { return inGreen; }
        set
        {
            inGreen = value;
            CalculateMap(inGreen, outGreen, mapGreen);
        }
    }

    /// <summary>
    /// Input range for RGB components.
    /// </summary>
    /// 
    /// <remarks>The property allows to set red, green and blue input ranges to the same value.</remarks>
    /// 
    public IntRange Input
    {
        set
        {
            inRed = inGreen = inBlue = value;
            CalculateMap(inRed, outRed, mapRed);
            CalculateMap(inGreen, outGreen, mapGreen);
            CalculateMap(inBlue, outBlue, mapBlue);
        }
    }

    /// <summary>
    /// Red component's output range.
    /// </summary>
    public IntRange OutRed
    {
        get { return outRed; }
        set
        {
            outRed = value;
            CalculateMap(inRed, outRed, mapRed);
        }
    }

    /// <summary>
    /// Green component's output range.
    /// </summary>
    public IntRange OutGreen
    {
        get { return outGreen; }
        set
        {
            outGreen = value;
            CalculateMap(inGreen, outGreen, mapGreen);
        }
    }

    /// <summary>
    /// Blue component's output range.
    /// </summary>
    public IntRange OutBlue
    {
        get { return outBlue; }
        set
        {
            outBlue = value;
            CalculateMap(inBlue, outBlue, mapBlue);
        }
    }

    /// <summary>
    /// Gray component's output range.
    /// </summary>
    public IntRange OutGray
    {
        get { return outGreen; }
        set
        {
            outGreen = value;
            CalculateMap(inGreen, outGreen, mapGreen);
        }
    }

    /// <summary>
    /// Output range for RGB components.
    /// </summary>
    /// 
    /// <remarks>The property allows to set red, green and blue output ranges to the same value.</remarks>
    /// 
    public IntRange Output
    {
        set
        {
            outRed = outGreen = outBlue = value;
            CalculateMap(inRed, outRed, mapRed);
            CalculateMap(inGreen, outGreen, mapGreen);
            CalculateMap(inBlue, outBlue, mapBlue);
        }
    }

    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="LevelsLinear"/> class.
    /// </summary>
    public LevelsLinear()
    {
        CalculateMap(inRed, outRed, mapRed);
        CalculateMap(inGreen, outGreen, mapGreen);
        CalculateMap(inBlue, outBlue, mapBlue);

        formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
        formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
    }

    /// <summary>
    /// Process the filter on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image data.</param>
    /// <param name="rect">Image rectangle for processing by the filter.</param>
    ///
    protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
    {
        int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

        // processing start and stop X,Y positions
        int startX = rect.Left;
        int startY = rect.Top;
        int stopX = startX + rect.Width;
        int stopY = startY + rect.Height;
        int offset = image.Stride - rect.Width * pixelSize;

        // do the job
        byte* ptr = (byte*)image.ImageData.ToPointer();

        // allign pointer to the first pixel to process
        ptr += (startY * image.Stride + startX * pixelSize);

        if (image.PixelFormat == PixelFormat.Format8bppIndexed)
        {
            // grayscale image
            for (int y = startY; y < stopY; y++)
            {
                for (int x = startX; x < stopX; x++, ptr++)
                {
                    // gray
                    *ptr = mapGreen[*ptr];
                }
                ptr += offset;
            }
        }
        else
        {
            // RGB image
            for (int y = startY; y < stopY; y++)
            {
                for (int x = startX; x < stopX; x++, ptr += pixelSize)
                {
                    // red
                    ptr[RGB.R] = mapRed[ptr[RGB.R]];
                    // green
                    ptr[RGB.G] = mapGreen[ptr[RGB.G]];
                    // blue
                    ptr[RGB.B] = mapBlue[ptr[RGB.B]];
                }
                ptr += offset;
            }
        }
    }


    /// <summary>
    /// Calculate conversion map.
    /// </summary>
    /// 
    /// <param name="inRange">Input range.</param>
    /// <param name="outRange">Output range.</param>
    /// <param name="map">Conversion map.</param>
    /// 
    private void CalculateMap(IntRange inRange, IntRange outRange, byte[] map)
    {
        double k = 0, b = 0;

        if (inRange.Max != inRange.Min)
        {
            k = (double)(outRange.Max - outRange.Min) / (double)(inRange.Max - inRange.Min);
            b = (double)(outRange.Min) - k * inRange.Min;
        }

        for (int i = 0; i < 256; i++)
        {
            byte v = (byte)i;

            if (v >= inRange.Max)
                v = (byte)outRange.Max;
            else if (v <= inRange.Min)
                v = (byte)outRange.Min;
            else
                v = (byte)(k * v + b);

            map[i] = v;
        }
    }
}

public abstract class BaseInPlacePartialFilter
{
    /// <summary>
    /// Format translations dictionary.
    /// </summary>
    /// 
    /// <remarks><para>The dictionary defines, which pixel formats are supported for
    /// source images and which pixel format will be used for resulting image.
    /// </para>
    /// 
    /// <para>See <see cref="IFilterInformation.FormatTranslations"/> for more information.</para>
    /// </remarks>
    ///
    public abstract Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }

    /// <summary>
    /// Apply filter to an image.
    /// </summary>
    /// 
    /// <param name="image">Source image to apply filter to.</param>
    /// 
    /// <returns>Returns filter's result obtained by applying the filter to
    /// the source image.</returns>
    /// 
    /// <remarks>The method keeps the source image unchanged and returns
    /// the result of image processing filter as new image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    ///
    public Bitmap Apply(Bitmap image)
    {
        // lock source bitmap data
        BitmapData srcData = image.LockBits(
            new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, image.PixelFormat);

        Bitmap dstImage = null;

        try
        {
            // apply the filter
            dstImage = Apply(srcData);
            dstImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        }
        finally
        {
            // unlock source image
            image.UnlockBits(srcData);
        }

        return dstImage;
    }

    /// <summary>
    /// Apply filter to an image.
    /// </summary>
    /// 
    /// <param name="imageData">Source image to apply filter to.</param>
    /// 
    /// <returns>Returns filter's result obtained by applying the filter to
    /// the source image.</returns>
    /// 
    /// <remarks>The filter accepts bitmap data as input and returns the result
    /// of image processing filter as new image. The source image data are kept
    /// unchanged.</remarks>
    ///
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    ///
    public Bitmap Apply(BitmapData imageData)
    {
        // destination image format
        PixelFormat dstPixelFormat = imageData.PixelFormat;

        // check pixel format of the source image
        CheckSourceFormat(dstPixelFormat);

        // get image dimension
        int width = imageData.Width;
        int height = imageData.Height;

        // create new image of required format
        Bitmap dstImage = (dstPixelFormat == PixelFormat.Format8bppIndexed) ?
            Image.CreateGrayscaleImage(width, height) :
            new Bitmap(width, height, dstPixelFormat);

        // lock destination bitmap data
        BitmapData dstData = dstImage.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.ReadWrite, dstPixelFormat);

        // copy image
       SystemTools.CopyUnmanagedMemory(dstData.Scan0, imageData.Scan0, imageData.Stride * height);

        try
        {
            // process the filter
            ProcessFilter(new UnmanagedImage(dstData), new Rectangle(0, 0, width, height));
        }
        finally
        {
            // unlock destination images
            dstImage.UnlockBits(dstData);
        }

        return dstImage;
    }

    /// <summary>
    /// Apply filter to an image in unmanaged memory.
    /// </summary>
    /// 
    /// <param name="image">Source image in unmanaged memory to apply filter to.</param>
    /// 
    /// <returns>Returns filter's result obtained by applying the filter to
    /// the source image.</returns>
    /// 
    /// <remarks>The method keeps the source image unchanged and returns
    /// the result of image processing filter as new image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    ///
    public UnmanagedImage Apply(UnmanagedImage image)
    {
        // check pixel format of the source image
        CheckSourceFormat(image.PixelFormat);

        // create new destination image
        UnmanagedImage dstImage = UnmanagedImage.Create(image.Width, image.Height, image.PixelFormat);

        Apply(image, dstImage);

        return dstImage;
    }

    /// <summary>
    /// Apply filter to an image in unmanaged memory.
    /// </summary>
    /// 
    /// <param name="sourceImage">Source image in unmanaged memory to apply filter to.</param>
    /// <param name="destinationImage">Destination image in unmanaged memory to put result into.</param>
    /// 
    /// <remarks><para>The method keeps the source image unchanged and puts result of image processing
    /// into destination image.</para>
    /// 
    /// <para><note>The destination image must have the same width and height as source image. Also
    /// destination image must have pixel format, which is expected by particular filter (see
    /// <see cref="FormatTranslations"/> property for information about pixel format conversions).</note></para>
    /// </remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    /// <exception cref="InvalidImagePropertiesException">Incorrect destination pixel format.</exception>
    /// <exception cref="InvalidImagePropertiesException">Destination image has wrong width and/or height.</exception>
    ///
    public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
    {
        // check pixel format of the source image
        CheckSourceFormat(sourceImage.PixelFormat);

        // ensure destination image has correct format
        if (destinationImage.PixelFormat != sourceImage.PixelFormat)
        {
            throw new InvalidImagePropertiesException("Destination pixel format must be the same as pixel format of source image.");
        }

        // ensure destination image has correct size
        if ((destinationImage.Width != sourceImage.Width) || (destinationImage.Height != sourceImage.Height))
        {
            throw new InvalidImagePropertiesException("Destination image must have the same width and height as source image.");
        }

        // usually stride will be the same for 2 images of the size size/format,
        // but since this a public a method and users may provide any evil, we to need check it
        int dstStride = destinationImage.Stride;
        int srcStride = sourceImage.Stride;
        int lineSize = Math.Min(srcStride, dstStride);

        unsafe
        {
            byte* dst = (byte*)destinationImage.ImageData.ToPointer();
            byte* src = (byte*)sourceImage.ImageData.ToPointer();

            // copy image
            for (int y = 0, height = sourceImage.Height; y < height; y++)
            {
                SystemTools.CopyUnmanagedMemory(dst, src, lineSize);
                dst += dstStride;
                src += srcStride;
            }
        }

        // process the filter
        ProcessFilter(destinationImage, new Rectangle(0, 0, destinationImage.Width, destinationImage.Height));
    }

    /// <summary>
    /// Apply filter to an image.
    /// </summary>
    /// 
    /// <param name="image">Image to apply filter to.</param>
    /// 
    /// <remarks>The method applies the filter directly to the provided source image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    /// 
    public void ApplyInPlace(Bitmap image)
    {
        // apply the filter
        ApplyInPlace(image, new Rectangle(0, 0, image.Width, image.Height));
    }

    /// <summary>
    /// Apply filter to an image.
    /// </summary>
    /// 
    /// <param name="imageData">Image data to apply filter to.</param>
    /// 
    /// <remarks>The method applies the filter directly to the provided source image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    ///
    public void ApplyInPlace(BitmapData imageData)
    {
        // check pixel format of the source image
        CheckSourceFormat(imageData.PixelFormat);

        // apply the filter
        ProcessFilter(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
    }

    /// <summary>
    /// Apply filter to an unmanaged image.
    /// </summary>
    /// 
    /// <param name="image">Unmanaged image to apply filter to.</param>
    /// 
    /// <remarks>The method applies the filter directly to the provided source unmanaged image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    ///
    public void ApplyInPlace(UnmanagedImage image)
    {
        // check pixel format of the source image
        CheckSourceFormat(image.PixelFormat);

        // process the filter
        ProcessFilter(image, new Rectangle(0, 0, image.Width, image.Height));
    }

    /// <summary>
    /// Apply filter to an image or its part.
    /// </summary>
    /// 
    /// <param name="image">Image to apply filter to.</param>
    /// <param name="rect">Image rectangle for processing by the filter.</param>
    /// 
    /// <remarks>The method applies the filter directly to the provided source image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    /// 
    public void ApplyInPlace(Bitmap image, Rectangle rect)
    {
        // lock source bitmap data
        BitmapData data = image.LockBits(
            new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadWrite, image.PixelFormat);

        try
        {
            // apply the filter
            ApplyInPlace(new UnmanagedImage(data), rect);
        }
        finally
        {
            // unlock image
            image.UnlockBits(data);
        }
    }

    /// <summary>
    /// Apply filter to an image or its part.
    /// </summary>
    /// 
    /// <param name="imageData">Image data to apply filter to.</param>
    /// <param name="rect">Image rectangle for processing by the filter.</param>
    /// 
    /// <remarks>The method applies the filter directly to the provided source image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    ///
    public void ApplyInPlace(BitmapData imageData, Rectangle rect)
    {
        // apply the filter
        ApplyInPlace(new UnmanagedImage(imageData), rect);
    }

    /// <summary>
    /// Apply filter to an unmanaged image or its part.
    /// </summary>
    /// 
    /// <param name="image">Unmanaged image to apply filter to.</param>
    /// <param name="rect">Image rectangle for processing by the filter.</param>
    /// 
    /// <remarks>The method applies the filter directly to the provided source image.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
    /// 
    public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
    {
        // check pixel format of the source image
        CheckSourceFormat(image.PixelFormat);

        // validate rectangle
        rect.Intersect(new Rectangle(0, 0, image.Width, image.Height));

        // process the filter if rectangle is not empty
        if ((rect.Width | rect.Height) != 0)
            ProcessFilter(image, rect);
    }

    /// <summary>
    /// Process the filter on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image data.</param>
    /// <param name="rect">Image rectangle for processing by the filter.</param>
    ///
    protected abstract unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect);

    // Check pixel format of the source image
    private void CheckSourceFormat(PixelFormat pixelFormat)
    {
        if (!FormatTranslations.ContainsKey(pixelFormat))
            throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
    }
}

public class BrightnessCorrection : BaseInPlacePartialFilter
{
    private LevelsLinear baseFilter = new LevelsLinear();
    private int adjustValue;

    /// <summary>
    /// Brightness adjust value, [-255, 255].
    /// </summary>
    /// 
    /// <remarks>Default value is set to <b>10</b>, which corresponds to increasing
    /// RGB values of each pixel by 10.</remarks>
    ///
    public int AdjustValue
    {
        get { return adjustValue; }
        set
        {
            adjustValue = Math.Max(-255, Math.Min(255, value));

            if (adjustValue > 0)
            {
                baseFilter.InRed = baseFilter.InGreen = baseFilter.InBlue = baseFilter.InGray =
                    new IntRange(0, 255 - adjustValue);
                baseFilter.OutRed = baseFilter.OutGreen = baseFilter.OutBlue = baseFilter.OutGray =
                    new IntRange(adjustValue, 255);
            }
            else
            {
                baseFilter.InRed = baseFilter.InGreen = baseFilter.InBlue = baseFilter.InGray =
                    new IntRange(-adjustValue, 255);
                baseFilter.OutRed = baseFilter.OutGreen = baseFilter.OutBlue = baseFilter.OutGray =
                    new IntRange(0, 255 + adjustValue);
            }
        }
    }

    /// <summary>
    /// Format translations dictionary.
    /// </summary>
    /// 
    /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
    /// documentation for additional information.</para></remarks>
    ///
    public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
    {
        get { return baseFilter.FormatTranslations; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrightnessCorrection"/> class.
    /// </summary>
    /// 
    public BrightnessCorrection()
    {
        AdjustValue = 10;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrightnessCorrection"/> class.
    /// </summary>
    /// 
    /// <param name="adjustValue">Brightness <see cref="AdjustValue">adjust value</see>.</param>
    /// 
    public BrightnessCorrection(int adjustValue)
    {
        AdjustValue = adjustValue;
    }

    /// <summary>
    /// Process the filter on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image data.</param>
    /// <param name="rect">Image rectangle for processing by the filter.</param>
    ///
    protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
    {
        baseFilter.ApplyInPlace(image, rect);
    }
}

public static class Image
{
    /// <summary>
    /// Check if specified 8 bpp image is grayscale.
    /// </summary>
    /// 
    /// <param name="image">Image to check.</param>
    /// 
    /// <returns>Returns <b>true</b> if the image is grayscale or <b>false</b> otherwise.</returns>
    /// 
    /// <remarks>The methods checks if the image is a grayscale image of 256 gradients.
    /// The method first examines if the image's pixel format is
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
    /// and then it examines its palette to check if the image is grayscale or not.</remarks>
    /// 
    public static bool IsGrayscale(Bitmap image)
    {
        bool ret = false;

        // check pixel format
        if (image.PixelFormat == PixelFormat.Format8bppIndexed)
        {
            ret = true;
            // check palette
            ColorPalette cp = image.Palette;
            Color c;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                c = cp.Entries[i];
                if ((c.R != i) || (c.G != i) || (c.B != i))
                {
                    ret = false;
                    break;
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// Create and initialize new 8 bpp grayscale image.
    /// </summary>
    /// 
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// 
    /// <returns>Returns the created grayscale image.</returns>
    /// 
    /// <remarks>The method creates new 8 bpp grayscale image and initializes its palette.
    /// Grayscale image is represented as
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
    /// image with palette initialized to 256 gradients of gray color.</remarks>
    /// 
    public static Bitmap CreateGrayscaleImage(int width, int height)
    {
        // create new image
        Bitmap image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
        // set palette to grayscale
        SetGrayscalePalette(image);
        // return new image
        return image;
    }

    /// <summary>
    /// Set pallete of the 8 bpp indexed image to grayscale.
    /// </summary>
    /// 
    /// <param name="image">Image to initialize.</param>
    /// 
    /// <remarks>The method initializes palette of
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
    /// image with 256 gradients of gray color.</remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Provided image is not 8 bpp indexed image.</exception>
    /// 
    public static void SetGrayscalePalette(Bitmap image)
    {
        // check pixel format
        if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            throw new UnsupportedImageFormatException("Source image is not 8 bpp image.");

        // get palette
        ColorPalette cp = image.Palette;
        // init palette
        for (int i = 0; i < 256; i++)
        {
            cp.Entries[i] = Color.FromArgb(i, i, i);
        }
        // set palette back
        image.Palette = cp;
    }

    /// <summary>
    /// Clone image.
    /// </summary>
    /// 
    /// <param name="source">Source image.</param>
    /// <param name="format">Pixel format of result image.</param>
    /// 
    /// <returns>Returns clone of the source image with specified pixel format.</returns>
    ///
    /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
    /// does not produce the desired result - it does not create a clone with specified pixel format.
    /// More of it, the original method does not create an actual clone - it does not create a copy
    /// of the image. That is why this method was implemented to provide the functionality.</remarks> 
    ///
    public static Bitmap Clone(Bitmap source, PixelFormat format)
    {
        // copy image if pixel format is the same
        if (source.PixelFormat == format)
            return Clone(source);

        int width = source.Width;
        int height = source.Height;

        // create new image with desired pixel format
        Bitmap bitmap = new Bitmap(width, height, format);

        // draw source image on the new one using Graphics
        Graphics g = Graphics.FromImage(bitmap);
        g.DrawImage(source, 0, 0, width, height);
        g.Dispose();

        return bitmap;
    }

    /// <summary>
    /// Clone image.
    /// </summary>
    /// 
    /// <param name="source">Source image.</param>
    /// 
    /// <returns>Return clone of the source image.</returns>
    /// 
    /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
    /// does not produce the desired result - it does not create an actual clone (it does not create a copy
    /// of the image). That is why this method was implemented to provide the functionality.</remarks> 
    /// 
    public static Bitmap Clone(Bitmap source)
    {
        // lock source bitmap data
        BitmapData sourceData = source.LockBits(
            new Rectangle(0, 0, source.Width, source.Height),
            ImageLockMode.ReadOnly, source.PixelFormat);

        // create new image
        Bitmap destination = Clone(sourceData);

        // unlock source image
        source.UnlockBits(sourceData);

        //
        if (
            (source.PixelFormat == PixelFormat.Format1bppIndexed) ||
            (source.PixelFormat == PixelFormat.Format4bppIndexed) ||
            (source.PixelFormat == PixelFormat.Format8bppIndexed) ||
            (source.PixelFormat == PixelFormat.Indexed))
        {
            ColorPalette srcPalette = source.Palette;
            ColorPalette dstPalette = destination.Palette;

            int n = srcPalette.Entries.Length;

            // copy pallete
            for (int i = 0; i < n; i++)
            {
                dstPalette.Entries[i] = srcPalette.Entries[i];
            }

            destination.Palette = dstPalette;
        }

        return destination;
    }

    /// <summary>
    /// Clone image.
    /// </summary>
    /// 
    /// <param name="sourceData">Source image data.</param>
    ///
    /// <returns>Clones image from source image data. The message does not clone pallete in the
    /// case if the source image has indexed pixel format.</returns>
    /// 
    public static Bitmap Clone(BitmapData sourceData)
    {
        // get source image size
        int width = sourceData.Width;
        int height = sourceData.Height;

        // create new image
        Bitmap destination = new Bitmap(width, height, sourceData.PixelFormat);

        // lock destination bitmap data
        BitmapData destinationData = destination.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.ReadWrite, destination.PixelFormat);

        SystemTools.CopyUnmanagedMemory(destinationData.Scan0, sourceData.Scan0, height * sourceData.Stride);

        // unlock destination image
        destination.UnlockBits(destinationData);

        return destination;
    }

    /// <summary>
    /// Format an image.
    /// </summary>
    /// 
    /// <param name="image">Source image to format.</param>
    /// 
    /// <remarks><para>Formats the image to one of the formats, which are supported
    /// by the <b>AForge.Imaging</b> library. The image is left untouched in the
    /// case if it is already of
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see> or
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppRgb</see> or
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppArgb</see> or
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format48bppRgb</see> or
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppArgb</see>
    /// format or it is <see cref="IsGrayscale">grayscale</see>, otherwise the image
    /// is converted to <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>
    /// format.</para>
    /// 
    /// <para><note>The method is deprecated and <see cref="Clone(Bitmap, PixelFormat)"/> method should
    /// be used instead with specifying desired pixel format.</note></para>
    /// </remarks>
    ///
    [Obsolete("Use Clone(Bitmap, PixelFormat) method instead and specify desired pixel format")]
    public static void FormatImage(ref Bitmap image)
    {
        if (
            (image.PixelFormat != PixelFormat.Format24bppRgb) &&
            (image.PixelFormat != PixelFormat.Format32bppRgb) &&
            (image.PixelFormat != PixelFormat.Format32bppArgb) &&
            (image.PixelFormat != PixelFormat.Format48bppRgb) &&
            (image.PixelFormat != PixelFormat.Format64bppArgb) &&
            (image.PixelFormat != PixelFormat.Format16bppGrayScale) &&
            (IsGrayscale(image) == false)
            )
        {
            Bitmap tmp = image;
            // convert to 24 bits per pixel
            image = Clone(tmp, PixelFormat.Format24bppRgb);
            // delete old image
            tmp.Dispose();
        }
    }

    /// <summary>
    /// Load bitmap from file.
    /// </summary>
    /// 
    /// <param name="fileName">File name to load bitmap from.</param>
    /// 
    /// <returns>Returns loaded bitmap.</returns>
    /// 
    /// <remarks><para>The method is provided as an alternative of <see cref="System.Drawing.Image.FromFile(string)"/>
    /// method to solve the issues of locked file. The standard .NET's method locks the source file until
    /// image's object is disposed, so the file can not be deleted or overwritten. This method workarounds the issue and
    /// does not lock the source file.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// Bitmap image = AForge.Imaging.Image.FromFile( "test.jpg" );
    /// </code>
    /// </remarks>
    /// 
    public static System.Drawing.Bitmap FromFile(string fileName)
    {
        Bitmap loadedImage = null;
        FileStream stream = null;

        try
        {
            // read image to temporary memory stream
            stream = File.OpenRead(fileName);
            MemoryStream memoryStream = new MemoryStream();

            byte[] buffer = new byte[10000];
            while (true)
            {
                int read = stream.Read(buffer, 0, 10000);

                if (read == 0)
                    break;

                memoryStream.Write(buffer, 0, read);
            }

            loadedImage = (Bitmap)Bitmap.FromStream(memoryStream);
        }
        finally
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }
        }

        return loadedImage;
    }

    /// <summary>
    /// Convert bitmap with 16 bits per plane to a bitmap with 8 bits per plane.
    /// </summary>
    /// 
    /// <param name="bimap">Source image to convert.</param>
    /// 
    /// <returns>Returns new image which is a copy of the source image but with 8 bits per plane.</returns>
    /// 
    /// <remarks><para>The routine does the next pixel format conversions:
    /// <list type="bullet">
    /// <item><see cref="PixelFormat.Format16bppGrayScale">Format16bppGrayScale</see> to
    /// <see cref="PixelFormat.Format8bppIndexed">Format8bppIndexed</see> with grayscale palette;</item>
    /// <item><see cref="PixelFormat.Format48bppRgb">Format48bppRgb</see> to
    /// <see cref="PixelFormat.Format24bppRgb">Format24bppRgb</see>;</item>
    /// <item><see cref="PixelFormat.Format64bppArgb">Format64bppArgb</see> to
    /// <see cref="PixelFormat.Format32bppArgb">Format32bppArgb</see>;</item>
    /// <item><see cref="PixelFormat.Format64bppPArgb">Format64bppPArgb</see> to
    /// <see cref="PixelFormat.Format32bppPArgb">Format32bppPArgb</see>.</item>
    /// </list>
    /// </para></remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Invalid pixel format of the source image.</exception>
    /// 
    public static Bitmap Convert16bppTo8bpp(Bitmap bimap)
    {
        Bitmap newImage = null;
        int layers = 0;

        // get image size
        int width = bimap.Width;
        int height = bimap.Height;

        // create new image depending on source image format
        switch (bimap.PixelFormat)
        {
            case PixelFormat.Format16bppGrayScale:
                // create new grayscale image
                newImage = CreateGrayscaleImage(width, height);
                layers = 1;
                break;

            case PixelFormat.Format48bppRgb:
                // create new color 24 bpp image
                newImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                layers = 3;
                break;

            case PixelFormat.Format64bppArgb:
                // create new color 32 bpp image
                newImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                layers = 4;
                break;

            case PixelFormat.Format64bppPArgb:
                // create new color 32 bpp image
                newImage = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                layers = 4;
                break;

            default:
                throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
        }

        // lock both images
        BitmapData sourceData = bimap.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly, bimap.PixelFormat);
        BitmapData newData = newImage.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadWrite, newImage.PixelFormat);

        unsafe
        {
            // base pointers
            byte* sourceBasePtr = (byte*)sourceData.Scan0.ToPointer();
            byte* newBasePtr = (byte*)newData.Scan0.ToPointer();
            // image strides
            int sourceStride = sourceData.Stride;
            int newStride = newData.Stride;

            for (int y = 0; y < height; y++)
            {
                ushort* sourcePtr = (ushort*)(sourceBasePtr + y * sourceStride);
                byte* newPtr = (byte*)(newBasePtr + y * newStride);

                for (int x = 0, lineSize = width * layers; x < lineSize; x++, sourcePtr++, newPtr++)
                {
                    *newPtr = (byte)(*sourcePtr >> 8);
                }
            }
        }

        // unlock both image
        bimap.UnlockBits(sourceData);
        newImage.UnlockBits(newData);

        return newImage;
    }

    /// <summary>
    /// Convert bitmap with 8 bits per plane to a bitmap with 16 bits per plane.
    /// </summary>
    /// 
    /// <param name="bimap">Source image to convert.</param>
    /// 
    /// <returns>Returns new image which is a copy of the source image but with 16 bits per plane.</returns>
    /// 
    /// <remarks><para>The routine does the next pixel format conversions:
    /// <list type="bullet">
    /// <item><see cref="PixelFormat.Format8bppIndexed">Format8bppIndexed</see> (grayscale palette assumed) to
    /// <see cref="PixelFormat.Format16bppGrayScale">Format16bppGrayScale</see>;</item>
    /// <item><see cref="PixelFormat.Format24bppRgb">Format24bppRgb</see> to
    /// <see cref="PixelFormat.Format48bppRgb">Format48bppRgb</see>;</item>
    /// <item><see cref="PixelFormat.Format32bppArgb">Format32bppArgb</see> to
    /// <see cref="PixelFormat.Format64bppArgb">Format64bppArgb</see>;</item>
    /// <item><see cref="PixelFormat.Format32bppPArgb">Format32bppPArgb</see> to
    /// <see cref="PixelFormat.Format64bppPArgb">Format64bppPArgb</see>.</item>
    /// </list>
    /// </para></remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Invalid pixel format of the source image.</exception>
    /// 
    public static Bitmap Convert8bppTo16bpp(Bitmap bimap)
    {
        Bitmap newImage = null;
        int layers = 0;

        // get image size
        int width = bimap.Width;
        int height = bimap.Height;

        // create new image depending on source image format
        switch (bimap.PixelFormat)
        {
            case PixelFormat.Format8bppIndexed:
                // create new grayscale image
                newImage = new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
                layers = 1;
                break;

            case PixelFormat.Format24bppRgb:
                // create new color 48 bpp image
                newImage = new Bitmap(width, height, PixelFormat.Format48bppRgb);
                layers = 3;
                break;

            case PixelFormat.Format32bppArgb:
                // create new color 64 bpp image
                newImage = new Bitmap(width, height, PixelFormat.Format64bppArgb);
                layers = 4;
                break;

            case PixelFormat.Format32bppPArgb:
                // create new color 64 bpp image
                newImage = new Bitmap(width, height, PixelFormat.Format64bppPArgb);
                layers = 4;
                break;

            default:
                throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
        }

        // lock both images
        BitmapData sourceData = bimap.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly, bimap.PixelFormat);
        BitmapData newData = newImage.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadWrite, newImage.PixelFormat);

        unsafe
        {
            // base pointers
            byte* sourceBasePtr = (byte*)sourceData.Scan0.ToPointer();
            byte* newBasePtr = (byte*)newData.Scan0.ToPointer();
            // image strides
            int sourceStride = sourceData.Stride;
            int newStride = newData.Stride;

            for (int y = 0; y < height; y++)
            {
                byte* sourcePtr = (byte*)(sourceBasePtr + y * sourceStride);
                ushort* newPtr = (ushort*)(newBasePtr + y * newStride);

                for (int x = 0, lineSize = width * layers; x < lineSize; x++, sourcePtr++, newPtr++)
                {
                    *newPtr = (ushort)(*sourcePtr << 8);
                }
            }
        }

        // unlock both image
        bimap.UnlockBits(sourceData);
        newImage.UnlockBits(newData);

        return newImage;
    }
}

public class UnmanagedImage : IDisposable
{
    // pointer to image data in unmanaged memory
    private IntPtr imageData;
    // image size
    private int width, height;
    // image stride (line size)
    private int stride;
    // image pixel format
    private PixelFormat pixelFormat;
    // flag which indicates if the image should be disposed or not
    private bool mustBeDisposed = false;

    /// <summary>
    /// Pointer to image data in unmanaged memory.
    /// </summary>
    public IntPtr ImageData
    {
        get { return imageData; }
    }

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width
    {
        get { return width; }
    }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height
    {
        get { return height; }
    }

    /// <summary>
    /// Image stride (line size in bytes).
    /// </summary>
    public int Stride
    {
        get { return stride; }
    }

    /// <summary>
    /// Image pixel format.
    /// </summary>
    public PixelFormat PixelFormat
    {
        get { return pixelFormat; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnmanagedImage"/> class.
    /// </summary>
    /// 
    /// <param name="imageData">Pointer to image data in unmanaged memory.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="stride">Image stride (line size in bytes).</param>
    /// <param name="pixelFormat">Image pixel format.</param>
    /// 
    /// <remarks><para><note>Using this constructor, make sure all specified image attributes are correct
    /// and correspond to unmanaged memory buffer. If some attributes are specified incorrectly,
    /// this may lead to exceptions working with the unmanaged memory.</note></para></remarks>
    /// 
    public UnmanagedImage(IntPtr imageData, int width, int height, int stride, PixelFormat pixelFormat)
    {
        this.imageData = imageData;
        this.width = width;
        this.height = height;
        this.stride = stride;
        this.pixelFormat = pixelFormat;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnmanagedImage"/> class.
    /// </summary>
    /// 
    /// <param name="bitmapData">Locked bitmap data.</param>
    /// 
    /// <remarks><note>Unlike <see cref="FromManagedImage(BitmapData)"/> method, this constructor does not make
    /// copy of managed image. This means that managed image must stay locked for the time of using the instance
    /// of unamanged image.</note></remarks>
    /// 
    public UnmanagedImage(BitmapData bitmapData)
    {
        this.imageData = bitmapData.Scan0;
        this.width = bitmapData.Width;
        this.height = bitmapData.Height;
        this.stride = bitmapData.Stride;
        this.pixelFormat = bitmapData.PixelFormat;
    }

    /// <summary>
    /// Destroys the instance of the <see cref="UnmanagedImage"/> class.
    /// </summary>
    /// 
    ~UnmanagedImage()
    {
        Dispose(false);
    }

    /// <summary>
    /// Dispose the object.
    /// </summary>
    /// 
    /// <remarks><para>Frees unmanaged resources used by the object. The object becomes unusable
    /// after that.</para>
    /// 
    /// <par><note>The method needs to be called only in the case if unmanaged image was allocated
    /// using <see cref="Create"/> method. In the case if the class instance was created using constructor,
    /// this method does not free unmanaged memory.</note></par>
    /// </remarks>
    /// 
    public void Dispose()
    {
        Dispose(true);
        // remove me from the Finalization queue 
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose the object.
    /// </summary>
    /// 
    /// <param name="disposing">Indicates if disposing was initiated manually.</param>
    /// 
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // dispose managed resources
        }
        // free image memory if the image was allocated using this class
        if ((mustBeDisposed) && (imageData != IntPtr.Zero))
        {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(imageData);
            imageData = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Clone the unmanaged images.
    /// </summary>
    /// 
    /// <returns>Returns clone of the unmanaged image.</returns>
    /// 
    /// <remarks><para>The method does complete cloning of the object.</para></remarks>
    /// 
    public UnmanagedImage Clone()
    {
        // allocate memory for the image
        IntPtr newImageData = System.Runtime.InteropServices.Marshal.AllocHGlobal(stride * height);

        UnmanagedImage newImage = new UnmanagedImage(newImageData, width, height, stride, pixelFormat);
        newImage.mustBeDisposed = true;

        SystemTools.CopyUnmanagedMemory(newImageData, imageData, stride * height);

        return newImage;
    }

    /// <summary>
    /// Copy unmanaged image.
    /// </summary>
    /// 
    /// <param name="destImage">Destination image to copy this image to.</param>
    /// 
    /// <remarks><para>The method copies current unmanaged image to the specified image.
    /// Size and pixel format of the destination image must be exactly the same.</para></remarks>
    /// 
    /// <exception cref="InvalidImagePropertiesException">Destination image has different size or pixel format.</exception>
    /// 
    public void Copy(UnmanagedImage destImage)
    {
        if (
            (width != destImage.width) || (height != destImage.height) ||
            (pixelFormat != destImage.pixelFormat))
        {
            throw new InvalidImagePropertiesException("Destination image has different size or pixel format.");
        }

        if (stride == destImage.stride)
        {
            // copy entire image
            SystemTools.CopyUnmanagedMemory(destImage.imageData, imageData, stride * height);
        }
        else
        {
            unsafe
            {
                int dstStride = destImage.stride;
                int copyLength = (stride < dstStride) ? stride : dstStride;

                byte* src = (byte*)imageData.ToPointer();
                byte* dst = (byte*)destImage.imageData.ToPointer();

                // copy line by line
                for (int i = 0; i < height; i++)
                {
                    SystemTools.CopyUnmanagedMemory(dst, src, copyLength);

                    dst += dstStride;
                    src += stride;
                }
            }
        }
    }

    /// <summary>
    /// Allocate new image in unmanaged memory.
    /// </summary>
    /// 
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="pixelFormat">Image pixel format.</param>
    /// 
    /// <returns>Return image allocated in unmanaged memory.</returns>
    /// 
    /// <remarks><para>Allocate new image with specified attributes in unmanaged memory.</para>
    /// 
    /// <para><note>The method supports only
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>,
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format16bppGrayScale</see>,
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>,
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppRgb</see>,
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppArgb</see>,
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppPArgb</see>,
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format48bppRgb</see>,
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppArgb</see> and
    /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppPArgb</see> pixel formats.
    /// In the case if <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
    /// format is specified, pallete is not not created for the image (supposed that it is
    /// 8 bpp grayscale image).
    /// </note></para>
    /// </remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format was specified.</exception>
    /// <exception cref="InvalidImagePropertiesException">Invalid image size was specified.</exception>
    /// 
    public static UnmanagedImage Create(int width, int height, PixelFormat pixelFormat)
    {
        int bytesPerPixel = 0;

        // calculate bytes per pixel
        switch (pixelFormat)
        {
            case PixelFormat.Format8bppIndexed:
                bytesPerPixel = 1;
                break;
            case PixelFormat.Format16bppGrayScale:
                bytesPerPixel = 2;
                break;
            case PixelFormat.Format24bppRgb:
                bytesPerPixel = 3;
                break;
            case PixelFormat.Format32bppRgb:
            case PixelFormat.Format32bppArgb:
            case PixelFormat.Format32bppPArgb:
                bytesPerPixel = 4;
                break;
            case PixelFormat.Format48bppRgb:
                bytesPerPixel = 6;
                break;
            case PixelFormat.Format64bppArgb:
            case PixelFormat.Format64bppPArgb:
                bytesPerPixel = 8;
                break;
            default:
                throw new UnsupportedImageFormatException("Can not create image with specified pixel format.");
        }

        // check image size
        if ((width <= 0) || (height <= 0))
        {
            throw new InvalidImagePropertiesException("Invalid image size specified.");
        }

        // calculate stride
        int stride = width * bytesPerPixel;

        if (stride % 4 != 0)
        {
            stride += (4 - (stride % 4));
        }

        // allocate memory for the image
        IntPtr imageData = System.Runtime.InteropServices.Marshal.AllocHGlobal(stride * height);
        SystemTools.SetUnmanagedMemory(imageData, 0, stride * height);

        UnmanagedImage image = new UnmanagedImage(imageData, width, height, stride, pixelFormat);
        image.mustBeDisposed = true;

        return image;
    }

    /// <summary>
    /// Create managed image from the unmanaged.
    /// </summary>
    /// 
    /// <returns>Returns managed copy of the unmanaged image.</returns>
    /// 
    /// <remarks><para>The method creates a managed copy of the unmanaged image with the
    /// same size and pixel format (it calls <see cref="ToManagedImage(bool)"/> specifying
    /// <see langword="true"/> for the <b>makeCopy</b> parameter).</para></remarks>
    /// 
    public Bitmap ToManagedImage()
    {
        return ToManagedImage(true);
    }

    /// <summary>
    /// Create managed image from the unmanaged.
    /// </summary>
    /// 
    /// <param name="makeCopy">Make a copy of the unmanaged image or not.</param>
    /// 
    /// <returns>Returns managed copy of the unmanaged image.</returns>
    /// 
    /// <remarks><para>If the <paramref name="makeCopy"/> is set to <see langword="true"/>, then the method
    /// creates a managed copy of the unmanaged image, so the managed image stays valid even when the unmanaged
    /// image gets disposed. However, setting this parameter to <see langword="false"/> creates a managed image which is
    /// just a wrapper around the unmanaged image. So if unmanaged image is disposed, the
    /// managed image becomes no longer valid and accessing it will generate an exception.</para></remarks>
    /// 
    /// <exception cref="InvalidImagePropertiesException">The unmanaged image has some invalid properties, which results
    /// in failure of converting it to managed image. This may happen if user used the
    /// <see cref="UnmanagedImage(IntPtr, int, int, int, PixelFormat)"/> constructor specifying some
    /// invalid parameters.</exception>
    /// 
    public Bitmap ToManagedImage(bool makeCopy)
    {
        Bitmap dstImage = null;

        try
        {
            if (!makeCopy)
            {
                dstImage = new Bitmap(width, height, stride, pixelFormat, imageData);
                if (pixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Image.SetGrayscalePalette(dstImage);
                }
            }
            else
            {
                // create new image of required format
                dstImage = (pixelFormat == PixelFormat.Format8bppIndexed) ?
                    Image.CreateGrayscaleImage(width, height) :
                    new Bitmap(width, height, pixelFormat);

                // lock destination bitmap data
                BitmapData dstData = dstImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite, pixelFormat);

                int dstStride = dstData.Stride;
                int lineSize = Math.Min(stride, dstStride);

                unsafe
                {
                    byte* dst = (byte*)dstData.Scan0.ToPointer();
                    byte* src = (byte*)imageData.ToPointer();

                    if (stride != dstStride)
                    {
                        // copy image
                        for (int y = 0; y < height; y++)
                        {
                            SystemTools.CopyUnmanagedMemory(dst, src, lineSize);
                            dst += dstStride;
                            src += stride;
                        }
                    }
                    else
                    {
                        SystemTools.CopyUnmanagedMemory(dst, src, stride * height);
                    }
                }

                // unlock destination images
                dstImage.UnlockBits(dstData);
            }

            return dstImage;
        }
        catch (Exception)
        {
            if (dstImage != null)
            {
                dstImage.Dispose();
            }

            throw new InvalidImagePropertiesException("The unmanaged image has some invalid properties, which results in failure of converting it to managed image.");
        }
    }

    /// <summary>
    /// Create unmanaged image from the specified managed image.
    /// </summary>
    /// 
    /// <param name="image">Source managed image.</param>
    /// 
    /// <returns>Returns new unmanaged image, which is a copy of source managed image.</returns>
    /// 
    /// <remarks><para>The method creates an exact copy of specified managed image, but allocated
    /// in unmanaged memory.</para></remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of source image.</exception>
    /// 
    public static UnmanagedImage FromManagedImage(Bitmap image)
    {
        UnmanagedImage dstImage = null;

        BitmapData sourceData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, image.PixelFormat);

        try
        {
            dstImage = FromManagedImage(sourceData);
        }
        finally
        {
            image.UnlockBits(sourceData);
        }

        return dstImage;
    }

    /// <summary>
    /// Create unmanaged image from the specified managed image.
    /// </summary>
    /// 
    /// <param name="imageData">Source locked image data.</param>
    /// 
    /// <returns>Returns new unmanaged image, which is a copy of source managed image.</returns>
    /// 
    /// <remarks><para>The method creates an exact copy of specified managed image, but allocated
    /// in unmanaged memory. This means that managed image may be unlocked right after call to this
    /// method.</para></remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of source image.</exception>
    /// 
    public static UnmanagedImage FromManagedImage(BitmapData imageData)
    {
        PixelFormat pixelFormat = imageData.PixelFormat;

        // check source pixel format
        if (
            (pixelFormat != PixelFormat.Format8bppIndexed) &&
            (pixelFormat != PixelFormat.Format16bppGrayScale) &&
            (pixelFormat != PixelFormat.Format24bppRgb) &&
            (pixelFormat != PixelFormat.Format32bppRgb) &&
            (pixelFormat != PixelFormat.Format32bppArgb) &&
            (pixelFormat != PixelFormat.Format32bppPArgb) &&
            (pixelFormat != PixelFormat.Format48bppRgb) &&
            (pixelFormat != PixelFormat.Format64bppArgb) &&
            (pixelFormat != PixelFormat.Format64bppPArgb))
        {
            throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
        }

        // allocate memory for the image
        IntPtr dstImageData = System.Runtime.InteropServices.Marshal.AllocHGlobal(imageData.Stride * imageData.Height);

        UnmanagedImage image = new UnmanagedImage(dstImageData, imageData.Width, imageData.Height, imageData.Stride, pixelFormat);
        SystemTools.CopyUnmanagedMemory(dstImageData, imageData.Scan0, imageData.Stride * imageData.Height);
        image.mustBeDisposed = true;

        return image;
    }

    /// <summary>
    /// Collect pixel values from the specified list of coordinates.
    /// </summary>
    /// 
    /// <param name="points">List of coordinates to collect pixels' value from.</param>
    /// 
    /// <returns>Returns array of pixels' values from the specified coordinates.</returns>
    /// 
    /// <remarks><para>The method goes through the specified list of points and for each point retrievs
    /// corresponding pixel's value from the unmanaged image.</para>
    /// 
    /// <para><note>For grayscale image the output array has the same length as number of points in the
    /// specified list of points. For color image the output array has triple length, containing pixels'
    /// values in RGB order.</note></para>
    /// 
    /// <para><note>The method does not make any checks for valid coordinates and leaves this up to user.
    /// If specified coordinates are out of image's bounds, the result is not predictable (crash in most cases).
    /// </note></para>
    /// 
    /// <para><note>This method is supposed for images with 8 bpp channels only (8 bpp grayscale image and
    /// 24/32 bpp color images).</note></para>
    /// </remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image. Use Collect16bppPixelValues() method for
    /// images with 16 bpp channels.</exception>
    /// 
    public byte[] Collect8bppPixelValues(List<IntPoint> points)
    {
        int pixelSize = Bitmap.GetPixelFormatSize(pixelFormat) / 8;

        if ((pixelFormat == PixelFormat.Format16bppGrayScale) || (pixelSize > 4))
        {
            throw new UnsupportedImageFormatException("Unsupported pixel format of the source image. Use Collect16bppPixelValues() method for it.");
        }

        byte[] pixelValues = new byte[points.Count * ((pixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3)];

        unsafe
        {
            byte* basePtr = (byte*)imageData.ToPointer();
            byte* ptr;

            if (pixelFormat == PixelFormat.Format8bppIndexed)
            {
                int i = 0;

                foreach (IntPoint point in points)
                {
                    ptr = basePtr + stride * point.Y + point.X;
                    pixelValues[i++] = *ptr;
                }
            }
            else
            {
                int i = 0;

                foreach (IntPoint point in points)
                {
                    ptr = basePtr + stride * point.Y + point.X * pixelSize;
                    pixelValues[i++] = ptr[RGB.R];
                    pixelValues[i++] = ptr[RGB.G];
                    pixelValues[i++] = ptr[RGB.B];
                }
            }
        }

        return pixelValues;
    }

    /// <summary>
    /// Collect coordinates of none black pixels in the image.
    /// </summary>
    /// 
    /// <returns>Returns list of points, which have other than black color.</returns>
    /// 
    public List<IntPoint> CollectActivePixels()
    {
        return CollectActivePixels(new Rectangle(0, 0, width, height));
    }

    /// <summary>
    /// Collect coordinates of none black pixels within specified rectangle of the image.
    /// </summary>
    /// 
    /// <param name="rect">Image's rectangle to process.</param>
    /// 
    /// <returns>Returns list of points, which have other than black color.</returns>
    ///
    public List<IntPoint> CollectActivePixels(Rectangle rect)
    {
        List<IntPoint> pixels = new List<IntPoint>();

        int pixelSize = Bitmap.GetPixelFormatSize(pixelFormat) / 8;

        // correct rectangle
        rect.Intersect(new Rectangle(0, 0, width, height));

        int startX = rect.X;
        int startY = rect.Y;
        int stopX = rect.Right;
        int stopY = rect.Bottom;

        unsafe
        {
            byte* basePtr = (byte*)imageData.ToPointer();

            if ((pixelFormat == PixelFormat.Format16bppGrayScale) || (pixelSize > 4))
            {
                int pixelWords = pixelSize >> 1;

                for (int y = startY; y < stopY; y++)
                {
                    ushort* ptr = (ushort*)(basePtr + y * stride + startX * pixelSize);

                    if (pixelWords == 1)
                    {
                        // grayscale images
                        for (int x = startX; x < stopX; x++, ptr++)
                        {
                            if (*ptr != 0)
                            {
                                pixels.Add(new IntPoint(x, y));
                            }
                        }
                    }
                    else
                    {
                        // color images
                        for (int x = startX; x < stopX; x++, ptr += pixelWords)
                        {
                            if ((ptr[RGB.R] != 0) || (ptr[RGB.G] != 0) || (ptr[RGB.B] != 0))
                            {
                                pixels.Add(new IntPoint(x, y));
                            }
                        }
                    }
                }
            }
            else
            {
                for (int y = startY; y < stopY; y++)
                {
                    byte* ptr = basePtr + y * stride + startX * pixelSize;

                    if (pixelSize == 1)
                    {
                        // grayscale images
                        for (int x = startX; x < stopX; x++, ptr++)
                        {
                            if (*ptr != 0)
                            {
                                pixels.Add(new IntPoint(x, y));
                            }
                        }
                    }
                    else
                    {
                        // color images
                        for (int x = startX; x < stopX; x++, ptr += pixelSize)
                        {
                            if ((ptr[RGB.R] != 0) || (ptr[RGB.G] != 0) || (ptr[RGB.B] != 0))
                            {
                                pixels.Add(new IntPoint(x, y));
                            }
                        }
                    }
                }
            }
        }

        return pixels;
    }

    /// <summary>
    /// Set pixels with the specified coordinates to the specified color.
    /// </summary>
    /// 
    /// <param name="coordinates">List of points to set color for.</param>
    /// <param name="color">Color to set for the specified points.</param>
    /// 
    /// <remarks><para><note>For images having 16 bpp per color plane, the method extends the specified color
    /// value to 16 bit by multiplying it by 256.</note></para></remarks>
    ///
    public void SetPixels(List<IntPoint> coordinates, Color color)
    {
        unsafe
        {
            int pixelSize = Bitmap.GetPixelFormatSize(pixelFormat) / 8;
            byte* basePtr = (byte*)imageData.ToPointer();

            byte red = color.R;
            byte green = color.G;
            byte blue = color.B;
            byte alpha = color.A;

            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    {
                        byte grayValue = (byte)(0.2125 * red + 0.7154 * green + 0.0721 * blue);

                        foreach (IntPoint point in coordinates)
                        {
                            if ((point.X >= 0) && (point.Y >= 0) && (point.X < width) && (point.Y < height))
                            {
                                byte* ptr = basePtr + point.Y * stride + point.X;
                                *ptr = grayValue;
                            }
                        }
                    }
                    break;

                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    {


                        foreach (IntPoint point in coordinates)
                        {
                            if ((point.X >= 0) && (point.Y >= 0) && (point.X < width) && (point.Y < height))
                            {
                                byte* ptr = basePtr + point.Y * stride + point.X * pixelSize;
                                ptr[RGB.R] = red;
                                ptr[RGB.G] = green;
                                ptr[RGB.B] = blue;
                            }
                        }
                    }
                    break;

                case PixelFormat.Format32bppArgb:
                    {
                        foreach (IntPoint point in coordinates)
                        {
                            if ((point.X >= 0) && (point.Y >= 0) && (point.X < width) && (point.Y < height))
                            {
                                byte* ptr = basePtr + point.Y * stride + point.X * pixelSize;
                                ptr[RGB.R] = red;
                                ptr[RGB.G] = green;
                                ptr[RGB.B] = blue;
                                ptr[RGB.A] = alpha;
                            }
                        }
                    }
                    break;

                case PixelFormat.Format16bppGrayScale:
                    {
                        ushort grayValue = (ushort)((ushort)(0.2125 * red + 0.7154 * green + 0.0721 * blue) << 8);

                        foreach (IntPoint point in coordinates)
                        {
                            if ((point.X >= 0) && (point.Y >= 0) && (point.X < width) && (point.Y < height))
                            {
                                ushort* ptr = (ushort*)(basePtr + point.Y * stride) + point.X;
                                *ptr = grayValue;
                            }
                        }
                    }
                    break;

                case PixelFormat.Format48bppRgb:
                    {
                        ushort red16 = (ushort)(red << 8);
                        ushort green16 = (ushort)(green << 8);
                        ushort blue16 = (ushort)(blue << 8);

                        foreach (IntPoint point in coordinates)
                        {
                            if ((point.X >= 0) && (point.Y >= 0) && (point.X < width) && (point.Y < height))
                            {
                                ushort* ptr = (ushort*)(basePtr + point.Y * stride + point.X * pixelSize);
                                ptr[RGB.R] = red16;
                                ptr[RGB.G] = green16;
                                ptr[RGB.B] = blue16;
                            }
                        }
                    }
                    break;

                case PixelFormat.Format64bppArgb:
                    {
                        ushort red16 = (ushort)(red << 8);
                        ushort green16 = (ushort)(green << 8);
                        ushort blue16 = (ushort)(blue << 8);
                        ushort alpha16 = (ushort)(alpha << 8);

                        foreach (IntPoint point in coordinates)
                        {
                            if ((point.X >= 0) && (point.Y >= 0) && (point.X < width) && (point.Y < height))
                            {
                                ushort* ptr = (ushort*)(basePtr + point.Y * stride + point.X * pixelSize);
                                ptr[RGB.R] = red16;
                                ptr[RGB.G] = green16;
                                ptr[RGB.B] = blue16;
                                ptr[RGB.A] = alpha16;
                            }
                        }
                    }
                    break;

                default:
                    throw new UnsupportedImageFormatException("The pixel format is not supported: " + pixelFormat);
            }
        }
    }

    /// <summary>
    /// Set pixel with the specified coordinates to the specified color.
    /// </summary>
    /// 
    /// <param name="point">Point's coordiates to set color for.</param>
    /// <param name="color">Color to set for the pixel.</param>
    /// 
    /// <remarks><para>See <see cref="SetPixel(int, int, Color)"/> for more information.</para></remarks>
    ///
    public void SetPixel(IntPoint point, Color color)
    {
        SetPixel(point.X, point.Y, color);
    }

    /// <summary>
    /// Set pixel with the specified coordinates to the specified color.
    /// </summary>
    /// 
    /// <param name="x">X coordinate of the pixel to set.</param>
    /// <param name="y">Y coordinate of the pixel to set.</param>
    /// <param name="color">Color to set for the pixel.</param>
    /// 
    /// <remarks><para><note>For images having 16 bpp per color plane, the method extends the specified color
    /// value to 16 bit by multiplying it by 256.</note></para>
    /// 
    /// <para>For grayscale images this method will calculate intensity value based on the below formula:
    /// <code lang="none">
    /// 0.2125 * Red + 0.7154 * Green + 0.0721 * Blue
    /// </code>
    /// </para>
    /// </remarks>
    /// 
    public void SetPixel(int x, int y, Color color)
    {
        SetPixel(x, y, color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Set pixel with the specified coordinates to the specified value.
    /// </summary>
    ///
    /// <param name="x">X coordinate of the pixel to set.</param>
    /// <param name="y">Y coordinate of the pixel to set.</param>
    /// <param name="value">Pixel value to set.</param>
    /// 
    /// <remarks><para>The method sets all color components of the pixel to the specified value.
    /// If it is a grayscale image, then pixel's intensity is set to the specified value.
    /// If it is a color image, then pixel's R/G/B components are set to the same specified value
    /// (if an image has alpha channel, then it is set to maximum value - 255 or 65535).</para>
    /// 
    /// <para><note>For images having 16 bpp per color plane, the method extends the specified color
    /// value to 16 bit by multiplying it by 256.</note></para>
    /// </remarks>
    /// 
    public void SetPixel(int x, int y, byte value)
    {
        SetPixel(x, y, value, value, value, 255);
    }

    private void SetPixel(int x, int y, byte r, byte g, byte b, byte a)
    {
        if ((x >= 0) && (y >= 0) && (x < width) && (y < height))
        {
            unsafe
            {
                int pixelSize = Bitmap.GetPixelFormatSize(pixelFormat) / 8;
                byte* ptr = (byte*)imageData.ToPointer() + y * stride + x * pixelSize;
                ushort* ptr2 = (ushort*)ptr;

                switch (pixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        *ptr = (byte)(0.2125 * r + 0.7154 * g + 0.0721 * b);
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                        ptr[RGB.R] = r;
                        ptr[RGB.G] = g;
                        ptr[RGB.B] = b;
                        break;

                    case PixelFormat.Format32bppArgb:
                        ptr[RGB.R] = r;
                        ptr[RGB.G] = g;
                        ptr[RGB.B] = b;
                        ptr[RGB.A] = a;
                        break;

                    case PixelFormat.Format16bppGrayScale:
                        *ptr2 = (ushort)((ushort)(0.2125 * r + 0.7154 * g + 0.0721 * b) << 8);
                        break;

                    case PixelFormat.Format48bppRgb:
                        ptr2[RGB.R] = (ushort)(r << 8);
                        ptr2[RGB.G] = (ushort)(g << 8);
                        ptr2[RGB.B] = (ushort)(b << 8);
                        break;

                    case PixelFormat.Format64bppArgb:
                        ptr2[RGB.R] = (ushort)(r << 8);
                        ptr2[RGB.G] = (ushort)(g << 8);
                        ptr2[RGB.B] = (ushort)(b << 8);
                        ptr2[RGB.A] = (ushort)(a << 8);
                        break;

                    default:
                        throw new UnsupportedImageFormatException("The pixel format is not supported: " + pixelFormat);
                }
            }
        }
    }

    /// <summary>
    /// Get color of the pixel with the specified coordinates.
    /// </summary>
    /// 
    /// <param name="point">Point's coordiates to get color of.</param>
    /// 
    /// <returns>Return pixel's color at the specified coordinates.</returns>
    /// 
    /// <remarks><para>See <see cref="GetPixel(int, int)"/> for more information.</para></remarks>
    ///
    public Color GetPixel(IntPoint point)
    {
        return GetPixel(point.X, point.Y);
    }

    /// <summary>
    /// Get color of the pixel with the specified coordinates.
    /// </summary>
    /// 
    /// <param name="x">X coordinate of the pixel to get.</param>
    /// <param name="y">Y coordinate of the pixel to get.</param>
    /// 
    /// <returns>Return pixel's color at the specified coordinates.</returns>
    /// 
    /// <remarks>
    /// <para><note>In the case if the image has 8 bpp grayscale format, the method will return a color with
    /// all R/G/B components set to same value, which is grayscale intensity.</note></para>
    /// 
    /// <para><note>The method supports only 8 bpp grayscale images and 24/32 bpp color images so far.</note></para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentOutOfRangeException">The specified pixel coordinate is out of image's bounds.</exception>
    /// <exception cref="UnsupportedImageFormatException">Pixel format of this image is not supported by the method.</exception>
    /// 
    public Color GetPixel(int x, int y)
    {
        if ((x < 0) || (y < 0))
        {
            throw new ArgumentOutOfRangeException("x", "The specified pixel coordinate is out of image's bounds.");
        }

        if ((x >= width) || (y >= height))
        {
            throw new ArgumentOutOfRangeException("y", "The specified pixel coordinate is out of image's bounds.");
        }

        Color color = new Color();

        unsafe
        {
            int pixelSize = Bitmap.GetPixelFormatSize(pixelFormat) / 8;
            byte* ptr = (byte*)imageData.ToPointer() + y * stride + x * pixelSize;

            switch (pixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    color = Color.FromArgb(*ptr, *ptr, *ptr);
                    break;

                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    color = Color.FromArgb(ptr[RGB.R], ptr[RGB.G], ptr[RGB.B]);
                    break;

                case PixelFormat.Format32bppArgb:
                    color = Color.FromArgb(ptr[RGB.A], ptr[RGB.R], ptr[RGB.G], ptr[RGB.B]);
                    break;

                default:
                    throw new UnsupportedImageFormatException("The pixel format is not supported: " + pixelFormat);
            }
        }

        return color;
    }

    /// <summary>
    /// Collect pixel values from the specified list of coordinates.
    /// </summary>
    /// 
    /// <param name="points">List of coordinates to collect pixels' value from.</param>
    /// 
    /// <returns>Returns array of pixels' values from the specified coordinates.</returns>
    /// 
    /// <remarks><para>The method goes through the specified list of points and for each point retrievs
    /// corresponding pixel's value from the unmanaged image.</para>
    /// 
    /// <para><note>For grayscale image the output array has the same length as number of points in the
    /// specified list of points. For color image the output array has triple length, containing pixels'
    /// values in RGB order.</note></para>
    /// 
    /// <para><note>The method does not make any checks for valid coordinates and leaves this up to user.
    /// If specified coordinates are out of image's bounds, the result is not predictable (crash in most cases).
    /// </note></para>
    /// 
    /// <para><note>This method is supposed for images with 16 bpp channels only (16 bpp grayscale image and
    /// 48/64 bpp color images).</note></para>
    /// </remarks>
    /// 
    /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image. Use Collect8bppPixelValues() method for
    /// images with 8 bpp channels.</exception>
    ///
    public ushort[] Collect16bppPixelValues(List<IntPoint> points)
    {
        int pixelSize = Bitmap.GetPixelFormatSize(pixelFormat) / 8;

        if ((pixelFormat == PixelFormat.Format8bppIndexed) || (pixelSize == 3) || (pixelSize == 4))
        {
            throw new UnsupportedImageFormatException("Unsupported pixel format of the source image. Use Collect8bppPixelValues() method for it.");
        }

        ushort[] pixelValues = new ushort[points.Count * ((pixelFormat == PixelFormat.Format16bppGrayScale) ? 1 : 3)];

        unsafe
        {
            byte* basePtr = (byte*)imageData.ToPointer();
            ushort* ptr;

            if (pixelFormat == PixelFormat.Format16bppGrayScale)
            {
                int i = 0;

                foreach (IntPoint point in points)
                {
                    ptr = (ushort*)(basePtr + stride * point.Y + point.X * pixelSize);
                    pixelValues[i++] = *ptr;
                }
            }
            else
            {
                int i = 0;

                foreach (IntPoint point in points)
                {
                    ptr = (ushort*)(basePtr + stride * point.Y + point.X * pixelSize);
                    pixelValues[i++] = ptr[RGB.R];
                    pixelValues[i++] = ptr[RGB.G];
                    pixelValues[i++] = ptr[RGB.B];
                }

            }

            return pixelValues;
        }
    }
}

public class Class1
{
    public void displayMessage()
    {
        MessageBox.Show("Hello From AForge!", "Sample");
    }
}
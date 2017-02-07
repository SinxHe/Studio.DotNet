// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
//using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Sinx.Net.Http.Headers
{
    public static class HeaderUtilities
    {
		private static readonly int _int64MaxStringLength = 20;
        private static unsafe bool TryParseInt64FromHeaderValue(int startIndex, string headerValue, out long result)
        {
            // Trim leading whitespace
            startIndex += HttpRuleParser.GetWhitespaceLength(headerValue, startIndex);

            // Match and skip '=', it also can't be the last character in the headerValue
            if (startIndex >= headerValue.Length - 1 || headerValue[startIndex] != '=')
            {
                result = 0;
                return false;
            }
            startIndex++;

            // Trim trailing whitespace
            startIndex += HttpRuleParser.GetWhitespaceLength(headerValue, startIndex);

            // Try parse the number
            if (TryParseInt64(new StringSegment(headerValue, startIndex, HttpRuleParser.GetNumberLength(headerValue, startIndex, false)), out result))
            {
                return true;
            }

            result = 0;
            return false;
        }
        /// <summary>
        /// Try to convert a string representation of a positive number to its 64-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="value">
        /// A string containing a number to convert.
        /// </param>
        /// <param name="result">
        /// When this method returns, contains the 64-bit signed integer value equivalent of the number contained
        /// in the string, if the conversion succeeded, or zero if the conversion failed. The conversion fails if
        /// the string is null or String.Empty, is not of the correct format, is negative, or represents a number
        /// greater than Int64.MaxValue. This parameter is passed uninitialized; any value originally supplied in
        /// result will be overwritten.
        /// </param>
        /// <returns><code>true</code> if parsing succeeded; otherwise, <code>false</code>.</returns>
        public static bool TryParseInt64(string value, out long result)
        {
            return TryParseInt64(new StringSegment(value), out result);
        }
        /// <summary>
        /// Try to convert a <see cref="StringSegment"/> representation of a positive number to its 64-bit signed
        /// integer equivalent. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="value">
        /// A <see cref="StringSegment"/> containing a number to convert.
        /// </param>
        /// <param name="result">
        /// When this method returns, contains the 64-bit signed integer value equivalent of the number contained
        /// in the string, if the conversion succeeded, or zero if the conversion failed. The conversion fails if
        /// the <see cref="StringSegment"/> is null or String.Empty, is not of the correct format, is negative, or
        /// represents a number greater than Int64.MaxValue. This parameter is passed uninitialized; any value
        /// originally supplied in result will be overwritten.
        /// </param>
        /// <returns><code>true</code> if parsing succeeded; otherwise, <code>false</code>.</returns>
        public static unsafe bool TryParseInt64(StringSegment value, out long result)
        {
            if (string.IsNullOrEmpty(value.Buffer) || value.Length == 0)
            {
                result = 0;
                return false;
            }

            result = 0;
            fixed (char* ptr = value.Buffer)
            {
                var ch = (ushort*)ptr + value.Offset;
                var end = ch + value.Length;

                ushort digit = 0;
                while (ch < end && (digit = (ushort)(*ch - 0x30)) <= 9)
                {
                    // Check for overflow
                    if ((result = result * 10 + digit) < 0)
                    {
                        result = 0;
                        return false;
                    }

                    ch++;
                }

                if (ch != end)
                {
                    result = 0;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Converts the signed 64-bit numeric value to its equivalent string representation.
        /// </summary>
        /// <param name="value">
        /// The number to convert.
        /// </param>
        /// <returns>
        /// The string representation of the value of this instance, consisting of a minus sign if the value is
        /// negative, and a sequence of digits ranging from 0 to 9 with no leading zeroes.
        /// </returns>
        public static unsafe string FormatInt64(long value)
        {
            var position = _int64MaxStringLength;
            var negative = false;

            if (value < 0)
            {
                // Not possible to compute absolute value of MinValue, return the exact string instead.
                if (value == long.MinValue)
                {
                    return "-9223372036854775808";
                }
                negative = true;
                value = -value;
            }

            char* charBuffer = stackalloc char[_int64MaxStringLength];

            do
            {
                // Consider using Math.DivRem() if available
                var quotient = value / 10;
                charBuffer[--position] = (char)(0x30 + (value - quotient * 10)); // 0x30 = '0'
                value = quotient;
            }
            while (value != 0);

            if (negative)
            {
                charBuffer[--position] = '-';
            }

            return new string(charBuffer, position, _int64MaxStringLength - position);
        }
    }
}

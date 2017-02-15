// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace Sinx.Net.Http.Headers
{
    public static class HeaderUtilities
    {
        private static readonly int _int64MaxStringLength = 20;
        private const string QualityName = "q";
        internal const string BytesUnit = "bytes";

        internal static void SetQuality(IList<NameValueHeaderValue> parameters, double? value)
        {
            Contract.Requires(parameters != null);

            var qualityParameter = NameValueHeaderValue.Find(parameters, QualityName);
            if (value.HasValue)
            {
                // Note that even if we check the value here, we can't prevent a user from adding an invalid quality
                // value using Parameters.Add(). Even if we would prevent the user from adding an invalid value
                // using Parameters.Add() he could always add invalid values using HttpHeaders.AddWithoutValidation().
                // So this check is really for convenience to show users that they're trying to add an invalid
                // value.
                if ((value < 0) || (value > 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                var qualityString = ((double)value).ToString("0.0##", NumberFormatInfo.InvariantInfo);
                if (qualityParameter != null)
                {
                    qualityParameter.Value = qualityString;
                }
                else
                {
                    parameters.Add(new NameValueHeaderValue(QualityName, qualityString));
                }
            }
            else
            {
                // Remove quality parameter
                if (qualityParameter != null)
                {
                    parameters.Remove(qualityParameter);
                }
            }
        }

        internal static double? GetQuality(IList<NameValueHeaderValue> parameters)
        {
            Contract.Requires(parameters != null);

            var qualityParameter = NameValueHeaderValue.Find(parameters, QualityName);
            if (qualityParameter != null)
            {
                // Note that the RFC requires decimal '.' regardless of the culture. I.e. using ',' as decimal
                // separator is considered invalid (even if the current culture would allow it).
                double qualityValue;
                if (double.TryParse(qualityParameter.Value, NumberStyles.AllowDecimalPoint,
                    NumberFormatInfo.InvariantInfo, out qualityValue))
                {
                    return qualityValue;
                }
            }
            return null;
        }

        internal static void CheckValidToken(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("An empty string is not allowed.", parameterName);
            }

            if (HttpRuleParser.GetTokenLength(value, 0) != value.Length)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid token '{0}.", value));
            }
        }

        internal static void CheckValidQuotedString(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("An empty string is not allowed.", parameterName);
            }

            int length;
            if ((HttpRuleParser.GetQuotedStringLength(value, 0, out length) != HttpParseResult.Parsed) ||
                (length != value.Length)) // no trailing spaces allowed
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Invalid quoted string '{0}'.", value));
            }
        }

        internal static bool AreEqualCollections<T>(ICollection<T> x, ICollection<T> y)
        {
            return AreEqualCollections(x, y, null);
        }

        internal static bool AreEqualCollections<T>(ICollection<T> x, ICollection<T> y, IEqualityComparer<T> comparer)
        {
            if (x == null)
            {
                return (y == null) || (y.Count == 0);
            }

            if (y == null)
            {
                return (x.Count == 0);
            }

            if (x.Count != y.Count)
            {
                return false;
            }

            if (x.Count == 0)
            {
                return true;
            }

            // We have two unordered lists. So comparison is an O(n*m) operation which is expensive. Usually
            // headers have 1-2 parameters (if any), so this comparison shouldn't be too expensive.
            var alreadyFound = new bool[x.Count];
            var i = 0;
            foreach (var xItem in x)
            {
                Contract.Assert(xItem != null);

                i = 0;
                var found = false;
                foreach (var yItem in y)
                {
                    if (!alreadyFound[i])
                    {
                        if (((comparer == null) && xItem.Equals(yItem)) ||
                            ((comparer != null) && comparer.Equals(xItem, yItem)))
                        {
                            alreadyFound[i] = true;
                            found = true;
                            break;
                        }
                    }
                    i++;
                }

                if (!found)
                {
                    return false;
                }
            }

            // Since we never re-use a "found" value in 'y', we expecte 'alreadyFound' to have all fields set to 'true'.
            // Otherwise the two collections can't be equal and we should not get here.
            Contract.Assert(Contract.ForAll(alreadyFound, value => { return value; }),
                "Expected all values in 'alreadyFound' to be true since collections are considered equal.");

            return true;
        }

        internal static int GetNextNonEmptyOrWhitespaceIndex(
            string input,
            int startIndex,
            bool skipEmptyValues,
            out bool separatorFound)
        {
            Contract.Requires(input != null);
            Contract.Requires(startIndex <= input.Length); // it's OK if index == value.Length.

            separatorFound = false;
            var current = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);

            if ((current == input.Length) || (input[current] != ','))
            {
                return current;
            }

            // If we have a separator, skip the separator and all following whitespaces. If we support
            // empty values, continue until the current character is neither a separator nor a whitespace.
            separatorFound = true;
            current++; // skip delimiter.
            current = current + HttpRuleParser.GetWhitespaceLength(input, current);

            if (skipEmptyValues)
            {
                while ((current < input.Length) && (input[current] == ','))
                {
                    current++; // skip delimiter.
                    current = current + HttpRuleParser.GetWhitespaceLength(input, current);
                }
            }

            return current;
        }

        private static int AdvanceCacheDirectiveIndex(int current, string headerValue)
        {
            // Skip until the next potential name
            current += HttpRuleParser.GetWhitespaceLength(headerValue, current);

            // Skip the value if present
            if (current < headerValue.Length && headerValue[current] == '=')
            {
                current++; // skip '='
                current += NameValueHeaderValue.GetValueLength(headerValue, current);
            }

            // Find the next delimiter
            current = headerValue.IndexOf(',', current);

            if (current == -1)
            {
                // If no delimiter found, skip to the end
                return headerValue.Length;
            }

            current++; // skip ','
            current += HttpRuleParser.GetWhitespaceLength(headerValue, current);

            return current;
        }

        /// <summary>
        /// Try to find a target header value among the set of given header values and parse it as a
        /// <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="headerValues">
        /// The <see cref="StringValues"/> containing the set of header values to search.
        /// </param>
        /// <param name="targetValue">
        /// The target header value to look for.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the parsed <see cref="TimeSpan"/>, if the parsing succeeded, or
        /// null if the parsing failed. The conversion fails if the <paramref name="targetValue"/> was not
        /// found or could not be parsed as a <see cref="TimeSpan"/>. This parameter is passed uninitialized;
        /// any value originally supplied in result will be overwritten.
        /// </param>
        /// <returns>
        /// <code>true</code> if <paramref name="targetValue"/> is found and successfully parsed; otherwise,
        /// <code>false</code>.
        /// </returns>
        // e.g. { "headerValue=10, targetHeaderValue=30" }
        public static bool TryParseSeconds(StringValues headerValues, string targetValue, out TimeSpan? value)
        {
            if (StringValues.IsNullOrEmpty(headerValues) || string.IsNullOrEmpty(targetValue))
            {
                value = null;
                return false;
            }

            for (var i = 0; i < headerValues.Count; i++)
            {
                // Trim leading white space
                var current = HttpRuleParser.GetWhitespaceLength(headerValues[i], 0);

                while (current < headerValues[i].Length)
                {
                    long seconds;
                    var initial = current;
                    var tokenLength = HttpRuleParser.GetTokenLength(headerValues[i], current);
                    if (tokenLength == targetValue.Length
                        && string.Compare(headerValues[i], current, targetValue, 0, tokenLength, StringComparison.OrdinalIgnoreCase) == 0
                        && TryParseInt64FromHeaderValue(current + tokenLength, headerValues[i], out seconds))
                    {
                        // Token matches target value and seconds were parsed
                        value = TimeSpan.FromSeconds(seconds);
                        return true;
                    }

                    current = AdvanceCacheDirectiveIndex(current + tokenLength, headerValues[i]);

                    // Ensure index was advanced
                    if (current <= initial)
                    {
                        Debug.Assert(false, $"Index '{nameof(current)}' not advanced, this is a bug.");
                        value = null;
                        return false;
                    }
                }
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Check if a target directive exists among the set of given cache control directives.
        /// </summary>
        /// <param name="cacheControlDirectives">
        /// The <see cref="StringValues"/> containing the set of cache control directives.
        /// </param>
        /// <param name="targetDirectives">
        /// The target cache control directives to look for.
        /// </param>
        /// <returns>
        /// <code>true</code> if <paramref name="targetDirectives"/> is contained in <paramref name="cacheControlDirectives"/>;
        /// otherwise, <code>false</code>.
        /// </returns>
        public static bool ContainsCacheDirective(StringValues cacheControlDirectives, string targetDirectives)
        {
            if (StringValues.IsNullOrEmpty(cacheControlDirectives) || string.IsNullOrEmpty(targetDirectives))
            {
                return false;
            }

            for (var i = 0; i < cacheControlDirectives.Count; i++)
            {
                // Trim leading white space
                var current = HttpRuleParser.GetWhitespaceLength(cacheControlDirectives[i], 0);

                while (current < cacheControlDirectives[i].Length)
                {
                    var initial = current;

                    var tokenLength = HttpRuleParser.GetTokenLength(cacheControlDirectives[i], current);
                    if (tokenLength == targetDirectives.Length
                        && string.Compare(cacheControlDirectives[i], current, targetDirectives, 0, tokenLength, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Token matches target value
                        return true;
                    }

                    current = AdvanceCacheDirectiveIndex(current + tokenLength, cacheControlDirectives[i]);

                    // Ensure index was advanced
                    if (current <= initial)
                    {
                        Debug.Assert(false, $"Index '{nameof(current)}' not advanced, this is a bug.");
                        return false;
                    }
                }
            }

            return false;
        }

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

        internal static bool TryParseInt32(string value, out int result)
        {
            return TryParseInt32(new StringSegment(value), out result);
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

        internal static unsafe bool TryParseInt32(StringSegment value, out int result)
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
        public unsafe static string FormatInt64(long value)
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

        public static bool TryParseDate(string input, out DateTimeOffset result)
        {
            return HttpRuleParser.TryStringToDate(input, out result);
        }

        public static string FormatDate(DateTimeOffset dateTime)
        {
            return FormatDate(dateTime, false);
        }

        public static string FormatDate(DateTimeOffset dateTime, bool quoted)
        {
            return dateTime.ToRfc1123String(quoted);
        }

        public static string RemoveQuotes(string input)
        {
            if (!string.IsNullOrEmpty(input) && input.Length >= 2 && input[0] == '"' && input[input.Length - 1] == '"')
            {
                input = input.Substring(1, input.Length - 2);
            }
            return input;
        }

        internal static void ThrowIfReadOnly(bool isReadOnly)
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException("The object cannot be modified because it is read-only.");
            }
        }
    }
}

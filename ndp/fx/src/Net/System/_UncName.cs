#if !PLATFORM_UNIX
//------------------------------------------------------------------------------
// <copyright file="_UncName.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Globalization;

namespace System {

    // The class designed as to keep minimal the working set of Uri class.
    // The idea is to stay with static helper methods and strings
    internal class UncNameHelper {

    // fields

        internal const int MaximumInternetNameLength = 256;

        private UncNameHelper() {
        }


    // properties

    // methods
        internal static string ParseCanonicalName(string str, int start, int end, ref bool loopback) {
            return DomainNameHelper.ParseCanonicalName(str, start, end, ref loopback);
        }

        //
        // IsValid
        //
        //
        //   ATTN: This class has been re-designed as to conform to XP+ UNC hostname format
        //         It is now similar to DNS name but can contain Unicode characters as well
        //         This class will be removed and replaced by IDN specification later,
        //         but for now we violate URI RFC cause we never escape Unicode characters on the wire
        //         For the same reason we never unescape UNC host names since we never accept
        //         them in escaped format.
        //
        //
        //      Valid UNC server name chars:
        //          a Unicode Letter    (not allowed as the only in a segment)
        //          a Latin-1 digit
        //          '-'    45 0x2D
        //          '.'    46 0x2E    (only as a host domain delimiter)
        //          '_'    95 0x5F
        //
        //
        // Assumption is the caller will check on the resulting name length
        // Remarks:  MUST NOT be used unless all input indexes are are verified and trusted.
        internal unsafe static bool IsValid(char* name, ushort start, ref int returnedEnd, bool notImplicitFile) {
            ushort end = (ushort) returnedEnd;

            if (start==end)
                return false;
            //
            // First segment could consist of only '_' or '-' but it cannot be all digits or empty
            //
            bool validShortName = false;
            ushort i = start;
            for (; i < end; ++i)
            {
                if (name[i] == '/' || name[i] == '\\' || (notImplicitFile && (name[i] == ':' || name[i] == '?' || name[i] == '#')))
                {
                    end = i;
                    break;
                }
                else if (name[i] == '.')
                {
                    ++i;
                    break;
                }
                if (Char.IsLetter(name[i]) || name[i] == '-' || name[i] == '_')
                {
                    validShortName = true;
                }
                else if (name[i] < '0' || name[i] > '9')
                    return false;
            }

            if (!validShortName)
                return false;

            //
            // Subsequent segments must start with a letter or a digit
            //

            for (; i < end; ++i)
            {
                if (name[i] == '/' || name[i] == '\\' || (notImplicitFile && (name[i] == ':' || name[i] == '?' || name[i] == '#')))
                {
                    end = i;
                    break;
                }
                else if (name[i] == '.')
                {
                    if (!validShortName || ((i-1) >= start && name[i-1] == '.'))
                        return false;

                    validShortName = false;
                }
                else if (name[i] == '-' || name[i] == '_')
                {
                    if (!validShortName)
                        return false;
                }
                else if (Char.IsLetter(name[i]) || (name[i] >= '0' && name[i] <= '9'))
                {
                    if (!validShortName)
                        validShortName = true;
                }
                else
                    return false;
            }

            // last segment can end with the dot
            if (((i-1) >= start && name[i-1] == '.'))
                validShortName = true;

            if (!validShortName)
                return false;

            //  caller must check for (end - start <= MaximumInternetNameLength)

            returnedEnd = end;
            return true;
        }
    }
}
#endif // !PLATFORM_UNIX

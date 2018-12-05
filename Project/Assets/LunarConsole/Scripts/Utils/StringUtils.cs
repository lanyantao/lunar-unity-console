//
//  StringUtils.cs
//
//  Lunar Unity Mobile Console
//  https://github.com/SpaceMadness/lunar-unity-console
//
//  Copyright 2018 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace LunarConsolePluginInternal
{
    public static class StringUtils
    {
        private static readonly Regex kRichTagRegex = new Regex("(<color=.*?>)|(<b>)|(<i>)|(</color>)|(</b>)|(</i>)");

        #region Format

        internal static string Format(string format, params object[] args)
        {
            if (format != null && args != null && args.Length > 0)
            {
                try
                {
                    return string.Format(format, args);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while formatting string: " + e.Message);
                }
            }

            return format;
        }

        #endregion

        #region Prefix & Suffix

        public static bool StartsWithIgnoreCase(string str, string prefix)
        {
            return str != null && prefix != null && str.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Parsing

        public static float ParseFloat(string str, float defValue = 0.0f)
        {
            if (!string.IsNullOrEmpty(str))
            {
                float value;
                bool succeed = float.TryParse(str, out value);
                return succeed ? value : defValue;
            }

            return defValue;
        }

        #endregion

        #region Rich Text Tags

        public static string RemoveRichTextTags(string line)
        {
            return kRichTagRegex.Replace(line, String.Empty);
        }

        #endregion

        #region string representation

        internal static string ToString(int value)
        {
            return value.ToString();
        }

        internal static string ToString(float value)
        {
            return value.ToString("G");
        }

        internal static string ToString(bool value)
        {
            return value.ToString();
        }

        #endregion

        #region Join

        public static string Join<T>(IList<T> list, string separator = ",")
        {
            var builder = new StringBuilder();
            for (int i = 0; i < list.Count; ++i)
            {
                builder.Append(list[i]);
                if (i < list.Count-1) builder.Append(separator);
            }
            return builder.ToString();
        }

        #endregion

        #region Display name

        public static String ToDisplayName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var result = new StringBuilder();

            char prevChr = '\0';
            for (int i = 0; i < value.Length; ++i)
            {
                var chr = value[i];

                if (i == 0)
                {
                    chr = Char.ToUpper(chr);
                }
                else if (Char.IsUpper(chr) || Char.IsDigit(chr) && !Char.IsDigit(prevChr))
                {
                    if (result.Length > 0)
                    {
                        result.Append(' ');
                    }
                }

                result.Append(chr);

                prevChr = chr;
            }

            return result.ToString();
        }

        #endregion

        #region Serialization

        public static IDictionary<string, string> DeserializeString(string data)
        {
            // can't use Json here since Unity doesn't support Json-to-Dictionary deserialization
            // don't want to use 3rd party so custom format it is
            string[] lines = data.Split('\n');
            IDictionary<string, string> dict = new Dictionary<string, string>();
            // foreach (string line in lines)
            for (int i = 0; i < lines.Length; ++i)
            {
                var line = lines[i];
                int index = line.IndexOf(':');
                string key = line.Substring(0, index);
                string value = line.Substring(index + 1, line.Length - (index + 1)).Replace(@"\n", "\n"); // restore new lines
                dict[key] = value;
            }
            return dict;
        }
        
        #endregion
    }
}
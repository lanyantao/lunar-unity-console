//
//  ClassUtils.cs
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
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;

namespace LunarConsolePluginInternal
{
    public delegate bool ListMethodsFilter(MethodInfo method);

    public static class ClassUtils
    {
        public static string TypeShortName(Type type)
        {
            if (type != null)
            {
                if (type == typeof(int)) return "int";
                if (type == typeof(float)) return "float";
                if (type == typeof(string)) return "string";
                if (type == typeof(long)) return "long";
                if (type == typeof(bool)) return "bool";

                return type.Name;
            }

            return null;
        }

        public static List<MethodInfo> ListInstanceMethods(Type type, ListMethodsFilter filter)
        {
            return ListInstanceMethods(new List<MethodInfo>(), type, filter);
        }

        public static List<MethodInfo> ListInstanceMethods(List<MethodInfo> outList, Type type, ListMethodsFilter filter)
        {
            return ListMethods(outList, type, filter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static List<MethodInfo> ListMethods(List<MethodInfo> outList, Type type, ListMethodsFilter filter, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            MethodInfo[] methods = type.GetMethods(flags);

            if (filter == null)
            {
                outList.AddRange(methods);
                return outList;
            }

            foreach (MethodInfo m in methods)
            {
                if (filter(m))
                {
                    outList.Add(m);
                }
            }
            return outList;
        }
    }
}

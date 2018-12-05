//
//  ReflectionUtils.cs
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
using System.Reflection;
using System.Text;

using UnityEngine;

namespace LunarConsolePluginInternal
{
    public delegate bool ReflectionTypeFilter(Type type);

    static class ReflectionUtils
    {
        #region Invocation

        private static readonly object[] EMPTY_INVOKE_ARGS = new object[0];

        public static bool Invoke(Delegate del)
        {
            if (del == null)
            {
                throw new ArgumentNullException("del");
            }

            return Invoke(del.Target, del.Method, EMPTY_INVOKE_ARGS);
        }

        private static bool Invoke(object target, MethodInfo method, object[] args)
        {
            if (method.ReturnType == typeof(bool))
            {
                return (bool)method.Invoke(target, args);
            }

            method.Invoke(target, args);
            return true;
        }

        #endregion

        #region Assembly Helper

        public static List<Type> FindAttributeTypes<T>(Assembly assembly) where T : Attribute
        {
            return FindAttributeTypes(assembly, typeof(T));
        }

        public static List<Type> FindAttributeTypes(Assembly assembly, Type attributeType)
        {
            return FindTypes(assembly, delegate(Type type) {
                var attributes = type.GetCustomAttributes(attributeType, false);
                return attributes != null && attributes.Length > 0;
            });
        }

        public static List<Type> FindTypes(Assembly assembly, ReflectionTypeFilter filter)
        {
            var list = new List<Type>();

            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (filter(type))
                    {
                        list.Add(type);
                    }
                }
            }
            catch (Exception e)
            {
                Log.e(e, "Unable to list types");
            }

            return list;
        }

        #endregion
    }
}
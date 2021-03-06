// Copyright 2014 Adrian Chlubek. This file is part of GTA Multiplayer IV project.
// Use of this source code is governed by a MIT license that can be
// found in the LICENSE file.
using System;
using System.Collections.Generic;
using System.Linq;

namespace MIVClient
{
    public abstract class IDControllerBase<T>
    {
        public Dictionary<uint, T> dict;

        public IDControllerBase()
        {
            dict = new Dictionary<uint, T>();
        }

        public uint Add(T instance)
        {
            uint key = findLowestFreeId();
            dict.Add(key, instance);
            return key;
        }

        public void Add(uint key, T instance)
        {
            dict.Add(key, instance);
        }

        public void Delete(T instance)
        {
            dict.Remove(GetKey(instance));
        }

        public bool Exists(T instance)
        {
            return dict.ContainsValue(instance);
        }

        public bool Exists(uint key)
        {
            return dict.ContainsKey(key);
        }

        public T GetInstance(uint id)
        {
            return dict[id];
        }

        public uint GetKey(T instance)
        {
            return dict.First(a => a.Value.Equals(instance)).Key;
        }

        private uint findLowestFreeId()
        {
            for (uint i = 1; i < uint.MaxValue; i++)
            {
                if (!dict.ContainsKey(i)) return i;
            }
            throw new Exception("No free ids");
        }
    }
}
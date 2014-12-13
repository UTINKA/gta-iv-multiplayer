// Copyright 2014 Adrian Chlubek. This file is part of GTA Multiplayer IV project.
// Use of this source code is governed by a MIT license that can be
// found in the LICENSE file.
using System;

namespace MIVServer
{
    internal class ServerLoader
    {
        public static void Main(string[] args)
        {
            var server = new Server();
            while (Console.ReadLine() != "c") ;
        }
    }
}
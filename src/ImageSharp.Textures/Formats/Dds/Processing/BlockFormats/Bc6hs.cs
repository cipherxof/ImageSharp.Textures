// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;
using SixLabors.ImageSharp.Textures.Common.Helpers;
using SixLabors.ImageSharp.Textures.Formats.Dds.Processing.PixelFormats;

namespace SixLabors.ImageSharp.Textures.Formats.Dds.Processing.BlockFormats
{
    /// <summary>
    /// Texture compressed with BC6HS, three color channels (16 bits:16 bits:16 bits) in "half" floating point.
    /// </summary>
    public struct Bc6hs : IBlock<Bc6hs>
    {
        // Code based on commit 138efff1b9c53fd9a5dd34b8c865e8f5ae798030 2019/10/24 in DirectXTex C++ library
        private static readonly Bc6hsModeDescriptor[][] MsADesc = new[]
        {
            new[]
            {
                // Mode 1 (0x00) - 10 5 5 5
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 2 (0x01) - 7 6 6 6
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 5), new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.GZ, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.BY, 5), new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BZ, 5), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RX, 5), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GX, 5), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BX, 5), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RY, 5), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RZ, 5), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 3 (0x02) - 11 5 4 4
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 10), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 10),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 10),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 4 (0x06) - 11 4 5 4
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 10),
                new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 10), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 10),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.BZ, 0),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 5 (0x0a) - 11 4 4 5
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 10),
                new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 10),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 10), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.BZ, 1),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 6 (0x0e) - 9 5 5 5
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 7 (0x12) - 8 6 5 5
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RX, 5), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RY, 5), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RZ, 5), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 8 (0x16) - 8 5 6 5
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GY, 5), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.GZ, 5), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GX, 5), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 9 (0x1a) - 8 5 5 6
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.BY, 5), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BZ, 5), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BX, 5), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 10 (0x1e) - 6 6 6 6
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.GZ, 4), new Bc6hsModeDescriptor(Bc6hEField.BZ, 0), new Bc6hsModeDescriptor(Bc6hEField.BZ, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 4), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GY, 5), new Bc6hsModeDescriptor(Bc6hEField.BY, 5), new Bc6hsModeDescriptor(Bc6hEField.BZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 4), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.GZ, 5), new Bc6hsModeDescriptor(Bc6hEField.BZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BZ, 5), new Bc6hsModeDescriptor(Bc6hEField.BZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RX, 5), new Bc6hsModeDescriptor(Bc6hEField.GY, 0), new Bc6hsModeDescriptor(Bc6hEField.GY, 1), new Bc6hsModeDescriptor(Bc6hEField.GY, 2), new Bc6hsModeDescriptor(Bc6hEField.GY, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GX, 5), new Bc6hsModeDescriptor(Bc6hEField.GZ, 0), new Bc6hsModeDescriptor(Bc6hEField.GZ, 1), new Bc6hsModeDescriptor(Bc6hEField.GZ, 2), new Bc6hsModeDescriptor(Bc6hEField.GZ, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BX, 5), new Bc6hsModeDescriptor(Bc6hEField.BY, 0), new Bc6hsModeDescriptor(Bc6hEField.BY, 1), new Bc6hsModeDescriptor(Bc6hEField.BY, 2), new Bc6hsModeDescriptor(Bc6hEField.BY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 0), new Bc6hsModeDescriptor(Bc6hEField.RY, 1), new Bc6hsModeDescriptor(Bc6hEField.RY, 2), new Bc6hsModeDescriptor(Bc6hEField.RY, 3), new Bc6hsModeDescriptor(Bc6hEField.RY, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RY, 5), new Bc6hsModeDescriptor(Bc6hEField.RZ, 0), new Bc6hsModeDescriptor(Bc6hEField.RZ, 1), new Bc6hsModeDescriptor(Bc6hEField.RZ, 2), new Bc6hsModeDescriptor(Bc6hEField.RZ, 3), new Bc6hsModeDescriptor(Bc6hEField.RZ, 4), new Bc6hsModeDescriptor(Bc6hEField.RZ, 5), new Bc6hsModeDescriptor(Bc6hEField.D, 0), new Bc6hsModeDescriptor(Bc6hEField.D, 1), new Bc6hsModeDescriptor(Bc6hEField.D, 2),
                new Bc6hsModeDescriptor(Bc6hEField.D, 3), new Bc6hsModeDescriptor(Bc6hEField.D, 4)
            },

            new[]
            {
                // Mode 11 (0x03) - 10 10
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RX, 5), new Bc6hsModeDescriptor(Bc6hEField.RX, 6), new Bc6hsModeDescriptor(Bc6hEField.RX, 7), new Bc6hsModeDescriptor(Bc6hEField.RX, 8), new Bc6hsModeDescriptor(Bc6hEField.RX, 9), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GX, 5), new Bc6hsModeDescriptor(Bc6hEField.GX, 6), new Bc6hsModeDescriptor(Bc6hEField.GX, 7), new Bc6hsModeDescriptor(Bc6hEField.GX, 8), new Bc6hsModeDescriptor(Bc6hEField.GX, 9), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BX, 5), new Bc6hsModeDescriptor(Bc6hEField.BX, 6), new Bc6hsModeDescriptor(Bc6hEField.BX, 7), new Bc6hsModeDescriptor(Bc6hEField.BX, 8), new Bc6hsModeDescriptor(Bc6hEField.BX, 9), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0)
            },

            new[]
            {
                // Mode 12 (0x07) - 11 9
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RX, 5), new Bc6hsModeDescriptor(Bc6hEField.RX, 6), new Bc6hsModeDescriptor(Bc6hEField.RX, 7), new Bc6hsModeDescriptor(Bc6hEField.RX, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 10), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GX, 5), new Bc6hsModeDescriptor(Bc6hEField.GX, 6), new Bc6hsModeDescriptor(Bc6hEField.GX, 7), new Bc6hsModeDescriptor(Bc6hEField.GX, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 10), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BX, 5), new Bc6hsModeDescriptor(Bc6hEField.BX, 6), new Bc6hsModeDescriptor(Bc6hEField.BX, 7), new Bc6hsModeDescriptor(Bc6hEField.BX, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 10), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0)
            },

            new[]
            {
                // Mode 13 (0x0b) - 12 8
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RX, 5), new Bc6hsModeDescriptor(Bc6hEField.RX, 6), new Bc6hsModeDescriptor(Bc6hEField.RX, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 11), new Bc6hsModeDescriptor(Bc6hEField.RW, 10), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GX, 5), new Bc6hsModeDescriptor(Bc6hEField.GX, 6), new Bc6hsModeDescriptor(Bc6hEField.GX, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 11), new Bc6hsModeDescriptor(Bc6hEField.GW, 10), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BX, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BX, 5), new Bc6hsModeDescriptor(Bc6hEField.BX, 6), new Bc6hsModeDescriptor(Bc6hEField.BX, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 11), new Bc6hsModeDescriptor(Bc6hEField.BW, 10), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0)
            },

            new[]
            {
                // Mode 14 (0x0f) - 16 4
                new Bc6hsModeDescriptor(Bc6hEField.M, 0), new Bc6hsModeDescriptor(Bc6hEField.M, 1), new Bc6hsModeDescriptor(Bc6hEField.M, 2), new Bc6hsModeDescriptor(Bc6hEField.M, 3), new Bc6hsModeDescriptor(Bc6hEField.M, 4), new Bc6hsModeDescriptor(Bc6hEField.RW, 0), new Bc6hsModeDescriptor(Bc6hEField.RW, 1), new Bc6hsModeDescriptor(Bc6hEField.RW, 2), new Bc6hsModeDescriptor(Bc6hEField.RW, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 5), new Bc6hsModeDescriptor(Bc6hEField.RW, 6), new Bc6hsModeDescriptor(Bc6hEField.RW, 7), new Bc6hsModeDescriptor(Bc6hEField.RW, 8), new Bc6hsModeDescriptor(Bc6hEField.RW, 9), new Bc6hsModeDescriptor(Bc6hEField.GW, 0), new Bc6hsModeDescriptor(Bc6hEField.GW, 1), new Bc6hsModeDescriptor(Bc6hEField.GW, 2), new Bc6hsModeDescriptor(Bc6hEField.GW, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 5), new Bc6hsModeDescriptor(Bc6hEField.GW, 6), new Bc6hsModeDescriptor(Bc6hEField.GW, 7), new Bc6hsModeDescriptor(Bc6hEField.GW, 8), new Bc6hsModeDescriptor(Bc6hEField.GW, 9), new Bc6hsModeDescriptor(Bc6hEField.BW, 0), new Bc6hsModeDescriptor(Bc6hEField.BW, 1), new Bc6hsModeDescriptor(Bc6hEField.BW, 2), new Bc6hsModeDescriptor(Bc6hEField.BW, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 4),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 5), new Bc6hsModeDescriptor(Bc6hEField.BW, 6), new Bc6hsModeDescriptor(Bc6hEField.BW, 7), new Bc6hsModeDescriptor(Bc6hEField.BW, 8), new Bc6hsModeDescriptor(Bc6hEField.BW, 9), new Bc6hsModeDescriptor(Bc6hEField.RX, 0), new Bc6hsModeDescriptor(Bc6hEField.RX, 1), new Bc6hsModeDescriptor(Bc6hEField.RX, 2), new Bc6hsModeDescriptor(Bc6hEField.RX, 3), new Bc6hsModeDescriptor(Bc6hEField.RW, 15),
                new Bc6hsModeDescriptor(Bc6hEField.RW, 14), new Bc6hsModeDescriptor(Bc6hEField.RW, 13), new Bc6hsModeDescriptor(Bc6hEField.RW, 12), new Bc6hsModeDescriptor(Bc6hEField.RW, 11), new Bc6hsModeDescriptor(Bc6hEField.RW, 10), new Bc6hsModeDescriptor(Bc6hEField.GX, 0), new Bc6hsModeDescriptor(Bc6hEField.GX, 1), new Bc6hsModeDescriptor(Bc6hEField.GX, 2), new Bc6hsModeDescriptor(Bc6hEField.GX, 3), new Bc6hsModeDescriptor(Bc6hEField.GW, 15),
                new Bc6hsModeDescriptor(Bc6hEField.GW, 14), new Bc6hsModeDescriptor(Bc6hEField.GW, 13), new Bc6hsModeDescriptor(Bc6hEField.GW, 12), new Bc6hsModeDescriptor(Bc6hEField.GW, 11), new Bc6hsModeDescriptor(Bc6hEField.GW, 10), new Bc6hsModeDescriptor(Bc6hEField.BX, 0), new Bc6hsModeDescriptor(Bc6hEField.BX, 1), new Bc6hsModeDescriptor(Bc6hEField.BX, 2), new Bc6hsModeDescriptor(Bc6hEField.BX, 3), new Bc6hsModeDescriptor(Bc6hEField.BW, 15),
                new Bc6hsModeDescriptor(Bc6hEField.BW, 14), new Bc6hsModeDescriptor(Bc6hEField.BW, 13), new Bc6hsModeDescriptor(Bc6hEField.BW, 12), new Bc6hsModeDescriptor(Bc6hEField.BW, 11), new Bc6hsModeDescriptor(Bc6hEField.BW, 10), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0),
                new Bc6hsModeDescriptor(Bc6hEField.NA, 0), new Bc6hsModeDescriptor(Bc6hEField.NA, 0)
            }
        };

        private static readonly Bc6hsModeInfo[] MsAInfo =
        {
            new Bc6hsModeInfo(0x00, 1, true,  3, new[] { new[] { new LdrColorA(10, 10, 10, 0), new LdrColorA(5, 5, 5, 0) }, new[] { new LdrColorA(5, 5, 5, 0), new LdrColorA(5, 5, 5, 0) } }), // Mode 1
            new Bc6hsModeInfo(0x01, 1, true,  3, new[] { new[] { new LdrColorA(7, 7, 7, 0), new LdrColorA(6, 6, 6, 0) }, new[] { new LdrColorA(6, 6, 6, 0), new LdrColorA(6, 6, 6, 0) } }), // Mode 2
            new Bc6hsModeInfo(0x02, 1, true,  3, new[] { new[] { new LdrColorA(11, 11, 11, 0), new LdrColorA(5, 4, 4, 0) }, new[] { new LdrColorA(5, 4, 4, 0), new LdrColorA(5, 4, 4, 0) } }), // Mode 3
            new Bc6hsModeInfo(0x06, 1, true,  3, new[] { new[] { new LdrColorA(11, 11, 11, 0), new LdrColorA(4, 5, 4, 0) }, new[] { new LdrColorA(4, 5, 4, 0), new LdrColorA(4, 5, 4, 0) } }), // Mode 4
            new Bc6hsModeInfo(0x0a, 1, true,  3, new[] { new[] { new LdrColorA(11, 11, 11, 0), new LdrColorA(4, 4, 5, 0) }, new[] { new LdrColorA(4, 4, 5, 0), new LdrColorA(4, 4, 5, 0) } }), // Mode 5
            new Bc6hsModeInfo(0x0e, 1, true,  3, new[] { new[] { new LdrColorA(9, 9, 9, 0), new LdrColorA(5, 5, 5, 0) }, new[] { new LdrColorA(5, 5, 5, 0), new LdrColorA(5, 5, 5, 0) } }), // Mode 6
            new Bc6hsModeInfo(0x12, 1, true,  3, new[] { new[] { new LdrColorA(8, 8, 8, 0), new LdrColorA(6, 5, 5, 0) }, new[] { new LdrColorA(6, 5, 5, 0), new LdrColorA(6, 5, 5, 0) } }), // Mode 7
            new Bc6hsModeInfo(0x16, 1, true,  3, new[] { new[] { new LdrColorA(8, 8, 8, 0), new LdrColorA(5, 6, 5, 0) }, new[] { new LdrColorA(5, 6, 5, 0), new LdrColorA(5, 6, 5, 0) } }), // Mode 8
            new Bc6hsModeInfo(0x1a, 1, true,  3, new[] { new[] { new LdrColorA(8, 8, 8, 0), new LdrColorA(5, 5, 6, 0) }, new[] { new LdrColorA(5, 5, 6, 0), new LdrColorA(5, 5, 6, 0) } }), // Mode 9
            new Bc6hsModeInfo(0x1e, 1, false, 3, new[] { new[] { new LdrColorA(6, 6, 6, 0), new LdrColorA(6, 6, 6, 0) }, new[] { new LdrColorA(6, 6, 6, 0), new LdrColorA(6, 6, 6, 0) } }), // Mode 10
            new Bc6hsModeInfo(0x03, 0, false, 4, new[] { new[] { new LdrColorA(10, 10, 10, 0), new LdrColorA(10, 10, 10, 0) }, new[] { new LdrColorA(0, 0, 0, 0), new LdrColorA(0, 0, 0, 0) } }), // Mode 11
            new Bc6hsModeInfo(0x07, 0, true,  4, new[] { new[] { new LdrColorA(11, 11, 11, 0), new LdrColorA(9, 9, 9, 0) }, new[] { new LdrColorA(0, 0, 0, 0), new LdrColorA(0, 0, 0, 0) } }), // Mode 12
            new Bc6hsModeInfo(0x0b, 0, true,  4, new[] { new[] { new LdrColorA(12, 12, 12, 0), new LdrColorA(8, 8, 8, 0) }, new[] { new LdrColorA(0, 0, 0, 0), new LdrColorA(0, 0, 0, 0) } }), // Mode 13
            new Bc6hsModeInfo(0x0f, 0, true,  4, new[] { new[] { new LdrColorA(16, 16, 16, 0), new LdrColorA(4, 4, 4, 0) }, new[] { new LdrColorA(0, 0, 0, 0), new LdrColorA(0, 0, 0, 0) } }), // Mode 14
        };

        private static readonly int[] MsAModeToInfo =
        {
             0, // Mode 1   - 0x00
             1, // Mode 2   - 0x01
             2, // Mode 3   - 0x02
             10, // Mode 11  - 0x03
             -1, // Invalid  - 0x04
             -1, // Invalid  - 0x05
             3, // Mode 4   - 0x06
             11, // Mode 12  - 0x07
             -1, // Invalid  - 0x08
             -1, // Invalid  - 0x09
             4, // Mode 5   - 0x0a
             12, // Mode 13  - 0x0b
             -1, // Invalid  - 0x0c
             -1, // Invalid  - 0x0d
             5, // Mode 6   - 0x0e
             13, // Mode 14  - 0x0f
             -1, // Invalid  - 0x10
             -1, // Invalid  - 0x11
             6, // Mode 7   - 0x12
             -1, // Reserved - 0x13
             -1, // Invalid  - 0x14
             -1, // Invalid  - 0x15
             7, // Mode 8   - 0x16
             -1, // Reserved - 0x17
             -1, // Invalid  - 0x18
             -1, // Invalid  - 0x19
             8, // Mode 9   - 0x1a
             -1, // Reserved - 0x1b
             -1, // Invalid  - 0x1c
             -1, // Invalid  - 0x1d
             9, // Mode 10  - 0x1e
             -1, // Resreved - 0x1f
        };

        /// <inheritdoc/>
        public int BitsPerPixel => 32;

        /// <inheritdoc/>
        public byte PixelDepthBytes => 4;

        /// <inheritdoc/>
        public byte DivSize => 4;

        /// <inheritdoc/>
        public byte CompressedBytesPerBlock => 16;

        /// <inheritdoc/>
        public bool Compressed => true;

        /// <inheritdoc/>
        public Image GetImage(byte[] blockData, int width, int height)
        {
            byte[] decompressedData = this.Decompress(blockData, width, height);
            return Image.LoadPixelData<ImageSharp.PixelFormats.Rgba32>(decompressedData, width, height);
        }

        /// <inheritdoc/>
        public byte[] Decompress(byte[] blockData, int width, int height)
        {
            IBlock self = this;
            byte[] currentBlock = new byte[this.CompressedBytesPerBlock];

            return Helper.InMemoryDecode<Bc6hs>(blockData, width, height, (stream, data, streamIndex, dataIndex, stride) =>
            {
                // I would prefer to use Span, but not sure if I should reference System.Memory in this project
                // copy data instead
                Buffer.BlockCopy(blockData, streamIndex, currentBlock, 0, currentBlock.Length);
                streamIndex += currentBlock.Length;

                uint uStartBit = 0;
                byte uMode = GetBits(currentBlock, ref uStartBit, 2u);
                if (uMode != 0x00 && uMode != 0x01)
                {
                    uMode = (byte)((GetBits(currentBlock, ref uStartBit, 3) << 2) | uMode);
                }

                Debug.Assert(uMode < 32, "uMode should be less then 32");

                if (MsAModeToInfo[uMode] >= 0)
                {
                    Debug.Assert(MsAModeToInfo[uMode] < MsAInfo.Length, "MsAModeToInfo[uMode] should be smaller then MsAInfo.Length");
                    Bc6hsModeDescriptor[] desc = MsADesc[MsAModeToInfo[uMode]];

                    Debug.Assert(MsAModeToInfo[uMode] < MsADesc.Length, "MsAModeToInfo[uMode] should be smaller then MsADesc.Length");
                    ref Bc6hsModeInfo info = ref MsAInfo[MsAModeToInfo[uMode]];

                    var aEndPts = new IntEndPntPair[Constants.BC6H_MAX_REGIONS];
                    for (int i = 0; i < aEndPts.Length; ++i)
                    {
                        aEndPts[i] = new IntEndPntPair(new IntColor(), new IntColor());
                    }

                    uint uShape = 0;

                    // Read header
                    uint uHeaderBits = info.UPartitions > 0 ? 82u : 65u;
                    while (uStartBit < uHeaderBits)
                    {
                        uint uCurBit = uStartBit;
                        if (GetBit(currentBlock, ref uStartBit) != 0)
                        {
                            switch (desc[uCurBit].MBc6HEField)
                            {
                                case Bc6hEField.D: uShape |= 1u << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.RW:
                                    aEndPts[0].A.R |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.RX:
                                    aEndPts[0].B.R |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.RY:
                                    aEndPts[1].A.R |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.RZ:
                                    aEndPts[1].B.R |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.GW:
                                    aEndPts[0].A.G |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.GX:
                                    aEndPts[0].B.G |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.GY:
                                    aEndPts[1].A.G |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.GZ:
                                    aEndPts[1].B.G |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.BW:
                                    aEndPts[0].A.B |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.BX:
                                    aEndPts[0].B.B |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.BY:
                                    aEndPts[1].A.B |= 1 << desc[uCurBit].Bit;
                                    break;
                                case Bc6hEField.BZ:
                                    aEndPts[1].B.B |= 1 << desc[uCurBit].Bit;
                                    break;
                                default:
                                {
                                    Debug.WriteLine("BC6H: Invalid header bits encountered during decoding");
                                    Helpers.FillWithErrorColors(data, ref dataIndex, Constants.NumPixelsPerBlock, self.DivSize, stride);
                                    return dataIndex;
                                }
                            }
                        }
                    }

                    Debug.Assert(uShape < 64, "uShape shoul be less then 64");

                    aEndPts[0].A.SignExtend(info.RgbaPrec[0][0]);

                    if (info.BTransformed)
                    {
                        Debug.Assert(info.UPartitions < Constants.BC6H_MAX_REGIONS, $"info.UPartitions should be less then {Constants.BC6H_MAX_REGIONS}");
                        for (int p = 0; p <= info.UPartitions; ++p)
                        {
                            if (p != 0)
                            {
                                aEndPts[p].A.SignExtend(info.RgbaPrec[p][0]);
                            }

                            aEndPts[p].B.SignExtend(info.RgbaPrec[p][1]);
                        }
                    }

                    // Inverse transform the end points.
                    if (info.BTransformed)
                    {
                        Helpers.TransformInverseSigned(aEndPts, info.RgbaPrec[0][0]);
                    }

                    // Read indices.
                    for (int i = 0; i < Constants.NumPixelsPerBlock; ++i)
                    {
                        uint uNumBits = Helpers.IsFixUpOffset(info.UPartitions, (byte)uShape, i) ? info.UIndexPrec - 1u : info.UIndexPrec;
                        if (uStartBit + uNumBits > 128)
                        {
                            Debug.WriteLine("BC6H: Invalid block encountered during decoding");
                            Helpers.FillWithErrorColors(data, ref dataIndex, Constants.NumPixelsPerBlock, self.DivSize, stride);
                            return dataIndex;
                        }

                        uint uIndex = GetBits(currentBlock, ref uStartBit, uNumBits);

                        if (uIndex >= ((info.UPartitions > 0) ? 8 : 16))
                        {
                            Debug.WriteLine("BC6H: Invalid index encountered during decoding");
                            Helpers.FillWithErrorColors(data, ref dataIndex, Constants.NumPixelsPerBlock, self.DivSize, stride);
                            return dataIndex;
                        }

                        uint uRegion = Constants.PartitionTable[info.UPartitions][uShape][i];
                        Debug.Assert(uRegion < Constants.BC6H_MAX_REGIONS, $"uRegion should be less then {Constants.BC6H_MAX_REGIONS}");

                        // Unquantize endpoints and interpolate
                        int r1 = Unquantize(aEndPts[uRegion].A.R, info.RgbaPrec[0][0].R);
                        int g1 = Unquantize(aEndPts[uRegion].A.G, info.RgbaPrec[0][0].G);
                        int b1 = Unquantize(aEndPts[uRegion].A.B, info.RgbaPrec[0][0].B);
                        int r2 = Unquantize(aEndPts[uRegion].B.R, info.RgbaPrec[0][0].R);
                        int g2 = Unquantize(aEndPts[uRegion].B.G, info.RgbaPrec[0][0].G);
                        int b2 = Unquantize(aEndPts[uRegion].B.B, info.RgbaPrec[0][0].B);
                        int[] aWeights = info.UPartitions > 0 ? Constants.Weights3 : Constants.Weights4;
                        var fc = new IntColor
                        {
                            R = FinishUnquantize(((r1 * (Constants.BC67_WEIGHT_MAX - aWeights[uIndex])) + (r2 * aWeights[uIndex]) + Constants.BC67_WEIGHT_ROUND) >> Constants.BC67_WEIGHT_SHIFT),
                            G = FinishUnquantize(((g1 * (Constants.BC67_WEIGHT_MAX - aWeights[uIndex])) + (g2 * aWeights[uIndex]) + Constants.BC67_WEIGHT_ROUND) >> Constants.BC67_WEIGHT_SHIFT),
                            B = FinishUnquantize(((b1 * (Constants.BC67_WEIGHT_MAX - aWeights[uIndex])) + (b2 * aWeights[uIndex]) + Constants.BC67_WEIGHT_ROUND) >> Constants.BC67_WEIGHT_SHIFT)
                        };

                        ushort[] rgb = new ushort[3];
                        fc.ToF16Signed(rgb);

                        // Clamp 0..1, and convert to byte (we're losing high dynamic range)
                        data[dataIndex++] = (byte)((Math.Max(0.0f, Math.Min(1.0f, FloatHelper.UnpackFloat16ToFloat(rgb[2]))) * 255.0f) + 0.5f); // blue
                        data[dataIndex++] = (byte)((Math.Max(0.0f, Math.Min(1.0f, FloatHelper.UnpackFloat16ToFloat(rgb[1]))) * 255.0f) + 0.5f); // green
                        data[dataIndex++] = (byte)((Math.Max(0.0f, Math.Min(1.0f, FloatHelper.UnpackFloat16ToFloat(rgb[0]))) * 255.0f) + 0.5f); // red
                        data[dataIndex++] = 255;

                        // Is mult 4?
                        if (((i + 1) & 0x3) == 0)
                        {
                            dataIndex += self.PixelDepthBytes * (stride - self.DivSize);
                        }
                    }
                }
                else
                {
                    string warnstr = "BC6H: Invalid mode encountered during decoding";
                    switch (uMode)
                    {
                        case 0x13: warnstr = "BC6H: Reserved mode 10011 encountered during decoding"; break;
                        case 0x17: warnstr = "BC6H: Reserved mode 10111 encountered during decoding"; break;
                        case 0x1B: warnstr = "BC6H: Reserved mode 11011 encountered during decoding"; break;
                        case 0x1F: warnstr = "BC6H: Reserved mode 11111 encountered during decoding"; break;
                    }

                    Debug.WriteLine(warnstr);

                    // Per the BC6H format spec, we must return opaque black
                    for (int i = 0; i < Constants.NumPixelsPerBlock; ++i)
                    {
                        data[dataIndex++] = 0;
                        data[dataIndex++] = 0;
                        data[dataIndex++] = 0;
                        data[dataIndex++] = 0;

                        // Is mult 4?
                        if (((i + 1) & 0x3) == 0)
                        {
                            dataIndex += self.PixelDepthBytes * (stride - self.DivSize);
                        }
                    }
                }

                return streamIndex;
            });
        }

        /// <summary>
        /// Gets a bit for a given position.
        /// </summary>
        /// <param name="currentBlock">The current block.</param>
        /// <param name="uStartBit">The start bit.</param>
        /// <returns>A bit at a given position.</returns>
        public static byte GetBit(byte[] currentBlock, ref uint uStartBit)
        {
            Guard.MustBeLessThan<uint>(uStartBit, 128, nameof(uStartBit));

            uint uIndex = uStartBit >> 3;
            byte ret = (byte)((currentBlock[uIndex] >> (int)(uStartBit - (uIndex << 3))) & 0x01);
            uStartBit++;
            return ret;
        }

        /// <summary>
        /// Gets n bits at a given start position.
        /// </summary>
        /// <param name="currentBlock">The current block.</param>
        /// <param name="uStartBit">The start bit.</param>
        /// <param name="uNumBits">The number of bits.</param>
        /// <returns>Bits at a given position.</returns>
        public static byte GetBits(byte[] currentBlock, ref uint uStartBit, uint uNumBits)
        {
            if (uNumBits == 0)
            {
                return 0;
            }

            Debug.Assert(uStartBit + uNumBits <= 128 && uNumBits <= 8, "uStartBit + uNumBits <= 128 && uNumBits <= 8");
            byte ret;
            uint uIndex = uStartBit >> 3;
            uint uBase = uStartBit - (uIndex << 3);
            if (uBase + uNumBits > 8)
            {
                uint uFirstIndexBits = 8 - uBase;
                uint uNextIndexBits = uNumBits - uFirstIndexBits;
                ret = (byte)((uint)(currentBlock[uIndex] >> (int)uBase) | ((currentBlock[uIndex + 1] & ((1u << (int)uNextIndexBits) - 1)) << (int)uFirstIndexBits));
            }
            else
            {
                ret = (byte)((currentBlock[uIndex] >> (int)uBase) & ((1 << (int)uNumBits) - 1));
            }

            Debug.Assert(ret < (1 << (int)uNumBits), $"GetBits return value should be less then {1 << (int)uNumBits}");
            uStartBit += uNumBits;
            return ret;
        }

        private static int Unquantize(int comp, byte uBitsPerComp)
        {
            int s = 0;
            int unq;
            if (uBitsPerComp >= 16)
            {
                unq = comp;
            }
            else
            {
                if (comp < 0)
                {
                    s = 1;
                    comp = -comp;
                }

                if (comp == 0)
                {
                    unq = 0;
                }
                else if (comp >= ((1 << (uBitsPerComp - 1)) - 1))
                {
                    unq = 0x7FFF;
                }
                else
                {
                    unq = ((comp << 15) + 0x4000) >> (uBitsPerComp - 1);
                }

                if (s != 0)
                {
                    unq = -unq;
                }
            }

            return unq;
        }

        private static int FinishUnquantize(int comp) => (comp < 0) ? -(((-comp) * 31) >> 5) : (comp * 31) >> 5; // scale the magnitude by 31/32
    }
}

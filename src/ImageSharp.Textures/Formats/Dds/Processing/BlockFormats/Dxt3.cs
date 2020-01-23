// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace SixLabors.ImageSharp.Textures.Formats.Dds.Processing
{
    using System;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Textures.Formats.Dds;
    using SixLabors.ImageSharp.Textures.Formats.Dds.Processing.BlockFormats;

    public struct Dxt3 : IBlock<Dxt3>
    {
        public int BitsPerPixel => 32;

        public ImageFormat Format => ImageFormat.Rgba32;

        public byte PixelDepthBytes => 4;

        public byte DivSize => 4;

        public byte CompressedBytesPerBlock => 16;

        public bool Compressed => true;

        public byte[] Decompress(byte[] blockData, int width, int height)
        {
            IBlock self = this;
            var colors = new ImageSharp.PixelFormats.Rgb24[4];

            return Helper.InMemoryDecode<Dxt3>(blockData, width, height, (byte[] stream, byte[] data, int streamIndex, int dataIndex, int stride) =>
            {

            /* 
             * Strategy for decompression:
             * -We're going to decode both alpha and color at the same time 
             * to save on space and time as we don't have to allocate an array 
             * to store values for later use.
             */

            // Remember where the alpha data is stored so we can decode simultaneously
            int alphaPtr = streamIndex;

            // Jump ahead to the color data
            streamIndex += 8;

            // Colors are stored in a pair of 16 bits
            ushort color0 = blockData[streamIndex++];
            color0 |= (ushort)(blockData[streamIndex++] << 8);

            ushort color1 = (blockData[streamIndex++]);
            color1 |= (ushort)(blockData[streamIndex++] << 8);

            // Extract R5G6B5 (in that order)
            colors[0].R = (byte)((color0 & 0x1f));
            colors[0].G = (byte)((color0 & 0x7E0) >> 5);
            colors[0].B = (byte)((color0 & 0xF800) >> 11);
            colors[0].R = (byte)(colors[0].R << 3 | colors[0].R >> 2);
            colors[0].G = (byte)(colors[0].G << 2 | colors[0].G >> 3);
            colors[0].B = (byte)(colors[0].B << 3 | colors[0].B >> 2);

            colors[1].R = (byte)((color1 & 0x1f));
            colors[1].G = (byte)((color1 & 0x7E0) >> 5);
            colors[1].B = (byte)((color1 & 0xF800) >> 11);
            colors[1].R = (byte)(colors[1].R << 3 | colors[1].R >> 2);
            colors[1].G = (byte)(colors[1].G << 2 | colors[1].G >> 3);
            colors[1].B = (byte)(colors[1].B << 3 | colors[1].B >> 2);

            // Used the two extracted colors to create two new colors
            // that are slightly different.
            colors[2].R = (byte)((2 * colors[0].R + colors[1].R) / 3);
            colors[2].G = (byte)((2 * colors[0].G + colors[1].G) / 3);
            colors[2].B = (byte)((2 * colors[0].B + colors[1].B) / 3);

            colors[3].R = (byte)((colors[0].R + 2 * colors[1].R) / 3);
            colors[3].G = (byte)((colors[0].G + 2 * colors[1].G) / 3);
            colors[3].B = (byte)((colors[0].B + 2 * colors[1].B) / 3);

                for (int i = 0; i < 4; i++)
                {
                    byte rowVal = blockData[streamIndex++];

                    // Each row of rgb values have 4 alpha values that  are
                    // encoded in 4 bits
                    ushort rowAlpha = blockData[alphaPtr++];
                    rowAlpha |= (ushort)(blockData[alphaPtr++] << 8);

                    for (int j = 0; j < 8; j += 2)
                    {
                        byte currentAlpha = (byte)((rowAlpha >> (j * 2)) & 0x0f);
                        currentAlpha |= (byte)(currentAlpha << 4);
                        var col = colors[((rowVal >> j) & 0x03)];
                        data[dataIndex++] = col.R;
                        data[dataIndex++] = col.G;
                        data[dataIndex++] = col.B;
                        data[dataIndex++] = currentAlpha;
                    }
                    dataIndex += self.PixelDepthBytes * (stride - self.DivSize);
                }

                return streamIndex;
            });

        }
    }
}
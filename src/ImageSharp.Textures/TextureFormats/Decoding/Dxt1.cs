// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp.Textures.Common.Helpers;

namespace SixLabors.ImageSharp.Textures.TextureFormats.Decoding
{
    /// <summary>
    /// Texture compressed with DXT1.
    /// </summary>
    internal struct Dxt1 : IBlock<Dxt1>
    {
        /// <inheritdoc/>
        public int BitsPerPixel => 24;

        /// <inheritdoc/>
        public byte PixelDepthBytes => 3;

        /// <inheritdoc/>
        public byte DivSize => 4;

        /// <inheritdoc/>
        public byte CompressedBytesPerBlock => 8;

        /// <inheritdoc/>
        public bool Compressed => true;

        /// <inheritdoc/>
        public Image GetImage(byte[] blockData, int width, int height)
        {
            byte[] decompressedData = Decompress(blockData, width, height);

            return Image.LoadPixelData<ImageSharp.PixelFormats.Rgba32>(decompressedData, width, height);
        }

        public byte[] Decompress(byte[] blockData, int sizeX, int sizeY)
        {
            int bitsPerSecond = sizeX * 4;
            int sizeOfPlane = bitsPerSecond * sizeY;
            byte[] rawData = new byte[sizeOfPlane + (sizeY * bitsPerSecond) + bitsPerSecond];
            var colors = new ImageSharp.PixelFormats.Rgba32[4];
            colors[0].A = 0xFF;
            colors[1].A = 0xFF;
            colors[2].A = 0xFF;
            int streamIndex = 0;


            for (int y = 0; y < sizeY; y += 4)
            {
                for (int x = 0; x < sizeX; x += 4)
                {
                    // Colors are stored in a pair of 16 bits.
                    ushort color0 = blockData[streamIndex++];
                    color0 |= (ushort)(blockData[streamIndex++] << 8);

                    ushort color1 = blockData[streamIndex++];
                    color1 |= (ushort)(blockData[streamIndex++] << 8);

                    // Extract R5G6B5.
                    PixelUtils.ExtractR5G6B5(color0, ref colors[0]);
                    PixelUtils.ExtractR5G6B5(color1, ref colors[1]);

                    uint bitmask = System.BitConverter.ToUInt32(blockData, streamIndex);

                    streamIndex += 4;

                    if (color0 > color1)
                    {
                        colors[2].R = (byte)(((2 * colors[0].R) + colors[1].R) / 3);
                        colors[2].G = (byte)(((2 * colors[0].G) + colors[1].G) / 3);
                        colors[2].B = (byte)(((2 * colors[0].B) + colors[1].B) / 3);

                        colors[3].R = (byte)((colors[0].R + (2 * colors[1].R)) / 3);
                        colors[3].G = (byte)((colors[0].G + (2 * colors[1].G)) / 3);
                        colors[3].B = (byte)((colors[0].B + (2 * colors[1].B)) / 3);
                        colors[3].A = 0xFF;
                    }
                    else
                    {
                        colors[2].R = (byte)((colors[0].R + colors[1].R) / 2);
                        colors[2].G = (byte)((colors[0].G + colors[1].G) / 2);
                        colors[2].B = (byte)((colors[0].B + colors[1].B) / 2);

                        colors[3].B = 0x00;
                        colors[3].G = 0x00;
                        colors[3].R = 0x00;
                        colors[3].A = 0x00;
                    }

                    for (int j = 0, k = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 4; i++, k++)
                        {
                            int select = (int)((bitmask & (0x03 << (k * 2))) >> (k * 2));
                            var col = colors[select];
                            if (((x + i) < sizeX) && ((y + j) < sizeY))
                            {
                                uint offset = (uint)((0 * sizeOfPlane) + ((y + j) * bitsPerSecond) + ((x + i) * 4));
                                rawData[offset + 0] = col.R;
                                rawData[offset + 1] = col.G;
                                rawData[offset + 2] = col.B;
                                rawData[offset + 3] = col.A;
                            }
                        }
                    }
                }
            }

            return rawData;
        }
    }
}

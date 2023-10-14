// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Textures.Common.Exceptions;
using SixLabors.ImageSharp.Textures.Common.Extensions;
using SixLabors.ImageSharp.Textures.Formats.Dds.Emums;
using SixLabors.ImageSharp.Textures.Formats.Dds.Extensions;
using SixLabors.ImageSharp.Textures.Formats.Dds.Processing;
using SixLabors.ImageSharp.Textures.TextureFormats;

namespace SixLabors.ImageSharp.Textures.Formats.Dds
{
    /// <summary>
    /// Performs the dds decoding operation.
    /// </summary>
    internal sealed class DdsDecoderCore
    {
        /// <summary>
        /// The file header containing general information about the texture.
        /// </summary>
        private DdsHeader ddsHeader;

        /// <summary>
        /// The dxt10 header if available
        /// </summary>
        private DdsHeaderDxt10 ddsDxt10header;

        /// <summary>
        /// The global configuration.
        /// </summary>
        private readonly Configuration configuration;

        /// <summary>
        /// Used for allocating memory during processing operations.
        /// </summary>
        private readonly MemoryAllocator memoryAllocator;

        /// <summary>
        /// The texture decoder options.
        /// </summary>
        private readonly IDdsDecoderOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DdsDecoderCore"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="options">The options.</param>
        public DdsDecoderCore(Configuration configuration, IDdsDecoderOptions options)
        {
            this.configuration = configuration;
            this.memoryAllocator = configuration.MemoryAllocator;
            this.options = options;
        }

        /// <summary>
        /// Decodes the texture from the specified stream.
        /// </summary>
        /// <param name="stream">The stream, where the texture should be decoded from. Cannot be null.</param>
        /// <returns>The decoded image.</returns>
        public Texture DecodeTexture(Stream stream)
        {
            try
            {
                this.ReadFileHeader(stream);

                if (this.ddsHeader.Width == 0 || this.ddsHeader.Height == 0)
                {
                    throw new UnknownTextureFormatException("Width or height cannot be 0");
                }

                var ddsProcessor = new DdsProcessor(this.ddsHeader, this.ddsDxt10header);

                int width = (int)this.ddsHeader.Width;
                int height = (int)this.ddsHeader.Height;
                int count = this.ddsHeader.TextureCount();

                if (this.ddsHeader.IsVolumeTexture())
                {
                    int depths = this.ddsHeader.ComputeDepth();

                    var texture = new VolumeTexture();
                    var surfaces = new FlatTexture[depths];

                    for (int i = 0; i < count; i++)
                    {
                        for (int depth = 0; depth < depths; depth++)
                        {
                            if (i == 0)
                            {
                                surfaces[depth] = new FlatTexture();
                            }

                            MipMap[] mipMaps = ddsProcessor.DecodeDds(stream, width, height, 1);
                            surfaces[depth].MipMaps.AddRange(mipMaps);
                        }

                        depths >>= 1;
                        width = Math.Max(1, width / 2);
                        height = Math.Max(1, height / 2);
                    }

                    texture.Slices.AddRange(surfaces);
                    return texture;
                }
                else if (this.ddsHeader.IsCubemap())
                {
                    DdsSurfaceType[] faces = this.ddsHeader.GetExistingCubemapFaces();

                    var texture = new CubemapTexture();
                    for (int face = 0; face < faces.Length; face++)
                    {
                        MipMap[] mipMaps = ddsProcessor.DecodeDds(stream, width, height, count);
                        if (faces[face] == DdsSurfaceType.CubemapPositiveX)
                        {
                            texture.PositiveX.MipMaps.AddRange(mipMaps);
                        }

                        if (faces[face] == DdsSurfaceType.CubemapNegativeX)
                        {
                            texture.NegativeX.MipMaps.AddRange(mipMaps);
                        }

                        if (faces[face] == DdsSurfaceType.CubemapPositiveY)
                        {
                            texture.PositiveY.MipMaps.AddRange(mipMaps);
                        }

                        if (faces[face] == DdsSurfaceType.CubemapNegativeY)
                        {
                            texture.NegativeY.MipMaps.AddRange(mipMaps);
                        }

                        if (faces[face] == DdsSurfaceType.CubemapPositiveZ)
                        {
                            texture.PositiveZ.MipMaps.AddRange(mipMaps);
                        }

                        if (faces[face] == DdsSurfaceType.CubemapNegativeZ)
                        {
                            texture.NegativeZ.MipMaps.AddRange(mipMaps);
                        }
                    }

                    return texture;
                }
                else
                {
                    var texture = new FlatTexture();
                    MipMap[] mipMaps = ddsProcessor.DecodeDds(stream, width, height, count);
                    texture.MipMaps.AddRange(mipMaps);
                    return texture;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new TextureFormatException("Dds image does not have a valid format.", e);
            }
        }

        /// <summary>
        /// Reads the raw image information from the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing image data.</param>
        public ITextureInfo Identify(Stream stream)
        {
            this.ReadFileHeader(stream);

            D3dFormat d3dFormat = this.ddsHeader.PixelFormat.GetD3DFormat();
            DxgiFormat dxgiFormat = this.ddsHeader.ShouldHaveDxt10Header() ? this.ddsDxt10header.DxgiFormat : DxgiFormat.Unknown;
            int bitsPerPixel = DdsTools.GetBitsPerPixel(d3dFormat, dxgiFormat);

            return new TextureInfo(
                new TextureTypeInfo(bitsPerPixel),
                (int)this.ddsHeader.Width,
                (int)this.ddsHeader.Height);
        }

        /// <summary>
        /// Reads the dds file header from the stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing texture data.</param>
        private void ReadFileHeader(Stream stream)
        {
            Span<byte> magicBuffer = stackalloc byte[4];
            stream.Read(magicBuffer, 0, 4);
            uint magicValue = BinaryPrimitives.ReadUInt32LittleEndian(magicBuffer);
            if (magicValue != DdsFourCc.DdsMagicWord)
            {
                throw new NotSupportedException("Invalid DDS magic value.");
            }

            byte[] ddsHeaderBuffer = new byte[DdsConstants.DdsHeaderSize];

            stream.Read(ddsHeaderBuffer, 0, DdsConstants.DdsHeaderSize);
            this.ddsHeader = DdsHeader.Parse(ddsHeaderBuffer);
            this.ddsHeader.Validate();

            if (this.ddsHeader.ShouldHaveDxt10Header())
            {
                byte[] ddsDxt10headerBuffer = new byte[DdsConstants.DdsDxt10HeaderSize];
                stream.Read(ddsDxt10headerBuffer, 0, DdsConstants.DdsDxt10HeaderSize);
                this.ddsDxt10header = DdsHeaderDxt10.Parse(ddsDxt10headerBuffer);
            }
        }
    }
}

// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp.Textures.Formats.Dds;
using SixLabors.ImageSharp.Textures.Tests.Enums;
using SixLabors.ImageSharp.Textures.Tests.TestUtilities.Attributes;
using SixLabors.ImageSharp.Textures.Tests.TestUtilities.TextureProviders;
using Xunit;

namespace SixLabors.ImageSharp.Textures.Tests.Formats.Dds
{
    public class DdsNvDxtCubemapDecoderTests
    {
        [Theory]
        [WithFile(TestTextureFormat.Dds, TestTextureType.Cubemap, TestTextureTool.NvDxt, "cubemap has-mips.dds")]
        public void DdsDecoder_CanDecode_Cubemap_NvDxt_Has_Mips(TestTextureProvider provider)
        {
            using Texture texture = provider.GetTexture(new DdsDecoder());
            provider.SaveTextures(texture);
            provider.CompareTextures(texture);
        }

        [Theory]
        [WithFile(TestTextureFormat.Dds, TestTextureType.Cubemap, TestTextureTool.NvDxt, "cubemap no-mips.dds")]
        public void DdsDecoder_CanDecode_Cubemap_NvDxt_No_Mips(TestTextureProvider provider)
        {
            using Texture texture = provider.GetTexture(new DdsDecoder());
            provider.SaveTextures(texture);
            provider.CompareTextures(texture);
        }
    }
}

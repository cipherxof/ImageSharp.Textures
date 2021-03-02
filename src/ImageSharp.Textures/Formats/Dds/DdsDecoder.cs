// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

/* Notes:
https://github.com/toji/texture
https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide
https://docs.microsoft.com/en-us/windows/uwp/gaming/complete-code-for-ddstextureloader
*/

using System.IO;

namespace SixLabors.ImageSharp.Textures.Formats.Dds
{
    /// <summary>
    /// Image decoder for DDS images.
    /// </summary>
    public sealed class DdsDecoder : ITextureDecoder, IDdsDecoderOptions, ITextureInfoDetector
    {
        /// <inheritdoc/>
        public Texture DecodeTexture(Configuration configuration, Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            return new DdsDecoderCore(configuration, this).DecodeTexture(stream);
        }

        /// <inheritdoc/>
        public ITextueInfo Identify(Configuration configuration, Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            return new DdsDecoderCore(configuration, this).Identify(stream);
        }
    }
}

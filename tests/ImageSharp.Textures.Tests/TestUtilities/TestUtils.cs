// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Textures.Tests.TestUtilities
{
    /// <summary>
    /// Various utility and extension methods.
    /// </summary>
    public static class TestUtils
    {
        private static readonly Dictionary<Type, PixelTypes> ClrTypes2PixelTypes = new Dictionary<Type, PixelTypes>();

        private static readonly Assembly ImageSharpAssembly = typeof(Rgba32).GetTypeInfo().Assembly;

        private static readonly Dictionary<PixelTypes, Type> PixelTypes2ClrTypes = new Dictionary<PixelTypes, Type>();

        private static readonly PixelTypes[] AllConcretePixelTypes = GetAllPixelTypes()
            .Except(new[] { PixelTypes.Undefined, PixelTypes.All })
            .ToArray();

        static TestUtils()
        {
            // Add Rgba32 Our default.
            Type defaultPixelFormatType = typeof(Rgba32);
            PixelTypes2ClrTypes[PixelTypes.Rgba32] = defaultPixelFormatType;
            ClrTypes2PixelTypes[defaultPixelFormatType] = PixelTypes.Rgba32;

            // Add PixelFormat types
            string nameSpace = typeof(A8).FullName;
            nameSpace = nameSpace.Substring(0, nameSpace.Length - typeof(A8).Name.Length - 1);
            foreach (PixelTypes pt in AllConcretePixelTypes.Where(pt => pt != PixelTypes.Rgba32))
            {
                string typeName = $"{nameSpace}.{pt}";
                Type t = ImageSharpAssembly.GetType(typeName);
                PixelTypes2ClrTypes[pt] = t ?? throw new InvalidOperationException($"Could not find: {typeName}");
                ClrTypes2PixelTypes[t] = pt;
            }
        }

        public static bool HasFlag(this PixelTypes pixelTypes, PixelTypes flag) => (pixelTypes & flag) == flag;

        public static string ToCsv<T>(this IEnumerable<T> items, string separator = ",") => string.Join(separator, items.Select(o => string.Format(CultureInfo.InvariantCulture, "{0}", o)));

        public static Type GetClrType(this PixelTypes pixelType) => PixelTypes2ClrTypes[pixelType];

        /// <summary>
        /// Returns the <see cref="PixelTypes"/> enumerations for the given type.
        /// </summary>
        /// <returns>The pixel type.</returns>
        public static PixelTypes GetPixelType(this Type colorStructClrType) => ClrTypes2PixelTypes[colorStructClrType];

        public static IEnumerable<KeyValuePair<PixelTypes, Type>> ExpandAllTypes(this PixelTypes pixelTypes)
        {
            if (pixelTypes == PixelTypes.Undefined)
            {
                return Enumerable.Empty<KeyValuePair<PixelTypes, Type>>();
            }
            else if (pixelTypes == PixelTypes.All)
            {
                // TODO: Need to return unknown types here without forcing CLR to load all types in ImageSharp assembly
                return PixelTypes2ClrTypes;
            }

            var result = new Dictionary<PixelTypes, Type>();
            foreach (PixelTypes pt in AllConcretePixelTypes)
            {
                if (pixelTypes.HasAll(pt))
                {
                    result[pt] = pt.GetClrType();
                }
            }

            return result;
        }

        internal static bool HasAll(this PixelTypes pixelTypes, PixelTypes flagsToCheck) =>
            (pixelTypes & flagsToCheck) == flagsToCheck;

        /// <summary>
        /// Enumerate all available <see cref="PixelTypes"/>-s
        /// </summary>
        /// <returns>The pixel types</returns>
        internal static PixelTypes[] GetAllPixelTypes() => (PixelTypes[])Enum.GetValues(typeof(PixelTypes));

        internal static Color GetColorByName(string colorName)
        {
            var f = (FieldInfo)typeof(Color).GetMember(colorName)[0];
            return (Color)f.GetValue(null);
        }

        internal static TPixel GetPixelOfNamedColor<TPixel>(string colorName)
            where TPixel : unmanaged, IPixel<TPixel> =>
            GetColorByName(colorName).ToPixel<TPixel>();

        public static string AsInvariantString(this FormattableString formattable) => FormattableString.Invariant(formattable);
    }
}

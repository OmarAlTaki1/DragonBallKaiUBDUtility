﻿using AuroraLib.Compression.Algorithms;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonBallKaiUBDLib
{
    public class DsoTexture
    {
        private const string MAGIC = "DSO";

        public byte[] Data { get; set; }
        public byte[] PixelData  { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }

        public List<SKColor> Palette { get; set; } = [];

        public DsoTexture(IEnumerable<byte> data)
        {
            if (!IO.ReadAsciiString(data, 0x00).Equals(MAGIC))
            {
                throw new ArgumentException("Not a DSO texture!");
            }

            Width = IO.ReadShort(data, 0x10);
            Height = IO.ReadShort(data, 0x12);

            int firstFFLoc = data.FindIndexOfSequence(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            int paletteLoc = IO.ReadInt(data,0x34);
            int fileStart = IO.ReadInt(data,0x38);
            int isCompressed = IO.ReadInt(data,0x40);

            /*for (int i = 0; i < fileStart; i += 2)
            {
                short color = BitConverter.ToInt16(data.Skip(i + paletteLoc).Take(2).ToArray());
                Palette.Add(new SKColor((byte)((color & 0x1F) << 3), (byte)((color >> 5 & 0x1F) << 3), (byte)((color >> 10 & 0x1F) << 3)));
            }*/

            LZ10 lz10 = new();
            bool overflow = fileStart>data.Count();
            if(!(isCompressed == 0x40000000 || isCompressed == 0x00000000))
            {
                PixelData = lz10.Decompress([.. data.Skip(fileStart)]);
                Data = [.. data.Take(fileStart).Concat(PixelData)];
            }
            else
            {
                PixelData = [.. data.Skip(fileStart)];
                Data = [..data];
            }
            
        }

        public SKBitmap GetImage()
        {
            SKBitmap bitmap = new(Width, Height);

            int pixelIndex = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (pixelIndex >= PixelData.Length)
                    {
                        break;
                    }
                    bitmap.SetPixel(x, y, Palette[PixelData[pixelIndex++]]);
                }
            }
            return bitmap;
        }
    }
}

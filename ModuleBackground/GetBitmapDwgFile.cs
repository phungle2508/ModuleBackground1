using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ModuleBackground;

internal static class GetBitmapDwgFile
{
	internal static Bitmap GetBitmap(string fileName)
	{
		using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			using BinaryReader binaryReader = new BinaryReader(fileStream);
			fileStream.Seek(13L, SeekOrigin.Begin);
			fileStream.Seek(20 + binaryReader.ReadInt32(), SeekOrigin.Begin);
			byte b = binaryReader.ReadByte();
			if (b <= 1)
			{
				return null;
			}
			for (short num = 1; num <= b; num++)
			{
				byte b2 = binaryReader.ReadByte();
				int num2 = binaryReader.ReadInt32();
				int count = binaryReader.ReadInt32();
				switch (b2)
				{
				case 2:
				{
					fileStream.Seek(num2, SeekOrigin.Begin);
					binaryReader.ReadBytes(14);
					ushort num3 = binaryReader.ReadUInt16();
					binaryReader.ReadBytes(4);
					uint num4 = binaryReader.ReadUInt32();
					fileStream.Seek(num2, SeekOrigin.Begin);
					byte[] buffer = binaryReader.ReadBytes(count);
					uint num5 = (uint)((num3 < 9) ? (4.0 * Math.Pow(2.0, (int)num3)) : 0.0);
					using MemoryStream memoryStream2 = new MemoryStream();
					using BinaryWriter binaryWriter = new BinaryWriter(memoryStream2);
					binaryWriter.Write((ushort)19778);
					binaryWriter.Write(54 + num5 + num4);
					binaryWriter.Write((ushort)0);
					binaryWriter.Write((ushort)0);
					binaryWriter.Write(54 + num5);
					binaryWriter.Write(buffer);
					return new Bitmap(memoryStream2);
				}
				case 6:
				{
					fileStream.Seek(num2, SeekOrigin.Begin);
					using MemoryStream memoryStream = new MemoryStream();
					fileStream.CopyTo(memoryStream, num2);
					Image original = Image.FromStream(memoryStream);
					return new Bitmap(original);
				}
				case 3:
					return null;
				}
			}
		}
		return null;
	}

	internal static Bitmap MakeBlackAndWhite(Image img)
	{
		Bitmap bitmap = new Bitmap(img);
		Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
		BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
		int num = Math.Abs(bitmapData.Stride) * img.Height;
		byte[] array = new byte[num];
		Marshal.Copy(bitmapData.Scan0, array, 0, num);
		for (int i = 0; i <= array.Length - 5; i += 4)
		{
			Color color = Color.FromArgb(array[i + 3], array[i + 2], array[i + 1], array[i]);
			byte b = 0;
			if (color.R + color.G + color.B == 765)
			{
				array[i + 2] = (array[i + 1] = (array[i] = 0));
				array[i + 3] = color.A;
			}
		}
		Marshal.Copy(array, 0, bitmapData.Scan0, num);
		bitmap.UnlockBits(bitmapData);
		return bitmap;
	}
}

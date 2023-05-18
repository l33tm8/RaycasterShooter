using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GLShooter
{
    class Texture
    {
        
        public Bitmap Image { get => image; }
        public Texture(Bitmap image)
        {
            this.image = image;
            this.finalPixels = new Color[image.Width, image.Height];
        }

        public Texture(string fileDir)
        {
            this.image = new Bitmap(fileDir);
            this.finalPixels = new Color[image.Width, image.Height];
        }

        public Color GetPixel(int x, int y)
        {

            return finalPixels[x, y];
        }

        public void InitializeColorArray()
        {
            var bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly,
                image.PixelFormat);
            var bytesPerPixel = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
            var byteCount = bitmapData.Stride * image.Height;
            var pixels = new byte[byteCount];
            var ptrFirstPixel = bitmapData.Scan0;
            
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;

            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            for (var y = 0; y < heightInPixels; y++)
            { 
                int currentLine = y * bitmapData.Stride;
                var buffX = 0;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    var color = Color.FromArgb(pixels[currentLine + x + 2],
                        pixels[currentLine + x + 1], pixels[currentLine + x]);
                    finalPixels[buffX, y] = color;
                    buffX++;
                }
            }

        }

        private Bitmap image;
        private Color[,] finalPixels;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GLShooter.Geometry;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace GLShooter
{

    enum WeaponState
    {
        WeaponIdle,
        WeaponFire
    }

    public class Form1 : Form
    {
        private System.Windows.Forms.Timer t;
        private System.Windows.Forms.Timer fireTimer;
        private int tickCount = 0;
        private Camera camera;
        private List<Texture> textures = new();
        private List<Texture> weaponTextures = new();
        private List<Enemy> enemies = new();
        private WeaponState currentWeaponState;
        private Texture currentWeaponTexture;
        private Texture fireballTexture;
        private Color[,] buffer;
        private double[] zBuffer;

        public Form1()
        {
            this.Width = 600;
            this.Height = 600;
            buffer = new Color[Width, Height];
            zBuffer = new double[Width];
            this.DoubleBuffered = true;
            t = new System.Windows.Forms.Timer();
            t.Interval = 30;
            t.Tick += TimerLoop;
            fireTimer = new System.Windows.Forms.Timer();
            fireTimer.Interval = 2000;
            fireTimer.Tick += FireLoop;

            camera = new Camera(new Vector(12, 12), new Vector(-1.0, 0.0));
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            var basedTexture = new Texture("Images/wolftextures.png");
            for (var i = 0; i < 512; i+=64)
            {
                var clonedRect = new Rectangle(i, 0, 64, 64);
                var clonedImage = basedTexture.Image.Clone(clonedRect, basedTexture.Image.PixelFormat);
                var texture = new Texture(clonedImage);
                texture.InitializeColorArray();
                textures.Add(texture);
            }

            var basedWeaponTexture = new Texture("Images/weapon.png");
            var weaponIdleImage = basedWeaponTexture.Image.Clone(new Rectangle(0, 0, 150, 150), 
                basedWeaponTexture.Image.PixelFormat);
            var weaponIdleTexture = new Texture(weaponIdleImage);
            weaponIdleTexture.InitializeColorArray();
            var weaponFireImage = basedWeaponTexture.Image.Clone(new Rectangle(0, 0, 300, 150),
                basedWeaponTexture.Image.PixelFormat);
            var weaponFireTexture = new Texture(weaponFireImage);
            weaponFireTexture.InitializeColorArray();
            weaponTextures.Add(weaponIdleTexture);
            weaponTextures.Add(weaponFireTexture);
            currentWeaponState = WeaponState.WeaponIdle;

            var blueAdidas = new Texture("Images/adikblue.png");
            var redAdidas = new Texture("Images/adikred.png");
            redAdidas.InitializeColorArray();
            blueAdidas.InitializeColorArray();

            for (var i = 2; i < 10; i++)
            {
                var texture = i % 2 == 0 ? redAdidas : blueAdidas;
                var enemy = new Enemy(new Sprite(new Vector(i, i), texture), new Vector(i, i), 100.0);
                enemies.Add(enemy);
            }

            currentWeaponTexture = weaponTextures[0];
            
            t.Start();
            fireTimer.Start();
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.S)
                camera.Velocity = 0;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.D)
                camera.RotationSpeed = 0;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                camera.Velocity = 0.3;
            if (e.KeyCode == Keys.S)
                camera.Velocity = -0.3;
            if (e.KeyCode == Keys.A)
                camera.RotationSpeed = -1.5;
            if (e.KeyCode == Keys.D)
                camera.RotationSpeed = 1.5;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawFloors();
            DrawWalls();
            var sortedSprites = enemies
                .OrderByDescending(x => Math.Abs((camera.Position - x.Sprite.position).Length()))
                .Where(enemy => (camera.Position - enemy.Position).Length() < 7)
                .Select(x => x.Sprite)
                .ToArray();

            var fireballs = enemies.
                SelectMany(x => x.GetFireballs())
                .Select(fireball => fireball.Sprite)
                .ToArray();
            // DrawSprites(sortedSprites);
            // DrawSprites(fireballs);
            var bufferImage = bufferToImage(new Bitmap(Width, Height));
            e.Graphics.DrawImage(bufferImage, 0, 0);
        }

        private void DrawWalls()
        {
            Parallel.For(0, Width, (x) =>
            {
                var lineHeight = camera.CastWall(x, Width, Height, out int side, out int mapX, out int mapY, out int hitX, out double perpWallDist);
                if (lineHeight > 0)
                {
                    var drawStart = -lineHeight / 2 + Height / 2;

                    if (drawStart < 0) drawStart = 0;
                    var drawEnd = lineHeight / 2 + Height / 2;
                    if (drawEnd > Height) drawEnd = Height - 1;

                    var textureId = Map.mapObjects[mapX][mapY] - 1;

                    var step = 64 / ((double)lineHeight + 1);
                    var texY = 0.0;

                    for (var y = drawStart; y < drawEnd; y++)
                    {
                        texY += step;

                        var color = textures[textureId].GetPixel(hitX, (int)texY);

                        buffer[x, y] = color;
                        zBuffer[x] = perpWallDist;
                    }

                }
            });
        }

        private void DrawFloors()
        {
            for (var y = 0; y < Height; y++)
            {
                var rayDirLeft = camera.Direction - camera.Plane;
                var rayDirRight = camera.Direction + camera.Plane;

                var p = y - Height / 2;
                var posZ = 0.5 * Height;
                var rowDistance = posZ / p;

                var floorStepX = rowDistance * (rayDirRight.X - rayDirLeft.X) / Width;
                var floorStepY = rowDistance * (rayDirRight.Y - rayDirLeft.Y) / Width;

                var floorX = camera.Position.X + rowDistance * rayDirLeft.X;
                var floorY = camera.Position.Y + rowDistance * rayDirLeft.Y;

                for (var x = 0; x < Width; ++x)
                {
                    var cellX = (int)floorX;
                    var cellY = (int)floorY;

                    var tx = (int)(64 * (floorX - cellX)) & (64 - 1);
                    var ty = (int)(64 * (floorY - cellY)) & (64 - 1);

                    floorX += floorStepX;
                    floorY += floorStepY;

                    buffer[x, y] = textures[3].GetPixel(tx, ty);
                    buffer[x, Height - y - 1] = textures[4].GetPixel(tx, ty);
                }
            }
        }

        private Bitmap bufferToImage(Bitmap bufferImage)
        {
            var bitmapData = bufferImage.LockBits(new Rectangle(0, 0, bufferImage.Width, bufferImage.Height), ImageLockMode.ReadWrite,
                            bufferImage.PixelFormat);
            var bytesPerPixel = Bitmap.GetPixelFormatSize(bufferImage.PixelFormat) / 8;
            var byteCount = bitmapData.Stride * bufferImage.Height;
            var pixels = new byte[byteCount];
            var ptrFirstPixel = bitmapData.Scan0;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;

            for (var x = 200; x < 200 + currentWeaponTexture.Image.Width; x++)
                for (var y = 300; y < 300 + currentWeaponTexture.Image.Height; y++)
                {
                    buffer[x, y] = currentWeaponTexture.GetPixel(x - 200, y - 300);
                }

            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            Parallel.For(0, heightInPixels, (y) =>
            {
                int currentLine = y * bitmapData.Stride;
                var buffX = 0;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {

                    pixels[currentLine + x] = buffer[buffX, y].B;
                    pixels[currentLine + x + 1] = buffer[buffX, y].G;
                    pixels[currentLine + x + 2] = buffer[buffX, y].R;
                    pixels[currentLine + x + 3] = 255;
                    buffX++;
                }
            }

            );

            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bufferImage.UnlockBits(bitmapData);
            return bufferImage;
        }

        private void DrawSprites(Sprite[] sortedSprites)
        {
            for (var i = 0; i < sortedSprites.Length; i++)
            {
                var sprite = sortedSprites[i];
                var spriteRelatedPosition = sprite.position - camera.Position;

                var invDet = 1.0 / (camera.Plane.X * camera.Direction.Y - camera.Direction.X * camera.Plane.Y);
                var transformX = invDet * (camera.Direction.Y * spriteRelatedPosition.X
                    - camera.Direction.X * spriteRelatedPosition.Y);
                var transformY = invDet * (-camera.Plane.Y * spriteRelatedPosition.X
                    + camera.Plane.X * spriteRelatedPosition.Y);

                var spriteScreenX = (int)((Width / 2) * (1 + transformX / transformY));

                var spriteHeight = transformY == 0 ? 0 : Math.Abs((int)(Height / transformY));
                var drawStartY = -spriteHeight / 2 + Height / 2;
                if (drawStartY < 0) drawStartY = 0;
                var drawEndY = spriteHeight / 2 + Height / 2;
                if (drawEndY > Width) drawEndY = Width - 1;

                var spriteWidth = transformY == 0 ? 0 : Math.Abs((int)(Width / transformY));
                var drawStartX = -spriteWidth / 2 + spriteScreenX;
                if (drawStartX < 0) drawStartX = 0;
                var drawEndX = spriteWidth / 2 + spriteScreenX;
                if (drawEndX >= Width) drawEndX = Width - 1;

                for (var stripe = drawStartX; stripe < drawEndX; stripe++)
                {
                    var texX = (int)(256 * (stripe
                        - (-spriteWidth / 2 + spriteScreenX)) * 64 / spriteWidth) / 256;
                    if (transformY > 0 && stripe > 0 && stripe < Width && transformY < zBuffer[stripe])
                        for (var y = drawStartY; y < drawEndY; y++)
                        {
                            var d = (y) * 256 - Height * 128 + spriteHeight * 128;
                            var texY = ((d * 64) / spriteHeight) / 256;
                            if (texX < 0 || texY < 0)
                                continue;
                            var color = sprite.texture.GetPixel(texX, texY);
                            if (color.R == 0 && color.G == 0 && color.B == 0)
                                continue;
                            buffer[stripe, y] = color;
                        }
                }
            }
        }

        private void TimerLoop(object sender, EventArgs e)
        {
            tickCount++;
            camera.Move();
            camera.Rotate(0.1);
            foreach (var enemy in enemies)
            {
                var distVector = camera.Position - enemy.Position;
                if (distVector.Length() > 2)
                    enemy.Direction = distVector.Normalize() * 0.1;
                else
                    enemy.Direction = new Vector(0, 0);
                enemy.Move();
            }
            Refresh();
        }

        private void FireLoop(object sender, EventArgs e)
        {
            var nearestEnemy = enemies.
                OrderBy(x => (x.Position - camera.Position).Length())
                .FirstOrDefault();
            if (nearestEnemy != null)
                nearestEnemy.Fire();
        }
    }
}

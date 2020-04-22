using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace LJC.NetCoreFrameWork.Comm
{
    public static class ImageHelper
    {
        public enum Align
        {
            Left,
            Center,
            Right
        }


        public static Font GetFont(string fontName, float emSize)
        {
            var ft = new Font(fontName, emSize);

            return ft;
        }

        public static Bitmap CreateImg(int width, int height, Color fillcolor)
        {
            var img = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(img))
            {
                using (var brush = new SolidBrush(fillcolor))
                {
                    g.FillRectangle(brush, 0, 0, width, height);
                }
            }


            return img;
        }

        public static int CalMinWidth(Font font, string[] lines, int left)
        {
            var marginright = 5;
            var maxwidth = left + marginright;
            using (Bitmap img = new Bitmap(100, 20))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                        var sizef = g.MeasureString(line, font);
                        if (sizef.Width + left + marginright > maxwidth)
                        {
                            maxwidth = (int)(sizef.Width + left + marginright);
                        }
                    }
                }
            }

            return maxwidth;
        }

        public static float DrawText(Font font, Bitmap img, string text, Color fontcolor, Align align, float left, float top, float lineheight)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return top + lineheight;
            }
            using (Graphics g = Graphics.FromImage(img))
            {
                var x = left;
                var y = top;
                var sizef = g.MeasureString(text, font);
                if (align == Align.Center)
                {
                    x = (img.Width - sizef.Width) / 2;
                }
                if (lineheight > sizef.Height)
                {
                    y = top + (lineheight - sizef.Height) / 2;
                }
                using (var brush = new SolidBrush(fontcolor))
                {
                    g.DrawString(text, font, brush, x, y);
                }

                if (lineheight > sizef.Height)
                {
                    return top + lineheight;
                }

                return top + sizef.Height;
            }
        }

        public static float DrawTable(Bitmap img, DataTable table, int width, int headerheight, int cellheight, Color headerbgcolor, Color headercolor,
            Color cellbgcolor, Color cellchangebgcolor, Color fontcolor,
            Font headerfont, Font textfont, Color bordercolor, int borderwidth, int left, int top)
        {
            var headerwidth = width * 1.0f / table.Columns.Count;

            using (Graphics g = Graphics.FromImage(img))
            {
                //头部
                float offsety = top;
                float offsetx = left;
                using (var brush = new SolidBrush(headerbgcolor))
                {
                    g.FillRectangle(brush, left, top, width, headerheight);
                    offsety += headerheight;
                }

                using (var brush1 = new SolidBrush(cellbgcolor))
                {
                    using (var brush2 = new SolidBrush(cellchangebgcolor))
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            if (i % 2 == 0)
                            {
                                g.FillRectangle(brush1, left, offsety, width, cellheight);
                            }
                            else
                            {
                                g.FillRectangle(brush2, left, offsety, width, cellheight);
                            }
                            offsety += cellheight;
                        }
                    }
                }

                using (var pen = new Pen(bordercolor, borderwidth))
                {
                    offsety = top;
                    g.DrawLine(pen, left, offsety, left + width, offsety);
                    offsety += headerheight;

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        g.DrawLine(pen, left, offsety, left + width, offsety);
                        offsety += cellheight;
                    }

                    g.DrawLine(pen, left, offsety, left + width, offsety);


                    offsetx = left;
                    g.DrawLine(pen, offsetx, top, offsetx, offsety);
                    offsetx += headerwidth;

                    for (int j = 1; j < table.Columns.Count; j++)
                    {
                        g.DrawLine(pen, offsetx, top, offsetx, offsety);
                        offsetx += headerwidth;
                    }

                    g.DrawLine(pen, left + width, top, left + width, offsety);
                }

                offsetx = left;
                //字符
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    var column = table.Columns[j].ColumnName;

                    if (!string.IsNullOrEmpty(column))
                    {
                        using (var brush = new SolidBrush(headercolor))
                        {
                            var sizef = g.MeasureString(column, headerfont);
                            g.DrawString(column, headerfont, brush, offsetx + (headerwidth - sizef.Width) / 2, top + (headerheight - sizef.Height) / 2);
                        }
                    }
                    offsetx += headerwidth;
                }


                offsety = top + headerheight;
                using (var brush = new SolidBrush(fontcolor))
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        offsetx = left;
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            var text = table.Rows[i][j]?.ToString();

                            if (!string.IsNullOrEmpty(text))
                            {
                                var sizef = g.MeasureString(text, headerfont);
                                g.DrawString(text, headerfont, brush, offsetx + (headerwidth - sizef.Width) / 2, offsety + (cellheight - sizef.Height) / 2);
                            }
                            offsetx += headerwidth;
                        }
                        offsety += cellheight;
                    }
                }

                return offsety;
            }
        }

        public static float DrawImg(Bitmap img, Bitmap drawimg, int height, int width, int left, float top)
        {
            var img2 = new Bitmap(drawimg);
            try
            {
                if (drawimg.Height > height || drawimg.Width > width)
                {
                    var hSize = height * 1.0f / drawimg.Height;
                    var wSize = width * 1.0f / drawimg.Width;
                    img2.Dispose();
                    img2 = PicReSize(drawimg, Math.Max(hSize, wSize), Math.Max(hSize, wSize), ImageFormat.Png);
                }

                //居中绘制
                var ajleft = left;
                var ajtop = top;
                ajleft += (width - img2.Width) / 2;
                ajtop += (height - img2.Height) / 2;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img2, ajleft, ajtop);
                }

                return top + height;
            }
            finally
            {
                img2.Dispose();
            }
        }

        /// <summary>
        /// 放大缩小图片
        /// </summary>
        /// <param name="originBmp"></param>
        /// <param name="iSize"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Bitmap PicReSize(Bitmap originBmp, float wSize, float hSize, ImageFormat format)
        {
            int w = (int)(originBmp.Width * wSize);
            int h = (int)(originBmp.Height * hSize);
            Bitmap resizedBmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(resizedBmp))
            {
                //设置高质量插值法  
                g.InterpolationMode = InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度  
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                //消除锯齿
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(originBmp, new Rectangle(0, 0, w, h), new Rectangle(0, 0, originBmp.Width, originBmp.Height), GraphicsUnit.Pixel);
                return resizedBmp;
            }
        }

        public static Bitmap Cut(Bitmap originBmp, int left, int top, int width, int height)
        {
            Rectangle cropRect = new Rectangle(new Point(left, top), new Size(width, height));
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(originBmp, new Rectangle(0, 0, target.Width, target.Height),
                      cropRect,
                      GraphicsUnit.Pixel);
                return target;
            }
        }

        public static byte[] GetBuffer(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                byte[] data = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(data, 0, Convert.ToInt32(ms.Length));
                return data;
            }
        }

        public static void Save(Bitmap img, string file)
        {
            using (var fs = new System.IO.FileStream(file, System.IO.FileMode.Create))
            {
                img.Save(fs, ImageFormat.Png);
            }
        }
    }
}

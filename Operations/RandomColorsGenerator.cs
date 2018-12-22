using System;
using System.Collections.Generic;
using System.Windows.Media;
using VANET_SIM.Operations;

public class RandomColorsGenerator
{
    /// <summary>
    /// Creates color with corrected brightness.
    /// </summary>
    /// <param name="color">Color to correct.</param>
    /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
    /// Negative values produce darker colors.</param>
    /// <returns>
    /// Corrected <see cref="Color"/> structure.
    /// </returns>
    public static Color ChangeColorBrightness(Color color, double correctionFactor)
    {
        double red = (float)color.R;
        double green = (float)color.G;
        double blue = (float)color.B;

        if (correctionFactor < 0)
        {
            correctionFactor = 1 + correctionFactor;
            red *= correctionFactor;
            green *= correctionFactor;
            blue *= correctionFactor;
        }
        else
        {
            red = (255 - red) * correctionFactor + red;
            green = (255 - green) * correctionFactor + green;
            blue = (255 - blue) * correctionFactor + blue;
        }

        return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
    }

    public static Color RandomColor()
    {
        Random rnd = new Random();
        double a = (rnd.Next(200) * DateTime.Now.Ticks * DateTime.Now.Millisecond ) % 255;
        if (a < 0) a *= -1;
        byte abyte = Convert.ToByte(a);

        double r = (rnd.Next(200) * DateTime.Now.Ticks * DateTime.Now.Millisecond ) % 255;
        if (r < 0) r *= -1;
        byte rbyte = Convert.ToByte(r);

        double g = (rnd.Next(200) * abyte * rbyte * DateTime.Now.Ticks * DateTime.Now.Millisecond ) % 255;
        if (g < 0) g *= -1;
        byte gbyte = Convert.ToByte(g);

        double b = (rnd.Next(200) * abyte * rbyte * DateTime.Now.Ticks * DateTime.Now.Millisecond ) % 255;
        if (b < 0) b *= -1;
        byte bbyte = Convert.ToByte(b);
        Color randomColor = Color.FromArgb(abyte, rbyte, gbyte, bbyte);
        Color xx = ChangeColorBrightness(randomColor, -0.5); 
        return xx;
    }

    public static List<Color> RandomColor(int numberofcolors)
    {
        List<Color> list = new List<Color>(numberofcolors);
        for (int i = 1; i <= numberofcolors; i++)
        {
            Random rnd = new Random();
            double a = (rnd.Next(1000) * DateTime.Now.Ticks * DateTime.Now.Millisecond * i) % 255;
            if (a < 0) a *= -1;
            byte abyte = Convert.ToByte(a);

            double r = (rnd.Next(2000) * DateTime.Now.Ticks * DateTime.Now.Millisecond * i) % 255;
            if (r < 0) r *= -1;
            byte rbyte = Convert.ToByte(r);

            double g = (rnd.Next(3000) * abyte * rbyte * DateTime.Now.Ticks * DateTime.Now.Millisecond * i) % 255;
            if (g < 0) g *= -1;
            byte gbyte = Convert.ToByte(g);

            double b = (rnd.Next(4000) * abyte * rbyte * DateTime.Now.Ticks * DateTime.Now.Millisecond * i) % 255;
            if (b < 0) b *= -1;
            byte bbyte = Convert.ToByte(b);
            Color randomColor = Color.FromArgb(abyte, rbyte, gbyte, bbyte);
            Color xr = ChangeColorBrightness(randomColor, -0.7);
            list.Add(xr);
        }
        return list;
    }

    public class DarkColore
    {
        public List<string> ColList = new List<string>();
        public DarkColore()
        {
            ColList.Add("#E52B50"); ColList.Add("#FFBF00"); ColList.Add("#9966CC"); ColList.Add("#FFFF00"); //4
            ColList.Add("#FBCEB1"); ColList.Add("#0000FF"); ColList.Add("#0095B6"); ColList.Add("#808000"); //4
            ColList.Add("#FFF700"); ColList.Add("#00FFFF"); ColList.Add("#007BA7"); ColList.Add("#DE5D83");//4
            ColList.Add("#FFC0CB"); ColList.Add("#FF00AF"); ColList.Add("#4B0082"); ColList.Add("#DC143C");//4
            ColList.Add("#003153"); ColList.Add("#800000"); ColList.Add("#F5F5DC"); ColList.Add("#008080");//4
            ColList.Add("#FF0000"); ColList.Add("#A7FC00"); ColList.Add("#0047AB"); ColList.Add("#007FFF");//4
            ColList.Add("#FA8072"); ColList.Add("#00FF7F"); ColList.Add("#00FF7F"); ColList.Add("#FFA500");//4

            ColList.Add("#FFDEAD"); ColList.Add("#FF69B4"); ColList.Add("#FF00FF"); ColList.Add("#00BFFF");
            ColList.Add("#87CEFA"); ColList.Add("#7CFC00"); ColList.Add("#8A2BE2"); ColList.Add("#1E90FF");
            ColList.Add("#BC8F8F"); ColList.Add("#FF1493"); ColList.Add("#800080"); ColList.Add("#4169E1");
            ColList.Add("#00FFFF"); ColList.Add("#32CD32"); ColList.Add("#EE82EE"); ColList.Add("#0000FF");
            ColList.Add("#800000"); ColList.Add("#DB7093"); ColList.Add("#00CED1"); ColList.Add("#ADFF2F");
            ColList.Add("#7FFFD4"); ColList.Add("#00FF00"); ColList.Add("#2F4F4F"); ColList.Add("#FFC0CB");



            ColList.Add("#FF7F50"); ColList.Add("#FF4500"); ColList.Add("#DC143C"); ColList.Add("#FA8072"); ColList.Add("#E6E6FA");
            ColList.Add("#FF6347"); ColList.Add("#FFA500"); ColList.Add("#E9967A"); ColList.Add("#DC143C"); ColList.Add("#D8BFD8");


        }
        /// <summary>
        /// indexier.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public SolidColorBrush this[int pos]
        {
            get
            {
                string col = ColList[pos];
                BrushConverter conv = new BrushConverter();
                SolidColorBrush brush = conv.ConvertFromString(col) as SolidColorBrush;
                return brush;
            }
        }

        public SolidColorBrush Random
        {
            get
            {
                double random1 = RandomeNumberGenerator.GetUniform(ColList.Count - 1);
                int x = Convert.ToInt16(random1);
                return this[x];
            }
        }


    }
}
using System;
using System.Windows.Forms;
using IrrlichtLime.Video;

namespace ColorBar
{
    public class ColorManager
    {
        private static ColorMap colorMap = new ColorMap();

        //static bool IsColorMap = true;
        private static int[,] cMap = colorMap.Jet();
       // private static bool isVertical = false;
       // private static bool isCheckColor = true;

        private static int currentSelectIndex = 0;

        public int CurrentSelectIndex
        {
            get { return currentSelectIndex; }
            set { currentSelectIndex = value; }
        }
        public int[,] CMap
        {
            get { return cMap; }
            set { cMap = value; }
        }

        private static int tickGrid = 12;

        public int TickGrid
        {
            get { return ColorManager.tickGrid; }
            set { ColorManager.tickGrid = value; }
        }
        private static int m_Width = 20;
        private static int m_Height = 300;
        public int Width
        {
            get { return ColorManager.m_Width; }
            set { ColorManager.m_Width = value; }
        }

        public int Height
        {
            get { return ColorManager.m_Height; }
            set { ColorManager.m_Height = value; }
        }

        //注意 此color与system color不同
        public Color GetColorByTemperature(float value, double zmin, double zmax)
        {
            if (cMap == null)
            {
                MessageBox.Show("颜色值为空");
            }
            int colorLength = cMap.GetLength(0);
            int cindex = (int)Math.Round((colorLength * (value - zmin) + (zmax - value)) / (zmax - zmin));
            if (cindex < 1)
                cindex = 1;
            if (cindex > colorLength)
                cindex = colorLength;
            Color color = new Color(cMap[cindex - 1, 1], cMap[cindex - 1, 2], cMap[cindex - 1, 3], cMap[cindex - 1, 0]);

            return color;
        }

    }
}

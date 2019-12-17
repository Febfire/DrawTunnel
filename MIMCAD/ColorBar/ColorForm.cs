using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;

namespace ColorBar
{
    public partial class ColorForm : Form
    {
        private ToolTip _toolTip = new ToolTip();
        private Point _offset;
        private Rectangle _mouseDownRect;
        private int _resizeBorderWidth = 5;
        private Point _mouseDownPoint;
        private ResizeRegion _resizeRegion = ResizeRegion.None;
        private ContextMenu _menu = new ContextMenu();
        private MenuItem _verticalMenuItem;
        private MenuItem _toolTipMenuItem = new MenuItem();

        private ColorManager colorManager = new ColorManager();
        private int[,] cMap;
        static public short maxTemperature = 100;
        private double mouseX, mouseY;

        #region ResizeRegion enum
        private enum ResizeRegion
        {
            None, N, NE, E, SE, S, SW, W, NW
        }
        #endregion

        public ColorForm()
        {
            InitializeComponent();

            this.SuspendLayout();
            this.Name = "Form1";
            this.ResumeLayout(false);
            ResourceManager resources = new ResourceManager(typeof(ColorForm));

            SetUpMenu();

            Text = "Ruler";
            BackColor = Color.White;
            ClientSize = new Size(250, 50);
            FormBorderStyle = FormBorderStyle.None;
            Opacity = 0.75;
            ContextMenu = _menu;
            Font = new Font("Tahoma", 10);

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.TopMost = false;
            this.MaximumSize = new Size(250,250);
            cMap = colorManager.CMap;
        }


        private bool IsVertical
        {
            get { return _verticalMenuItem.Checked; }
            set { _verticalMenuItem.Checked = value; }
        }

        private bool ShowToolTip
        {
            get { return _toolTipMenuItem.Checked; }
            set
            {
                _toolTipMenuItem.Checked = value;
                if (value)
                {
                    SetToolTip();
                }
            }
        }

        private void SetUpMenu()
        {
            //AddMenuItem("保持最顶层");
            _verticalMenuItem = AddMenuItem("竖向尺子");
            _toolTipMenuItem = AddMenuItem("温度提示");

            //默认光标停留时显示尺寸大小
            _toolTipMenuItem.Checked = true;

            //MenuItem temperatureMenuItem = AddMenuItem("最高温度");
            MenuItem opacityMenuItem = AddMenuItem("透明度");
            AddMenuItem("-");
            AddMenuItem("退出");

            for (int i = 10; i <= 100; i += 10)
            {
                MenuItem subMenu = new MenuItem(i + "%");
                subMenu.Click += new EventHandler(OpacityMenuHandler);
                opacityMenuItem.MenuItems.Add(subMenu);
            }

            //for (int i = 50; i <= 100; i += 50)
            //{
            //    MenuItem subMenu = new MenuItem(i + "°C");
            //    subMenu.Click += ChangeMaxTemperature;
            //    temperatureMenuItem.MenuItems.Add(subMenu);
            //}
        }

        private void ChangeMaxTemperature(object sender, EventArgs e)
        {
            System.Windows.Forms.MenuItem item = sender as System.Windows.Forms.MenuItem;
            short temperature = Convert.ToInt16(item.Text.Replace("°C", ""));
            maxTemperature = temperature;
            this.Refresh();
        }

        private MenuItem AddMenuItem(string text)
        {
            return AddMenuItem(text, Shortcut.None);
        }

        private MenuItem AddMenuItem(string text, Shortcut shortcut)
        {
            MenuItem mi = new MenuItem(text);
            mi.Click += new EventHandler(MenuHandler);
            mi.Shortcut = shortcut;
            _menu.MenuItems.Add(mi);

            return mi;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _offset = new Point(MousePosition.X - Location.X, MousePosition.Y - Location.Y);
            _mouseDownPoint = MousePosition;
            _mouseDownRect = ClientRectangle;

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _resizeRegion = ResizeRegion.None;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            SetToolTip();

            if (_resizeRegion != ResizeRegion.None)
            {
                HandleResize();
                return;
            }

            Point clientCursorPos = PointToClient(MousePosition);
            Rectangle resizeInnerRect = ClientRectangle;
            resizeInnerRect.Inflate(-_resizeBorderWidth, -_resizeBorderWidth);

            bool inResizableArea = ClientRectangle.Contains(clientCursorPos) && !resizeInnerRect.Contains(clientCursorPos);

            if (inResizableArea)
            {
                ResizeRegion resizeRegion = GetResizeRegion(clientCursorPos);
                SetResizeCursor(resizeRegion);

                if (e.Button == MouseButtons.Left)
                {
                    _resizeRegion = resizeRegion;
                    HandleResize();
                }
            }
            else
            {
                Cursor = Cursors.Default;

                if (e.Button == MouseButtons.Left)
                {
                    Location = new Point(MousePosition.X - _offset.X, MousePosition.Y - _offset.Y);
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (ShowToolTip)
            {
                SetToolTip();
            }

            base.OnResize(e);
        }

        private void SetToolTip()
        {
            double mouseTemperature;
            if (!IsVertical)
            {
                mouseTemperature = Math.Round(mouseX / this.Width * maxTemperature, 2);
            }
            else
            {
                mouseTemperature = Math.Round(maxTemperature - mouseY / this.Height * maxTemperature, 2);
            }

            if (ShowToolTip)
                _toolTip.SetToolTip(this, string.Format("温度:{0}", mouseTemperature));
            else
                _toolTip.SetToolTip(this, "");

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    HandleMoveResizeKeystroke(e);
                    break;

                case Keys.Space:
                    ChangeOrientation();
                    break;
            }

            base.OnKeyDown(e);
        }

        private void HandleMoveResizeKeystroke(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                if (e.Control)
                {
                    if (e.Shift)
                    {
                        Width += 1;
                    }
                    else
                    {
                        Left += 1;
                    }
                }
                else
                {
                    Left += 5;
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (e.Control)
                {
                    if (e.Shift)
                    {
                        Width -= 1;
                    }
                    else
                    {
                        Left -= 1;
                    }
                }
                else
                {
                    Left -= 5;
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (e.Control)
                {
                    if (e.Shift)
                    {
                        Height -= 1;
                    }
                    else
                    {
                        Top -= 1;
                    }
                }
                else
                {
                    Top -= 5;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (e.Control)
                {
                    if (e.Shift)
                    {
                        Height += 1;
                    }
                    else
                    {
                        Top += 1;
                    }
                }
                else
                {
                    Top += 5;
                }
            }
        }

        private void HandleResize()
        {
            int diff = 0;
            switch (_resizeRegion)
            {
                case ResizeRegion.E:
                    diff = MousePosition.X - _mouseDownPoint.X;
                    Width = _mouseDownRect.Width + diff;
                    break;

                case ResizeRegion.S:
                    diff = MousePosition.Y - _mouseDownPoint.Y;
                    Height = _mouseDownRect.Height + diff;
                    break;

                case ResizeRegion.SE:
                    Width = _mouseDownRect.Width + MousePosition.X - _mouseDownPoint.X;
                    Height = _mouseDownRect.Height + MousePosition.Y - _mouseDownPoint.Y;
                    break;

                case ResizeRegion.NW:
                    Width = _mouseDownRect.Width - MousePosition.X + _mouseDownPoint.X;
                    Height = _mouseDownRect.Height - MousePosition.Y + _mouseDownPoint.Y;
                    break;

                default:
                    break;
            }

            //int tmp;
            //int height = Height;
            //int width = Width;
            //if (IsVertical)
            //{
            //    tmp = Height;
            //    Height = Width;
            //    Width = tmp;
            //}

            this.Invalidate();
        }

        private void SetResizeCursor(ResizeRegion region)
        {
            switch (region)
            {
                case ResizeRegion.N:
                case ResizeRegion.S:
                    Cursor = Cursors.SizeNS;
                    break;

                case ResizeRegion.E:
                case ResizeRegion.W:
                    Cursor = Cursors.SizeWE;
                    break;

                case ResizeRegion.NW:
                case ResizeRegion.SE:
                    Cursor = Cursors.SizeNWSE;
                    break;

                default:
                    Cursor = Cursors.SizeNESW;
                    break;
            }
        }

        private ResizeRegion GetResizeRegion(Point clientCursorPos)
        {
            if (clientCursorPos.Y <= _resizeBorderWidth)
            {
                if (clientCursorPos.X <= _resizeBorderWidth) return ResizeRegion.NW;
                else if (clientCursorPos.X >= Width - _resizeBorderWidth) return ResizeRegion.NE;
                else return ResizeRegion.N;
            }
            else if (clientCursorPos.Y >= Height - _resizeBorderWidth)
            {
                if (clientCursorPos.X <= _resizeBorderWidth) return ResizeRegion.SW;
                else if (clientCursorPos.X >= Width - _resizeBorderWidth) return ResizeRegion.SE;
                else return ResizeRegion.S;
            }
            else
            {
                if (clientCursorPos.X <= _resizeBorderWidth) return ResizeRegion.W;
                else return ResizeRegion.E;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            int height;
            int width;

            if (IsVertical)
            {
                height = Width;
                width = Height;
            }
            else
            {
                height = Height;
                width = Width;
            }


            System.Drawing.Bitmap barBitmap = new System.Drawing.Bitmap(width, height);
            graphics.DrawImage(barBitmap, new PointF(0, 0));

            int bmin = 0;
            int bmax = width;
            int dy = width / (bmax - bmin);
            int x = 0, y = 0;

            int m = 255;

            if (IsVertical)
            {
                for (int i = 0; i < bmax; i++)
                {
                    int index = m - 1 - (int)((i - bmin) * m / (bmax - bmin));
                    SolidBrush aBrush = new SolidBrush(System.Drawing.Color.FromArgb(cMap[index, 0], cMap[index, 1], cMap[index, 2], cMap[index, 3]));

                    graphics.FillRectangle(aBrush, x, y + i * dy, width, dy);
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    aBrush.Dispose();
                }
            }
            else
            {
                for (int i = 0; i < bmax; i++)
                {
                    int index = (int)((i - bmin) * m / (bmax - bmin));
                    SolidBrush aBrush = new SolidBrush(System.Drawing.Color.FromArgb(cMap[index, 0], cMap[index, 1], cMap[index, 2], cMap[index, 3]));
                    graphics.FillRectangle(aBrush, y + i * dy, x, dy, width);
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    aBrush.Dispose();
                }
            }


            DrawRuler(graphics, width, height);
            graphics.Flush();
        }


        private void DrawRuler(Graphics g, int formWidth, int formHeight)
        {
            // 边框
            if (IsVertical)
            {
                g.DrawRectangle(Pens.Black, 0, 0, formHeight - 1, formWidth - 1);
            }
            else
            {
                g.DrawRectangle(Pens.Black, 0, 0, formWidth - 1, formHeight - 1);
            }

            for (int i = 0; i <= maxTemperature; i++)
            {
                int length = formWidth * i / maxTemperature;
                if (i % 2 == 0)
                {
                    int tickHeight = 5;
                    if (i % (maxTemperature * 0.1) == 0)
                    {
                        tickHeight = 10;
                    }
                    if (i % (maxTemperature * 0.2) == 0)
                    {
                        tickHeight = 15;
                        if (i != 0 && i != maxTemperature)
                        {
                            if (IsVertical)
                                DrawTickLabel(g, (maxTemperature-i).ToString(), length, formHeight, tickHeight);
                            else
                                DrawTickLabel(g, i.ToString(), length, formHeight, tickHeight);
                        }
                            
                    }

                    DrawTick(g, length, formHeight, tickHeight);
                }

            }
        }

        private void DrawTick(Graphics g, int xPos, int formHeight, int tickHeight)
        {
            if (IsVertical)
            {
                g.DrawLine(Pens.Black, formHeight - Font.Height * 2 - tickHeight, xPos, formHeight - Font.Height * 2, xPos);
            }
            else
            {
                g.DrawLine(Pens.Black, xPos, formHeight - Font.Height * 2, xPos, tickHeight + formHeight - Font.Height * 2);

            }
        }

        private void DrawTickLabel(Graphics g, string text, int xPos, int formHeight, int height)
        {
            if (IsVertical)
            {
                g.DrawString(text, Font, Brushes.Black, 0, xPos);
            }
            else
            {
                g.DrawString(text, Font, Brushes.Black, xPos, formHeight - height);
            }

        }



        private void OpacityMenuHandler(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Opacity = double.Parse(mi.Text.Replace("%", "")) / 100;
        }

        private void MenuHandler(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "退出":
                    Close();
                    break;

                case "温度提示":
                    ShowToolTip = !ShowToolTip;
                    break;

                case "竖向尺子":
                    ChangeOrientation();
                    break;

                case "保持最顶层":
                    mi.Checked = !mi.Checked;
                    TopMost = mi.Checked;
                    break;

                default:
                    MessageBox.Show("未有菜单项目。");
                    break;
            }
        }

        // 改变尺子放置方向（横向/竖向）
        private void ChangeOrientation()
        {
            IsVertical = !IsVertical;

            int width = Width;

            Width = Height;
            Height = width;


            this.Invalidate();
        }
    }
}
﻿using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    class HexSurface : SadConsole.ScreenSurface
    {
        int lastCell = -1;
        bool lastCellHexRow = false;

        public HexSurface(int width, int height) : base(width, height)
        {
            UseMouse = true;
            IsVisible = false;
            Position = new Point(3, 2);
            Surface.Fill(Color.Red, null, glyph: 176);

            for (int x = 0; x < width; x++)
            {
                bool isHexColumn = x % 2 == 1;

                for (int y = 0; y < height; y++)
                {
                    bool isHexRow = y % 2 == 1;

                    if (isHexColumn && isHexRow)
                    {
                        Surface.SetForeground(x, y, Color.Blue);
                    }
                    else if (isHexRow)
                    {
                        Surface.SetForeground(x, y, Color.AliceBlue);
                    }
                    else if (isHexColumn)
                    {
                        Surface.SetForeground(x, y, Color.DarkRed);
                    }
                }
            }

#if SFML
#elif MONOGAME
            Renderer = new FeatureDemo.HostSpecific.MonoGame.HexSurfaceRenderer();
#endif
        }

        public override bool ProcessMouse(SadConsole.Input.MouseScreenObjectState info)
        {
            var worldLocation = info.Mouse.ScreenPosition.PixelLocationToSurface(FontSize.X, FontSize.Y);
            var consoleLocation = new Point(worldLocation.X - Position.X, worldLocation.Y - Position.Y);

            // Check if mouse is within the upper/lower bounds of the console
            if (Surface.View.Contains(info.SurfaceCellPosition))
            {
                bool isHexRow = info.SurfaceCellPosition.Y % 2 == 1;

                var newMouse = info.Mouse.Clone();

                // Check if mouse is on an alternating row
                if (isHexRow)
                    newMouse.ScreenPosition = (newMouse.ScreenPosition.X - FontSize.X / 2, newMouse.ScreenPosition.Y);

                var newInfo = new MouseScreenObjectState(this, newMouse);

                if (info.ScreenObject == this)
                {
                    FillHexes(lastCell, 176, lastCellHexRow);
                    lastCell = info.SurfaceCellPosition.ToIndex(Surface.Width);
                    lastCellHexRow = isHexRow;
                    FillHexes(lastCell, 45, isHexRow);
                    IsDirty = true;
                    return true;
                }
            }

            //base.ProcessMouse(info);

            FillHexes(lastCell, 176, lastCellHexRow);

            return true;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
        }

        private void FillHexes(int index, int glyphIndex, bool isHexRow)
        {
            SetGlyph(GetHexCellIndex(HexDirection.TopLeft, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.TopRight, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.Left, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.Right, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.BottomLeft, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.BottomRight, index, isHexRow), glyphIndex);

            if (index >= 0 && index < Surface.Count)
                SetGlyph(index, glyphIndex);
        }

        private void SetGlyph(int cellIndex, int glyphIndex)
        {
            if (cellIndex != -1)
                Surface[cellIndex].Glyph = glyphIndex;
        }

        public int GetHexCellIndex(HexDirection direction, int sourceHex, bool isHexRow)
        {
            int returnHex = -1;

            switch (direction)
            {
                case HexDirection.TopLeft:
                    returnHex = isHexRow ? sourceHex - Surface.Width : sourceHex - Surface.Width - 1;
                    break;
                case HexDirection.TopRight:
                    returnHex = isHexRow ? sourceHex - Surface.Width + 1 : sourceHex - Surface.Width ;
                    break;
                case HexDirection.Left:
                    returnHex = sourceHex - 1;
                    break;
                case HexDirection.Right:
                    returnHex = sourceHex + 1;
                    break;
                case HexDirection.BottomLeft:
                    returnHex = isHexRow ? sourceHex + Surface.Width : sourceHex + Surface.Width - 1;
                    break;
                case HexDirection.BottomRight:
                    returnHex = isHexRow ? sourceHex + Surface.Width + 1 : sourceHex + Surface.Width;
                    break;
                default:
                    break;
            }

            if (returnHex < 0 || returnHex > Surface.Count - 1)
                returnHex = -1;

            return returnHex;
        }

        public enum HexDirection
        {
            TopLeft,
            TopRight,
            Left,
            Right,
            BottomLeft,
            BottomRight
        }

    }
}

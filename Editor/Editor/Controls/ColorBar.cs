﻿using SadRogue.Primitives;
using SadConsole;
using System;
using System.Linq;
using SadConsole.UI.Themes;
using SadConsole.UI.Controls;

namespace SadConsoleEditor.Controls
{
    public class ColorBar : SadConsole.UI.Controls.ControlBase
    {
        public class ThemeType : ThemeBase
        {
            /// <inheritdoc />
            public override void Attached(ControlBase control)
            {
                if (!(control is ColorBar)) throw new Exception($"Theme can only be added to a {nameof(ColorBar)}");

                control.Surface = new CellSurface(control.Width, control.Height);
                control.Surface.Clear();
            }

            /// <inheritdoc />
            public override void UpdateAndDraw(ControlBase control, TimeSpan time)
            {
                if (!(control is ColorBar bar)) return;
                if (!bar.IsDirty) return;

                ColoredGlyph appearance;

                RefreshTheme(control.FindThemeColors(), control);

                if (Helpers.HasFlag((int)control.State, (int)ControlStates.Disabled))
                    appearance = ControlThemeState.Disabled;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(presenter.State, ControlStates.MouseRightButtonDown))
                //    appearance = MouseDown;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseOver))
                //    appearance = MouseOver;

                else if (Helpers.HasFlag((int)control.State, (int)ControlStates.Focused))
                    appearance = ControlThemeState.Focused;

                else
                    appearance = ControlThemeState.Normal;

                control.Surface.Fill(Color.White, Color.Black, 0, null);

                bar._positions = control.Width;
                bar._colorSteps = bar.StartingColor.LerpSteps(bar.EndingColor, control.Width);

                for (int x = 0; x < control.Width; x++)
                {
                    control.Surface[x, 0].Glyph = 219;
                    control.Surface[x, 0].Foreground = bar._colorSteps[x];
                }

                control.Surface[bar._selectedPosition, 1].Glyph = 30;
                control.Surface[bar._selectedPosition, 1].Foreground = Color.LightGray;//this[_selectedPosition, 0].Foreground;

                control.IsDirty = false;
            }

            /// <inheritdoc />
            public override ThemeBase Clone()
            {
                return new ThemeType()
                {
                    ControlThemeState = ControlThemeState.Clone()
                };
            }
        }

        public event EventHandler ColorChanged;

        public Color StartingColor { get { return _startingColor; } set { _startingColor = value; IsDirty = true; Theme?.UpdateAndDraw(this, TimeSpan.Zero); } }
        public Color EndingColor { get { return _endingColor; } set { _endingColor = value; IsDirty = true; Theme?.UpdateAndDraw(this, TimeSpan.Zero); } }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                SetClosestIndex(value);

                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        private Color SelectedColorSafe
        {
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        private int _positions;
        private int _selectedPosition;
        private Color[] _colorSteps;

        private Color _selectedColor;
        private Color _startingColor;
        private Color _endingColor;

        public ColorBar(int width) : base(width, 2)
        {
            StartingColor = Color.White;
            EndingColor = Color.Black;
            CanFocus = false;
        }

        private void SetClosestIndex(Color color)
        {
            ColorMine.ColorSpaces.Rgb rgbColorStop = new ColorMine.ColorSpaces.Rgb() { R = color.R, G = color.G, B = color.B };
            Tuple<Color, double, int>[] colorWeights = new Tuple<Color, double, int>[Width];

            // Create a color weight for every cell compared to the color stop
            for (int x = 0; x < Width; x++)
            {
                ColorMine.ColorSpaces.Rgb rgbColor = new ColorMine.ColorSpaces.Rgb() { R = Surface[x, 0].Foreground.R, G = Surface[x, 0].Foreground.G, B = Surface[x, 0].Foreground.B };
                ColorMine.ColorSpaces.Cmy cmyColor = rgbColor.To<ColorMine.ColorSpaces.Cmy>();

                colorWeights[x] = new Tuple<Color, double, int>(Surface[x, 0].Foreground, rgbColorStop.Compare(cmyColor, new ColorMine.ColorSpaces.Comparisons.Cie1976Comparison()), x);
            }

            var foundColor = colorWeights.OrderBy(t => t.Item2).First();
            _selectedPosition = foundColor.Item3;
            this.IsDirty = true;
        }

        protected override void OnMouseIn(ControlMouseState info)
        {
            base.OnMouseIn(info);

            if (Parent.Host.CapturedControl == null)
            {
                if (info.OriginalMouseState.Mouse.LeftButtonDown)
                {
                    var location = info.MousePosition;

                    _selectedPosition = location.X;
                    SelectedColorSafe = Surface[_selectedPosition, 0].Foreground;
                    IsDirty = true;

                    Parent.Host.CaptureControl(this);
                }
            }
        }

        public override bool ProcessMouse(SadConsole.Input.MouseScreenObjectState info)
        {
            if (Parent.Host.CapturedControl == this)
            {
                if (info.Mouse.LeftButtonDown == false)
                    Parent.Host.ReleaseControl();
                else
                {
                    var newState = new ControlMouseState(this, info);
                    var location = newState.MousePosition;

                    //if (info.ConsolePosition.X >= Position.X && info.ConsolePosition.X < Position.X + Width)
                    if (location.X >= 0 && location.X <= base.Width - 1 && location.Y > -4 && location.Y < Height + 3)
                    {
                        _selectedPosition = location.X;
                        SelectedColorSafe = Surface[_selectedPosition, 0].Foreground;
                    }

                    IsDirty = true;
                }
            }

            return base.ProcessMouse(info);
        }
    }
}

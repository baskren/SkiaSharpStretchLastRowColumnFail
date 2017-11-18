using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace SkiaLattice
{
    public class Page1 : ContentPage
    {
        SKBitmap _skBitmap = null;

        Slider _startDivSlider = new Slider(0, 50, 25);
        Slider _endDivSlider = new Slider(0, 50, 25);

        SKCanvasView _skCanvasView = new SKCanvasView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 500
        };

        Label _startDivLabel = new Label();
        Label _endDivLabel = new Label();

        Switch _useLatticeSwitch = new Switch { IsToggled = true };
        Switch _use9PatchSwitch = new Switch();


        public Page1()
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("SkiaLattice.redGridBox.png"))
            using (var skStream = new SKManagedStream(stream))
                _skBitmap = SKBitmap.Decode(skStream);

            System.Diagnostics.Debug.WriteLine("");

            _skCanvasView.PaintSurface += SkCanvasView_PaintSurface;

            var offsetGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                }
            };
            offsetGrid.Children.Add(_startDivLabel, 0, 0);
            offsetGrid.Children.Add(_startDivSlider, 0, 1);
            offsetGrid.Children.Add(_endDivLabel, 1, 0);
            offsetGrid.Children.Add(_endDivSlider, 1, 1);

            _startDivSlider.ValueChanged += DivValueChanged;
            _endDivSlider.ValueChanged += DivValueChanged;

            UpdateDivLabels();

            var _test1Button = new Button { Text = "Test1" };
            _test1Button.Clicked += _test1Button_Clicked;
            var _test2Button = new Button { Text = "Test2" };
            _test2Button.Clicked += _test2Button_Clicked;

            var _test3Button = new Button { Text = "Test3" };
            _test3Button.Clicked += _test3Button_Clicked;
            var _test4Button = new Button { Text = "Test4" };
            _test4Button.Clicked += _test4Button_Clicked;

            var testGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition{ Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1,GridUnitType.Star)},
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto)},
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto)}
                }
            };
            testGrid.Children.Add(_test1Button, 0, 1);
            testGrid.Children.Add(_test2Button, 1, 1);
            testGrid.Children.Add(_test3Button, 2, 1);
            testGrid.Children.Add(_test4Button, 3, 1);


            _useLatticeSwitch.Toggled += (sender0, e0) =>
            {
                _use9PatchSwitch.IsToggled = !_useLatticeSwitch.IsToggled;
                _skCanvasView.InvalidateSurface();
            };

            _use9PatchSwitch.Toggled += (sender0, e0) =>
            {
                _useLatticeSwitch.IsToggled = !_use9PatchSwitch.IsToggled;
                _skCanvasView.InvalidateSurface();
            };

            testGrid.Children.Add(new Label { Text = "use DrawBitmapLattice:", HorizontalTextAlignment=TextAlignment.End },0,0);
            testGrid.Children.Add(_useLatticeSwitch, 1, 0);
            testGrid.Children.Add(new Label { Text = "use DrawBitmapNinePatch:", HorizontalTextAlignment = TextAlignment.End }, 2, 0);
            testGrid.Children.Add(_use9PatchSwitch, 3, 0);

            Content = new StackLayout
            {
                Padding = 20,
                Children = {
                    new Label { Text = "Last Column (or row) not stretchable in DrawBitmapLattice -or- DrawBitmapNinePatch", HorizontalOptions = LayoutOptions.Center },

                    testGrid,
                    offsetGrid,
                    _skCanvasView,
                }
            };
        }

        private void _test1Button_Clicked(object sender, EventArgs e)
        {
            _startDivSlider.Value = 0;
            _endDivSlider.Value = 25;
        }

        private void _test2Button_Clicked(object sender, EventArgs e)
        {
            _startDivSlider.Value = 1;
            _endDivSlider.Value = 25;
        }

        private void _test3Button_Clicked(object sender, EventArgs e)
        {
            _startDivSlider.Value = 25;
            _endDivSlider.Value = 49;
        }

        private void _test4Button_Clicked(object sender, EventArgs e)
        {
            _startDivSlider.Value = 25;
            _endDivSlider.Value = 50;
        }

        void UpdateDivLabels()
        {
            _startDivLabel.Text = "START DIV: ["+_startDivSlider.Value+"]";
            _endDivLabel.Text = "END DIV: ["+_endDivSlider.Value+"]";
        }

        private void DivValueChanged(object sender, ValueChangedEventArgs e)
        {
            UpdateDivLabels();
            _skCanvasView.InvalidateSurface();
        }

        private void SkCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear();

            if (_useLatticeSwitch.IsToggled)
            {
                var lattice = new SKLattice();
                lattice.Bounds = _skBitmap.Info.Rect;
                lattice.XDivs = new int[] { (int)_startDivSlider.Value, (int)_endDivSlider.Value };
                lattice.YDivs = new int[] { 0, 49 };

                System.Diagnostics.Debug.WriteLine("lattice .Bounds=[" + lattice.Bounds + "] .XDivs=[" + lattice.XDivs[0] + "," + lattice.XDivs[1] + "] .YDivs=[" + lattice.YDivs[0] + "," + lattice.YDivs[1] + "]");
                System.Diagnostics.Debug.WriteLine("e.Info.Rect=[" + e.Info.Rect + "]");
                e.Surface.Canvas.DrawBitmapLattice(_skBitmap, lattice, e.Info.Rect);
            }
            else
            {
                var center = new SKRectI((int)_startDivSlider.Value, 0, (int)_endDivSlider.Value, _skBitmap.Height - 1);
                e.Surface.Canvas.DrawBitmapNinePatch(_skBitmap, center, e.Info.Rect);
            }
        }
    }
}
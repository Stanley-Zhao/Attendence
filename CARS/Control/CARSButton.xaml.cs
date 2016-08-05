using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CARS.SourceCode;
using System.Windows.Media.Effects;

namespace CARS.Control
{
	public partial class CARSButton : UserControl, IClickable
	{
		#region Memebers
		private bool isMouseDown = false; // used to check the click
		private static readonly Color DEFAULT_ACTIVE = ColorUtil.SlateGary;
		private static readonly Color DEFAULT_NORMAL = Colors.Gray;
		private static readonly Color DEFAULT_BASE = Colors.White;
        private static readonly Color DEFAULT_FRONT_NORMAL = Colors.Black;
		private Color activeColor = ColorUtil.SlateGary;
		private Color normalColor = Colors.DarkGray;
		private Color baseColor = ColorUtil.WhiteSmoke;
		private Color borderColor = Colors.Black;
        private Color activeFrontColor = Colors.White;        
        private bool changeFrontWhenActive = true;
		private static readonly Size DEFAULT_SIZE = new Size(150, 40);
		private const string DEFAULT_TEXT = "CARSButton";
		#endregion

		#region Public Event
		public event MouseButtonEventHandler Click;
		#endregion

		#region Properties
        public bool ChangeFrontWhenActive
        {
            get { return changeFrontWhenActive; }
            set { changeFrontWhenActive = value; }
        }

		public Color BaseColor
		{
			get { return baseColor; }
			set { baseColor = value; }
		}

		public Color NormalColor
		{
			get { return normalColor; }
			set { normalColor = value; }
		}

		public Color ActiveColor
		{
			get { return activeColor; }
			set { activeColor = value; }
		}

		public Brush BorderColor
		{
			get { return borderPanel.BorderBrush; }
			set { borderPanel.BorderBrush = value; }
		}

		public new Thickness BorderThickness
		{
			get { return borderPanel.BorderThickness; }
			set { borderPanel.BorderThickness = value; }
		}

		public string Text
		{
			get { return (string)contentText.Text; }
			set { contentText.Text = value; }
		}

		public new double Height
		{
			get { return LayoutRoot.Height; }
			set
			{
				root.Height = LayoutRoot.Height = borderPanel.Height = stackPanel.Height = value;
				SetCenter();
			}
		}

		public new double Width
		{
			get { return LayoutRoot.Width; }
			set
			{
				root.Width = LayoutRoot.Width = borderPanel.Width = stackPanel.Width = value;
			}
		}

		public HorizontalAlignment TextHorizontalAligment
		{
			get { return contentText.HorizontalAlignment; }
			set
			{
				contentText.HorizontalAlignment = value;
				double marginTop = contentText.Margin.Top;
				if (value == HorizontalAlignment.Left)
				{
					contentText.Margin = new Thickness(10, marginTop, 0, 0);
				}
				else
				{
					contentText.Margin = new Thickness(0, marginTop, 0, 0);
				}
			}
		}
		#endregion

		#region Constructors
		public CARSButton()
			: this("carsButtonName")
		{
			// do nothing
		}

		public CARSButton(string name)
			: this(name, DEFAULT_TEXT, DEFAULT_SIZE)
		{
			// do nothing
		}

		public CARSButton(string name, string textPara)
			: this(name, textPara, DEFAULT_SIZE)
		{
			// do nothing
		}

		public CARSButton(string name, string textPara, Color activeColorPara)
			: this(name, textPara, DEFAULT_BASE, activeColorPara, DEFAULT_NORMAL, DEFAULT_SIZE, HorizontalAlignment.Left)
		{
			// do nothing
		}

		public CARSButton(string name, string textPara, Size size, Color activeColorPara, HorizontalAlignment alignPara)
			: this(name, textPara, DEFAULT_BASE, activeColorPara, DEFAULT_NORMAL, size, alignPara)
		{
			// do nothing
		}

		public CARSButton(string name, string textPara, Size size, HorizontalAlignment alignPara)
			: this(name, textPara, DEFAULT_BASE, ColorUtil.SlateGary, DEFAULT_NORMAL, size, alignPara)
		{
			// do nothing
		}

		public CARSButton(string name, string textPara, Size size)
			: this(name, textPara, DEFAULT_BASE, ColorUtil.SlateGary, DEFAULT_NORMAL, size, HorizontalAlignment.Left)
		{
			// do nothing
		}

		public CARSButton(string name, string textPara, Color baseColorPara, Color activeColorPara, Color normalColorPara, Size size, HorizontalAlignment alilgmentPara)
		{
			Initialization();

			this.Name = name;

			Text = textPara;
			if (baseColorPara != null)
				baseColor = baseColorPara;

			if (activeColorPara != null)
				activeColor = activeColorPara;

			if (normalColorPara != null)
				normalColor = normalColorPara;

			if (size != null)
			{
				Height = size.Height;
				Width = size.Width;
			}

			TextHorizontalAligment = alilgmentPara;

			Normal();

			SetCenter();
		}
		#endregion

		#region Event Handler
		private void Border_MouseEnter(object sender, MouseEventArgs e)
		{
			Active();
		}

		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			Normal();
		}

		private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = true;
			Pressed();
		}

		private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (isMouseDown)
			{
				// do something about click
				MouseButtonEventHandler handler = Click;
				if (handler != null)
					handler(sender, e);

				isMouseDown = false;

				Active();
			}
		}
		#endregion

		#region Private Methods
		private Effect CreateLightEffect()
		{
			DropShadowEffect dse = new DropShadowEffect();
			dse.Color = activeColor;
			dse.BlurRadius = 30;
			dse.Opacity = 0.8;
			dse.ShadowDepth = 0;
			return dse;
		}

		private void Initialization()
		{
			InitializeComponent();
			Normal();
		}

		private void SetCenter()
		{
			try
			{
				double marginTop = (stackPanel.Height - contentText.Height) / 2;
				double marginLeft = contentText.Margin.Left;
				contentText.Margin = new Thickness(marginLeft, marginTop, 0, 0);
			}
			catch { }
		}
		#endregion

		#region IClickable Members

		public void Active()
		{
			borderPanel.Background = ColorUtil.GetLinearGradientBrush(baseColor, 0, activeColor, 1, 90);
			borderPanel.Effect = CreateLightEffect();

            if(changeFrontWhenActive)
            {
                contentText.Foreground = new SolidColorBrush(activeFrontColor);
            }
		}

		public void Normal()
		{
			borderPanel.Background = ColorUtil.GetLinearGradientBrush(baseColor, 0, normalColor, 1, 90);
			borderPanel.Effect = null;

            if (changeFrontWhenActive)
            {
                contentText.Foreground = new SolidColorBrush(DEFAULT_FRONT_NORMAL);
            }
		}

		public void Pressed()
		{
			borderPanel.Background = ColorUtil.GetLinearGradientBrush(activeColor, 0, baseColor, 1, 90);
			borderPanel.Effect = CreateLightEffect();

            if (changeFrontWhenActive)
            {
                contentText.Foreground = new SolidColorBrush(activeFrontColor);
            }
		}

		#endregion
	}
}

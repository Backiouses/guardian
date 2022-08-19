using Guardian.Utilities;
using UnityEngine;

namespace Guardian.UI
{
	internal class GuiSkins
	{
		public static GUIStyle Box;

		public static GUIStyle Button;

		public static GUIStyle TextField;

		public static GUIStyle TextArea;

		public static GUIStyle HorizontalScrollbar;

		public static GUIStyle HorizontalScrollbarThumb;

		public static GUIStyle VerticalScrollbar;

		public static GUIStyle VerticalScrollbarThumb;

		private static bool IsFirstInit = true;

		public static void InitSkins()
		{
			if (IsFirstInit)
			{
				IsFirstInit = false;
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Boxes/normal.png", out var value))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Boxes/normal.png", out value);
				}
				Box = new GUIStyle(GUI.skin.box);
				Box.normal.background = value;
				GUI.skin.box = Box;
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Buttons/normal.png", out var value2))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Buttons/normal.png", out value2);
				}
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Buttons/hover.png", out var value3))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Buttons/hover.png", out value3);
				}
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Buttons/active.png", out var value4))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Buttons/active.png", out value4);
				}
				Button = new GUIStyle(GUI.skin.button);
				Button.normal.background = value2;
				Button.hover.background = value3;
				Button.active.background = value4;
				GUI.skin.button = Button;
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Text/normal.png", out var value5))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Text/normal.png", out value5);
				}
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Text/hover.png", out var value6))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Text/hover.png", out value6);
				}
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Text/active.png", out var value7))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Text/active.png", out value7);
				}
				TextField = new GUIStyle(GUI.skin.textField);
				TextField.normal.background = value5;
				TextField.hover.background = value6;
				TextField.focused.background = value7;
				TextField.focused.textColor = Color.white;
				TextField.active.background = value7;
				GUI.skin.textField = TextField;
				TextArea = new GUIStyle(GUI.skin.textArea);
				TextArea.normal.background = value5;
				TextArea.hover.background = value6;
				TextArea.focused.background = value7;
				TextArea.focused.textColor = Color.white;
				TextArea.active.background = value7;
				GUI.skin.textArea = TextArea;
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Scrollbars/background.png", out var value8))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Scrollbars/background.png", out value8);
				}
				if (!ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/UI/Scrollbars/bar.png", out var value9))
				{
					ResourceLoader.TryGetAsset<Texture2D>("Textures/UI/Scrollbars/bar.png", out value9);
				}
				HorizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
				HorizontalScrollbar.normal.background = value8;
				GUI.skin.horizontalScrollbar = HorizontalScrollbar;
				HorizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
				HorizontalScrollbarThumb.normal.background = value9;
				GUI.skin.horizontalScrollbarThumb = HorizontalScrollbarThumb;
				VerticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
				VerticalScrollbar.normal.background = value8;
				GUI.skin.verticalScrollbar = VerticalScrollbar;
				VerticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
				VerticalScrollbarThumb.normal.background = value9;
				GUI.skin.verticalScrollbarThumb = VerticalScrollbarThumb;
			}
		}
	}
}

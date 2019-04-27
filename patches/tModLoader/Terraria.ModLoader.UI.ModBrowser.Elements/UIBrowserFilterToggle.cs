using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Terraria.ModLoader.UI.ModBrowser.Elements
{
	internal class UIBrowserFilterToggle<T> : UICycleImage where T : struct, Enum
	{
		private static readonly Texture2D Texture = Texture2D.FromStream(Main.instance.GraphicsDevice, Assembly.GetExecutingAssembly().GetManifestResourceStream("Terraria.ModLoader.UI.UIModBrowserIcons.png"));

		public T State {
			get;
			protected set;
		}

		public UIBrowserFilterToggle(int textureOffsetX, int textureOffsetY, int padding = 2) 
			: base(Texture, Enum.GetValues(typeof(T)).Length, 32, 32, textureOffsetX, textureOffsetY, padding) {
			
			OnClick += UpdateToNext;
			OnRightClick += UpdateToPrevious;
		}

		public void SetCurrentState(T @enum) {
			State = @enum;
			base.SetCurrentState((int)(object)State);
		}

		private void UpdateToNext(UIMouseEvent @event, UIElement element) {
			SetCurrentState(State.NextEnum());
			Interface.modBrowser.updateNeeded = true;
		}

		private void UpdateToPrevious(UIMouseEvent @event, UIElement element) {
			SetCurrentState(State.PreviousEnum());
			Interface.modBrowser.updateNeeded = true;
		}
	}
}
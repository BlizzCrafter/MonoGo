using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MonoGo.Samples.Demos
{
	public class InputDemo : Entity
	{
		public static readonly string Description =
            "Input > ${FC:FFDB5F}" + KeyboardTestButton + "${RESET} / ${FC:FFDB5F}" + GamepadTestButton + "${RESET} / ${FC:FFDB5F}" + MouseTestButton + "${RESET}" + Environment.NewLine +
            "Keyboard > ${FC:FFDB5F}Press any Key${RESET}" + Environment.NewLine +
            "Gamepad > ${FC:96FF5F}Rumble${RESET}: ${FC:FFDB5F}Move Triggers${RESET}";

		Color _mainColor = Color.White;

		StringBuilder _keyboardInput = new StringBuilder();
		int _keyboardInputMaxLength = 32;

		public const Buttons KeyboardTestButton = Buttons.A;
		public const Buttons GamepadTestButton = Buttons.GamepadA;
		public const Buttons MouseTestButton = Buttons.MouseLeft;

		public InputDemo(Layer layer) : base(layer)
		{
        }

		public override void Update()
		{
			base.Update();

			// Leaving only letters and digits.
			_keyboardInput.Append(Regex.Replace(Input.KeyboardString, @"[^A-Za-z0-9]+", string.Empty));

			// Backspace.
			if (Input.KeyboardKey == Microsoft.Xna.Framework.Input.Keys.Back)
			{
				if (_keyboardInput.Length >= 2)
				{
					_keyboardInput.Remove(_keyboardInput.Length - 2, 2);
				}
				else
				{
					_keyboardInput.Clear();
				}
			}

			// Limiting string length.
			if (_keyboardInput.Length > _keyboardInputMaxLength)
			{
				var overflow = _keyboardInput.Length - _keyboardInputMaxLength;
				_keyboardInput.Remove(0, overflow);
			}

			// Time to get your Rumble Pak (tm), kidz!
			Input.GamepadSetVibration(
				0,
				Input.GamepadGetLeftTrigger(0),
				Input.GamepadGetRightTrigger(0)
			);
		}

		public override void Draw()
		{
			base.Draw();

			var startingPosition = new Vector2(64, 64);
			var position = startingPosition;

			GraphicsMgr.CurrentColor = _mainColor;

			// This position only accounts for screen transformation.
			// When the camera will move, it will offset.
			CircleShape.Draw(Input.ScreenMousePosition, 8, ShapeFill.Outline);

			// You can also get mouse position from any camera.
			// This method can be used in Update, when no camera is active.
			CircleShape.Draw(GraphicsMgr.CurrentCamera.GetRelativeMousePosition(), 12, ShapeFill.Outline);

			Text.Draw("Keyboard input: " + _keyboardInput.ToString(), position);

			// Gamepad, mouse and keyboard buttons are using the same method. 
			position += Vector2.UnitY * 64;
			if (Input.CheckButton(KeyboardTestButton))
			{ 
				CircleShape.Draw(position, 16, ShapeFill.Solid);
			}
			else
			{ 
				CircleShape.Draw(position, 16, ShapeFill.Outline);
			}
			position += Vector2.UnitX * 64;
			if (Input.CheckButton(GamepadTestButton))
			{
				CircleShape.Draw(position, 16, ShapeFill.Solid);
			}
			else
			{
				CircleShape.Draw(position, 16, ShapeFill.Outline);
			}
			position += Vector2.UnitX * 64;
			if (Input.CheckButton(MouseTestButton))
			{
				CircleShape.Draw(position, 16, ShapeFill.Solid);
			}
			else
			{
				CircleShape.Draw(position, 16, ShapeFill.Outline);
			}

			position = new Vector2(200, 200);

			if (Input.GamepadConnected(0))
			{
				Text.Draw("Gamepad is connected!", position);
			}
			else
			{
				Text.Draw("Gamepad is not connected.", position);
			}

			// Sticks.
			position += Vector2.UnitY * 96;
			CircleShape.Draw(position, 64, ShapeFill.Outline);
			CircleShape.Draw(position + Input.GamepadGetLeftStick(0) * 64 * new Vector2(1, -1), 16, ShapeFill.Solid);
			position += Vector2.UnitX * (128 + 64);
			CircleShape.Draw(position, 64, ShapeFill.Outline);
			CircleShape.Draw(position + Input.GamepadGetRightStick(0) * 64 * new Vector2(1, -1), 16, ShapeFill.Solid);

			// Triggers.
			position -= Vector2.UnitX * (64 + 16);
			RectangleShape.DrawBySize(position + Vector2.UnitY * Input.GamepadGetRightTrigger(0) * 64, Vector2.One * 8, ShapeFill.Solid);
			LineShape.Draw(position, position + Vector2.UnitY * 64);
			position -= Vector2.UnitX * 32;
			RectangleShape.DrawBySize(position + Vector2.UnitY * Input.GamepadGetLeftTrigger(0) * 64, Vector2.One * 8, ShapeFill.Solid);
			LineShape.Draw(position, position + Vector2.UnitY * 64);
		}

		public override void Destroy()
		{
			Input.GamepadSetVibration(0, 0, 0);
		}
	}
}

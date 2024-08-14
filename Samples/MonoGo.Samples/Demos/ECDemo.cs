using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Samples.Misc;
using System;

namespace MonoGo.Samples.Demos
{
	public class ECDemo : Entity
	{
		public static readonly string Description =
			"Move > ${FC:FFDB5F}WASD${RESET}" + Environment.NewLine +
			"Bots > ${FC:96FF5F}Update${RESET}: ${FC:FFDB5F}" + ToggleEnabledButton + " ${FC:96FF5F}Draw${RESET}: ${FC:FFDB5F}" + ToggleVisibilityButton + " ${RESET}" + Environment.NewLine +
			"Camera > ${FC:96FF5F}Move${RESET}: ${FC:FFDB5F}" + CameraController.UpButton + "${RESET} / ${FC:FFDB5F}" + CameraController.DownButton + "${RESET} / ${FC:FFDB5F}" + CameraController.LeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RightButton + "${RESET}" + Environment.NewLine +
			"Camera > ${FC:96FF5F}Rotate${RESET}: ${FC:FFDB5F}" + CameraController.RotateLeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RotateRightButton + "${RESET}" + Environment.NewLine +
			"Camera > ${FC:96FF5F}Zoom${RESET}: ${FC:FFDB5F}" + CameraController.ZoomInButton + "${RESET} / ${FC:FFDB5F}" + CameraController.ZoomOutButton + "${RESET}" + Environment.NewLine;


        public const Buttons ToggleVisibilityButton = Buttons.N;
		public const Buttons ToggleEnabledButton = Buttons.M;

		public ECDemo(Layer layer) : base(layer)
		{
			for (var i = 0; i < 20; i += 1)
			{
				var bot = new Bot(layer);
				var position = bot.GetComponent<PositionComponent>();
				position.Position = new Vector2(GameController.Random.Next(100, 700), GameController.Random.Next(100, 500));
			}

			var player = new Player(layer, new Vector2(400, 300));

			// Now player will be drawn below all the bots, even though he was created last.
			// Reaordering puts him on the top of entity list.
			layer.ReorderEntityToTop(player);
			//layer.ReorderEntityToBottom(player); // Will have no effect.
			//layer.ReorderEntity(player, 2); // Player will be updated and drawn only after two entities above him.
		}

		public override void Update()
		{
			base.Update();

			if (Input.CheckButtonPress(ToggleVisibilityButton))
			{
				// This will turn off Draw events for bot's entity and all of its components.
				foreach (var bot in Layer.GetEntityList<Bot>())
				{
					bot.Visible = !bot.Visible;
				}
			}

			if (Input.CheckButtonPress(ToggleEnabledButton))
			{
				// This will turn off Update events for bot's entity and all of its components.
				foreach(var bot in Layer.GetEntityList<Bot>())
				{
					bot.Enabled = !bot.Enabled;
				}
			}
		}
	}
}

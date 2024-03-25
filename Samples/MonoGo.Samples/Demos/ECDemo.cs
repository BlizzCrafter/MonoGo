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
            "Move > {{YELLOW}}WASD{{DEFAULT}}" + Environment.NewLine +
            "Bots > {{L_GREEN}}Update{{DEFAULT}}: {{YELLOW}}" + ToggleEnabledButton + "{{L_GREEN}} Draw{{DEFAULT}}: {{YELLOW}}" + ToggleVisibilityButton + "{{DEFAULT}}";

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

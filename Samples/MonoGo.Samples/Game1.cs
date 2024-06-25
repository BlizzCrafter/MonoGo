using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using MonoGo.Resources;
using MonoGo.Tiled;
using MonoGo.Tiled.MapStructure;

namespace MonoGo.Samples
{
    /// <summary>
    /// This is the main type for your game.
	/// Note: You need to inherit from the MonoGoGame class!
    /// </summary>
    public class Game1 : MonoGoGame
    {
		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
        {
			base.Initialize();

            TiledEntityFactoryPool.InitFactoryPool();
            new GameController();
        }

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
        {
			base.LoadContent();

            new DirectoryResourceBox<TiledMap>("Maps", "Maps");
            new SpriteGroupResourceBox("DemoSprites", "Demo");

            var fontSprite = ResourceHub.GetResource<Sprite>("DemoSprites", "Font");
            var fontBox = ResourceHub.GetResourceBox("Fonts") as ResourceBox<IFont>;
            fontBox.AddResource("FancyFont", new TextureFont(fontSprite, 1, 1, TextureFont.Ascii, false));
        }

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{			
			base.Update(gameTime);

			//You update logic here!
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
        {
			base.Draw(gameTime);

            //You draw logic here!
        }
    }
}

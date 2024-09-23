using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGo.Engine;

namespace MGNamespace
{
    // Inherit from MonoGoGame to start using the MonoGo Engine!
    public class Game1 : MonoGoGame
    {
        protected override void Initialize()
        {
            base.Initialize();

            // This is just a sample.
            // Remove or modify the GameController as you wish!
            new GameController();
        }

        protected override void LoadContent()
        {
            // IMPORTANT: Don't delete; it loads engine specific stuff.
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}

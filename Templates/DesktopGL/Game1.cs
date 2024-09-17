using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGo.Engine;

namespace MGNamespace
{
    public class Game1 : MonoGoGame
    {
        protected override void Initialize()
        {
            base.Initialize();

            new GameController();
        }

        protected override void LoadContent()
        {
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DataOrientedEngine.Engine;
using DataOrientedEngine.Components;

namespace DataOrientedEngine
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch _primitiveBatch;

        private Scene mainScene;
        private Color clearColor;
        private float roundedness = 0;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void UnloadContent()
        {
            Scene.ActiveScene.Destroy(Content);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            clearColor = new Color(0xff181818);

            mainScene = new Scene("Main Scene");
            Scene.ActiveScene = mainScene;

            Entity simpleEntity = new Entity("Example");
            mainScene.AddEntity(simpleEntity);

            simpleEntity.AddComponent(new Movement(simpleEntity.ID, 100, 150));
            simpleEntity.AddComponent(new SpriteRenderer(simpleEntity.ID, Content.Load<Texture2D>("TileSet1")) { Scale = Vector2.One * 2});
            simpleEntity.AddComponent(new Animator(simpleEntity.ID));
            Systems.AnimatorComps[simpleEntity.ID].Animations.Add("Acid", new DataOrientedEngine.Components.Utility.Animation("Acid", new Rectangle[] { new Rectangle(0, 0, 64, 64), new Rectangle(64, 0, 64, 64), new Rectangle(128, 0, 64, 64), new Rectangle(192, 0, 64, 64), new Rectangle(256, 0, 64, 64) }) { Loop = true, Speed = 2 });
            Systems.AnimatorComps[simpleEntity.ID].Play("Acid");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Scene.ActiveScene.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Systems.MovementComps[0].DeltaX += (float)gameTime.ElapsedGameTime.TotalSeconds * 50;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Systems.MovementComps[0].DeltaX -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Systems.MovementComps[0].DeltaY += (float)gameTime.ElapsedGameTime.TotalSeconds * 50;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Systems.MovementComps[0].DeltaY -= (float)gameTime.ElapsedGameTime.TotalSeconds * 50;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                roundedness += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                roundedness -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                Systems.AnimatorComps[0].Play("Acid");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(clearColor);

            // TODO: Add your drawing code here
            Scene.ActiveScene.Draw(_spriteBatch);

            _primitiveBatch.Begin(PrimitiveType.LineList);

            _primitiveBatch.DrawRoundedRectangle(new Rectangle(50, 50, 200, 100), roundedness, Color.White);

            _primitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DataOrientedEngine.Engine;
using DataOrientedEngine.Components;
using DataOrientedEngine.Components.UI;

namespace DataOrientedEngine
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch _primitiveBatch;

        private Scene mainScene;
        private Color clearColor;
        private float roundedness = 0.5f;
        private SpriteFont Font;

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
            //_graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //_graphics.PreferMultiSampling = true;
            //GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            //_graphics.ApplyChanges();

            clearColor = new Color(0xff181818);

            Font = Content.Load<SpriteFont>("Font");

            mainScene = new Scene("Main Scene");
            Scene.ActiveScene = mainScene;

            Entity simpleEntity = new Entity("Example");
            mainScene.AddEntity(simpleEntity);

            /*
            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            */

            new Movement(simpleEntity.ID, 100, 150);
            new ParticleGenerator(simpleEntity.ID, GraphicsDevice);
            new Text(simpleEntity.ID, Font, Mouse.GetState().Position.ToString(), Vector2.Zero) { Color = Color.LightGreen };

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(clearColor);

            // TODO: Add your drawing code here
            // Scene
            Scene.ActiveScene.Draw(_spriteBatch);

            // Primitives
            _primitiveBatch.Begin(PrimitiveType.TriangleList);

            //_primitiveBatch.BezierLine(new Point(50, 50), Mouse.GetState().Position, new Point(150, 50), 5, Color.White, 16);

            _primitiveBatch.End();

            // UI
            _spriteBatch.Begin();

            Systems.TextRendering(_spriteBatch);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}

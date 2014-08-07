#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Tao.Sdl;
using TiledSharp;
#endregion

namespace Eteephonehome
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Eteephonehome : Game
    {

        protected int x_resolution;
        protected int y_resolution;
        protected int tile_dimension;
        protected int status_bar_dimension = 75;
        LevLoader map;
        Menu main_menu;
        public string curScreen;
        public MapLoader Loader;
        GameScreen CurScreen;
        SplashScreen Splash;
        int curLevel;
        public SongManager audio;
        bool firstmap = true;

        Controls controls;
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;

        public Eteephonehome(int x_resolution, int y_resolution, int tile_dimension)
        {
            audio = new SongManager(Content);
            Loader = new MapLoader("mappack");
            this.x_resolution = x_resolution;
            this.y_resolution = y_resolution;
            this.tile_dimension = tile_dimension;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            main_menu = new Menu("Main", true, "emulogic", 960, 715, this);

        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = x_resolution;
            graphics.PreferredBackBufferHeight = y_resolution;
            graphics.ApplyChanges();
            base.Initialize();
            Joystick.Init();
            curScreen = "menu";
            controls = new Controls();
        }
        public void StartGame()
        {
            main_menu.Hide();
            map.Show();
            curScreen = "game";
        }
        public void NextLevel()
        {
            //Ask for next map in pack
            int MapToLoad = Loader.GetNextMap();
            if(MapToLoad !=0)
            { 
            //if we get a number back i.e. there is another map:
            map = new LevLoader(this.Content, graphics, tile_dimension, tile_dimension, x_resolution, y_resolution, this, MapToLoad);

            map.loadLevel(Content);
            map.Show();
            curScreen = "game";
            curLevel = MapToLoad;
            }
            else
            {
                //What happens if we beat every level i.e. YOU WIN
                LoadSplash("WinScreen.png",true);
            }

        }
        public void LoadSplash(String splashname,bool winGame)
        {
            curScreen = "splash";
            Splash = new SplashScreen(splashname,this,winGame);
            Splash.Show();

        }
        public void ShowMenu()
        {
            Splash.Hide();
            main_menu.Show();

        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            main_menu.LoadContent(this.Content);
            main_menu.Show();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            controls.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (curScreen == "game")
            {
                map.Update(controls, gameTime);

                base.Update(gameTime);
                return;
            }
            if (curScreen == "splash")
            {
                Splash.Update(controls, gameTime);

                base.Update(gameTime);
                return;
            }
            if (curScreen == "menu")
            {
                main_menu.Update(controls, gameTime);

                base.Update(gameTime);
                return;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();

            if (curScreen == "game")
                map.Draw(spriteBatch);
            if (curScreen == "menu")
                main_menu.Draw(spriteBatch);
            if (curScreen == "splash")
                Splash.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        public void ReloadLevel()
        {
            map = new LevLoader(this.Content, graphics, tile_dimension, tile_dimension, x_resolution, y_resolution, this, curLevel);
            map.loadLevel(Content);
            map.Show();
        }
    }

}


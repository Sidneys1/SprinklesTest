using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1 {
    public class Game1 : Game {
        private readonly Random _r = new Random();
        private readonly NetworkInterface _slectedNic;

        private readonly List<FallingSprite> _sprites = new List<FallingSprite>();
        private readonly UnicastIPAddressInformation _uniCastIpInfo;

        private long _brlast;
        private SpriteFont _font;

        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphics;
        private Texture2D _img;
        private SpriteBatch _spriteBatch;
        private string _text = string.Empty;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };

            Content.RootDirectory = "Content";

            var allNetworkInterfaces = NetworkInterface
                .GetAllNetworkInterfaces();
            _slectedNic = allNetworkInterfaces[0];


            if (_slectedNic?.GetIPProperties().UnicastAddresses == null) return;

            var ipInfo = _slectedNic.GetIPProperties().UnicastAddresses;

            foreach (var item in ipInfo) {
                if (item.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                _uniCastIpInfo = item;
                break;
            }
        }

        public long BandwidthCalculator(UnicastIPAddressInformation ipInfo, NetworkInterface selecNic) {
            try {
                if (selecNic == null)
                    return 0;
                var interfaceData = selecNic.GetIPv4Statistics();
                return (interfaceData.BytesReceived + interfaceData.BytesSent) / 1024;
            }
            catch {
                return 0;
            }
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _img = Content.Load<Texture2D>("Untitled");
            _font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var width = GraphicsDevice.Viewport.Width;
            var height = GraphicsDevice.Viewport.Height;

            foreach (var fallingSprite in _sprites)
                fallingSprite.Tick(gameTime);

            for (var i = 0; i < _sprites.Count; i++)
                if (_sprites[i].Position.Y > GraphicsDevice.Viewport.Height)
                    _sprites.RemoveAt(i);

            var recv = BandwidthCalculator(_uniCastIpInfo, _slectedNic);
            _text = recv.ToString();
            if (_brlast != 0)
                for (var i = 0; i < recv - _brlast; i++)
                    _sprites.Add(new FallingSprite(
                        (float) (_r.NextDouble() * (GraphicsDevice.Viewport.Width - 24)),
                        (float) (_r.NextDouble() * 400 - 200),
                        (float) (_r.NextDouble() * 400 - 200),
                        width, height,
                        _r.Next(5, 9) / 10f));
            _brlast = recv;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin();

            foreach (var fallingSprite in _sprites)
                _spriteBatch.Draw(_img, fallingSprite.Position);

            _spriteBatch.DrawString(_font, _text, Vector2.Zero, Color.White);
            _spriteBatch.DrawString(_font, $"{_sprites.Count}", new Vector2(0, 20), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
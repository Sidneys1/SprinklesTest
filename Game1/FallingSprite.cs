using Microsoft.Xna.Framework;

namespace Game1 {
    public class FallingSprite {
        private const float ACCEL = 9.8f;

        private readonly float _bounce;

        private readonly int _height;
        private readonly int _width;

        private bool _passthrough;

        public FallingSprite(float x, float hspeed, float vspeed, int width, int height, float bounciness) {
            Position = new Vector2(x, -24);
            Speed = new Vector2(hspeed, vspeed);
            _height = height;
            _width = width;
            _bounce = -bounciness;
        }

        public Vector2 Position { get; private set; }

        public Vector2 Speed { get; private set; }

        public void Tick(GameTime t) {
            Speed = new Vector2(Speed.X, Speed.Y + ACCEL);
            Position = Position + Speed * (float) t.ElapsedGameTime.TotalSeconds;

            if (Position.X <= 0) {
                Speed *= new Vector2(-1, 1);
                Position *= new Vector2(-1, 1);
            }

            if (Position.X + 27 >= _width) {
                Speed *= new Vector2(_bounce, 1);
                Position = new Vector2(_width + (_width - (Position.X + 27)) - 27, Position.Y);
            }

            if (_passthrough || !(Position.Y + 27 >= _height)) return;

            Speed *= new Vector2(1, _bounce);
            var diff = _height - (Position.Y + 27);
            if (Speed.Y > -50) {
                Speed *= new Vector2(1, 0);
                diff = 0;
                _passthrough = true;
            }
            Position = new Vector2(Position.X, _height + diff - 27);
        }
    }
}
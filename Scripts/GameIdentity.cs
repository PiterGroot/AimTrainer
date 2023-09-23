using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstMonoGame.Scripts {
    public class GameIdentity {

        public GameIdentity(string objectName = "", Texture2D texture = null, int renderOrder = 0) {
            Name = objectName == string.Empty ? "NewGameIdentity" : objectName;
            UniqueId = GameHelper.RandomHandler.GetRandomIntNumber(0, 99999);

            Transform = new Transform();
            Visual = new Visual(texture, Color.White);

            RenderOrder = renderOrder;
            Visual.targetTexture = texture;
        }

        public string Name { get; set; }
        public int UniqueId { get; set; }

        public int RenderOrder { get; set; }

        public Transform Transform { get; set; }
        public Visual Visual { get; set; }

    }

    public class Visual {
        public Visual(Texture2D targetTexture, Color textureColor) {
            this.targetTexture = targetTexture;
            this.textureColor = textureColor;
        }

        public Color textureColor = Color.White;
        public Texture2D targetTexture;
    }

    public class Transform {
        public Vector2 position = Vector2.Zero;
        public Quaternion rotation;
        public Vector2 scale = Vector2.One;
    }
}

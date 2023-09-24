using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FirstMonoGame.Scripts {
    public class GameIdentity{

        public GameIdentity(string objectName = "", string texture = "", int renderOrder = 0) {
            Name = objectName == string.Empty ? "NewGameIdentity" : objectName;
            UniqueId = GameHelper.RandomHandler.GetRandomIntNumber(0, 99999);

            Texture2D chosenTexture = GameIdentityManager.Instance.ContentManager.Load<Texture2D>(texture);

            Transform = new Transform();
            Visual = new Visual(chosenTexture, Color.White);

            RenderOrder = renderOrder;
            Active = true;
        }

        public string Name { get; set; }
        public int UniqueId { get; set; }

        public bool Active { get; set; }

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
        public float rotation;
        public Vector2 scale = Vector2.One;
    }
}

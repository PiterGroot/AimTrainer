using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FirstMonoGame.Scripts {
    public class GameIdentity{

        public GameIdentity(string objectName = "", string texture = "", int renderOrder = 0, bool orginSelf = true) {
            Name = objectName == string.Empty ? "NewGameIdentity" : objectName;

            while (true) {
                UniqueId = GameHelper.RandomHandler.GetRandomIntNumber(0, 99999);
                if (GameIdentityManager.Instance.IsUniqueIdentity(UniqueId)) break;
            }

            Texture2D loadedTexture = null;
            try {
                loadedTexture = GameIdentityManager.Instance.ContentManager.Load<Texture2D>(texture);
            }
            catch {
                loadedTexture = GameIdentityManager.Instance.ContentManager.Load<Texture2D>("null");
            }

            Transform = new Transform();
            Visual = new GameVisual(loadedTexture, Color.White);

            if(orginSelf) Transform.originOffset = new Vector2(loadedTexture.Width / 2f, loadedTexture.Height / 2f);

            RenderOrder = renderOrder;
            Active = true;
        }

        public string Name { get; set; }
        public int UniqueId { get; set; }

        public bool Active { get; set; }
        public int RenderOrder { get; set; }

        public Transform Transform { get; set; }
        public GameVisual Visual { get; set; }

    }

    public class GameVisual {
        public GameVisual(Texture2D targetTexture, Color textureColor) {
            this.targetTexture = targetTexture;
            this.textureColor = textureColor;
        }

        public Color textureColor = Color.White;
        public Texture2D targetTexture;
    }

    public class Transform {
        public float rotation = 0;
        public Vector2 position = Vector2.Zero;
        public Vector2 scale = Vector2.One;
        public Vector2 originOffset = Vector2.Zero;
    }
}

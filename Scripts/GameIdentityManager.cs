using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace FirstMonoGame.Scripts {
    public class GameIdentityManager {

        public static GameIdentityManager Instance;
        
        public List<GameIdentity> ActiveGameIdentities { get; set; }
        public ContentManager ContentManager { get; set; }

        public GameIdentityManager(ContentManager manager) {
            ActiveGameIdentities = new List<GameIdentity>();
            ContentManager = manager;
            Instance = this;
        }

        public void InstantiateIdentity(GameIdentity gameIdentity) {
            if (!ActiveGameIdentities.Contains(gameIdentity)) {
                ActiveGameIdentities.Add(gameIdentity);
                UpdateGameIdentitiesOrder();
            }
            else {
                string message = $"GameIdentity {gameIdentity.Name}[{gameIdentity.UniqueId}] is already instantiated";
                GameHelper.ExitWithDebugMessage(message);
            }
        }

        public void DestroyIdentity(GameIdentity gameIdentity) {
            if (ActiveGameIdentities.Contains(gameIdentity)) {
                ActiveGameIdentities.Remove(gameIdentity);
                UpdateGameIdentitiesOrder();
            }
            else {
                string message = $"GameIdentity {gameIdentity.Name}[{gameIdentity.UniqueId}] cannot be destroyed because it does not exist";
                GameHelper.ExitWithDebugMessage(message);
            }
        }

        private void UpdateGameIdentitiesOrder() {
            ActiveGameIdentities = ActiveGameIdentities.OrderByDescending(identity => identity.RenderOrder).ToList();
            ActiveGameIdentities.Reverse();
        }

        public void DrawGameIdentities(SpriteBatch spriteBatch, GraphicsDevice device) {
            device.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            int l = ActiveGameIdentities.Count;
            for (int i = 0; i < l; i++) {
                GameIdentity identity = ActiveGameIdentities[i];
                if (!identity.Active) continue;

                DrawIdentity(spriteBatch, identity);
            }

            spriteBatch.End();
        }

        private void DrawIdentity(SpriteBatch batch, GameIdentity gameIdentity) {
            batch.Draw(gameIdentity.Visual.targetTexture, gameIdentity.Transform.position, null, 
            gameIdentity.Visual.textureColor, gameIdentity.Transform.rotation, gameIdentity.Transform.originOffset,
            gameIdentity.Transform.scale, SpriteEffects.None, 0);
        }
    }
}

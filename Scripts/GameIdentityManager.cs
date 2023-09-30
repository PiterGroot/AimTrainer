using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace FirstMonoGame.Scripts {
    public class GameIdentityManager {

        public static GameIdentityManager Instance;
        
        private Dictionary<int, GameIdentity> ActiveGameIdentities { get; set; }
        public ContentManager ContentManager { get; set; }

        public GameIdentityManager(ContentManager manager) {
            ActiveGameIdentities = new Dictionary<int, GameIdentity>();
            ContentManager = manager;
            Instance = this;
        }

        public void InstantiateIdentity(GameIdentity gameIdentity) {
            if (!ActiveGameIdentities.ContainsKey(gameIdentity.UniqueId)) {
                ActiveGameIdentities.Add(gameIdentity.UniqueId, gameIdentity);
                UpdateGameIdentitiesOrder();
            }
            else {
                string message = $"GameIdentity {gameIdentity.Name}[{gameIdentity.UniqueId}] is already instantiated";
                GameHelper.ExitWithDebugMessage(message);
            }
        }

        public void DestroyIdentity(GameIdentity gameIdentity) {
            if (ActiveGameIdentities.ContainsKey(gameIdentity.UniqueId)) {
                ActiveGameIdentities.Remove(gameIdentity.UniqueId);
                UpdateGameIdentitiesOrder();
            }
            else {
                string message = $"GameIdentity {gameIdentity.Name}[{gameIdentity.UniqueId}] cannot be destroyed because it does not exist";
                GameHelper.ExitWithDebugMessage(message);
            }
        }

        public bool IsUniqueIdentity(int identityId) => !ActiveGameIdentities.ContainsKey(identityId);

        private void UpdateGameIdentitiesOrder() {
            List<KeyValuePair<int, GameIdentity>> identityList = ActiveGameIdentities.ToList();
            identityList.Sort((identityA, identityB) => identityA.Value.RenderOrder.CompareTo(identityB.Value.RenderOrder));

            ActiveGameIdentities = identityList.ToDictionary(key => key.Key, value => value.Value);
        }

        public void DrawGameIdentities(SpriteBatch spriteBatch, GraphicsDevice device) {
            device.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            foreach (GameIdentity identity in ActiveGameIdentities.Values) {
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

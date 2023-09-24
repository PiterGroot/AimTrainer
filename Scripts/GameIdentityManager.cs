using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

namespace FirstMonoGame.Scripts {
    public class GameIdentityManager {

        public static GameIdentityManager Instance;
        
        public List<GameIdentity> ActiveGameIdentities { get; set; }
        
        public GameIdentityManager() {
            ActiveGameIdentities = new List<GameIdentity>();
            Instance = this;
        }

        public void InstantiateIdentity(GameIdentity gameIdentity) {
            if (!ActiveGameIdentities.Contains(gameIdentity)) {
                ActiveGameIdentities.Add(gameIdentity);
                UpdateGameIdentitiesOrder();
            }
            else {
                Debug.WriteLine("");
                Debug.WriteLine($"GameIdentity {gameIdentity.Name}[{gameIdentity.UniqueId}] already instantiated");
                Debug.WriteLine("");
                Environment.Exit(0);
            }
        }

        public void DestroyIdentity(GameIdentity gameIdentity) {
            if (ActiveGameIdentities.Contains(gameIdentity)) {
                ActiveGameIdentities.Remove(gameIdentity);
                UpdateGameIdentitiesOrder();
            }
            else {
                Debug.WriteLine("");
                Debug.WriteLine($"GameIdentity {gameIdentity.Name}[{gameIdentity.UniqueId}] cannot be destroyed because it does not exist");
                Debug.WriteLine("");
                Environment.Exit(0);
            }
        }

        private void UpdateGameIdentitiesOrder() {
            ActiveGameIdentities = ActiveGameIdentities.OrderByDescending(identity => identity.RenderOrder).ToList();
            ActiveGameIdentities.Reverse();
        }
    }
}

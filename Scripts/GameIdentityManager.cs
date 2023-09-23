using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FirstMonoGame.Scripts {
    public class GameIdentityManager {

        public static GameIdentityManager Instance;
        
        public List<GameIdentity> ActiveGameIdentities { get; set; }
        
        public GameIdentityManager() {
            ActiveGameIdentities = new List<GameIdentity>();
            Instance = this;
        }

        public void RegisterGameIdentity(GameIdentity gameIdentity) {
            if (!ActiveGameIdentities.Contains(gameIdentity)) {
                ActiveGameIdentities.Add(gameIdentity);
                ActiveGameIdentities = ActiveGameIdentities.OrderByDescending(identity => identity.RenderOrder).ToList();
                ActiveGameIdentities.Reverse();
            }
        }
    }
}

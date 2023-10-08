using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace AndromedaAP.UI
{
    [Autoload(Side = ModSide.Client)]
    public class AndromedaAPUISystem : ModSystem
    {
        //Set the state and the interface itself.
        internal LiftoffBarState liftoffbar;
        private UserInterface _liftoffbar;

        //Set the state of the interface to show the liftoff bar itself.
        public void showLiftoffBar() { _liftoffbar.SetState(liftoffbar); }

        //Set the state of the interface to hide the liftoff bar itself.
        public void hideLiftoffBar() { _liftoffbar.SetState(null); }

        //Basically initialize the state and activate it before initializing the interface.
        public override void Load()
        {
            liftoffbar = new LiftoffBarState();
            _liftoffbar = new UserInterface();

            liftoffbar.Activate();

        }

        //UpdateUI stuff.
        public override void UpdateUI(GameTime gameTime)
        {
            _liftoffbar?.Update(gameTime);
        }


        //The magic black box code, basically just inserting a new layer on the UI interface.
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "AndromedaAP: Liftoff Bar",
                    delegate
                    {
                        if (_liftoffbar?.CurrentState != null) { 
                        _liftoffbar.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}

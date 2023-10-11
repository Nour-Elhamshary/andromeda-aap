using AndromedaAP.UI;
using CalamityMod;
using CalamityMod.Tiles.FurnitureVoid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace AndromedaAP.Players
{
    public class AAPEquippedPlayer : ModPlayer
    {
        //If the wings are equipped
        public bool isEquipped;

        /*
        Liftoff stats.
        maxLiftoff is the maximum amount to charge until liftoff
        currentLiftoff is how many liftoff charge is available.
        liftoffCharge is a boolean to check if UP+DOWN are pressed.
        liftoffDischarge is a boolen to check if liftoff is decreasing continuously.
        */
        
        public float maxLiftoff = 20f;
        public float currentLiftoff = 0f;
        public bool liftoffready;
        public bool liftoffCharge;
        public bool liftoffDischarge; //haha funny

        float currentWingTime = 0f;
        bool hasWingTimeChanged = true;

        public bool flag21 = false;
        void changeWingTime(Player player)
        {
            if (hasWingTimeChanged) return;
            else { player.wingTime = currentWingTime; hasWingTimeChanged = true; }
        }

        //Keep on putting them by default if nothing changed.
        public override void ResetEffects()
        {
            isEquipped = false;
            flag21 = false;

            liftoffCharge = false;
            if (currentLiftoff < maxLiftoff && currentLiftoff == 0f) liftoffready = false;
            if (liftoffready == false) liftoffDischarge = false;
        }



        //General update tasks, check if UP+DOWN are pressed and try to make sure the currentLiftoff are clamped
        public override void PostUpdateMiscEffects()
        {

            currentLiftoff = Utils.Clamp(currentLiftoff, 0, maxLiftoff);

            if (currentLiftoff == maxLiftoff && !liftoffready)
            {
                SoundStyle ting = new SoundStyle("AndromedaAP/Assets/Sounds/ting!") with
                {
                    MaxInstances = 1,
                    IsLooped = false,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                };

                liftoffready = true;
                SoundEngine.PlaySound(ting);
            }
        }

        //When charging liftoff, disable down being pressed for a bit.
        public override void SetControls()
        {
            if (liftoffCharge) { Main.LocalPlayer.controlDown = false; Main.LocalPlayer.controlDownHold = false; }
        }

        public override void PostUpdateBuffs()
        {
            //If UP+DOWN is pressed
            if (liftoffCharge)  Main.LocalPlayer.wingTimeMax = 0;

        }

        public override void PostUpdateEquips() {

            //If UP+DOWN is pressed, then its charging!!!
            if (PlayerInput.Triggers.Current.Up && PlayerInput.Triggers.Current.Down) liftoffCharge = true;

            //While its charging:
            if (liftoffCharge) { 
                //Set the wingTimeMax and wingTime to zero, and since WingTime won't be changed for a long
                //bit while liftoff is charged, set that to false.
                Main.LocalPlayer.wingTimeMax = 0; Main.LocalPlayer.wingTime = 0; hasWingTimeChanged = false;
                //Charge the liftoff...
                currentLiftoff += 1f / 10f;
            }
            else
            {
                //Execute a function to change it. By default, it keeps on considering
                //that the wing has changed to the last saved value, so it won't execute.
                //However, if it has changed to false and gets to else statement again
                //that means that it got there AFTER liftoff charge, so the function
                //gets executed and then immediately set HasWingTimeChanged to true.

                changeWingTime(Main.LocalPlayer);

                //If not charging, then just casually update the currentWingTime
                if (Main.LocalPlayer.wingTimeMax != 0) currentWingTime = Main.LocalPlayer.wingTime;


            }


            //See if the wing is equipped. If yes, show the Liftoff UI, if not, then don't show it! :provg:
            if (isEquipped == true) ModContent.GetInstance<AndromedaAPUISystem>().showLiftoffBar();
            else ModContent.GetInstance<AndromedaAPUISystem>().hideLiftoffBar();
        }
    }
}

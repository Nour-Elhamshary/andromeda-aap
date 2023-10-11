using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CatalystMod.Projectiles.Magic;
using CatalystMod.Items.Materials;
using CatalystMod.Items;
using CatalystMod;
using AndromedaAP.UI;
using AndromedaAP.Players;
using Terraria.GameInput;
using CatalystMod.Items.Armor.Intergelactic;
using AndromedaAP.Projectiles;
using Terraria.Audio;
using Terraria.ID;
using CatalystMod.Items.Potions;
using ReLogic.Utilities;
using System.Linq;
using System;
using CalamityMod.CalPlayer;

namespace AndromedaAP.Items.Equips.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class AndromedaAeropack : ModItem
    {

        //Get the soundStyles, one for liftoff explosion, another for jetpack thrust sound
        SoundStyle liftoffExplosion = new SoundStyle("AndromedaAP/Assets/Sounds/LiftoffExplosion") with
        {
            MaxInstances = 2
        };

        SlotId andromedaThrustId;
        SoundStyle thrust = new SoundStyle("AndromedaAP/Assets/Sounds/AndromedaThrust") with
        {
            MaxInstances = 1,
            IsLooped = false,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };



        //Function that checks if the player is flying in general similar to vanilla
        //courtesy of absoluteAquarium
        void checkIfPlayerFlies(AAPEquippedPlayer modWingPlayer)
        {
            //Basically it checks if he wears any vanilla wings, and checks if he holds jump
            //and checks if there's wingTime, and probably if the jump button is no longer just being pressed
            //and if velocity of Y is not equal to 0f.
            if (Main.LocalPlayer.wingsLogic > 0 && Main.LocalPlayer.controlJump
                && Main.LocalPlayer.wingTime > 0f && Main.LocalPlayer.jump == 0
                && Main.LocalPlayer.velocity.Y != 0f) modWingPlayer.flag21 = true;
        }




        //Usual wing stats stuff.
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Terraria.ID.ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new
            Terraria.DataStructures.WingStats(250, 9f, 2.5f);
            Item.ResearchUnlockCount = 1;
        }

        //astrageldon value !!!!!
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 34;
            Item.value = CatalystItem.AstrageldonValue;
            Item.rare = CatalystItem.RaritySuperboss;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            
            //Add your modPlayers, one for Andropack, one for metaball, and one for catalyst 
            var modWingPlayer = player.GetModPlayer<AAPEquippedPlayer>();
            ParticlePlayer modPlayer = ParticlePlayer.ModPlayer(player);
            CatalystPlayer catPlayer = CatalystPlayer.ModPlayer(player);

            checkIfPlayerFlies(modWingPlayer);

            //If updating accessory it means its equipped, set it to true !
            player.GetModPlayer<AAPEquippedPlayer>().isEquipped = true;

            //A single boolean to check if the player has any intergelactic head set
            bool intergelacticHead = (player.armor[0].type == ModContent.ItemType<IntergelacticHeadMagic>() 
                || player.armor[0].type == ModContent.ItemType<IntergelacticHeadMelee>() 
                || player.armor[0].type == ModContent.ItemType<IntergelacticHeadRanged>() 
                || player.armor[0].type == ModContent.ItemType<IntergelacticHeadRogue>() 
                || player.armor[0].type == ModContent.ItemType<IntergelacticHeadSummon>()
                );

            //If the player wears intergelactic armor, give him the bonuses! 5% crit and 5% damage
            if (intergelacticHead && player.armor[1].type == ModContent.ItemType<IntergelacticBreastplate>() && player.armor[2].type == ModContent.ItemType<IntergelacticGreaves>())
            {
                player.GetCritChance(DamageClass.Generic) += 0.05f;
                player.GetDamage(DamageClass.Generic) += 0.05f;
            }

            //If the player has charged the liftoff fully, then let him soar to the skies next time he flies!
            if (modWingPlayer.flag21 || modWingPlayer.liftoffDischarge || player.dashDelay == -1) { 
                if (modWingPlayer.liftoffready)
                {

                        modWingPlayer.liftoffDischarge = true;
                        Vector2 amountToSubtract;
                        if (player.direction == 1) amountToSubtract = new Vector2(15, 0);
                        else amountToSubtract = new Vector2(-15, 0);

                        //quotient
                        float quotient = modWingPlayer.currentLiftoff / modWingPlayer.maxLiftoff;
                        quotient = Utils.Clamp(quotient, 0f, 1f);



                        //Another magnum opus...
                        //Basically, make the funny smoke rings with size differing from how low the boost charge is.
                        if (quotient == 1f && !(player.dashDelay == -1)) Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<NebulaLiftoff>(), 500, 0f, player.whoAmI, -1f);
                        modPlayer.foamParticleList1.Add(new NebulaFoam(Vector2.Subtract(player.Center, amountToSubtract), Vector2.Zero, 2f * quotient, noMovement: true, 0.99f));

                        if ((player.dashDelay == -1 && quotient == 1.0f) || quotient == 0.75f || quotient == 0.5f || quotient == 0.25f) { 
                            for (int  i = 0; i < 360; i++) {
                                //Length...
                                float length = Main.rand.NextFloat(8f, 10f);
                                //Funny circle!!
                                Vector2 circular = new Vector2(length, 0f).RotatedBy(MathHelper.ToRadians(i));
                                //If the guy is dashing, flip the rings vertically
                                if (player.dashDelay == -1 || player.dashDelay > 0) { 
                                circular.Y *= quotient;
                                circular.X *= 0.25f * quotient;
                                } 
                                else //Do it horizontally otherwise
                                {
                                circular.Y *= (0.25f * quotient);
                                circular.X *= quotient;
                                }
                                //Add the metaball
                                modPlayer.foamParticleList1.Add(new NebulaFoam(Vector2.Subtract(player.Center, new Vector2(15f,0f)), circular, Main.rand.NextFloat(0.9f, 1.2f), noMovement: true, 0.99f));
                            }
                        }

                        //fling! (usually vertically, but if dashed then horizontally)
                        if (quotient == 1f)
                        {
                            SoundEngine.SoundPlayer.Play(in liftoffExplosion);
                        if (player.dashDelay == -1)
                        {
                            if (player.direction == 1) player.velocity.X += 30;
                            else player.velocity.X -= 30;
                        }
                        else player.velocity.Y -= 50;
                        }
                    //Empty the liftoff off
                    modWingPlayer.currentLiftoff -= 1f/2f;
                }
                else modWingPlayer.currentLiftoff = 0; //He wasn't a gamer and tried to use not fully charged liftoff, remove it!!
           }    
        }


        public override bool WingUpdate(Player player, bool inUse)
        {
            
            //Get the AAPEquippedPlayer so that we know the liftoff charge and if its even equipped
            //(important for UI)
            var modWingPlayer = Main.LocalPlayer.GetModPlayer<AAPEquippedPlayer>();

            //Set the players
            ParticlePlayer modPlayer = ParticlePlayer.ModPlayer(player);

            //While the player is flying (and not discharging (hehe))...
            if (inUse == true && !modWingPlayer.liftoffDischarge)
            {
                //Play the custom thrust sound
                if (!SoundEngine.TryGetActiveSound(andromedaThrustId, out var activeThrustSound))
                {
                    andromedaThrustId = SoundEngine.PlaySound(in thrust);
                } 


                //amountToSubtract helps in setting the direction of the meta flame 
                Vector2 amountToSubtract;
                if (player.direction == 1) amountToSubtract = new Vector2(15, 0);
                else amountToSubtract = new Vector2(-15, 0);

                //Rotational for some spreaded metaball (borrowed from Catalyst lmfao)
                Vector2 rotational = new Vector2(0f, 8f).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360f)));

                //In a for loop, add in the foam flame
                for (int i = 0; i < 9; i++) { 
                    modPlayer.foamParticleList1.Add(new NebulaFoam(Vector2.Subtract(player.Center, amountToSubtract), new Vector2(0, Main.rand.NextFloat(0f, 3f)), Main.rand.NextFloat(0.5f, 0.7f), noMovement: true, 0.99f));
                    modPlayer.foamParticleList1.Add(new NebulaFoam(Vector2.Subtract(player.Center, amountToSubtract), rotational, Main.rand.NextFloat(0.2f, 0.5f), noMovement: true, 0.99f));
                }
            }
            else
            {
                //If he stops flying, then stop the sound! First, check if the thrust is even running.
                if (SoundEngine.TryGetActiveSound(andromedaThrustId, out var activeThrustSound))
                {
                    
                    if (activeThrustSound.Volume > 0f) activeThrustSound.Volume -= 1f / 30f; //First by fading it out, this is done so by keeping on lowering the volume at every tick
                    else activeThrustSound.Stop(); //Then after you made sure that its equal to 0 or lower, then stop!
                }


            }
            return false;
        }


        //Typical vertical wing speeds adjustment
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
                                                ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, 
                                                ref float constantAscend)
        {

                ascentWhenFalling = 0.85f;
                ascentWhenRising = 0.15f;
                maxCanAscendMultiplier = 1f;
                maxAscentMultiplier = 3f;
                constantAscend = 0.135f;
        }

        //Add in the recipes. Since I don't know the balance of recipes, i just make it use 1 jetpack, 5 metanova bars and 12 astral jelly
        public override void AddRecipes()
        {
            base.CreateRecipe().AddIngredient(ItemID.Jetpack, 1).AddIngredient<MetanovaBar>(5).AddIngredient<AstraJelly>(12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}

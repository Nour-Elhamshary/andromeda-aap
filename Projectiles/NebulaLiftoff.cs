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
using System;
using CalamityMod.Tiles.FurnitureVoid;

namespace AndromedaAP.Projectiles
{
    public class NebulaLiftoff : ModProjectile
    {
        //Oops now it has purpose lmao
        public bool runOnce = true;
        public override void SetDefaults()
        {
            Projectile.height = 240;
            Projectile.width = 760;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 20;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override bool PreAI()
        {
            ParticlePlayer modPlayer = ParticlePlayer.ModPlayer(Main.player[base.Projectile.owner]);
            if (runOnce) { 
                for (int i = 0; i < 360; i++)
                {
                    float length = 10f;
                    Vector2 circular = new Vector2(length, 0f).RotatedBy(MathHelper.ToRadians(i));
                    circular.Y *= (0.25f);
                    circular.X *= 4f;
                    modPlayer.foamParticleList1.Add(new NebulaFoam(Main.player[base.Projectile.owner].Center, circular, Main.rand.NextFloat(0.9f, 1.2f), noMovement: true, 0.99f));
                }
                runOnce = false;
            }
            return false;
        }

        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[base.Projectile.owner] = 0;
        }
    }
}

using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CatalystMod;

namespace AndromedaAP.Projectiles
{
    public class NebulaLiftoff : ModProjectile
    {
        //Oops now it has purpose lmao
        
        //A simple boolean to run something once.
        public bool runOnce = true;

        //Setting defaults of the (invisible) projectile
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
        
        //The magnum opus...
        public override bool PreAI()
        {
            //Importing Catalyst's ParticlePlayer
            ParticlePlayer modPlayer = ParticlePlayer.ModPlayer(Main.player[base.Projectile.owner]);

            //This should be executed ONCE. This basically is the main horizontal smoke ring that
            //shows the particle visually.
            if (runOnce) { 
                //Since we're making a circle, this gets executed 360 times more.
                for (int i = 0; i < 360; i++)
                {
                    //Length is... length. Probably the diameter, but not sure yet.
                    float length = 10f;
                    
                    //The circle! (or at least a part of it! :0)
                    Vector2 circular = new Vector2(length, 0f).RotatedBy(MathHelper.ToRadians(i));
                    
                    //Change its size to look more 2d-ish
                    circular.Y *= (0.25f);
                    circular.X *= 4f;

                    //Then we add the foam particle.
                    modPlayer.foamParticleList1.Add(new NebulaFoam(Main.player[base.Projectile.owner].Center, circular, Main.rand.NextFloat(0.9f, 1.2f), noMovement: true, 0.99f));
                }
                
                //Since this is supposed to be ran once, set it to false so it won't be executed again until the particle dies.
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

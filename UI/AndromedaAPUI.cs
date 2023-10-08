using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using AndromedaAP.Players;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent;

namespace AndromedaAP.UI
{

    class LiftoffBarState : UIState
    {
        //Set the elements for the state, with UIElement area having the image
        //of the frame and the "bar" itself (more of interoplated gradient)
        private UIElement area;
        public UIImage liftoffbar;

        //public UIImage star;
        //public UIImage trail;

        private Color gradientA;
        private Color gradientB;
        private Color liftoffReadyColor;
        public override void OnInitialize()
        {
            //Set the area itself
            area = new UIElement();
   
            area.Width.Set(52f, 0f); //52x18 size
            area.Height.Set(18f, 0f);


            //Then center the dang element dot dot
            area.HAlign = area.VAlign = 0.5f;

            //Then adjust the top.
            area.Top.Set(-65f, 0f);

            //Set the frame image
            liftoffbar = new UIImage(ModContent.Request<Texture2D>("AndromedaAP/UI/emptybar"));
            liftoffbar.Left.Set(0f, 0f);
            liftoffbar.Top.Set(0f, 0f);
            liftoffbar.Width.Set(52f, 0f);
            liftoffbar.Height.Set(18f, 0f);

            //Then set the bar contents (the star and trail)
            //The star
            //star = new UIImage(ModContent.Request<Texture2D>("AndromedaAP/UI/head"));
            //star.Left.Set(6f, 0f);
            //star.Top.Set(6f, 0f);
            //star.Width.Set(6f, 0f);
            //star.Height.Set(6f, 0f);

            //The trail
            //trail = new UIImage(ModContent.Request<Texture2D>("AndromedaAP/UI/trail"));
            //trail.Left.Set(6f, 0f);
            //trail.Top.Set(6f, 0f);
            //trail.Width.Set(2f, 0f);
            //trail.Height.Set(6f, 0f);

            //Set the "bar" (just two colors to interpolate a gradient from)
            gradientA = new Color(123, 25, 138); // A dark purple
            gradientB = new Color(187, 91, 201); // A light purple
            liftoffReadyColor = new Color(255, 231, 189);

            area.Append(liftoffbar);
            //area.Append(trail);
            //area.Append(star);
            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            //Get the modplayer for liftoff values
            var modPlayer = Main.LocalPlayer.GetModPlayer<AAPEquippedPlayer>();

            float quotient = modPlayer.currentLiftoff / modPlayer.maxLiftoff; //Quotient means value between 0f and 1f
            quotient = Utils.Clamp(quotient, 0f, 1f); //Making sure it will never be lower than 0f or higer than 1f


            //The "bar" itself in the form of an inner frame size.
            Rectangle hitbox = liftoffbar.GetInnerDimensions().ToRectangle();

            hitbox.X += 6;
            hitbox.Width -= 12;
            hitbox.Y += 6;
            hitbox.Height -= 12;


            //star.Left.Pixels = (float)Utils.Lerp(6f, hitbox.Width, quotient);
            //trail.Width.Pixels = (float)Utils.Lerp(6f, hitbox.Width, quotient);
            Recalculate();
            base.Update(gameTime);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<AAPEquippedPlayer>();

            float quotient = modPlayer.currentLiftoff / modPlayer.maxLiftoff; //Quotient means value between 0f and 1f
            quotient = Utils.Clamp(quotient, 0f, 1f); //Making sure it will never be lower than 0f or higer than 1f


            Rectangle hitbox = liftoffbar.GetInnerDimensions().ToRectangle();

            hitbox.X += 6;
            hitbox.Width -= 12;
            hitbox.Y += 6;
            hitbox.Height -= 12;

            Rectangle starbox = hitbox;
            Rectangle trailbox = hitbox;

            starbox.Width = trailbox.Width = 6;

            Texture2D trail = (Texture2D)ModContent.Request<Texture2D>("AndromedaAP/UI/trail2");
            Texture2D star = (Texture2D)ModContent.Request<Texture2D>("AndromedaAP/UI/head");
            Texture2D pink = (Texture2D)ModContent.Request<Texture2D>("AndromedaAP/UI/emptbarpink");

            base.DrawChildren(spriteBatch);

            starbox.X = (int)Utils.Lerp(hitbox.Left, hitbox.Right - starbox.Width, quotient);
            trailbox.Width = (int)Utils.Lerp(0, hitbox.Width, quotient);

            if (quotient == 1f) spriteBatch.Draw(pink, liftoffbar.GetInnerDimensions().ToRectangle(), Color.White);
            spriteBatch.Draw(trail, trailbox, Color.White);
            spriteBatch.Draw(star, starbox, Color.White);
            
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);




            //if (modPlayer.liftoffready)
            //{
            //    spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, liftoffReadyColor);
            //}
            //else { 
            //    for (int i = 0; i < steps; i += 1)
            //    {
            //        // float percent = (float)i / steps; // Alternate Gradient Approach
            //        float percent = (float)i / (right - left);
            //        trail.Width.Set(hitbox.Width, percent);
            //        //spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(gradientA, gradientB, percent));       
            //        //spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.White);
            //    }
            //}

        }
    }

    

}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using AndromedaAP.Players;
using Terraria.GameContent.UI.Elements;

namespace AndromedaAP.UI
{

    class LiftoffBarState : UIState
    {
        //Set the elements for the state, with UIElement area having the image
        //of the frame.
        private UIElement area;
        public UIImage liftoffbar;

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

            //Then add in the bar itself to the area UI element
            area.Append(liftoffbar);
            Append(area);
        }


        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<AAPEquippedPlayer>(); //Liftoff values

            float quotient = modPlayer.currentLiftoff / modPlayer.maxLiftoff; //Quotient means value between 0f and 1f
            quotient = Utils.Clamp(quotient, 0f, 1f); //Making sure it will never be lower than 0f or higer than 1f

            //Set the hitbox of the inner bar itself
            Rectangle hitbox = liftoffbar.GetInnerDimensions().ToRectangle();

            hitbox.X += 6;
            hitbox.Width -= 12;
            hitbox.Y += 6;
            hitbox.Height -= 12;

            //Then set it to two separate hitboxes for star and its trail
            //Made like that because the star goes first before the trail
            Rectangle starbox = hitbox;
            Rectangle trailbox = hitbox;

            //Both are of same width initially so... set that too.
            starbox.Width = trailbox.Width = 6;

            //Then load in the textures for trail and star (and even when the bar itself turns pink)
            Texture2D trail = (Texture2D)ModContent.Request<Texture2D>("AndromedaAP/UI/trail");
            Texture2D star = (Texture2D)ModContent.Request<Texture2D>("AndromedaAP/UI/head");
            Texture2D pink = (Texture2D)ModContent.Request<Texture2D>("AndromedaAP/UI/emptbarpink");

            //The whole UI itself
            base.DrawChildren(spriteBatch);

            //You set the starbox X position and the source rectangle width of trailbox 
            //to where it stretches to the end of the bar according to quotient
            starbox.X = (int)Utils.Lerp(hitbox.Left, hitbox.Right - starbox.Width, quotient);
            trailbox.Width = (int)Utils.Lerp(0, hitbox.Width, quotient);

            //If quotient is 1, then make the bar pink...
            if (quotient == 1f) spriteBatch.Draw(pink, liftoffbar.GetInnerDimensions().ToRectangle(), Color.White);
            //Whether one or not, you draw the rest! The "layers" work 
            //where the last texture drawn is above the first texture drawn.
            spriteBatch.Draw(trail, trailbox, Color.White);
            spriteBatch.Draw(star, starbox, Color.White);
            
        }
    }

    

}

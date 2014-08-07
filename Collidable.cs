using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;

namespace Eteephonehome
{

    class Collidable : Sprite
    {

        public int Type { get; set; }

        public Collidable(ContentManager content, int x, int y, int height, int width, int Type)
            : base(content, x, y, height, width)
        {
            this.Type = Type;
        }

        public override void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("Map/Level1Map/empty.png");
        }

        public new void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

        public string getType()
        {
            switch (Type)
            {
                case 0:
                    return "Obstacle";
                case 1:
                    return "Water";
                case 2:
                    return "Mud";
                default:
                    return "";
            }
        }

    }


    
}

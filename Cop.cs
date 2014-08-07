using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Tao.Sdl;

namespace Eteephonehome
{
    class Cop : MovingSprite
    {
        Rectangle[] hitBoxList = new Rectangle[7];
        protected int x_direction = 0;
        protected int y_direction = 0;
        protected int directionOnPath;
        protected int position;

        public List<Tuple<int, int>> path { get; set; }
        Texture2D hitBoxTexture;

        public Cop(ContentManager content, int x, int y, int width, int height, List<Tuple<int,int>> path)
        {
            hitBoxTexture = content.Load<Texture2D>("Level1Map/black.png");
            speed = 1;
            directionOnPath = 1;
            position = 0;
            this.path = path;
            animationframes = new List<List<Texture2D>>();
            this.HitBox = new Rectangle(x, y, spriteWidth, spriteHeight);
            setX(x);
            setY(y);
            this.spriteHeight = height;
            this.spriteWidth = width;
            movedX = 1;
            movedY = 1;
            turningDirection = 0;
            turningCounter = 0;
            turningCounterTime = 30;
            currentHold = 0;
            currentDirectionFrame = 2;
            currentwalkingframe = 0;
            pauseIntervalInFrames = 0;
            frameIntervalDuration = 5;
            moving = false; 
            reaching = false;
            setX(x);
            setY(y);
            this.spriteHeight = height;
            this.spriteWidth = width;
            //CHANGE INDEX
            updateHitBox(x, y, width, height);
            LoadContent(content);
            // Needed before Move()
            calculateBothDirections();
        }

        public new void Draw(SpriteBatch sb)
        {
            createHitBox(sb);
            base.Draw(sb);
        }

        public void Draw(SpriteBatch sb, Color color, bool animationPause) 
        {
           // Rectangle rect = verticalHitLine(spriteX / 16, spriteY / 16, 16, -1);
            //sb.Draw(hitBoxTexture, HitBox, Color.White * .15f);
            createHitBox(sb);
            if (!animationPause)
            {
                base.Draw(sb, Color.White); // color
            }
            else
            {
                sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
            }
        }

        public new bool overlaps(Rectangle rect)
        {
            for (int i = 0; i < hitBoxList.Length; i++)
            {
                if (rect.Intersects(hitBoxList[i]))
                {
                    return true;
                }
            }
            return rect.Intersects(this.HitBox);
        }

        public bool overlapsWithActualCop(Rectangle rect)
        {
            return rect.Intersects(this.HitBox);
        }

        public void createHitBox(SpriteBatch sb)
        {
            switch (updateDirectionFrame(movedX, movedY))
            {
                case 0:
                    getVerticalHitLines((spriteX + 24) / 16, (spriteY + 48) / 16, -1, 0);
                    drawHitBoxes(sb);
                    break;
                case 1:
                    getHorizontalHitLines((spriteX) / 16, (spriteY + 24) / 16, 1, 0);
                    drawHitBoxes(sb);
                    break;
                case 2:
                    getVerticalHitLines((spriteX + 24) / 16, spriteY / 16, 1, 0);
                    drawHitBoxes(sb);
                    break;
                case 3:
                    getHorizontalHitLines((spriteX + spriteWidth) / 16, (spriteY + 24) / 16, -1, 0);
                    drawHitBoxes(sb);
                    break;
            }
        }

        //offsets are used to make the angled shape of the cop vision area
        public int getOffset(int temp)
        {
            temp = Math.Abs(temp);
            if (temp != 0)
            {
                temp--;
            }
            return temp;
        }

        public int reverseOffset(int temp)
        {
            switch (Math.Abs(temp))
            {
                case 0:
                    return 3;
                case 1:
                    return 2;
                default:
                    return 0;
            }
        }

        // X AND Y -3 MIGHT BE PAST THE SIDES

        public void getVerticalHitLines(int x, int y, int dir, int characterOffset)
        {
            int j = 0;
            for (int i = x - 3; i <= x + 3; i++)
            {
                hitBoxList[j] = verticalHitLine(i, y, 16, dir, dir * getOffset(j-3), characterOffset);
                j++;
            }
        }

        public void getHorizontalHitLines(int x, int y, int dir, int characterOffset)
        {
            int j = 0;
            for (int i = y - 3; i <= y + 3; i++)
            {
                hitBoxList[j] = horizontalHitLine(x, i, 16, dir, dir * getOffset(j-3), characterOffset);
                j++;
            }
        }

        public Rectangle verticalHitLine(int x, int y, int w, int y_dir, int y_offset, int characterOffset)
        {

            y += y_offset;
            int downY = y;
            int h = 0;

            int counter = 18 + reverseOffset(y_offset);

            while (y > 0 && y < screenGrid.y_max && counter > 0 && (getTile(x, y) as Hedge) == null && (getTile(x, y) as SpaceShipPart) == null && (getTile(x, y) as PowerUp) == null)
            {
                h += 16;
                counter--;
                y += y_dir;
            }
            if (y_dir == -1)
            {
                y++;
                return new Rectangle(x * 16, y * 16, w, h + (spriteY % 16) - 16);
            }
            else if (y_dir == 1)
            {
                return new Rectangle(x * 16, (downY * 16) + (spriteY % 16), w, h - (spriteY % 16));
            }
            else
            {
                return new Rectangle(0, 0, 0, 0);
            }
        }

        public Rectangle horizontalHitLine(int x, int y, int h, int x_dir, int x_offset, int characterOffset)
        {
            x += x_offset;
            int forwardX = x;
            int w = 0;

            int counter = 18 + reverseOffset(x_offset);

            while (x > 0 && x < screenGrid.x_max && counter > 0 && (getTile(x, y) as Hedge) == null && (getTile(x, y) as SpaceShipPart) == null && (getTile(x, y) as PowerUp) == null)
            {
                w += 16;
                counter--;
                x += x_dir;
            }
            if (x_dir == -1)
            {
                x++;
                return new Rectangle((x * 16), y * 16, w - (32 - spriteX % 16), h);
            }
            else if (x_dir == 1)
            {
                return new Rectangle((forwardX * 16) + (spriteX % 16), y * 16, w - (spriteX % 16), h);
            }
            else
            {
                return new Rectangle(0, 0, 0, 0);
            }
        }

        public void drawHitBoxes(SpriteBatch sb)
        {
            for (int i = 0; i < 7; i++)
            {
                sb.Draw(hitBoxTexture, hitBoxList[i], Color.White * .15f);
            }
        }

        public override void LoadContent(ContentManager content)
        {
            for (int i = 0; i < 5; i++)
            {
                animationframes.Add(new List<Texture2D>());
            }

            animationframes[0].Add(content.Load<Texture2D>("Cops/cop_up.png"));
            animationframes[0].Add(content.Load<Texture2D>("Cops/cop_up1.png"));
            animationframes[0].Add(content.Load<Texture2D>("Cops/cop_up2.png"));
            animationframes[1].Add(content.Load<Texture2D>("Cops/cop_right.png"));
            animationframes[1].Add(content.Load<Texture2D>("Cops/cop_right1.png"));
            animationframes[1].Add(content.Load<Texture2D>("Cops/cop_right2.png"));
            animationframes[2].Add(content.Load<Texture2D>("Cops/cop_down.png"));
            animationframes[2].Add(content.Load<Texture2D>("Cops/cop_down1.png"));
            animationframes[2].Add(content.Load<Texture2D>("Cops/cop_down2.png"));
            animationframes[3].Add(content.Load<Texture2D>("Cops/cop_left.png"));
            animationframes[3].Add(content.Load<Texture2D>("Cops/cop_left1.png"));
            animationframes[3].Add(content.Load<Texture2D>("Cops/cop_left2.png"));
            //-----temp thing for offset in animatedsprite
            animationframes[4].Add(content.Load<Texture2D>("Cops/cop_left2.png"));

            if (path.Count==0)
            {
                Console.WriteLine("Generating default path");
                Tuple<int, int> t1 = new Tuple<int, int>(550, 420);
                Tuple<int, int> t2 = new Tuple<int, int>(550, 320);
                Tuple<int, int> t3 = new Tuple<int, int>(405, 320);
                Tuple<int, int> t4 = new Tuple<int, int>(405, 20);
                Tuple<int, int> t5 = new Tuple<int, int>(240, 20);

                path.Add(t1);
                path.Add(t2);
                path.Add(t3);
                path.Add(t4);
                path.Add(t5);
            }

            image = animationframes[0][0];

            base.LoadContent(content);
        }

        public void Update(GameTime gameTime, ref List<Sprite> spriteList)
        {
            Move();
            base.Update(ref spriteList);
            moving = true;

        }

        public void Update(List<Sprite> spriteList)
        {
            Move(spriteList);
            updateDirectionFrame(movedX, movedY);
        }

        public int check(int a, int b, int val, Func<int,int,int> function)
        {
            if (a < b)
                return function(val, a);
            else
                return val;
        }

        public int check2(int a, int b, int val, Func<int, int, int> function)
        {
            if (a < b)
                return function(val, b);
            else
                return val;
        }

        #region Movement Region

        public void Move(List<Sprite> spriteList)
        {
            updateListPosition();
            calculateBothDirections();
            movedX = (int) (x_direction * speed);
            movedY = (int) (y_direction * speed);
            //detectCollisions(ref spriteList);
            spriteX += movedX;
            spriteY += movedY;
            updateHitBox(spriteX, spriteY, spriteWidth, spriteHeight);
            base.Move();
        }

        public void updateListPosition()
        {
            if (movedX == 0 && movedY == 0)
            {
                position += directionOnPath;
                if (position >= path.Count - 1)
                {
                    currentHold = holdLength;
                    setDirection(path.Count - 1, -1);
                }
                else if (position <= 0)
                {
                    currentHold = holdLength;
                    setDirection(0, 1);
                }
            }
        }

        public void reverseDirection()
        {
            HitBox = new Rectangle(0, 0, 0, 0);
            movedX = 0;
            movedY = 0;
            position += directionOnPath;
            directionOnPath = directionOnPath * -1;
        }

        public void setDirection(int pos, int direction)
        {
            position = pos;
            directionOnPath = direction;
        }

        public void calculateBothDirections()
        {
            
            
            x_direction = calculateDirection(spriteX, path[position + directionOnPath].Item1);
            y_direction = calculateDirection(spriteY, path[position + directionOnPath].Item2);
        }

        public int calculateDirection(int a, int b)
        {
            if (a < b)
                return 1;
            if (a > b)
                return -1;
            else
                return 0;
        }

        #endregion

        protected new void updateAnimation()
        {
            base.updateAnimation();
        }

        // b == true if the character is walking, false if not walking
        private void updateCurrentWalkingFrame(bool b)
        {
            if (b)
            {
                currentwalkingframe++;
                // FIX HERE, 3 should not be hardcoded
                if (currentwalkingframe >= 3 || currentwalkingframe < 0)
                {
                    currentwalkingframe = 0;
                    return;
                }
            }
            else if (currentwalkingframe > 0)
            {
                currentwalkingframe--;
            }
        }

        protected override int updateDirectionFrame(int movedX, int movedY)
        {
            if (movedX > 0)
            {
                return 1;
            }
            else if (movedX < 0)
            {
                return 3;
            }
            else if (movedY > 0)
            {
                return 2;
            }
            else if (movedY < 0)
            {
                return 0;
            }
            return 0;
        }

        protected override bool handleCollision(Sprite sprite)
        {
            return false;
        }


    }
}
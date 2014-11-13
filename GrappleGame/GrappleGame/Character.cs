using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GrappleGame
{
    class Character
    {
        /// <summary>
        /// This Enum designates all of the different actions the dude can perform (with the exception of grappling)
        /// </summary>
        enum Action
        {
            Standing,
            Walking,
            Attacking,
            Jumping,
            Blocking,
            Falling,
            Damaged,
            Talking,
        }

        /// <summary>
        /// This specific instance of the Action enum keeps track of the dudes current action, with no action represented as standing
        /// </summary>
        Action currentAction = Action.Standing;

        /// <summary>
        /// This Enum designates all of the different states an action can be in. Standby is waiting for an action, and the rest are how far the action the dude is performing has progressed
        /// </summary>
        enum ActionState
        {
            Standby,
            Starting,
            InProgress,
            Finishing,
            Done,
        }

        /// <summary>
        /// This specific instance of the ActionState enum keeps track of the dudes current progress through an action
        /// </summary>
        ActionState currentActionState = ActionState.Standby;

        /// <summary>
        /// This enum designates the four possible directions in the 2-D space. Note that all direction controlled processes in the dude are constrained to vertical and horizontal directions
        /// </summary>
        enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        /// <summary>
        /// This specific instance of the Direction enum keeps track of the direction the dude is facing
        /// </summary>
        Direction currentFacingDirection = Direction.Down;

        /// <summary>
        /// This specific instance of the direction enum keeps track of the direction the grapple is shooting
        /// </summary>
        Direction currentMovingDirection = Direction.Down;

        /// <summary>
        /// This enum designates the different ways the dude is drawn. Covered is when the dude is covered by shadow. Hidden is when the dude is behind an object.
        /// </summary>
        enum Visibility
        {
            Normal,
            Covered,
            Hidden,
            Covering,
            Uncovering,
            Hiding,
            Unhiding,
        }

        /// <summary>
        /// This specific instance of the visibility enum keeps track of how to draw the dude
        /// </summary>
        Visibility currentVisibility = Visibility.Normal;

        /// <summary>
        /// This enum keeps track of the state of the grappling hook. Runs seperate from Actions and Actions states so that certain actions can be performed simultaneously while grappling. This enum performs the function of both the action and actionstate enums for the grapple
        /// </summary>
        enum Grapple
        {
            Static,
            Starting,
            Extending,
            Hit,
            Miss,
            Finishing,
            Done,
        }

        /// <summary>
        /// This specific instance of the Grapple enum keeps track of the grapple state and progress
        /// </summary>
        Grapple currentGrapple = Grapple.Static;

        /// <summary>
        /// Contains the standing and walking images of the dude
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Contains the attacking images of the dude
        /// </summary>
        private Texture2D attack;

        /// <summary>
        /// Contains the grapple images
        /// </summary>
        private Texture2D grapple;

        /// <summary>
        /// contains shadow image
        /// </summary>
        private Texture2D shadow;

        /// <summary>
        /// Creates the worldview. Contains the transformation that accurately places the camera looking at the world over the dude
        /// </summary>
        public Matrix transform;

        /// <summary>
        /// the zoom scale. increasing the scale moves the camera closer to the dude, and vice versa
        /// </summary>
        public float zoom = 1;

        /// <summary>
        /// The position of the dude in tile count. The "Official" position of the dude.
        /// </summary>
        public Vector2 tilePosition;

        /// <summary>
        /// This position of the dude in pixel count. Used for transitioning between tiles
        /// </summary>
        public Vector2 pixelPosition;

        /// <summary>
        /// kkeps track of the tile position the dude is moving towards
        /// </summary>
        private Vector2 newPosition;

        /// <summary>
        /// keeps track of the previous tile position the dude is moving away from.
        /// </summary>
        private Vector2 previousPosition;

        /// <summary>
        /// Sets the walkspeed of the dude. The units are in pixels per update cycle
        /// </summary>
        public int walkSpeed = 2;

        /// <summary>
        /// sets the amount of health of the dude
        /// </summary>
        public int Health = 10;

        /// <summary>
        /// sets the total amount of health possible for the dude
        /// </summary>
        public int totalHealth = 10;

        /// <summary>
        /// sets the grapple speed in pixels per update cycle
        /// </summary>
        private int grappleSpeed = 1;

        /// <summary>
        /// Contains the generic constants for the game
        /// </summary>
        Constants Constants = new Constants();

        /// <summary>
        /// The current walking image the dude is on. 
        /// </summary>
        private int walkFrame = 0;

        /// <summary>
        /// The current standing image the dude is on. 
        /// </summary>
        private int standFrame = 0;

        /// <summary>
        /// The maximum length of the grapple shot
        /// </summary>
        private int grappleLength = 8;

        /// <summary>
        /// How far the grapple traveled during a shot
        /// </summary>
        private int grappleShotLength;

        /// <summary>
        /// the unit vector that contains the direction of movement
        /// </summary>
        private Point movingDirection = new Point(0, 0);

        /// <summary>
        /// the current size of the grapple. Increases as the grapple is shot out and decreases as the grapple is pulled in
        /// </summary>
        private int grappleSize = 0;

        /// <summary>
        /// rotation variable for drawing the grapple in the correct direction
        /// </summary>
        private float rotation;

        /// <summary>
        /// indicates whether the dude has moved
        /// </summary>
        public bool dudeMoved = false;

        /// <summary>
        /// the unit vector that contains the direction the dude is facing
        /// </summary>
        private Point facingDirection = new Point(0, 0); //may not be needed, to be determined

        /// <summary>
        /// Keeps track of the current height of the dude
        /// </summary>
        private float height;

        /// <summary>
        /// keeps track of height difference between dude and ground
        /// </summary>
        private float heightDifOld, heightDifNew;

        /// <summary>
        /// Contains all textures, properties, variables pertaining the the user controlled character
        /// </summary>
        /// <param name="newtilePosition">starting position of the dude on the map</param>
        /// <param name="newTexture">standing and walking textures</param>
        /// <param name="newAttack">attacking textures</param>
        /// <param name="newGrapple">grapple textures</param>
        public Character(Vector2 tilePosition, float height, ContentManager Content)
        {
            this.tilePosition = tilePosition;
            pixelPosition = this.tilePosition * Constants.tilesize;
            texture = Content.Load<Texture2D>("Characters/dude/dude");
            attack = Content.Load<Texture2D>("Characters/dude/dudeattack");
            grapple = Content.Load<Texture2D>("Characters/dude/Grapple2");
            shadow = Content.Load<Texture2D>("Characters/dude/dudeshadow");
            this.height = height;
        }

        #region camera
        /// <summary>
        /// the viewport for the player to see the gameworld. This functions has the visible screen follow the dude with the dude being centered on the screen
        /// </summary>
        /// <param name="device">XNA variable keeping track of game window variables</param>
        /// <param name="editor">detects whether editor mode is on. needed becuase the calculation to keep the dude centered on screen needs to be adjusted when editor mode is running</param>
        public void CameraCalculation(GraphicsDevice device, bool editor)
        {
            float editorScreenAdjustment = 0;
            if (editor)
                editorScreenAdjustment = 100;
            Viewport viewPort = device.Viewport;
            transform = Matrix.CreateTranslation(new Vector3(-pixelPosition.X - (Constants.tilesize / 2), -pixelPosition.Y - (Constants.tilesize / 2), 0))
                        * Matrix.CreateScale(new Vector3(zoom, zoom, 1))
                        * Matrix.CreateTranslation(new Vector3(viewPort.Width * 0.5f - editorScreenAdjustment, viewPort.Height * 0.5f, 0));
        }


        private void CameraMagnification(KeyboardState keys)
        {
            if (keys.IsKeyDown(Keys.P))
            {
                zoom = zoom + 0.01f;
                if (zoom >= 3f)
                    zoom = 3f;
            }
            else
            {
                zoom = zoom - 0.01f;
                if (zoom <= 0.1f)
                    zoom = 0.1f;
            }
        }
        #endregion

        /// <summary>
        /// Draws the dude
        /// </summary>
        /// <param name="sb">Drawing tool for XNA</param>
        public void Draw(SpriteBatch sb)
        {
            if (heightDifNew == heightDifOld)
            {
                if (heightDifOld == 0)
                {
                    //sb.Draw(shadow, new Vector2(pixelPosition.X + Constants.tilesize / 2, pixelPosition.Y + Constants.tilesize), new Rectangle(0, 0, shadow.Width, shadow.Height), Color.White, 0f, new Vector2(shadow.Width / 2, shadow.Height - 3), 1f, SpriteEffects.None, 0.901f);
                }
                else
                {
                }
            }
            else
            {
                switch (currentMovingDirection)
                {
                    case Direction.Left:
                        //new tile shadow
                        float temp1 = pixelPosition.X + (3 * (1 + heightDifNew)) - (shadow.Width / 2) * heightDifNew;
                        float temp2 = ((float)shadow.Width / (float)Constants.tilesize) * (Constants.tilesize - (pixelPosition.X - (newPosition.X * Constants.tilesize)));
                        sb.Draw(shadow, new Vector2(temp1, pixelPosition.Y + Constants.tilesize * (1 + heightDifNew)), new Rectangle(0, 0, (int)temp2, shadow.Height), Color.White, 0f, new Vector2(0, shadow.Height - 3), 1f + heightDifNew, SpriteEffects.None, 0.901f);
                        //old tile shadow
                        sb.Draw(shadow, new Vector2(previousPosition.X * Constants.tilesize, pixelPosition.Y + Constants.tilesize * (1 + heightDifOld)), new Rectangle((int)(((float)shadow.Width / (float)Constants.tilesize) * (Constants.tilesize - (pixelPosition.X - (newPosition.X * Constants.tilesize)))), 0, shadow.Width - (int)(((float)shadow.Width / (float)Constants.tilesize) * (Constants.tilesize - (pixelPosition.X - (newPosition.X * Constants.tilesize)))), shadow.Height), Color.White, 0f, new Vector2(0, shadow.Height - 3), 1f + heightDifOld, SpriteEffects.None, 0.901f);

                        //sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, (int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                        break;
                    case Direction.Right:
                        break;
                    case Direction.Up:
                        break;
                    case Direction.Down:
                        break;
                }
            }
            int drawFrame = standFrame;
            switch (currentAction)
            {
                case Action.Standing:
                    drawFrame = standFrame;
                    break;
                case Action.Walking:
                    drawFrame = walkFrame;
                    break;
                case Action.Attacking:
                    drawFrame = standFrame;
                    break;
            }
            switch (currentVisibility)
            {
                case Visibility.Normal:
                    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                    break;
                case Visibility.Hidden:
                    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                    break;
                case Visibility.Covered:
                    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                    break;
                case Visibility.Covering:
                    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                    switch (currentMovingDirection)
                    {
                        case Direction.Up:
                            //sb.Draw(texture, new Vector2(pixelPosition.X, previousPosition.Y * Constants.tilesize), new Rectangle(Constants.tilesize * drawFrame, (int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y, Constants.tilesize, Constants.tilesize - ((int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize - ((int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Left:
                            sb.Draw(texture, new Vector2(previousPosition.X * Constants.tilesize, pixelPosition.Y), new Rectangle(Constants.tilesize * drawFrame + (int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X, 0, Constants.tilesize - ((int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Down:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize - ((int)pixelPosition.Y - (int)(previousPosition.Y * Constants.tilesize))), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Right:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize - ((int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize)), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                    }
                    break;
                case Visibility.Uncovering:
                    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                    switch (currentMovingDirection)
                    {
                        case Direction.Up:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, (int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Left:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, (int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Down:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, (int)pixelPosition.Y - (int)(previousPosition.Y * Constants.tilesize)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Right:
                            sb.Draw(texture, newPosition * Constants.tilesize, new Rectangle(Constants.tilesize * drawFrame + Constants.tilesize - ((int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize)), 0, (int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                    }
                    break;
                case Visibility.Hiding:
                    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                    switch (currentMovingDirection)
                    {
                        case Direction.Up:
                            sb.Draw(texture, new Vector2(pixelPosition.X, previousPosition.Y * Constants.tilesize), new Rectangle(Constants.tilesize * drawFrame, (int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y, Constants.tilesize, Constants.tilesize - ((int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Left:
                            sb.Draw(texture, new Vector2(previousPosition.X * Constants.tilesize, pixelPosition.Y), new Rectangle(Constants.tilesize * drawFrame + (int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X, 0, Constants.tilesize - ((int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Down:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize - ((int)pixelPosition.Y - (int)(previousPosition.Y * Constants.tilesize))), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Right:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize - ((int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize)), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                    }
                    break;
                case Visibility.Unhiding:
                    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                    switch (currentFacingDirection)
                    {
                        case Direction.Up:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, (int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Left:
                            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, (int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Down:
                            sb.Draw(texture, new Vector2(pixelPosition.X, newPosition.Y * Constants.tilesize), new Rectangle(Constants.tilesize * drawFrame, Constants.tilesize - (int)pixelPosition.Y + (int)(previousPosition.Y * Constants.tilesize), Constants.tilesize, (int)pixelPosition.Y - (int)(previousPosition.Y * Constants.tilesize)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                        case Direction.Right:
                            sb.Draw(texture, new Vector2(newPosition.X * Constants.tilesize, (int)pixelPosition.Y), new Rectangle(Constants.tilesize * drawFrame + Constants.tilesize - ((int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize)), 0, (int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                            break;
                    }
                    break;
            }
            switch (currentGrapple)
            {
                case Grapple.Extending:
                    switch (currentMovingDirection)
                    {
                        case Direction.Right:
                            sb.Draw(grapple, new Vector2(pixelPosition.X + (2 * Constants.tilesize) + grappleSize, pixelPosition.Y + Constants.tilesize),
                            new Rectangle(0, 0, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                        case Direction.Left:
                            sb.Draw(grapple, new Vector2(pixelPosition.X - Constants.tilesize - grappleSize, pixelPosition.Y),
                            new Rectangle(0, 0, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                        case Direction.Up:
                            sb.Draw(grapple, new Vector2(pixelPosition.X + Constants.tilesize, pixelPosition.Y - Constants.tilesize - grappleSize),
                            new Rectangle(0, 0, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                        case Direction.Down:
                            sb.Draw(grapple, new Vector2(pixelPosition.X, pixelPosition.Y + (2 * Constants.tilesize) + grappleSize),
                            new Rectangle(0, 0, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                    }
                    break;
                case Grapple.Hit:
                    switch (currentMovingDirection)
                    {
                        case Direction.Right:
                            sb.Draw(grapple, new Vector2(pixelPosition.X + (2 * Constants.tilesize) + grappleSize, pixelPosition.Y + Constants.tilesize),
                            new Rectangle(0, Constants.tilesize, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                        case Direction.Left:
                            sb.Draw(grapple, new Vector2(pixelPosition.X - Constants.tilesize - grappleSize, pixelPosition.Y),
                            new Rectangle(0, Constants.tilesize, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                        case Direction.Up:
                            sb.Draw(grapple, new Vector2(pixelPosition.X + Constants.tilesize, pixelPosition.Y - Constants.tilesize - grappleSize),
                            new Rectangle(0, Constants.tilesize, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                        case Direction.Down:
                            sb.Draw(grapple, new Vector2(pixelPosition.X, pixelPosition.Y + (2 * Constants.tilesize) + grappleSize),
                            new Rectangle(0, Constants.tilesize, Constants.tilesize + grappleSize, Constants.tilesize), Color.White, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                            break;
                    }
                    break;
                case Grapple.Miss:
                    goto case Grapple.Hit;
            }
        }

        #region DudeInput
        /// <summary>
        /// Tracks user input of the game through the keyboard. Default control scheme
        /// </summary>
        /// <param name="keys">Tracks the current state of the keyboard</param>
        public void UserInput(KeyboardState keys)
        {
            if (keys.IsKeyDown(Keys.P)) //zoom in button, camera zooms in on map
                CameraMagnification(keys);
            if (keys.IsKeyDown(Keys.L)) //zoom out button, camera zooms out on map
                CameraMagnification(keys);
            if (currentActionState == ActionState.Standby)
            {
                if (keys.IsKeyDown(Keys.Up))
                {//face up
                    currentFacingDirection = Direction.Up;
                    setFacingDirectionConstants();
                }
                if (keys.IsKeyDown(Keys.Down))
                {//face down
                    currentFacingDirection = Direction.Down;
                    setFacingDirectionConstants();
                }
                if (keys.IsKeyDown(Keys.Right))
                {//face right
                    currentFacingDirection = Direction.Right;
                    setFacingDirectionConstants();
                }
                if (keys.IsKeyDown(Keys.Left))
                {//face left
                    currentFacingDirection = Direction.Left;
                    setFacingDirectionConstants();
                }
                if (keys.IsKeyDown(Keys.LeftShift))
                {
                    if (currentGrapple == Grapple.Static)
                    {
                        currentGrapple = Grapple.Starting;
                        currentMovingDirection = currentFacingDirection;
                        setMovingDirectionConstants();
                        return;
                    }
                }
                //if (keys.IsKeyDown(Keys.Space))
                //{
                //    currentAction = Action.Attacking;
                //    currentActionState = ActionState.Starting;
                //    return;
                //}
                if (keys.IsKeyDown(Keys.W))
                {
                    if (currentGrapple == Grapple.Static)
                    {
                        currentAction = Action.Walking;
                        currentFacingDirection = Direction.Up;
                        currentMovingDirection = Direction.Up;
                        setFacingDirectionConstants();
                        setMovingDirectionConstants();
                        currentActionState = ActionState.Starting;
                        return;
                    }
                }
                if (keys.IsKeyDown(Keys.A))
                {
                    if (currentGrapple == Grapple.Static)
                    {
                        currentAction = Action.Walking;
                        currentFacingDirection = Direction.Left;
                        currentMovingDirection = Direction.Left;
                        setFacingDirectionConstants();
                        setMovingDirectionConstants();
                        currentActionState = ActionState.Starting;
                        return;
                    }
                }
                if (keys.IsKeyDown(Keys.D))
                {
                    if (currentGrapple == Grapple.Static)
                    {
                        currentAction = Action.Walking;
                        currentFacingDirection = Direction.Right;
                        currentMovingDirection = Direction.Right;
                        setFacingDirectionConstants();
                        setMovingDirectionConstants();
                        currentActionState = ActionState.Starting;
                        return;
                    }
                }
                if (keys.IsKeyDown(Keys.S))
                {
                    if (currentGrapple == Grapple.Static)
                    {
                        currentAction = Action.Walking;
                        currentFacingDirection = Direction.Down;
                        currentMovingDirection = Direction.Down;
                        setFacingDirectionConstants();
                        setMovingDirectionConstants();
                        currentActionState = ActionState.Starting;
                        return;
                    }
                }
                
            }
        }

        /// <summary>
        /// Gives the dude direct commands of action and direction. Bypasses the typical user input of keyboard or controller
        /// </summary>
        /// <param name="action">Name of the action desired. The following inputs are acceptable: "Walk" , "Grapple" , null (when no action change needed)</param>
        /// <param name="direction">Name of the direction desired. The following inputs are acceptable: "Up" , "Left" , "Right" , "Down" , null (when no direction change needed)"</param>
        public void ForceInput(string action, string direction)
        {
            pixelPosition = tilePosition * Constants.tilesize;

            switch (action)
            {
                case "Walk":
                    currentAction = Action.Walking;
                    currentActionState = ActionState.Starting;
                    break;
                case "Grapple":
                    currentGrapple = Grapple.Starting;
                    break;
                default:
                    break;
            }
            switch (direction)
            {
                case "Left":
                    currentFacingDirection = Direction.Left;
                    currentMovingDirection = Direction.Left;
                    setFacingDirectionConstants();
                    setMovingDirectionConstants();
                    break;
                case "Right":
                    currentFacingDirection = Direction.Right;
                    currentMovingDirection = Direction.Right;
                    setFacingDirectionConstants();
                    setMovingDirectionConstants();
                    break;
                case "Up":
                    currentFacingDirection = Direction.Up;
                    currentMovingDirection = Direction.Up;
                    setFacingDirectionConstants();
                    setMovingDirectionConstants();
                    break;
                case "Down":
                    currentFacingDirection = Direction.Down;
                    currentMovingDirection = Direction.Down;
                    setFacingDirectionConstants();
                    setMovingDirectionConstants();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Tracks user input of the game through an xbox 360 controller
        /// </summary>
        /// <param name="gamePad1">keeps track of state of xbox 360 controller</param>
        public void UserInput(GamePadState gamePad1)
        {
            if (currentActionState == ActionState.Standby)
            {
                Direction newDirection = currentFacingDirection;
                if (gamePad1.ThumbSticks.Left.Y >= .5f && gamePad1.Triggers.Left >= .5f)
                    newDirection = Direction.Up;
                else if (gamePad1.ThumbSticks.Left.Y <= -.5f && gamePad1.Triggers.Left >= .5f)
                    newDirection = Direction.Down;
                else if (gamePad1.ThumbSticks.Left.X <= .5f && gamePad1.Triggers.Left >= .5f)
                    newDirection = Direction.Right;
                else if (gamePad1.ThumbSticks.Left.X >= -.5f && gamePad1.Triggers.Left >= .5f)
                    newDirection = Direction.Left;

                if (currentGrapple == Grapple.Static)
                {
                    if (gamePad1.ThumbSticks.Left.Y >= .5f && gamePad1.Triggers.Left <= .5f)
                    {
                        currentAction = Action.Walking;
                        newDirection = Direction.Up;
                    }
                    else if (gamePad1.ThumbSticks.Left.Y <= -.5f && gamePad1.Triggers.Left <= .5f)
                    {
                        currentAction = Action.Walking;
                        newDirection = Direction.Down;
                    }
                    else if (gamePad1.ThumbSticks.Left.X <= .5f && gamePad1.Triggers.Left <= .5f)
                    {
                        currentAction = Action.Walking;
                        newDirection = Direction.Right;
                    }
                    else if (gamePad1.ThumbSticks.Left.X >= -.5f && gamePad1.Triggers.Left <= .5f)
                    {
                        currentAction = Action.Walking;
                        newDirection = Direction.Left;
                    }


                    if (gamePad1.ThumbSticks.Left.Y >= .5f)
                    {
                        currentGrapple = Grapple.Starting;
                        currentMovingDirection = Direction.Up;
                        setMovingDirectionConstants();
                        newDirection = Direction.Up;
                    }
                    else if (gamePad1.ThumbSticks.Left.Y <= -.5f)
                    {
                        currentGrapple = Grapple.Starting;
                        currentMovingDirection = Direction.Down;
                        setMovingDirectionConstants();
                        newDirection = Direction.Down;
                    }
                    else if (gamePad1.ThumbSticks.Left.X <= .5f)
                    {
                        currentGrapple = Grapple.Starting;
                        currentMovingDirection = Direction.Right;
                        setMovingDirectionConstants();
                        newDirection = Direction.Right;
                    }
                    else if (gamePad1.ThumbSticks.Left.X >= -.5f)
                    {
                        currentGrapple = Grapple.Starting;
                        currentMovingDirection = Direction.Left;
                        setMovingDirectionConstants();
                        newDirection = Direction.Left;
                    }
                }
                //if (gamePad1.Buttons.A == ButtonState.Pressed)
                //{
                //    currentAction = Action.Attacking;
                //}

                //Not yet implemented
                //if (keys.IsKeyDown(Keys.RightControl))
                //    currentAction = Action.Blocking;

                //if (keys.IsKeyDown(Keys.RightShift))
                //    currentAction = Action.Jumping;

                if (newDirection != currentFacingDirection || currentAction != Action.Standing)
                {
                    currentFacingDirection = newDirection;
                    setFacingDirectionConstants();
                }
                if (currentAction != Action.Standing)
                    currentActionState = ActionState.Starting;
            }
            //else call the queue function
        }

        /// <summary>
        /// Sets the variables that are dependent on where the dude is looking
        /// </summary>
        private void setFacingDirectionConstants()
        {
            switch (currentFacingDirection)
            {
                case Direction.Down:
                    standFrame = 0;
                    facingDirection.X = 0;
                    facingDirection.Y = 1;
                    break;
                case Direction.Up:
                    standFrame = 3;
                    facingDirection.X = 0;
                    facingDirection.Y = -1;
                    break;
                case Direction.Right:
                    standFrame = 6;
                    facingDirection.X = 1;
                    facingDirection.Y = 0;
                    break;
                case Direction.Left:
                    standFrame = 9;
                    facingDirection.X = -1;
                    facingDirection.Y = 0;
                    break;
            }
        }

        /// <summary>
        /// sets the variables that are dependent on where the dude is moving
        /// </summary>
        private void setMovingDirectionConstants()
        {
            switch (currentMovingDirection)
            {
                case Direction.Down:
                    movingDirection.X = 0;
                    movingDirection.Y = 1;
                    if (walkFrame == 1)
                        walkFrame = 2;
                    else walkFrame = 1;
                    rotation = -1.57f;
                    break;
                case Direction.Up:
                    movingDirection.X = 0;
                    movingDirection.Y = -1;
                    if (walkFrame == 4)
                        walkFrame = 5;
                    else walkFrame = 4;
                    rotation = 1.57f;
                    break;
                case Direction.Right:
                    movingDirection.X = 1;
                    movingDirection.Y = 0;
                    if (walkFrame == 7)
                        walkFrame = 8;
                    else walkFrame = 7;
                    rotation = (float)(Math.PI);
                    break;
                case Direction.Left:
                    movingDirection.X = -1;
                    movingDirection.Y = 0;
                    if (walkFrame == 10)
                        walkFrame = 11;
                    else walkFrame = 10;
                    rotation = 0f;
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Updates the dude. Excludes drawing.
        /// </summary>
        /// <param name="map">the current map the dude is on</param>
        /// <param name="editor">is the editor mode on?</param>
        public void Update(ref Map map, ref bool editor)
        {
            if (currentActionState != ActionState.Standby)
            {

                switch (currentAction)
                {
                    case Action.Standing:
                        break;
                    case Action.Walking:
                        updateWalking(ref map, ref editor);
                        break;
                    //Not yet implemented
                    //case Action.Attacking:

                    //    break;

                    //case Action.Jumping:

                    //    break;
                    //case Action.Blocking:

                    case Action.Falling:
                        Falling(ref map);
                        break;
                    //case Action.Damaged:

                    //    break;
                    //case Action.Talking:

                    // break;
                }
                
            }
            if (currentGrapple != Grapple.Static)
                updateGrappling(ref map);
            updateVisiblity(ref map);
        }
        private void Falling(ref Map map)
        {
            switch (currentActionState)
            {
                case ActionState.Starting:
                    int i = 0;
                    while (true)
                    {
                        if (height > map.tileData[(int)tilePosition.X, (int)tilePosition.Y + i].height)
                        {
                            i++;
                            height--;
                        }
                        else
                        {
                            height = map.tileData[(int)tilePosition.X, (int)tilePosition.Y + i].height;
                            tilePosition.Y += i;
                            currentActionState = ActionState.InProgress;
                            goto case ActionState.InProgress;
                        }
                    }
                case ActionState.InProgress:
                    if (pixelPosition.Y < tilePosition.Y * Constants.tilesize)
                    {//if absolute value of next position is closer to target tile than current position
                        pixelPosition.Y += walkSpeed;
                    }
                    else
                    {
                        pixelPosition.Y = tilePosition.Y * Constants.tilesize;
                        goto case ActionState.Finishing;
                    }
                    break;
                case ActionState.Finishing:
                    goto case ActionState.Done;
                case ActionState.Done:
                    currentActionState = ActionState.Standby;
                    currentAction = Action.Standing;
                    break;
            }
        }
        /// <summary>
        /// updates the visibility of the dude on the map.
        /// </summary>
        /// <param name="map">the current map the dude is on</param>
        private void updateVisiblity(ref Map map)//currently running every update, may be changed to running only during movement
        {//problem, clamping does not necessarily mean old and etc.
            heightDifOld = 0;
            heightDifNew = 0;
            Vector2 truePosition = pixelPosition / Constants.tilesize;
            switch (currentMovingDirection)
            {
                case Direction.Up:
                    newPosition = new Vector2(truePosition.X, (float)Math.Floor((decimal)truePosition.Y));
                    previousPosition = new Vector2(truePosition.X, (float)Math.Ceiling((decimal)truePosition.Y));
                    break;
                case Direction.Down:
                    previousPosition = new Vector2(truePosition.X, (float)Math.Floor((decimal)truePosition.Y));
                    newPosition = new Vector2(truePosition.X, (float)Math.Ceiling((decimal)truePosition.Y));
                    break;
                case Direction.Left:
                    newPosition = new Vector2((float)Math.Floor((decimal)truePosition.X), truePosition.Y);
                    previousPosition = new Vector2((float)Math.Ceiling((decimal)truePosition.X), truePosition.Y);
                    break;
                case Direction.Right:
                    previousPosition = new Vector2((float)Math.Floor((decimal)truePosition.X), truePosition.Y);
                    newPosition = new Vector2((float)Math.Ceiling((decimal)truePosition.X), truePosition.Y);
                    break;
            }
            tileClass oldTile = map.tileData[(int)previousPosition.X, (int)previousPosition.Y];
            tileClass newTile = map.tileData[(int)newPosition.X, (int)newPosition.Y];
            heightDifOld = height - oldTile.height;
            heightDifNew = height - newTile.height;
            if (oldTile.coverage == newTile.coverage)
            {
                if (oldTile.coverage == -1)
                {
                    currentVisibility = Visibility.Normal;
                    return;
                }
                if (oldTile.coverage == 0)
                {
                    currentVisibility = Visibility.Covered;
                    return;
                }
                if (oldTile.coverage > 1)
                {
                    currentVisibility = Visibility.Hidden;
                    return;
                }
            }
            else
            {
                if (oldTile.coverage == 0 && newTile.coverage > 1)
                    currentVisibility = Visibility.Hidden;
                if (oldTile.coverage > 1 && newTile.coverage == 0)
                    currentVisibility = Visibility.Covered;
                if (oldTile.coverage == -1 && newTile.coverage == 0)
                    currentVisibility = Visibility.Covering;
                if (oldTile.coverage == 0 && newTile.coverage == -1)
                    currentVisibility = Visibility.Uncovering;
                if (oldTile.coverage == -1 && newTile.coverage > 1)
                    currentVisibility = Visibility.Hiding;
                if (oldTile.coverage > 1 && newTile.coverage == -1)
                    currentVisibility = Visibility.Unhiding;
            }
        }

        /// <summary>
        /// Performs all calculations pertaining to the dude's walking action
        /// </summary>
        /// <param name="map">the map the dude is currently on</param>
        /// <param name="editor">is the editor mode on?</param>
        private void updateWalking(ref Map map, ref bool editor)
        {
            switch (currentActionState)
            {
                case ActionState.Starting:
                    if (tilePosition.X + movingDirection.X >= 0 && tilePosition.X + movingDirection.X < map.SizeX && tilePosition.Y + movingDirection.Y >= 0 && tilePosition.Y + movingDirection.Y < map.SizeY)
                    {//if dude is moving inside the map
                        tileClass startingTile = map.tileData[(int)tilePosition.X, (int)tilePosition.Y];
                        tileClass endingTile = map.tileData[(int)tilePosition.X + movingDirection.X, (int)tilePosition.Y + movingDirection.Y];
                        if (startingTile == null || (!endingTile.impassible && Math.Abs(startingTile.height - endingTile.height) <= 0.5f
                            && endingTile.coverage != 1f) || editor)
                        {//if target tile is movable to
                            //move approved, start moving process
                            tilePosition.X += movingDirection.X;
                            tilePosition.Y += movingDirection.Y;
                            height = endingTile.height;
                            currentActionState = ActionState.InProgress;
                            goto case ActionState.InProgress;
                        }
                        else goto case ActionState.Done;//cant move there, walking action skips to end
                    }
                    else goto case ActionState.Done;//cant move there, walking action skips to end
                case ActionState.InProgress:
                    if (pixelPosition != Constants.tilesize * tilePosition && Math.Abs(pixelPosition.X + (walkSpeed * movingDirection.X) - (tilePosition.X * Constants.tilesize)) <= Math.Abs(pixelPosition.X - (tilePosition.X * Constants.tilesize)) &&
                       Math.Abs(pixelPosition.Y + (walkSpeed * movingDirection.Y) - (tilePosition.Y * Constants.tilesize)) <= Math.Abs(pixelPosition.Y - (tilePosition.Y * Constants.tilesize)))
                    {//if absolute value of next position is closer to target tile than current position
                        pixelPosition.Y += (walkSpeed * movingDirection.Y);
                        pixelPosition.X += (walkSpeed * movingDirection.X);
                    }
                    else goto case ActionState.Finishing;//else walking complete, move to next state
                    break;
                case ActionState.Finishing:
                    pixelPosition = tilePosition * Constants.tilesize;//ensure dude fits directly on tilemap
                    dudeMoved = true;
                    goto case ActionState.Done;//go to last state                    
                case ActionState.Done:
                    //call queue, if has prepicked input, go to that, else go to standby and standing action
                    //set dude states to stationary states.
                    currentActionState = ActionState.Standby;
                    currentAction = Action.Standing;
                    break;
            }

        }

        /// <summary>
        /// performs all calculations pertaining to the dude's grappling action
        /// </summary>
        /// <param name="map"></param>
        private void updateGrappling(ref Map map)
        {
            switch (currentGrapple)
            {
                case Grapple.Starting://determines grapple shot length, determines if player is grappling to new position
                    currentGrapple = Grapple.Extending;
                    for (int i = 1; i < grappleLength; i++)
                    {
                        if (tilePosition.X + (movingDirection.X * i) >= 0 && tilePosition.X + (movingDirection.X * i) < map.SizeX && tilePosition.Y + (movingDirection.Y * i) >= 0 && tilePosition.Y + (movingDirection.Y * i) < map.SizeY)
                        {//is grapple inside map?
                            tileClass startingTile = map.tileData[(int)tilePosition.X, (int)tilePosition.Y];
                            tileClass endingTile = map.tileData[(int)tilePosition.X + (movingDirection.X * i), (int)tilePosition.Y + (movingDirection.Y * i)];
                            float alteredcoverage = endingTile.coverage > -1 ? endingTile.coverage : 0;
                            if ((endingTile.grappable == true && endingTile.height == startingTile.height) || (alteredcoverage == 1f && endingTile.height == startingTile.height) || endingTile.height > startingTile.height || (endingTile.height < startingTile.height && endingTile.height + alteredcoverage > startingTile.height))
                            {
                                grappleShotLength = i - 1;
                                tilePosition.Y += grappleShotLength * movingDirection.Y;
                                tilePosition.X += grappleShotLength * movingDirection.X;
                                goto case Grapple.Extending;
                            }
                        }
                    }
                    grappleShotLength = grappleLength - 2;
                    goto case Grapple.Extending;
                case Grapple.Extending://initial extending of the grapple
                    if (grappleSize < (grappleShotLength * Constants.tilesize))
                        grappleSize += (grappleSpeed * 2);
                    else
                    {
                        grappleSize = grappleShotLength * Constants.tilesize;
                        if (pixelPosition != Constants.tilesize * tilePosition)
                        {
                            currentGrapple = Grapple.Hit;
                            goto case Grapple.Hit;
                        }
                        else
                        {
                            currentGrapple = Grapple.Miss;
                            goto case Grapple.Miss;
                        }
                    }
                    break;
                case Grapple.Hit:
                    if (pixelPosition != Constants.tilesize * tilePosition)//controls player moving to new position if new position was calculated
                    {
                        if (Math.Abs(pixelPosition.X + (grappleSpeed * movingDirection.X) - (tilePosition.X * Constants.tilesize)) <= Math.Abs(pixelPosition.X - (tilePosition.X * Constants.tilesize)) &&
                        Math.Abs(pixelPosition.Y + (grappleSpeed * movingDirection.Y) - (tilePosition.Y * Constants.tilesize)) <= Math.Abs(pixelPosition.Y - (tilePosition.Y * Constants.tilesize)))
                        {
                            pixelPosition.X += movingDirection.X * grappleSpeed;
                            pixelPosition.Y += movingDirection.Y * grappleSpeed;
                            grappleSize -= grappleSpeed;
                        }
                        if (pixelPosition == Constants.tilesize * tilePosition)
                        {
                            dudeMoved = true;
                            pixelPosition = tilePosition * Constants.tilesize;
                            grappleSize = 0;
                            if (map.tileData[(int)tilePosition.X, (int)tilePosition.Y].impassible == true)
                            {
                                tilePosition.X -= grappleShotLength * movingDirection.X;
                                tilePosition.Y -= grappleShotLength * movingDirection.Y;
                                pixelPosition = tilePosition * Constants.tilesize;
                                //set the vibration controls here
                                PlayerHasBeenHurt();
                                Health--;
                            }
                        }
                    }
                    else//grapple shot finished, set to static
                    {
                        grappleSize = 0;
                        currentGrapple = Grapple.Finishing;
                        goto case Grapple.Finishing;
                    }
                    break;
                case Grapple.Miss:
                    if (grappleSize > 0)//unextends grapple if new position not needed
                        grappleSize -= grappleSpeed * 2;
                    else//grapple shot finished, set to static
                    {
                        grappleSize = 0;
                        currentGrapple = Grapple.Finishing;
                        goto case Grapple.Finishing;
                    }
                    break;
                case Grapple.Finishing:
                    pixelPosition = tilePosition*32;
                    if (height > map.tileData[(int)tilePosition.X, (int)tilePosition.Y].height + 0.5)
                    {
                        currentAction = Action.Falling;
                        currentActionState = ActionState.Starting;
                    }
                    goto case Grapple.Done;
                case Grapple.Done:
                    //call queue, if has prepicked input, go to that, else go to standby and standing action
                    dudeMoved = true;
                    currentGrapple = Grapple.Static;
                    break;
            }
        }

        /// <summary>
        /// sends vibration to gamepad if dude is hurt
        /// </summary>
        public void PlayerHasBeenHurt()
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
        }
    }
}

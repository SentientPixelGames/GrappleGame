using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// This Enum designates all of the different actions the characters can perform
        /// </summary>
        enum Action
        {
            Standing,
            Walking,
            Falling,
            Talking,
            Editing,
        }

        /// <summary>
        /// This specific instance of the Action enum keeps track of the characters current action, with no action represented as standing
        /// </summary>
        Action currentAction = Action.Standing;

        /// <summary>
        /// This Enum designates the various states a character can be in when being edited
        /// </summary>
        enum CharacterEditing
        {
            None,
            TextEntry,
            DefineArea,
        }

        /// <summary>
        /// This specific instance of the CharacterEditing enum keeps track of the current editing state of a character
        /// </summary>
        CharacterEditing currentCharacterEditing = CharacterEditing.None;

        /// <summary>
        /// This Enum designates all of the different states an action can be in. Standby is waiting for an action, and the rest are how far the action the character is performing has progressed
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
        /// This specific instance of the ActionState enum keeps track of the characters current progress through an action
        /// </summary>
        volatile ActionState currentActionState = ActionState.Standby;

        /// <summary>
        /// This enum designates the four possible directions in the 2-D space. Note that all direction controlled processes in the character are constrained to vertical and horizontal directions
        /// </summary>
        enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        /// <summary>
        /// This specific instance of the Direction enum keeps track of the direction the character is facing
        /// </summary>
        Direction currentFacingDirection = Direction.Down;

        /// <summary>
        /// This specific instance of the direction enum keeps track of the direction the character is moving
        /// </summary>
        Direction currentMovingDirection = Direction.Down;

        /// <summary>
        /// This enum designates the different ways the dude is drawn. Covered is when the dude is covered by shadow. Hidden is when the dude is behind an object.
        /// </summary>
        enum Visibility
        {
            Normal,
            Hidden,
            Hiding,
            Unhiding,
        }

        /// <summary>
        /// This specific instance of the visibility enum keeps track of how to draw the dude
        /// </summary>
        Visibility currentVisibility = Visibility.Normal;

        public struct Save
        {
            public String TextureName;
            public Vector2 HomePosition;
            public Point Range;
            public List<String> Conversation;
            public int ID;
            public float Height;
        } public Save saveData;


        /// <summary>
        /// This is the thread that the character uses to poll and wait to see if the dude either decides to talk to them or walks away from them
        /// </summary>
        Thread thread;

        /// <summary>
        /// Contains the standing and walking images of the dude
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// contains shadow image
        /// </summary>
        private Texture2D shadow;

        /// <summary>
        /// contains the Xbox360 A button
        /// </summary>
        private Texture2D Abutton;

        /// <summary>
        /// contains editor new text for the character button
        /// </summary>
        private Texture2D e_newText;

        /// <summary>
        /// contains the rectangle for the new text button
        /// </summary>
        private Rectangle e_newTextButton;

        /// <summary>
        /// contains editor new area definition for the character button
        /// </summary>
        private Texture2D e_newArea;

        /// <summary>
        /// contains the rectangle for the new area button
        /// </summary>
        private Rectangle e_newAreaButton;

        /// <summary>
        /// contains editor close character editing button
        /// </summary>
        private Texture2D e_Close;

        /// <summary>
        /// contains the rectangle for the close button
        /// </summary>
        private Rectangle e_CloseButton;

        /// <summary>
        /// contains editor delete character button
        /// </summary>
        private Texture2D e_Delete;

        /// <summary>
        /// contains the rectangle for the delete button
        /// </summary>
        private Rectangle e_DeleteButton;

        /// <summary>
        /// contains editor blank tile for creating text boxes
        /// </summary>
        private Texture2D e_blank;

        /// <summary>
        /// contains the texture for characters speaking
        /// </summary>
        private Texture2D e_conversationBlock;

        /// <summary>
        /// The position of the char in tile count. The "Official" position of the char.
        /// </summary>
        public Vector2 tilePosition;

        /// <summary>
        /// The position of the character in tiles. Never gets updated as this is the beginning position of the character
        /// </summary>
        private Vector2 homePosition;

        /// <summary>
        /// This position of the char in pixel count. Used for transitioning between tiles
        /// </summary>
        public Vector2 pixelPosition;

        /// <summary>
        /// keeps track of the tile position the char is moving towards
        /// </summary>
        private Vector2 newPosition;

        /// <summary>
        /// keeps track of the previous tile position the char is moving away from.
        /// </summary>
        private Vector2 previousPosition;

        /// <summary>
        /// Sets the walkspeed of the char. The units are in pixels per update cycle
        /// </summary>
        public int walkSpeed = 2;

        /// <summary>
        /// sets the characters poosible conversations
        /// </summary>
        private List<string> Conversation;

        /// <summary>
        /// sets the characters current message from conversations
        /// </summary>
        private string currentConversation;

        /// <summary>
        /// points to the current string in the Conversation List
        /// </summary>
        private int conversationPointer = 0;

        /// <summary>
        /// used to create text streaming effect when talking to players
        /// </summary>
        private float letterPointer = 0;

        /// <summary>
        /// sets the tile range the character can move from original starting position
        /// </summary>
        private Point range;

        /// <summary>
        /// sets the characters Element Number in the Maps List
        /// </summary>
        public int ID;

        /// <summary>
        /// Contains the generic constants for the game
        /// </summary>
        Constants Constants = new Constants();

        /// <summary>
        /// Declare the font characters use to talk
        /// </summary>
        SpriteFont font;

        ///<summary>
        ///Contains the UserInput Typing Class for the CharacterEditing
        /// </summary>
        UserInput input = new UserInput();

        /// <summary>
        /// Contains the delegate for the character event handler in the Map class
        /// </summary>
        Constants.CharacterEventHandler eventHandler;

        /// <summary>
        /// The current walking image the char is on. 
        /// </summary>
        private int walkFrame = 0;

        /// <summary>
        /// The current standing image the char is on. 
        /// </summary>
        private int standFrame = 0;
        /// <summary>
        /// the unit vector that contains the direction of movement
        /// </summary>
        private Point movingDirection = new Point(0, 0);
        /// <summary>
        /// indicates whether the char has moved
        /// </summary>
        public bool charMoved = false;

        /// <summary>
        /// the unit vector that contains the direction the char is facing
        /// </summary>
        private Point facingDirection = new Point(0, 0); //may not be needed, to be determined

        /// <summary>
        /// Keeps track of the current height of the char
        /// </summary>
        private float height;
        /// <summary>
        /// keeps track of height difference between char and ground
        /// </summary>
        private float heightDifOld, heightDifNew;

        /// <summary>
        /// Contains all textures, properties, variables pertaining the the user controlled character
        /// Sets a Default Character to an Area Range of 5x5 tiles, and no conversation strings
        /// </summary>
        /// <param name="tilePosition">starting position of the character on the map</param>
        /// <param name="mainTexture">standing and walking textures</param>
        /// <param name="shadowTexture">texture for characters shadow</param>
        public Character(ContentManager Content, Vector2 tilePosition, float height, int ID, Texture2D mainTexture, Constants.CharacterEventHandler characterEventHandler)
        {
            this.tilePosition = tilePosition;
            homePosition = tilePosition;
            pixelPosition = this.tilePosition * Constants.tilesize;
            texture = mainTexture;
            shadow = Content.Load<Texture2D>("Characters/dude/dudeshadow");
            Abutton = Content.Load<Texture2D>("Characters/NPCs/Extras/A_button");
            e_newText = Content.Load<Texture2D>("Editor/NewTextButton");
            e_newArea = Content.Load<Texture2D>("Editor/TileAreaButton");
            e_Delete = Content.Load<Texture2D>("Editor/DeleteButton");
            e_Close = Content.Load<Texture2D>("Editor/CloseButton");
            e_blank = Content.Load<Texture2D>("Other/map");
            e_conversationBlock = Content.Load<Texture2D>("Game HUD/TextBox");
            font = Content.Load<SpriteFont>("Fonts/characterFont");

            e_newTextButton = new Rectangle(15, 50, e_newText.Width, e_newText.Height);
            e_newAreaButton = new Rectangle(15, 50 + 5 + e_newText.Height, e_newArea.Width, e_newArea.Height);
            e_DeleteButton = new Rectangle(15, 50 + 10 + e_newText.Height + e_newArea.Height, e_Delete.Width, e_Delete.Height);
            e_CloseButton = new Rectangle(15, 50 + 15 + e_newText.Height + e_newArea.Height + e_Delete.Height, e_Close.Width, e_Close.Height);

            this.height = height;
            Conversation = new List<string>();
            currentConversation = "";
            range = new Point(5, 5);
            this.ID = ID;
            this.eventHandler = new Constants.CharacterEventHandler(characterEventHandler);
        }

        /// <summary>
        /// Draws the char
        /// </summary>
        /// <param name="sb">Drawing tool for XNA</param>
        public void Draw(SpriteBatch sb)
        {
            if (heightDifNew == heightDifOld)
            {
                if (heightDifOld == 0)
                {
                    sb.Draw(shadow, new Vector2(pixelPosition.X + Constants.tilesize / 2, pixelPosition.Y + Constants.tilesize), new Rectangle(0, 0, shadow.Width, shadow.Height), Color.White, 0f, new Vector2(shadow.Width / 2, shadow.Height - 3), 1f, SpriteEffects.None, 0.7002f);
                }
                else
                {
                    float heightfactor = heightDifNew * 0.5f;
                    if (heightfactor > 0.5f)
                        heightfactor = 0.5f;
                    float temp1 = pixelPosition.X + (3 * (1 + heightfactor)) - (shadow.Width / 2) * heightfactor;
                    float temp2 = ((float)shadow.Width / (float)Constants.tilesize) * (Constants.tilesize - (pixelPosition.X - (newPosition.X * Constants.tilesize)));
                    sb.Draw(shadow, new Vector2(temp1, pixelPosition.Y + Constants.tilesize * (1 + heightDifNew)), new Rectangle(0, 0, shadow.Width, shadow.Height), Color.White, 0f, new Vector2(0, shadow.Height - 3), 1f + heightfactor, SpriteEffects.None, 0.7002f);
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
                case Action.Talking:
                    switch (currentActionState)
                    {
                        case ActionState.Standby:
                            sb.DrawString(font, "Press", new Vector2(tilePosition.X * Constants.tilesize - 50, (tilePosition.Y - 1) * Constants.tilesize), Color.White);
                            sb.Draw(Abutton, new Rectangle((int)(tilePosition.X*Constants.tilesize) + 3, (int)(tilePosition.Y - 1)*Constants.tilesize, 25, 25), Color.White);
                            sb.DrawString(font, "to talk", new Vector2(tilePosition.X * Constants.tilesize + 30, (tilePosition.Y - 1) * Constants.tilesize), Color.White);
                            break;
                        case ActionState.InProgress:
                            break;
                    }
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
                //case Visibility.Covered:
                //    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                //    break;
                //case Visibility.Covering:
                //    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                //    switch (currentMovingDirection)
                //    {
                //        case Direction.Up:
                //            //sb.Draw(texture, new Vector2(pixelPosition.X, previousPosition.Y * Constants.tilesize), new Rectangle(Constants.tilesize * drawFrame, (int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y, Constants.tilesize, Constants.tilesize - ((int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize - ((int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //        case Direction.Left:
                //            sb.Draw(texture, new Vector2(previousPosition.X * Constants.tilesize, pixelPosition.Y), new Rectangle(Constants.tilesize * drawFrame + (int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X, 0, Constants.tilesize - ((int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //        case Direction.Down:
                //            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize - ((int)pixelPosition.Y - (int)(previousPosition.Y * Constants.tilesize))), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //        case Direction.Right:
                //            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize - ((int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize)), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //    }
                //    break;
                //case Visibility.Uncovering:
                //    sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                //    switch (currentMovingDirection)
                //    {
                //        case Direction.Up:
                //            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, (int)(previousPosition.Y * Constants.tilesize) - (int)pixelPosition.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //        case Direction.Left:
                //            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, (int)(previousPosition.X * Constants.tilesize) - (int)pixelPosition.X, Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //        case Direction.Down:
                //            sb.Draw(texture, pixelPosition, new Rectangle(Constants.tilesize * drawFrame, 0, Constants.tilesize, (int)pixelPosition.Y - (int)(previousPosition.Y * Constants.tilesize)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //        case Direction.Right:
                //            sb.Draw(texture, newPosition * Constants.tilesize, new Rectangle(Constants.tilesize * drawFrame + Constants.tilesize - ((int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize)), 0, (int)pixelPosition.X - (int)(previousPosition.X * Constants.tilesize), Constants.tilesize), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.6f);
                //            break;
                //    }
                //    break;
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
        }
        /// <summary>
        /// Draws Character Text, Editable Options, etc. on the HUD of the Current Map
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawHUD(SpriteBatch spriteBatch)
        {
            switch (currentAction)
            {
                case Action.Editing:
                    switch (currentCharacterEditing)
                    {
                        case CharacterEditing.None:
                            spriteBatch.Draw(e_blank, new Rectangle(5, 40, 20 + e_newText.Width, 35 + e_newText.Height * 4), Color.DarkGray);
                            spriteBatch.Draw(e_blank, new Rectangle(10, 45, 10 + e_newText.Width, 25 + e_newText.Height * 4), Color.Gray);
                            spriteBatch.Draw(e_newText, e_newTextButton, Color.DarkGray);
                            spriteBatch.Draw(e_newArea, e_newAreaButton, Color.White);
                            spriteBatch.Draw(e_Delete, e_DeleteButton, Color.White);
                            spriteBatch.Draw(e_Close, e_CloseButton, Color.White);
                            break;
                        case CharacterEditing.TextEntry:
                            spriteBatch.DrawString(font, "Begin Typing The Characters Script:\n1. Use Tab if need more than 1 page\n2. Use Escape to Stop Entering Character Script", new Vector2(30, 30), Color.GhostWhite);
                            spriteBatch.Draw(e_conversationBlock, new Rectangle(0, 480, e_conversationBlock.Width, e_conversationBlock.Height), Color.White);
                            spriteBatch.DrawString(font, currentConversation, new Vector2(20, 495), Color.Black);
                            break;
                        case CharacterEditing.DefineArea:
                            spriteBatch.DrawString(font,
                                "Enter the Rectanglur Area that the Character May Walk Over\n1. Use a Comma to Seperate the Dimensions\n2. Use Enter to Finish Entering Range\nExample: 4,8\n\nUser Entered Area = "
                                + input.text, new Vector2(30, 30), Color.GhostWhite);
                            break;
                    }
                    break;
                case Action.Talking:
                    switch (currentActionState)
                    {
                        case ActionState.InProgress:
                            if ((int)Math.Floor(letterPointer) + 1 <= currentConversation.Length)
                                letterPointer += 0.5f;
                            spriteBatch.Draw(e_conversationBlock, new Rectangle(0, 480, e_conversationBlock.Width, e_conversationBlock.Height), Color.White);
                            spriteBatch.DrawString(font, currentConversation.Substring(0, (int)Math.Floor(letterPointer)), new Vector2(20, 495), Color.Black);
                            spriteBatch.Draw(Abutton, new Rectangle(e_conversationBlock.Width - 153, 437 + e_conversationBlock.Height, 25, 25), Color.White);
                            spriteBatch.DrawString(font, "to continue..", new Vector2(e_conversationBlock.Width - 125, 435 + e_conversationBlock.Height), Color.Black);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        #region CharacterInput
        /// <summary>
        /// Tracks user input of the game through the keyboard. Default control scheme
        /// </summary>
        /// <param name="keys">Tracks the current state of the keyboard</param>
        public void CharacterInput()
        {
            if (currentCharacterEditing == CharacterEditing.TextEntry)
            {
                if (input.userinput())
                {
                    if (input.text.EndsWith("\t"))
                    {
                        Conversation.Add(input.text.Remove(input.text.Length - 1));
                        input.text = "";
                    }
                    else
                    {
                        Conversation.Add(input.text);
                        currentCharacterEditing = CharacterEditing.None;
                        input.text = "";
                    }
                }

                currentConversation = input.text;
            }
            else if (currentCharacterEditing == CharacterEditing.DefineArea)
            {
                if (input.NumericalInput())
                {
                    //input.text.Split(',')[0];
                    range = new Point(Convert.ToInt32(input.text.Split(',')[0], 10), Convert.ToInt32(input.text.Split(',')[1], 10));
                    currentCharacterEditing = CharacterEditing.None;
                    input.text = "";
                }
            }
        }

        /// <summary>
        /// Gives the char direct commands of action and direction. Bypasses the typical user input of keyboard or controller
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
        /// Sets the variables that are dependent on where the char is looking
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
                    break;
                case Direction.Up:
                    movingDirection.X = 0;
                    movingDirection.Y = -1;
                    if (walkFrame == 4)
                        walkFrame = 5;
                    else walkFrame = 4;
                    break;
                case Direction.Right:
                    movingDirection.X = 1;
                    movingDirection.Y = 0;
                    if (walkFrame == 7)
                        walkFrame = 8;
                    else walkFrame = 7;
                    break;
                case Direction.Left:
                    movingDirection.X = -1;
                    movingDirection.Y = 0;
                    if (walkFrame == 10)
                        walkFrame = 11;
                    else walkFrame = 10;
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Updates the dude. Excludes drawing.
        /// </summary>
        /// <param name="map">the current map the dude is on</param>
        /// <param name="editor">is the editor mode on?</param>
        public void Update(int rndNum)
        {
            switch (currentAction)
            {
                case Action.Standing:
                    switch (rndNum % 750)
                    {
                        case 0:
                            if (eventHandler(0, "Left", this.ID) && tilePosition.X - 1 >= homePosition.X - range.X)
                            {
                                ForceInput("Walk", "Left");
                            }
                            break;
                        case 1:
                            if (eventHandler(0, "Right", this.ID) && tilePosition.X + 1 <= homePosition.X + range.X)
                            {
                                ForceInput("Walk", "Right");
                            }
                            break;
                        case 2:
                            if (eventHandler(0, "Up", this.ID) && tilePosition.Y - 1 >= homePosition.Y - range.Y)
                            {
                                ForceInput("Walk", "Up");
                            }
                            break;
                        case 3:
                            if (eventHandler(0, "Down", this.ID) && tilePosition.Y + 1 <= homePosition.Y + range.Y)
                            {
                                ForceInput("Walk", "Down");
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case Action.Walking:
                    updateWalking();
                    break;
                case Action.Falling:
                    //Falling(ref map);
                    break;
                case Action.Talking:
                    switch (currentActionState)
                    {
                        case ActionState.Standby:

                            break;
                        case ActionState.InProgress:
                            break;
                        case ActionState.Finishing:
                            currentAction = Action.Standing;
                            currentActionState = ActionState.Standby;
                            break;
                    }
                    break;
                case Action.Editing:
                    break;
            }
        }

        /// <summary>
        /// Returns if the character is finished being edited or not
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="oldmouse"></param>
        /// <returns></returns>
        public bool editUpdate(MouseState mouse, MouseState oldmouse)
        {
            switch (currentAction)
            {
                case Action.Standing:
                    break;
                case Action.Walking:
                    updateWalking();
                    break;
                case Action.Falling:
                    //Falling(ref map);
                    break;
                case Action.Talking:
                    break;
                case Action.Editing:
                    switch (currentCharacterEditing)
                    {
                        case CharacterEditing.None:
                            return Editing(mouse, oldmouse);
                        case CharacterEditing.TextEntry:
                            CharacterInput();
                            break;
                        case CharacterEditing.DefineArea:
                            CharacterInput();
                            break;
                    }

                    break;
            }
            return false;
        }
        private void Falling(ref Map map)
        {
            switch (currentActionState)
            {
                case ActionState.Starting:
                    int i = 0;
                    while (true)
                    {
                        if (height > map.tileData[(int)tilePosition.X, (int)tilePosition.Y + i].tileData.height)
                        {
                            i++;
                            height--;
                        }
                        else
                        {
                            height = map.tileData[(int)tilePosition.X, (int)tilePosition.Y + i].tileData.height;
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
            heightDifOld = height - oldTile.tileData.height;
            heightDifNew = height - newTile.tileData.height;
            if (oldTile.objectData.height == newTile.objectData.height)
            {
                if (oldTile.objectData.height == 0)
                {
                    currentVisibility = Visibility.Normal;
                    return;
                }
                if (oldTile.objectData.height > 1)
                {
                    currentVisibility = Visibility.Hidden;
                    return;
                }
            }
            else
            {
                if (oldTile.objectData.height > 1 && newTile.objectData.height == 0)
                    currentVisibility = Visibility.Unhiding;
                if (oldTile.objectData.height == 0 && newTile.objectData.height > 1)
                    currentVisibility = Visibility.Hiding;
            }
        }

        /// <summary>
        /// Performs all calculations pertaining to the dude's walking action
        /// </summary>
        /// <param name="map">the map the dude is currently on</param>
        /// <param name="editor">is the editor mode on?</param>
        private void updateWalking()
        {
            switch (currentActionState)
            {
                case ActionState.Starting:
                    tilePosition.X += movingDirection.X;
                    tilePosition.Y += movingDirection.Y;
                    currentActionState = ActionState.InProgress;
                    goto case ActionState.InProgress;
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
                    charMoved = true;
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
        /// Signals to Character to Stop walking around as the Dude is next to them and might want to talk
        /// </summary>
        public void PlayerNear(Dude theDude, string facingDir)
        {
            currentAction = Action.Talking;
            currentActionState = ActionState.Standby;
            switch (facingDir)
            {
                case "Up":
                    currentFacingDirection = Direction.Down;
                    break;
                case "Down":
                    currentFacingDirection = Direction.Up;
                    break;
                case "Left":
                    currentFacingDirection = Direction.Right;
                    break;
                case "Right":
                    currentFacingDirection = Direction.Left;
                    break;
            }
            setFacingDirectionConstants();
            thread = new Thread(() => PollDude(theDude));
            thread.Start();
        }

        public bool Talk()
        {
            //check to see if the thread is still looking and if it is then stop it
            if (thread.IsAlive)
                thread.Abort();

            switch (currentActionState)
            {
                case ActionState.Standby:
                    conversationPointer = 0;
                    letterPointer = 0;
                    currentActionState = ActionState.InProgress;
                    currentConversation = Conversation.ElementAt(conversationPointer);
                    conversationPointer++;
                    break;
                case ActionState.InProgress:
                    if (letterPointer >= currentConversation.Length - 1)
                    {
                        if (Conversation.Count > conversationPointer)
                        {
                            letterPointer = 0;
                            currentConversation = Conversation.ElementAt(conversationPointer);
                            conversationPointer++;
                        }
                        else
                        {
                            letterPointer = 0;
                            currentActionState = ActionState.Finishing;
                            conversationPointer = 0;
                            return false;
                        }
                    }
                    else letterPointer = currentConversation.Length - 1;
                    break;
            }
            return true;
            
        }

        /// <summary>
        /// Kicks off the character thread to start monitoring the dude to see if he no longer might want to talk with them.
        /// </summary>
        /// <param name="theDude">Send Dude Reference in so that character can monitor him</param>
        void PollDude(Dude theDude)
        {
            bool _deciding = false;
            while (!_deciding)
            {
                if (theDude.dudeMoved)
                {
                    _deciding = true;
                    currentActionState = ActionState.Finishing;
                }
            }

        }

        /// <summary>
        /// sends vibration to gamepad if dude is hurt
        /// </summary>
        public void PlayerHasBeenHurt()
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
        }

        /// <summary>
        /// Character was right clicked Edited
        /// </summary>
        public void wasRightClicked()
        {
            currentAction = Action.Editing;
        }



        /// <summary>
        /// Character is in Edit Mode
        /// </summary>
        private bool Editing(MouseState mouse, MouseState oldmouse)
        {
            if (MouseClicked(mouse, oldmouse, e_newTextButton))
            {
                currentCharacterEditing = CharacterEditing.TextEntry;
            }
            else if (MouseClicked(mouse, oldmouse, e_newAreaButton))
            {
                currentCharacterEditing = CharacterEditing.DefineArea;
            }
            else if (MouseClicked(mouse, oldmouse, e_DeleteButton))
            {
                eventHandler(1, null, this.ID);
            }
            else if (MouseClicked(mouse, oldmouse, e_CloseButton))
            {
                currentCharacterEditing = CharacterEditing.None;
                currentAction = Action.Standing;
                return true;
            }
            return false;
        }



        /// <summary>
        /// Method Used to quickly determine if the mouse has been "clicked"
        /// meaning that the current mouse is released and the previous mouse is pressed
        /// </summary>
        /// <param name="mouse">Current Mouse State</param>
        /// <param name="oldmouse">Mouse State from the Previous Update Cycle</param>
        /// <param name="area">Rectangle Area to check to see if click happened in
        /// if NULL then Method only checks to see if Mouse was clicked anywhere</param>
        /// <returns>True if Mouse Click occurred</returns>
        private bool MouseClicked(MouseState mouse, MouseState oldmouse, Rectangle area)
        {
            if (area == null)
            {
                if (mouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed)
                    return true;
                else return false;
            }
            else
            {
                if (mouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed &&
                    mouse.X >= area.X && mouse.X <= area.X + area.Width && mouse.Y >= area.Y && mouse.Y <= area.Y + area.Height)
                {
                    return true;
                }
                else return false;

            }
        }

        public void SetSaveValues()
        {
            saveData.TextureName = texture.Name;
            saveData.Range = range;
            saveData.HomePosition = homePosition;
            saveData.Conversation = Conversation;
            saveData.Height = this.height;
            saveData.ID = this.ID;
        }

        public void SetLoadValues(List<string> convo, Point range)
        {
            Conversation = convo;
            this.range = range;
        }
    }
}

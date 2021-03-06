using System;
using StardewModdingAPI.Framework.Input;

namespace StardewModdingAPI.Events
{
    /// <summary>Event arguments when a button is pressed.</summary>
    public class InputButtonPressedArgsInput : EventArgs
    {
        /*********
        ** Properties
        *********/
        /// <summary>The game's current input state.</summary>
        private readonly SInputState InputState;


        /*********
        ** Accessors
        *********/
        /// <summary>The button on the controller, keyboard, or mouse.</summary>
        public SButton Button { get; }

        /// <summary>The current cursor position.</summary>
        public ICursorPosition Cursor { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="button">The button on the controller, keyboard, or mouse.</param>
        /// <param name="cursor">The cursor position.</param>
        /// <param name="inputState">The game's current input state.</param>
        internal InputButtonPressedArgsInput(SButton button, ICursorPosition cursor, SInputState inputState)
        {
            this.Button = button;
            this.Cursor = cursor;
            this.InputState = inputState;
        }

        /// <summary>Whether a mod has indicated the key was already handled, so the game should handle it.</summary>
        public bool IsSuppressed()
        {
            return this.IsSuppressed(this.Button);
        }

        /// <summary>Whether a mod has indicated the key was already handled, so the game should handle it.</summary>
        /// <param name="button">The button to check.</param>
        public bool IsSuppressed(SButton button)
        {
            return this.InputState.SuppressButtons.Contains(button);
        }

        /// <summary>Get whether a given button was pressed or held.</summary>
        /// <param name="button">The button to check.</param>
        public bool IsDown(SButton button)
        {
            return this.InputState.IsDown(button);
        }
    }
}

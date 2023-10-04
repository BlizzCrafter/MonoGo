﻿#region File Description
//-----------------------------------------------------------------------------
// Generate message boxes and other prompts.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;


namespace Monofoxe.Extended.GUI.Utils
{
    /// <summary>
    /// Monofoxe.Extended.GUI.Utils contain different utilities and helper classes to use Monofoxe.Extended.GUI.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Helper class to generate message boxes and prompts.
    /// </summary>
    public static class MessageBox
    {
        /// <summary>
        /// Return object containing all the data of a message box instance.
        /// </summary>
        public class MessageBoxHandle
        {
            /// <summary>
            /// Message box panel.
            /// </summary>
            public Entities.Panel Panel;

            /// <summary>
            /// Object used to fade out the background.
            /// </summary>
            public Entities.EntityUI BackgroundFader;

            /// <summary>
            /// Hide / close the message box.
            /// </summary>
            public void Close()
            {
                if (Panel.Parent != null)
                {
                    Panel.RemoveFromParent();
                    if (BackgroundFader != null) { BackgroundFader.RemoveFromParent(); }
                    OpenedMsgBoxesCount--;
                }
            }
        }

        /// <summary>
        /// Default size to use for message boxes.
        /// </summary>
        public static Vector2 DefaultMsgBoxSize = new Vector2(480, -1);

        /// <summary>
        /// Default text for OK button.
        /// </summary>
        public static string DefaultOkButtonText = "OK";

        /// <summary>
        /// Will block and fade background with this color while messages are opened.
        /// </summary>
        public static Color BackgroundFaderColor = new Color(0, 0, 0, 100);

        /// <summary>
        /// Count currently opened message boxes.
        /// </summary>
        public static int OpenedMsgBoxesCount
        {
            get; private set;
        } = 0;

        /// <summary>
        /// Get if there's a message box currently opened.
        /// </summary>
        public static bool IsMsgBoxOpened
        {
            get { return OpenedMsgBoxesCount > 0; }
        }

        /// <summary>
        /// A button / option for a message box.
        /// </summary>
        public class MsgBoxOption
        {
            /// <summary>
            /// Option title (for the button).
            /// </summary>
            public string Title;

            /// <summary>
            /// Callback to run when clicked. Return false to leave message box opened (true will close it).
            /// </summary>
            public System.Func<bool> Callback;

            /// <summary>
            /// Create the message box option.
            /// </summary>
            /// <param name="title">Text to write on the button.</param>
            /// <param name="callback">Action when clicked. Return false if you want to abort and leave the message opened, return true to close it.</param>
            public MsgBoxOption(string title, System.Func<bool> callback)
            {
                Title = title;
                Callback = callback;
            }
        }

        /// <summary>
        /// Show a message box with yes/no options.
        /// </summary>
        /// <param name="header">Messagebox header.</param>
        /// <param name="text">Main text.</param>
        /// <param name="onYes">Callback to invoke when clicking yes. Return true to close the messagebox when done.</param>
        /// <param name="onNo">Callback to invoke when clicking no. Return true to close the messagebox when done.</param>
        /// <param name="yesText">Text to use for the 'yes' button.</param>
        /// <param name="noText">Text to use for the 'no' button.</param>
        /// <returns>Message box handle.</returns>
        public static MessageBoxHandle ShowYesNoMsgBox(string header, string text, System.Func<bool> onYes, System.Func<bool> onNo, string yesText = "Yes", string noText = "No")
        {
            return ShowMsgBox(header, text, 
                new MsgBoxOption[] {
                    new MsgBoxOption(yesText, onYes != null ? onYes : () => {return true; }),
                    new MsgBoxOption(noText, onNo != null ? onNo: () => {return true; }),
                });
        }

        /// <summary>
        /// Show a message box with custom buttons and callbacks.
        /// </summary>
        /// <param name="header">Messagebox header.</param>
        /// <param name="text">Main text.</param>
        /// <param name="options">Msgbox response options.</param>
        /// <param name="extraEntities">Optional array of entities to add to msg box under the text and above the buttons.</param>
        /// <param name="size">Alternative size to use.</param>
        /// <param name="onDone">Optional callback to call when this msgbox closes.</param>
        /// <param name="parent">Parent to add message box to (if not defined will use root)</param>
        /// <returns>Message box handle.</returns>
        public static MessageBoxHandle ShowMsgBox(string header, string text, MsgBoxOption[] options, Entities.EntityUI[] extraEntities = null, Vector2? size = null, System.Action onDone = null, Entities.EntityUI parent = null)
        {
            // object to return
            MessageBoxHandle ret = new MessageBoxHandle();

            // create panel for messagebox
            size = size ?? new Vector2(500, -1);
            var panel = new Entities.Panel(size.Value);
            ret.Panel = panel;
            panel.AddChild(new Entities.Header(header));
            panel.AddChild(new Entities.HorizontalLine());
            panel.AddChild(new Entities.RichParagraph(text));

            // add to opened boxes counter
            OpenedMsgBoxesCount++;

            // add rectangle to hide and lock background
            Entities.ColoredRectangle fader = null;
            if (BackgroundFaderColor.A != 0)
            {
                fader = new Entities.ColoredRectangle(Vector2.Zero, Entities.Anchor.Center);
                fader.FillColor = new Color(0, 0, 0, 100);
                fader.OutlineWidth = 0;
                fader.ClickThrough = false;
                UserInterface.Active.AddEntity(fader);
                ret.BackgroundFader = fader;
            }

            // add custom appended entities
            if (extraEntities != null)
            {
                foreach (var entity in extraEntities)
                {
                    panel.AddChild(entity);
                }
            }

            // add bottom buttons panel
            var buttonsPanel = new Entities.Panel(new Vector2(0, 70), 
                Entities.PanelSkin.None, size.Value.Y == -1 ? Entities.Anchor.Auto : Entities.Anchor.BottomCenter);
            buttonsPanel.Padding = Vector2.Zero;
            panel.AddChild(buttonsPanel);
            buttonsPanel.PriorityBonus = -10;

            // add all option buttons
            var btnSize = new Vector2(options.Length == 1 ? 0f : (1f / options.Length), 60);
            foreach (var option in options)
            {
                // add button entity
                var button = new Entities.Button(option.Title, anchor: Entities.Anchor.AutoInline, size: btnSize);

                // set click event
                button.OnClick += (Entities.EntityUI ent) =>
                {
                    // if need to close message box after clicking this button, close it:
                    if (option.Callback == null || option.Callback())
                    {
                        // remove fader and msg box panel
                        if (fader != null) { fader.RemoveFromParent(); }
                        panel.RemoveFromParent();

                        // decrease msg boxes count
                        OpenedMsgBoxesCount--;

                        // call on-done callback
                        onDone?.Invoke();
                    }
                };

                // add button to buttons panel
                buttonsPanel.AddChild(button);
            }

            // add panel to given parent
            if (parent != null)
            {
                parent.AddChild(panel);
            }
            // add panel to active ui root
            else
            {
                UserInterface.Active.AddEntity(panel);
            }
            return ret;
        }

        /// <summary>
        /// Show a message box with just "OK".
        /// </summary>
        /// <param name="header">Message box title.</param>
        /// <param name="text">Main text to write on the message box.</param>
        /// <param name="closeButtonTxt">Text for the closing button (if not provided will use default).</param>
        /// <param name="size">Message box size (if not provided will use default).</param>
        /// <param name="extraEntities">Optional array of entities to add to msg box under the text and above the buttons.</param>
        /// <param name="onDone">Optional callback to call when this msgbox closes.</param>
        /// <returns>Message box panel.</returns>
        public static MessageBoxHandle ShowMsgBox(string header, string text, string closeButtonTxt = null, Vector2? size = null, Entities.EntityUI[] extraEntities = null, System.Action onDone = null)
        {
            return ShowMsgBox(header, text, new MsgBoxOption[]
            {
                new MsgBoxOption(closeButtonTxt ?? DefaultOkButtonText, null)
            }, size: size ?? DefaultMsgBoxSize, extraEntities: extraEntities, onDone: onDone);
        }
    }
}

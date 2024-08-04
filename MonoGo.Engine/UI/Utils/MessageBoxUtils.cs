﻿using Microsoft.Xna.Framework;
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.UI.Defs;
using System;
using System.Collections.Generic;

namespace MonoGo.Engine.UI.Utils
{
    /// <summary>
    /// Utilities to generate message boxes.
    /// </summary>
    public static class MessageBoxUtils
    {
        /// <summary>
        /// Button and action for a message box.
        /// </summary>
        public struct MessageBoxButtons
        {
            public string Text;
            public Action? Action;
            public bool CloseMessageBox;
            public MessageBoxButtons(string text, Action? action = null, bool closeMessageBox = true)
            {
                Text = text;
                Action = action;
                CloseMessageBox = closeMessageBox;
            }
        }

        /// <summary>
        /// Message box options.
        /// </summary>
        public struct MessageBoxOptions
        {
            /// <summary>
            /// If true, message box will adjust height automatically.
            /// </summary>
            public bool AutoHeight;

            /// <summary>
            /// Message box size (note: Y is ignored is AutoHeight is true).
            /// </summary>
            public Point Size;

            /// <summary>
            /// If true, will make the message box draggable.
            /// </summary>
            public bool Draggable;

            /// <summary>
            /// If true, will add backdrop Control to block interaction with background entities.
            /// </summary>
            public bool AddBackdrop;

            public MessageBoxOptions(bool autoHeight, Point size, bool draggable, bool addBackdrop)
            {
                AutoHeight = autoHeight;
                Size = size;
                Draggable = draggable;
                AddBackdrop = addBackdrop;
            }
        }

        /// <summary>
        /// Default message box options to use when options are not provided.
        /// </summary>
        public static MessageBoxOptions DefaultOptions = new MessageBoxOptions(true, new Point(700, 600), true, true);

        /// <summary>
        /// Contains all the created entities for a message box, returned to caller.
        /// </summary>
        public struct MessageBoxHandle
        {
            /// <summary>
            /// Backdrop Control (the part that hides the background).
            /// </summary>
            public Control Backdrop;

            /// <summary>
            /// Message box main panel.
            /// </summary>
            public Panel MessageBoxPanel;

            /// <summary>
            /// An empty Control to contain entities above the bottom buttons.
            /// </summary>
            public Control ContentContainer;

            /// <summary>
            /// Message box buttons.
            /// </summary>
            public Button[] Buttons;

            /// <summary>
            /// Close the message box.
            /// </summary>
            public void Close()
            {
                Backdrop.RemoveSelf();
                MessageBoxPanel.RemoveSelf();
            }
        }

        /// <summary>
        /// Show a message box.
        /// </summary>
        /// <param name="title">Message box title.</param>
        /// <param name="text">Message box text.</param>
        /// <param name="buttons">Message box options.</param>
        /// <param name="options">Message box options, or null to use defaults.</param>
        /// <returns>Newly created message box handle.</returns>
        public static MessageBoxHandle ShowMessageBox(string title, string text, MessageBoxButtons[] buttons, MessageBoxOptions? options = null)
        {
            // get default options
            options = options ?? DefaultOptions;

            // create the message box backdrop
            Control backdrop = null!;
            if (options.Value.AddBackdrop)
            {
                backdrop = new Control(UISystem.DefaultStylesheets.MessageBoxBackdrop);
                backdrop.Size.SetPercents(100f, 100f);
                backdrop.Anchor = Anchor.Center;
                backdrop.Locked = true;
                UISystem.Root.AddChild(backdrop);
            }

            // create the message box panel
            var panel = new Panel(UISystem.DefaultStylesheets.MessageBoxPanels ?? UISystem.DefaultStylesheets.Panels);
            var size = options.Value.Size;
            panel.Size.SetPixels(size.X, size.Y);
            panel.AutoHeight = options.Value.AutoHeight;
            panel.Anchor = Anchor.Center;

            // make draggable
            if (options.Value.Draggable)
            {
                panel.DraggableMode = DraggableMode.DraggableConfinedToScreen;
            }

            // add title
            panel.AddChild(new Title(UISystem.DefaultStylesheets.MessageBoxTitles ?? UISystem.DefaultStylesheets.Titles ?? UISystem.DefaultStylesheets.Paragraphs, title));
            panel.AddChild(new HorizontalLine());

            // add text
            panel.AddChild(new Paragraph(UISystem.DefaultStylesheets.MessageBoxParagraphs ?? UISystem.DefaultStylesheets.Paragraphs, text));
            panel.AddChild(new RowsSpacer());

            // empty container for optional content
            var contentContainer = panel.AddChild(new Control(null));
            contentContainer.Anchor = Anchor.AutoCenter;
            contentContainer.Size.X.SetPercents(100f);
            contentContainer.AutoHeight = true;

            // add buttons
            var optionsPanel = panel.AddChild(new Panel(null!));
            optionsPanel.AutoHeight = true;
            optionsPanel.AutoWidth = true;
            optionsPanel.Size.X.SetPercents(100f);
            optionsPanel.Anchor = Anchor.AutoCenter;
            var buttonWidth = MathF.Floor((float)size.X / buttons.Length) - 20;
            var buttonsList = new List<Button>();
            foreach (var option in buttons)
            {
                var button = optionsPanel.AddChild(new Button(UISystem.DefaultStylesheets.MessageBoxButtons ?? UISystem.DefaultStylesheets.Buttons, option.Text));
                button.Anchor = Anchor.AutoInlineLTR;
                button.Size.X.SetPixels((int)buttonWidth);
                button.Events.OnClick = (ent) =>
                {
                    option.Action?.Invoke();

                    if (option.CloseMessageBox)
                    {
                        panel.RemoveSelf();
                        backdrop?.RemoveSelf();
                    }
                };
                buttonsList.Add(button);
            }

            // add message box panel
            UISystem.Root.AddChild(panel);

            // return handle
            return new MessageBoxHandle()
            {
                Backdrop = backdrop,
                Buttons = buttonsList.ToArray(),
                ContentContainer = contentContainer,
                MessageBoxPanel = panel
            };
        }

        /// <summary>
        /// Show a message box with confirm / cancel buttons.
        /// </summary>
        /// <param name="title">Message box title.</param>
        /// <param name="text">Message box text.</param>
        /// <param name="onConfirm">Action for confirmation.</param>
        /// <param name="onCancel">Action for cancel.</param>
        /// <param name="confirmText">Text to show on confirm button.</param>
        /// <param name="cancelText">Text to show on cancel button.</param>
        /// <param name="options">Message box options, or null to use defaults.</param>
        /// <returns>Newly created message box handle.</returns>
        public static MessageBoxHandle ShowConfirmMessageBox(string title, string text, Action? onConfirm = null, Action? onCancel = null, string confirmText = "Confirm", string cancelText = "Cancel", MessageBoxOptions? options = null)
        {
            return ShowMessageBox(title, text, new MessageBoxButtons[]
            {
                new MessageBoxButtons(confirmText, onConfirm),
                new MessageBoxButtons(cancelText, onCancel)
            }, options);
        }
    }
}

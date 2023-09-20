#region File Description
//-----------------------------------------------------------------------------
// This file pre-load and hold all the resources (textures, fonts, etc..) that
// Monofoxe.Extended.UI needs. If you edit and add new files to content, you probably
// need to update this file as well.
//
// Author: Ronen Ness.
// Since: 2016.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using Monofoxe.Extended.UI.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using GeonBit.UI.DataTypes;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.Resources;
using System.IO;

namespace Monofoxe.Extended.UI
{
    /// <summary>
    /// A class to get texture with index and constant path part.
    /// Used internally.
    /// </summary>
    public class TexturesGetter<TEnum> where TEnum : Enum, IConvertible
    {
        // textures we already loaded
        Texture2D[] _loadedTextures;

        /// <summary>
        /// Get texture for enum state.
        /// This is for textures that don't have different states, like mouse hover, down, or default.
        /// </summary>
        /// <param name="i">Texture enum identifier.</param>
        /// <returns>Loaded texture.</returns>
        public Texture2D this[TEnum i]
        {
            // get texture for a given type
            get
            {
                int indx = GetIndex(i);
                if (_loadedTextures[indx] == null)
                {
                    var path = $"{_basepath}{EnumToString(i)}{_suffix}";
                    try
                    {
                        _loadedTextures[indx] = ResourceHub.GetResource<Sprite>("GeonBitSprites", path)[0].Texture;
                    }
                    catch (Microsoft.Xna.Framework.Content.ContentLoadException)
                    {
                        // for backward compatibility when alternative was called 'golden'
                        if (i.ToString() == PanelSkin.Alternative.ToString())
                        {
                            path = $"{_basepath}Golden{_suffix}";
                            _loadedTextures[indx] = ResourceHub.GetResource<Sprite>("GeonBitSprites", path)[0].Texture;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return _loadedTextures[indx];
            }

            // force-override texture for a given type
            set
            {
                int indx = GetIndex(i);
                _loadedTextures[indx] = value;
            }
        }

        /// <summary>
        /// Get texture for enum state and entity state.
        /// This is for textures that don't have different states, like mouse hover, down, or default.
        /// </summary>
        /// <param name="i">Texture enum identifier.</param>
        /// <param name="s">Entity state to get texture for.</param>
        /// <returns>Loaded texture.</returns>
        public Texture2D this[TEnum i, EntityState s]
        {
            // get texture for a given type and state
            get
            {
                int indx = GetIndex(i, s);
                if (_loadedTextures[indx] == null)
                {
                    var path = _basepath + EnumToString(i) + _suffix + StateEnumToString(s);
                    _loadedTextures[indx] = Resources._content.Load<Texture2D>(path);
                }
                return _loadedTextures[indx];
            }

            // force-override texture for a given type and state
            set
            {
                int indx = GetIndex(i, s);
                _loadedTextures[indx] = value;
            }
        }

        /// <summary>
        /// Get index from enum type with optional state.
        /// </summary>
        private int GetIndex(TEnum i, EntityState? s = null)
        {
            if (s != null)
                return Convert.ToInt32(i) + (_typesCount * (int)s);
            return Convert.ToInt32(i);
        }

        /// <summary>
        /// Convert enum to its string for filename.
        /// </summary>
        private string EnumToString(TEnum e)
        {
            // entity state enum
            if (typeof(TEnum) == typeof(EntityState))
            {
                return StateEnumToString((EntityState)(object)e);
            }

            // icon type enum
            if (typeof(TEnum) == typeof(IconType))
            {
                return e.ToString();
            }

            // all other type of enums
            return e.ToString();
        }

        /// <summary>
        /// Convert entity state enum to string.
        /// </summary>
        private string StateEnumToString(EntityState e)
        {
            switch (e)
            {
                case EntityState.MouseDown:
                    return "_Down";
                case EntityState.MouseHover:
                    return "_Hover";
                case EntityState.Default:
                    return string.Empty;
            }
            return null;
        }

        // base path of textures to load (index will be appended to them).
        string _basepath;

        // suffix to add to the end of texture path
        string _suffix;

        // do we use states like down / hover / default for these textures?
        bool _usesStates;

        // textures types count
        int _typesCount;

        /// <summary>
        /// Create the texture getter with base path.
        /// </summary>
        /// <param name="path">Resource path, under Monofoxe.Extended.UI content.</param>
        /// <param name="suffix">Suffix to add to the texture path after the enum part.</param>
        /// <param name="usesStates">If true, it means these textures may also use entity states, eg mouse hover / down / default.</param>
        public TexturesGetter(string path, string suffix = null, bool usesStates = true)
        {
            _basepath = path;
            _suffix = suffix ?? string.Empty;
            _usesStates = usesStates;
            _typesCount = Enum.GetValues(typeof(TEnum)).Length;
            _loadedTextures = new Texture2D[usesStates ? _typesCount * 3 : _typesCount];
        }
    }

    /// <summary>
    /// A static class to init and store all UI resources (textures, effects, fonts, etc.)
    /// </summary>
    public static class Resources
    {

        /// <summary>Lookup for char > string conversion</summary>
        private static Dictionary<char, string> charStringDict = new Dictionary<char, string>();

        /// <summary>Just a plain white texture, used internally.</summary>
        public static Texture2D WhiteTexture;

        /// <summary>Cursor textures.</summary>
        public static TexturesGetter<CursorType> Cursors = new TexturesGetter<CursorType>("Cursor_");

        /// <summary>Metadata about cursor textures.</summary>
        public static CursorTextureData[] CursorsData;

        /// <summary>All panel skin textures.</summary>
        public static TexturesGetter<PanelSkin> PanelTextures = new TexturesGetter<PanelSkin>("Panel_");

        /// <summary>Metadata about panel textures.</summary>
        public static TextureData[] PanelData;

        /// <summary>Button textures (accessed as [skin, state]).</summary>
        public static TexturesGetter<ButtonSkin> ButtonTextures = new TexturesGetter<ButtonSkin>("Button_");

        /// <summary>Metadata about button textures.</summary>
        public static TextureData[] ButtonData;

        /// <summary>CheckBox textures.</summary>
        public static TexturesGetter<EntityState> CheckBoxTextures = new TexturesGetter<EntityState>("Checkbox");

        /// <summary>Radio button textures.</summary>
        public static TexturesGetter<EntityState> RadioTextures = new TexturesGetter<EntityState>("Radio");

        /// <summary>ProgressBar texture.</summary>
        public static Texture2D ProgressBarTexture;

        /// <summary>Metadata about progressbar texture.</summary>
        public static TextureData ProgressBarData;

        /// <summary>ProgressBar fill texture.</summary>
        public static Texture2D ProgressBarFillTexture;

        /// <summary>HorizontalLine texture.</summary>
        public static Texture2D HorizontalLineTexture;

        /// <summary>Sliders base textures.</summary>
        public static TexturesGetter<SliderSkin> SliderTextures = new TexturesGetter<SliderSkin>("Slider_");

        /// <summary>Sliders mark textures (the sliding piece that shows current value).</summary>
        public static TexturesGetter<SliderSkin> SliderMarkTextures = new TexturesGetter<SliderSkin>("Slider_", "_Mark");

        /// <summary>Metadata about slider textures.</summary>
        public static TextureData[] SliderData;

        /// <summary>All icon textures.</summary>
        public static TexturesGetter<IconType> IconTextures = new TexturesGetter<IconType>("Icons/");

        /// <summary>Icons inventory background texture.</summary>
        public static Texture2D IconBackgroundTexture;

        /// <summary>Vertical scrollbar base texture.</summary>
        public static Texture2D VerticalScrollbarTexture;

        /// <summary>Vertical scrollbar mark texture.</summary>
        public static Texture2D VerticalScrollbarMarkTexture;

        /// <summary>Metadata about scrollbar texture.</summary>
        public static TextureData VerticalScrollbarData;

        /// <summary>Arrow-down texture (used in dropdown).</summary>
        public static Texture2D ArrowDown;

        /// <summary>Arrow-up texture (used in dropdown).</summary>
        public static Texture2D ArrowUp;

        /// <summary>Default font types.</summary>
        public static SpriteFont[] Fonts;

        /// <summary>Effect for disabled entities (greyscale).</summary>
        public static Effect DisabledEffect;

        /// <summary>An effect to draw just a silhouette of the texture.</summary>
        public static Effect SilhouetteEffect;

        /// <summary>Store the content manager instance</summary>
        internal static ContentManager _content;

        /// <summary>
        /// Load all Monofoxe.Extended.UI resources.
        /// </summary>
        /// <param name="content">Content manager to use.</param>
        /// <param name="theme">Which theme to load resources from.</param>
        static public void LoadContent(ContentManager content, string theme = "default")
        {
            InitialiseCharStringDict();

            _content = content;

            // set Texture2D static fields
            HorizontalLineTexture = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Horizontal_Line")[0].Texture;
            WhiteTexture = ResourceHub.GetResource<Sprite>("GeonBitSprites", "White_Texture")[0].Texture;
            IconBackgroundTexture = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Background")[0].Texture;
            VerticalScrollbarTexture = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Scrollbar")[0].Texture;
            VerticalScrollbarMarkTexture = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Scrollbar_Mark")[0].Texture;
            ArrowDown = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Arrow_Down")[0].Texture;
            ArrowUp = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Arrow_Up")[0].Texture;
            ProgressBarTexture = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Progressbar")[0].Texture;
            ProgressBarFillTexture = ResourceHub.GetResource<Sprite>("GeonBitSprites", "Progressbar_Fill")[0].Texture;

            // load cursors metadata
            CursorsData = new CursorTextureData[Enum.GetValues(typeof(CursorType)).Length];
            foreach (CursorType cursor in Enum.GetValues(typeof(CursorType)))
            {
                string cursorName = cursor.ToString();
                CursorsData[(int)cursor] = content.Load<CursorTextureData>("Cursor_" + cursorName + "_md");
            }

            // load panels
            PanelData = new TextureData[Enum.GetValues(typeof(PanelSkin)).Length];
            foreach (PanelSkin skin in Enum.GetValues(typeof(PanelSkin)))
            {
                // skip none panel skin
                if (skin == PanelSkin.None)
                {
                    continue;
                }

                // load panels metadata
                string skinName = skin.ToString();
                try
                {
                    PanelData[(int)skin] = content.Load<TextureData>("Panel_" + skinName + "_md");
                }
                catch (Microsoft.Xna.Framework.Content.ContentLoadException ex)
                {
                    // for backwards compatability from when it was called 'Golden'.
                    if (skin == PanelSkin.Alternative)
                    {
                        PanelData[(int)skin] = content.Load<TextureData>("Panel_Golden_md");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            // load scrollbar metadata
            VerticalScrollbarData = content.Load<TextureData>("Scrollbar_md");

            // load slider metadata
            SliderData = new TextureData[Enum.GetValues(typeof(SliderSkin)).Length];
            foreach (SliderSkin skin in Enum.GetValues(typeof(SliderSkin)))
            {
                string skinName = skin.ToString();
                SliderData[(int)skin] = content.Load<TextureData>("Slider_" + skinName + "_md");
            }

            // load fonts
            Fonts = new SpriteFont[Enum.GetValues(typeof(FontStyle)).Length];
            foreach (FontStyle style in Enum.GetValues(typeof(FontStyle)))
            {
                Fonts[(int)style] = ResourceHub.GetResource<IFont>("Fonts", style.ToString()).SpriteFont;
                Fonts[(int)style].LineSpacing += 2;
            }

            // load buttons metadata
            ButtonData = new TextureData[Enum.GetValues(typeof(ButtonSkin)).Length];
            foreach (ButtonSkin skin in Enum.GetValues(typeof(ButtonSkin)))
            {
                string skinName = skin.ToString();
                ButtonData[(int)skin] = content.Load<TextureData>("Button_" + skinName + "_md");
            }

            // load progress bar metadata
            ProgressBarData = content.Load<TextureData>("Progressbar_md");

            // load effects
            DisabledEffect = ResourceHub.GetResource<Effect>("Effects", "Disabled");
            SilhouetteEffect = ResourceHub.GetResource<Effect>("Effects", "Silhouette");

            // load default styleSheets
            LoadDefaultStyles( EntityUI.DefaultStyle, "Entity", content);
            LoadDefaultStyles( Paragraph.DefaultStyle, "Paragraph", content);
            LoadDefaultStyles( Button.DefaultStyle, "Button", content);
            LoadDefaultStyles( Button.DefaultParagraphStyle, "ButtonParagraph", content);
            LoadDefaultStyles( CheckBox.DefaultStyle, "CheckBox", content);
            LoadDefaultStyles( CheckBox.DefaultParagraphStyle, "CheckBoxParagraph", content);
            LoadDefaultStyles( ColoredRectangle.DefaultStyle, "ColoredRectangle", content);
            LoadDefaultStyles( DropDown.DefaultStyle, "DropDown", content);
            LoadDefaultStyles( DropDown.DefaultParagraphStyle, "DropDownParagraph", content);
            LoadDefaultStyles( DropDown.DefaultSelectedParagraphStyle, "DropDownSelectedParagraph", content);
            LoadDefaultStyles( Header.DefaultStyle, "Header", content);
            LoadDefaultStyles( HorizontalLine.DefaultStyle, "HorizontalLine", content);
            LoadDefaultStyles( Icon.DefaultStyle, "Icon", content);
            LoadDefaultStyles( Image.DefaultStyle, "Image", content);
            LoadDefaultStyles( Label.DefaultStyle, "Label", content);
            LoadDefaultStyles( Panel.DefaultStyle, "Panel", content);
            LoadDefaultStyles( ProgressBar.DefaultStyle, "ProgressBar", content);
            LoadDefaultStyles( ProgressBar.DefaultFillStyle, "ProgressBarFill", content);
            LoadDefaultStyles( RadioButton.DefaultStyle, "RadioButton", content);
            LoadDefaultStyles( RadioButton.DefaultParagraphStyle, "RadioButtonParagraph", content);
            LoadDefaultStyles( SelectList.DefaultStyle, "SelectList", content);
            LoadDefaultStyles( SelectList.DefaultParagraphStyle, "SelectListParagraph", content);
            LoadDefaultStyles( Slider.DefaultStyle, "Slider", content);
            LoadDefaultStyles( TextInput.DefaultStyle, "TextInput", content);
            LoadDefaultStyles( TextInput.DefaultParagraphStyle, "TextInputParagraph", content);
            LoadDefaultStyles( TextInput.DefaultPlaceholderStyle, "TextInputPlaceholder", content);
            LoadDefaultStyles( VerticalScrollbar.DefaultStyle, "VerticalScrollbar", content);
            LoadDefaultStyles( PanelTabs.DefaultButtonStyle, "PanelTabsButton", content);
            LoadDefaultStyles( PanelTabs.DefaultButtonParagraphStyle, "PanelTabsButtonParagraph", content);
        }


        /// <summary>
        /// Creates Dictionary containing char > string lookup
        /// </summary>
        private static void InitialiseCharStringDict()
        {
            charStringDict.Clear();

            var asciiValues = Enumerable.Range('\x1', 127).ToArray();

            for (var i = 0; i < asciiValues.Length; i++)
            {
                var c = (char)asciiValues[i];
                charStringDict.Add(c, c.ToString());
            }
        }

        /// <summary>
        /// Returns string from char > string lookup
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetStringForChar(char c)
        {
            if (!charStringDict.ContainsKey(c)) { return c.ToString(); }
            return charStringDict[c];
        }

        /// <summary>
        /// Load xml styles either directly from xml file, or from the content manager.
        /// </summary>
        /// <param name="name">XML name.</param>
        /// <param name="content">Content manager.</param>
        /// <returns>Default styles loaded from xml or xnb.</returns>
        private static DefaultStyles LoadXmlStyles(string name, ContentManager content)
        {
            // try to load xml directly from full path
            string fullPath = System.IO.Path.Combine(content.RootDirectory, name + ".xml");
            if (System.IO.File.Exists(fullPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DefaultStyles));
                using (var reader = System.IO.File.OpenText(fullPath))
                {
                    XmlDeserializationEvents eventsHandler = new XmlDeserializationEvents()
                    {
                        OnUnknownAttribute = (object sender, XmlAttributeEventArgs e) => { throw new System.Exception("Error parsing file '" + fullPath + "': invalid attribute '" + e.Attr.Name + "' at line " + e.LineNumber); },
                        OnUnknownElement = (object sender, XmlElementEventArgs e) => { throw new System.Exception("Error parsing file '" + fullPath + "': invalid element '" + e.Element.Name + "' at line " + e.LineNumber); },
                        OnUnknownNode = (object sender, XmlNodeEventArgs e) => { throw new System.Exception("Error parsing file '" + fullPath + "': invalid element '" + e.Name + "' at line " + e.LineNumber); },
                        OnUnreferencedObject = (object sender, UnreferencedObjectEventArgs e) => { throw new System.Exception("Error parsing file '" + fullPath + "': unreferenced object '" + e.UnreferencedObject.ToString() + "'"); },
                    };
                    return (DefaultStyles)serializer.Deserialize(System.Xml.XmlReader.Create(reader), eventsHandler);
                }
            }

            // if xml file not found, try to load xnb instead
            return content.Load<DefaultStyles>(name);
        }

        /// <summary>
        /// Load default stylesheets for a given entity name and put values inside the sheet.
        /// </summary>
        /// <param name="sheet">StyleSheet to load.</param>
        /// <param name="entityName">Entity unique identifier for file names.</param>
        /// <param name="themeRoot">Path of the current theme root directory.</param>
        /// <param name="content">Content manager to allow us to load xmls.</param>
        private static void LoadDefaultStyles(StyleSheet sheet, string entityName, ContentManager content)
        {
            // load default styles
            FillDefaultStyles(sheet, EntityState.Default, LoadXmlStyles($"{entityName}-Default", content));

            // load mouse-hover styles
            FillDefaultStyles(sheet, EntityState.MouseHover, LoadXmlStyles($"{entityName}-MouseHover", content));

            // load mouse-down styles
            FillDefaultStyles(sheet, EntityState.MouseDown, LoadXmlStyles($"{entityName}-MouseDown", content));
        }

        /// <summary>
        /// Fill a set of default styles into a given stylesheet.
        /// </summary>
        /// <param name="sheet">StyleSheet to fill.</param>
        /// <param name="state">State to fill values for.</param>
        /// <param name="styles">Default styles, as loaded from xml file.</param>
        private static void FillDefaultStyles(StyleSheet sheet, EntityState state, DefaultStyles styles)
        {
            if (styles.FillColor != null) { sheet[$"{state}.FillColor"] = new StyleProperty((Color)styles.FillColor); }
            if (styles.FontStyle != null) { sheet[$"{state}.FontStyle"] = new StyleProperty((int)styles.FontStyle); }
            if (styles.ForceAlignCenter != null) { sheet[$"{state}.ForceAlignCenter"] = new StyleProperty((bool)styles.ForceAlignCenter); }
            if (styles.OutlineColor != null) { sheet[$"{state}.OutlineColor"] = new StyleProperty((Color)styles.OutlineColor); }
            if (styles.OutlineWidth != null) { sheet[$"{state}.OutlineWidth"] = new StyleProperty((int)styles.OutlineWidth); }
            if (styles.Scale != null) { sheet[$"{state}.Scale"] = new StyleProperty((float)styles.Scale); }
            if (styles.SelectedHighlightColor != null) { sheet[$"{state}.SelectedHighlightColor"] = new StyleProperty((Color)styles.SelectedHighlightColor); }
            if (styles.ShadowColor != null) { sheet[$"{state}.ShadowColor"] = new StyleProperty((Color)styles.ShadowColor); }
            if (styles.ShadowOffset != null) { sheet[$"{state}.ShadowOffset"] = new StyleProperty((Vector2)styles.ShadowOffset); }
            if (styles.Padding != null) { sheet[$"{state}.Padding"] = new StyleProperty((Vector2)styles.Padding); }
            if (styles.SpaceBefore != null) { sheet[$"{state}.SpaceBefore"] = new StyleProperty((Vector2)styles.SpaceBefore); }
            if (styles.SpaceAfter != null) { sheet[$"{state}.SpaceAfter"] = new StyleProperty((Vector2)styles.SpaceAfter); }
            if (styles.ShadowScale != null) { sheet[$"{state}.ShadowScale"] = new StyleProperty((float)styles.ShadowScale); }
            if (styles.DefaultSize != null) { sheet[$"{state}.DefaultSize"] = new StyleProperty((Vector2)styles.DefaultSize); }
        }
    }
}

#region File Description
//-----------------------------------------------------------------------------
// This file pre-load and hold all the resources (textures, fonts, etc..) that
// MonoGo.Engine.UI needs. If you edit and add new files to content, you probably
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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using MonoGo.Engine.UI.Entities;
using MonoGo.Engine.UI.DataTypes;

namespace MonoGo.Engine.UI
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
                    var path = $"{Resources.Instance._root}{_basepath}{EnumToString(i)}{_suffix}";
                    try
                    {
                        _loadedTextures[indx] = ResourceHub.GetResource<Sprite>("GUISprites", path)[0].Texture;
                    }
                    catch (ContentLoadException)
                    {
                        // for backward compatibility when alternative was called 'golden'
                        if (i.ToString() == PanelSkin.Golden.ToString())
                        {
                            path = $"{Resources.Instance._root}{_basepath}Golden{_suffix}";
                            _loadedTextures[indx] = ResourceHub.GetResource<Sprite>("GUISprites", path)[0].Texture;
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
                    var path = Resources.Instance._root + _basepath + EnumToString(i) + _suffix + StateEnumToString(s);
                    _loadedTextures[indx] = ResourceHub.GetResource<Sprite>("GUISprites", path)[0].Texture;
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

        // textures types count
        int _typesCount;

        /// <summary>
        /// Create the texture getter with base path.
        /// </summary>
        /// <param name="path">Resource path, under MonoGo.Engine.UI content.</param>
        /// <param name="suffix">Suffix to add to the texture path after the enum part.</param>
        /// <param name="usesStates">If true, it means these textures may also use entity states, eg mouse hover / down / default.</param>
        public TexturesGetter(string path, string suffix = null, bool usesStates = true)
        {
            _basepath = path;
            _suffix = suffix ?? string.Empty;
            _typesCount = Enum.GetValues(typeof(TEnum)).Length;
            _loadedTextures = new Texture2D[usesStates ? _typesCount * 3 : _typesCount];
        }
    }

    /// <summary>
    /// A class to init and store all UI resources (textures, effects, fonts, etc.)
    /// </summary>
    public class Resources
    {
        /// <summary>
        /// Resources singleton instance.
        /// </summary>
        public static Resources Instance { get; private set; }

        /// <summary>
        /// Reset resources manager.
        /// </summary>
        public static void Reset()
        {
            Instance = new Resources();
        }

        /// <summary>Lookup for char > string conversion</summary>
        private Dictionary<char, string> charStringDict = new Dictionary<char, string>();

        /// <summary>Just a plain white texture, used internally.</summary>
        public Texture2D WhiteTexture;

        /// <summary>Cursor textures.</summary>
        public TexturesGetter<CursorType> Cursors = new TexturesGetter<CursorType>("Cursor_");

        /// <summary>Metadata about cursor textures.</summary>
        public CursorTextureData[] CursorsData;

        /// <summary>All panel skin textures.</summary>
        public TexturesGetter<PanelSkin> PanelTextures = new TexturesGetter<PanelSkin>("Panel_");

        /// <summary>Metadata about panel textures.</summary>
        public TextureData[] PanelData;

        /// <summary>Button textures (accessed as [skin, state]).</summary>
        public TexturesGetter<ButtonSkin> ButtonTextures = new TexturesGetter<ButtonSkin>("Button_");

        /// <summary>Metadata about button textures.</summary>
        public TextureData[] ButtonData;

        /// <summary>CheckBox textures.</summary>
        public TexturesGetter<EntityState> CheckBoxTextures = new TexturesGetter<EntityState>("Checkbox");

        /// <summary>Radio button textures.</summary>
        public TexturesGetter<EntityState> RadioTextures = new TexturesGetter<EntityState>("Radio");

        /// <summary>ProgressBar texture.</summary>
        public Texture2D ProgressBarTexture;

        /// <summary>Metadata about progressbar texture.</summary>
        public TextureData ProgressBarData;

        /// <summary>ProgressBar fill texture.</summary>
        public Texture2D ProgressBarFillTexture;

        /// <summary>HorizontalLine texture.</summary>
        public Texture2D HorizontalLineTexture;

        /// <summary>Sliders base textures.</summary>
        public TexturesGetter<SliderSkin> SliderTextures = new TexturesGetter<SliderSkin>("Slider_");

        /// <summary>Sliders mark textures (the sliding piece that shows current value).</summary>
        public TexturesGetter<SliderSkin> SliderMarkTextures = new TexturesGetter<SliderSkin>("Slider_", "_Mark");

        /// <summary>Metadata about slider textures.</summary>
        public TextureData[] SliderData;

        /// <summary>All icon textures.</summary>
        public TexturesGetter<IconType> IconTextures = new TexturesGetter<IconType>("");

        /// <summary>Icons inventory background texture.</summary>
        public Texture2D IconBackgroundTexture;

        /// <summary>Vertical scrollbar base texture.</summary>
        public Texture2D VerticalScrollbarTexture;

        /// <summary>Vertical scrollbar mark texture.</summary>
        public Texture2D VerticalScrollbarMarkTexture;

        /// <summary>Metadata about scrollbar texture.</summary>
        public TextureData VerticalScrollbarData;

        /// <summary>Arrow-down texture (used in dropdown).</summary>
        public Texture2D ArrowDown;

        /// <summary>Arrow-up texture (used in dropdown).</summary>
        public Texture2D ArrowUp;

        /// <summary>Default font types.</summary>
        public SpriteFont[] Fonts;

        /// <summary>Effect for disabled entities (greyscale).</summary>
        public Effect DisabledEffect;

        /// <summary>An effect to draw just a silhouette of the texture.</summary>
        public Effect SilhouetteEffect;

        /// <summary>Store the content manager instance</summary>
        internal ContentManager _content;

        /// <summary>Root for MonoGo.Engine.UI content</summary>
        internal string _root;

        /// <summary>
        /// Load all MonoGo.Engine.UI resources.
        /// </summary>
        /// <param name="content">Content manager to use.</param>
        public void LoadContent(ContentManager content)
        {
            InitialiseCharStringDict();

            _content = content;

            // set Texture2D fields
            HorizontalLineTexture = ResourceHub.GetResource<Sprite>("GUISprites", "Horizontal_Line")[0].Texture;
            WhiteTexture = ResourceHub.GetResource<Sprite>("GUISprites", "White_Texture")[0].Texture;
            IconBackgroundTexture = ResourceHub.GetResource<Sprite>("GUISprites", "Background")[0].Texture;
            VerticalScrollbarTexture = ResourceHub.GetResource<Sprite>("GUISprites", "Scrollbar")[0].Texture;
            VerticalScrollbarMarkTexture = ResourceHub.GetResource<Sprite>("GUISprites", "Scrollbar_Mark")[0].Texture;
            ArrowDown = ResourceHub.GetResource<Sprite>("GUISprites", "Arrow_Down")[0].Texture;
            ArrowUp = ResourceHub.GetResource<Sprite>("GUISprites", "Arrow_Up")[0].Texture;
            ProgressBarTexture = ResourceHub.GetResource<Sprite>("GUISprites", "Progressbar")[0].Texture;
            ProgressBarFillTexture = ResourceHub.GetResource<Sprite>("GUISprites", "Progressbar_Fill")[0].Texture;

            // load cursors metadata
            CursorsData = new CursorTextureData[Enum.GetValues(typeof(CursorType)).Length];
            foreach (CursorType cursor in Enum.GetValues(typeof(CursorType)))
            {
                string cursorName = cursor.ToString();
                CursorsData[(int)cursor] = LoadXmlCursorData("Cursor_" + cursorName + "_md");
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
                    PanelData[(int)skin] = LoadXmlTextureData("Panel_" + skinName + "_md");
                }
                catch (ContentLoadException ex)
                {
                    // for backwards compatability from when it was called 'Golden'.
                    if (skin == PanelSkin.Golden)
                    {
                        PanelData[(int)skin] = LoadXmlTextureData("Panel_Golden_md");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            // load scrollbar metadata
            VerticalScrollbarData = LoadXmlTextureData("Scrollbar_md");

            // load slider metadata
            SliderData = new TextureData[Enum.GetValues(typeof(SliderSkin)).Length];
            foreach (SliderSkin skin in Enum.GetValues(typeof(SliderSkin)))
            {
                string skinName = skin.ToString();
                SliderData[(int)skin] = LoadXmlTextureData("Slider_" + skinName + "_md");
            }

            // load fonts
            Fonts = new SpriteFont[Enum.GetValues(typeof(FontStyle)).Length];
            foreach (FontStyle style in Enum.GetValues(typeof(FontStyle)))
            {
                Fonts[(int)style] = ResourceHub.GetResource<IFont>("Fonts", style.ToString()).SpriteFont;
            }

            // load buttons metadata
            ButtonData = new TextureData[Enum.GetValues(typeof(ButtonSkin)).Length];
            foreach (ButtonSkin skin in Enum.GetValues(typeof(ButtonSkin)))
            {
                string skinName = skin.ToString();
                ButtonData[(int)skin] = LoadXmlTextureData("Button_" + skinName + "_md");
            }

            // load progress bar metadata
            ProgressBarData = LoadXmlTextureData("Progressbar_md");

            // load effects
            DisabledEffect = ResourceHub.GetResource<Effect>("Effects", "Disabled");
            SilhouetteEffect = ResourceHub.GetResource<Effect>("Effects", "Silhouette");

            // load default styleSheets
            LoadDefaultStyles( EntityUI.DefaultStyle, "Entity");
            LoadDefaultStyles( Paragraph.DefaultStyle, "Paragraph");
            LoadDefaultStyles( Button.DefaultStyle, "Button");
            LoadDefaultStyles( Button.DefaultParagraphStyle, "ButtonParagraph");
            LoadDefaultStyles( CheckBox.DefaultStyle, "CheckBox");
            LoadDefaultStyles( CheckBox.DefaultParagraphStyle, "CheckBoxParagraph");
            LoadDefaultStyles( ColoredRectangle.DefaultStyle, "ColoredRectangle");
            LoadDefaultStyles( DropDown.DefaultStyle, "DropDown");
            try
            {
                LoadDefaultStyles(DropDown.DefaultSelectedPanelStyle, "DropDownSelectedPanel");
            }
            catch (ContentLoadException)
            {
                LoadDefaultStyles(DropDown.DefaultSelectedPanelStyle, "Panel");
                DropDown.DefaultSelectedPanelStyle.SetStyleProperty("DefaultSize", new StyleProperty(Vector2.Zero));
            }
            LoadDefaultStyles(DropDown.DefaultParagraphStyle, "DropDownParagraph");
            LoadDefaultStyles(DropDown.DefaultSelectedParagraphStyle, "DropDownSelectedParagraph");
            LoadDefaultStyles(Header.DefaultStyle, "Header");
            LoadDefaultStyles(HorizontalLine.DefaultStyle, "HorizontalLine");
            LoadDefaultStyles(Icon.DefaultStyle, "Icon");
            LoadDefaultStyles(Image.DefaultStyle, "Image");
            LoadDefaultStyles(Label.DefaultStyle, "Label");
            LoadDefaultStyles(Panel.DefaultStyle, "Panel");
            LoadDefaultStyles(ProgressBar.DefaultStyle, "ProgressBar");
            LoadDefaultStyles(ProgressBar.DefaultFillStyle, "ProgressBarFill");
            LoadDefaultStyles(RadioButton.DefaultStyle, "RadioButton");
            LoadDefaultStyles(RadioButton.DefaultParagraphStyle, "RadioButtonParagraph");
            LoadDefaultStyles(SelectList.DefaultStyle, "SelectList");
            LoadDefaultStyles(SelectList.DefaultParagraphStyle, "SelectListParagraph");
            LoadDefaultStyles(Slider.DefaultStyle, "Slider");
            LoadDefaultStyles(TextInput.DefaultStyle, "TextInput");
            LoadDefaultStyles(TextInput.DefaultParagraphStyle, "TextInputParagraph");
            LoadDefaultStyles(TextInput.DefaultPlaceholderStyle, "TextInputPlaceholder");
            LoadDefaultStyles(VerticalScrollbar.DefaultStyle, "VerticalScrollbar");
            LoadDefaultStyles(PanelTabs.DefaultButtonStyle, "PanelTabsButton");
            LoadDefaultStyles(PanelTabs.DefaultButtonParagraphStyle, "PanelTabsButtonParagraph");
        }


        /// <summary>
        /// Creates Dictionary containing char > string lookup
        /// </summary>
        private void InitialiseCharStringDict()
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
        public string GetStringForChar(char c)
        {
            if (!charStringDict.ContainsKey(c)) { return c.ToString(); }
            return charStringDict[c];
        }
        /// <summary>
        /// Load xml file either directly from xml file, or from the content manager.
        /// </summary>
        /// <param name="name">XML file name.</param>
        /// <returns>T instance loaded from xml file or content manager.</returns>
        private T LoadXml<T>(string name) where T : new()
        {
            // try to load xml directly from full path
            string fullPath = Path.Combine(_content.RootDirectory, name + ".xml");
            if (File.Exists(fullPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var reader = File.OpenText(fullPath))
                {
                    XmlDeserializationEvents eventsHandler = new XmlDeserializationEvents()
                    {
                        OnUnknownAttribute = (object sender, XmlAttributeEventArgs e) => { throw new Exception("Error parsing file '" + fullPath + "': invalid attribute '" + e.Attr.Name + "' at line " + e.LineNumber); },
                        OnUnknownElement = (object sender, XmlElementEventArgs e) => { throw new Exception("Error parsing file '" + fullPath + "': invalid element '" + e.Element.Name + "' at line " + e.LineNumber); },
                        OnUnknownNode = (object sender, XmlNodeEventArgs e) => { throw new Exception("Error parsing file '" + fullPath + "': invalid element '" + e.Name + "' at line " + e.LineNumber); },
                        OnUnreferencedObject = (object sender, UnreferencedObjectEventArgs e) => { throw new Exception("Error parsing file '" + fullPath + "': unreferenced object '" + e.UnreferencedObject.ToString() + "'"); },
                    };
                    return (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader), eventsHandler);
                }
            }

            // if xml file not found, try to load xnb instead
            var ret = _content.Load<T>(name);
            return ret;
        }

        /// <summary>
        /// Load xml texture data either directly from xml file, or from the content manager.
        /// </summary>
        /// <param name="name">XML name.</param>
        /// <returns>Texture data loaded from xml or xnb.</returns>
        private TextureData LoadXmlTextureData(string name)
        {
            return LoadXml<TextureData>(name);
        }

        /// <summary>
        /// Load xml cursor data either directly from xml file, or from the content manager.
        /// </summary>
        /// <param name="name">XML name.</param>
        /// <returns>Cursor texture data loaded from xml or xnb.</returns>
        private CursorTextureData LoadXmlCursorData(string name)
        {
            return LoadXml<CursorTextureData>(name);
        }

        /// <summary>
        /// Load xml styles either directly from xml file, or from the content manager.
        /// </summary>
        /// <param name="name">XML name.</param>
        /// <returns>Default styles loaded from xml or xnb.</returns>
        private DefaultStyles LoadXmlStyles(string name)
        {
            return LoadXml<DefaultStyles>(name);
        }

        /// <summary>
        /// Load default stylesheets for a given entity name and put values inside the sheet.
        /// </summary>
        /// <param name="sheet">StyleSheet to load.</param>
        /// <param name="entityName">Entity unique identifier for file names.</param>
        private void LoadDefaultStyles(StyleSheet sheet, string entityName)
        {
            // load default styles
            FillDefaultStyles(sheet, EntityState.Default, LoadXmlStyles($"{entityName}-Default"));

            // load mouse-hover styles
            FillDefaultStyles(sheet, EntityState.MouseHover, LoadXmlStyles($"{entityName}-MouseHover"));

            // load mouse-down styles
            FillDefaultStyles(sheet, EntityState.MouseDown, LoadXmlStyles($"{entityName}-MouseDown"));
        }

        /// <summary>
        /// Load texture from path.
        /// </summary>
        /// <param name="path">Texture path, under theme folder.</param>
        /// <returns>Texture instance.</returns>
        public Texture2D LoadTexture(string path)
        {
            return ResourceHub.GetResource<Sprite>("GUISprites", path)[0].Texture;
        }

        /// <summary>
        /// Fill a set of default styles into a given stylesheet.
        /// </summary>
        /// <param name="sheet">StyleSheet to fill.</param>
        /// <param name="state">State to fill values for.</param>
        /// <param name="styles">Default styles, as loaded from xml file.</param>
        private void FillDefaultStyles(StyleSheet sheet, EntityState state, DefaultStyles styles)
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

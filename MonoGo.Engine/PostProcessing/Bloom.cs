using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;

namespace MonoGo.Engine.PostProcessing
{
    public enum BloomPresets
    {
        Wide,
        WeakWide,
        GlareWide,
        SuperWide,
        Focussed,
        Small,
        Cheap,
        One
    };

    public static class Bloom
    {
        public static Surface Surface { get; private set; }

        public static float StrengthMultiplier = 1.0f;

        public static bool UseLuminance = true;
        public static int DownsamplePasses = 5;

        public static float Threshold
        {
            get { return _threshold; }
            set
            {
                if (Math.Abs(_threshold - value) > 0.001f)
                {
                    _threshold = value;
                    _bloomThresholdParameter.SetValue(_threshold);
                }
            }
        }
        private static float _threshold;

        public static float StreakLength
        {
            get { return _streakLength; }
            set
            {
                if (Math.Abs(_streakLength - value) > 0.001f)
                {
                    _streakLength = value;
                    _bloomStreakLengthParameter.SetValue(_streakLength);
                }
            }
        }
        private static float _streakLength;

        public static void NextPreset()
        {
            Preset(_bloomPreset.Next());
        }
        public static void PreviousPreset()
        {
            Preset(_bloomPreset.Previous());
        }

        private static void Preset(BloomPresets preset)
        {
            switch (preset)
            {
                case BloomPresets.Wide:
                    {
                        _bloomStrength1 = 0.5f;
                        _bloomStrength2 = 1;
                        _bloomStrength3 = 2;
                        _bloomStrength4 = 1;
                        _bloomStrength5 = 2;
                        _bloomRadius5 = 4.0f;
                        _bloomRadius4 = 4.0f;
                        _bloomRadius3 = 2.0f;
                        _bloomRadius2 = 2.0f;
                        _bloomRadius1 = 1.0f;
                        StreakLength = 1;
                        DownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.WeakWide:
                    {
                        _bloomStrength1 = 0.5f;
                        _bloomStrength2 = 1;
                        _bloomStrength3 = 1;
                        _bloomStrength4 = 1;
                        _bloomStrength5 = 0.5f;
                        _bloomRadius5 = 4.0f;
                        _bloomRadius4 = 4.0f;
                        _bloomRadius3 = 2.0f;
                        _bloomRadius2 = 2.0f;
                        _bloomRadius1 = 1.0f;
                        StreakLength = 2f;
                        DownsamplePasses = 2;
                        break;
                    }
                case BloomPresets.GlareWide:
                    {
                        _bloomStrength1 = 0.5f;
                        _bloomStrength2 = 1;
                        _bloomStrength3 = 2;
                        _bloomStrength4 = 1;
                        _bloomStrength5 = 2;
                        _bloomRadius5 = 4.0f;
                        _bloomRadius4 = 4.0f;
                        _bloomRadius3 = 2.0f;
                        _bloomRadius2 = 2.0f;
                        _bloomRadius1 = 1.0f;
                        StreakLength = 0.3f;
                        DownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.SuperWide:
                    {
                        _bloomStrength1 = 0.9f;
                        _bloomStrength2 = 1;
                        _bloomStrength3 = 1;
                        _bloomStrength4 = 2;
                        _bloomStrength5 = 6;
                        _bloomRadius5 = 4.0f;
                        _bloomRadius4 = 2.0f;
                        _bloomRadius3 = 2.0f;
                        _bloomRadius2 = 2.0f;
                        _bloomRadius1 = 2.0f;
                        StreakLength = 1;
                        DownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Focussed:
                    {
                        _bloomStrength1 = 0.8f;
                        _bloomStrength2 = 1;
                        _bloomStrength3 = 1;
                        _bloomStrength4 = 1;
                        _bloomStrength5 = 2;
                        _bloomRadius5 = 4.0f;
                        _bloomRadius4 = 2.0f;
                        _bloomRadius3 = 2.0f;
                        _bloomRadius2 = 2.0f;
                        _bloomRadius1 = 2.0f;
                        StreakLength = 1;
                        DownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Small:
                    {
                        _bloomStrength1 = 0.8f;
                        _bloomStrength2 = 1;
                        _bloomStrength3 = 1;
                        _bloomStrength4 = 1;
                        _bloomStrength5 = 1;
                        _bloomRadius5 = 1;
                        _bloomRadius4 = 1;
                        _bloomRadius3 = 1;
                        _bloomRadius2 = 1;
                        _bloomRadius1 = 1;
                        StreakLength = 1;
                        DownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Cheap:
                    {
                        _bloomStrength1 = 0.8f;
                        _bloomStrength2 = 2;
                        _bloomRadius2 = 2;
                        _bloomRadius1 = 2;
                        StreakLength = 1;
                        DownsamplePasses = 2;
                        break;
                    }
                case BloomPresets.One:
                    {
                        _bloomStrength1 = 4f;
                        _bloomStrength2 = 1;
                        _bloomStrength3 = 1;
                        _bloomStrength4 = 1;
                        _bloomStrength5 = 2;
                        _bloomRadius5 = 1.0f;
                        _bloomRadius4 = 1.0f;
                        _bloomRadius3 = 1.0f;
                        _bloomRadius2 = 1.0f;
                        _bloomRadius1 = 1.0f;
                        StreakLength = 1;
                        DownsamplePasses = 5;
                        break;
                    }
            }
            _bloomPreset = preset;
        }
        private static BloomPresets _bloomPreset;

        private static Texture2D ScreenTexture 
        { 
            set { _bloomParameterScreenTexture.SetValue(value); } 
        }
        
        private static Vector2 InverseResolution
        {
            get { return _inverseResolution; }
            set
            {
                if (value != _inverseResolution)
                {
                    _inverseResolution = value;
                    _bloomInverseResolutionParameter.SetValue(_inverseResolution);
                }
            }
        }
        private static Vector2 _inverseResolution;

        private static Surface _bloomSurfaceMip0;
        private static Surface _bloomSurfaceMip1;
        private static Surface _bloomSurfaceMip2;
        private static Surface _bloomSurfaceMip3;
        private static Surface _bloomSurfaceMip4;
        private static Surface _bloomSurfaceMip5;

        private static SurfaceFormat _renderTargetFormat;

        private static Effect _shaderEffect;

        private static EffectTechnique _bloomPassExtract;
        private static EffectTechnique _bloomPassExtractLuminance;
        private static EffectTechnique _bloomPassDownsample;
        private static EffectTechnique _bloomPassUpsample;
        private static EffectTechnique _bloomPassUpsampleLuminance;

        private static EffectParameter _bloomParameterScreenTexture;
        private static EffectParameter _bloomInverseResolutionParameter;
        private static EffectParameter _bloomRadiusParameter;
        private static EffectParameter _bloomStrengthParameter;
        private static EffectParameter _bloomStreakLengthParameter;
        private static EffectParameter _bloomThresholdParameter;

        private static float _bloomRadius1 = 1.0f;
        private static float _bloomRadius2 = 1.0f;
        private static float _bloomRadius3 = 1.0f;
        private static float _bloomRadius4 = 1.0f;
        private static float _bloomRadius5 = 1.0f;

        private static float BloomRadius
        {
            get
            {
                return _bloomRadius;
            }

            set
            {
                if (Math.Abs(_bloomRadius - value) > 0.001f)
                {
                    _bloomRadius = value;
                    _bloomRadiusParameter.SetValue(_bloomRadius * _radiusMultiplier);
                }

            }
        }
        private static float _bloomRadius;

        private static float _bloomStrength1 = 1.0f;
        private static float _bloomStrength2 = 1.0f;
        private static float _bloomStrength3 = 1.0f;
        private static float _bloomStrength4 = 1.0f;
        private static float _bloomStrength5 = 1.0f;

        private static float BloomStrength
        {
            get { return _bloomStrength; }
            set
            {
                if (Math.Abs(_bloomStrength - value) > 0.001f)
                {
                    _bloomStrength = value;
                    _bloomStrengthParameter.SetValue(_bloomStrength * StrengthMultiplier);
                }

            }
        }
        private static float _bloomStrength;

        private static float _radiusMultiplier = 1.0f;

        /// <summary>
        /// Loads all needed components for the BloomEffect.
        /// </summary>
        /// <param name="renderTargetFormat">The intended format for the rendertargets. For normal, non-hdr, applications color or rgba1010102 are fine NOTE: For OpenGL, SurfaceFormat.Color is recommended for non-HDR applications.</param>
        public static void Init(SurfaceFormat renderTargetFormat = SurfaceFormat.Color)
        {
            UpdateResolution();

            _renderTargetFormat = renderTargetFormat;

            _shaderEffect = ResourceHub.GetResource<Effect>("Effects", "Bloom");
            _bloomInverseResolutionParameter = _shaderEffect.Parameters["InverseResolution"];
            _bloomRadiusParameter = _shaderEffect.Parameters["Radius"];
            _bloomStrengthParameter = _shaderEffect.Parameters["Strength"];
            _bloomStreakLengthParameter = _shaderEffect.Parameters["StreakLength"];
            _bloomThresholdParameter = _shaderEffect.Parameters["Threshold"];
            _bloomParameterScreenTexture = _shaderEffect.Parameters["ScreenTexture"];

            _bloomPassExtract = _shaderEffect.Techniques["Extract"];
            _bloomPassExtractLuminance = _shaderEffect.Techniques["ExtractLuminance"];
            _bloomPassDownsample = _shaderEffect.Techniques["Downsample"];
            _bloomPassUpsample = _shaderEffect.Techniques["Upsample"];
            _bloomPassUpsampleLuminance = _shaderEffect.Techniques["UpsampleLuminance"];

            Threshold = 0.9f;
            Preset(BloomPresets.WeakWide);
        }

        internal static void Process()
        {
            if (RenderMgr.BloomFX)
            {
                UpdateResolution();

                RenderTarget2D renderTarget;
                if (RenderMgr.ColorGradingFX) renderTarget = ColorGrading.Surface.RenderTarget;
                else renderTarget = RenderMgr.SceneSurface.RenderTarget;                

                _radiusMultiplier = Surface.Size.X / renderTarget.Width;

                GraphicsMgr.VertexBatch.RasterizerState = RasterizerState.CullNone;
                GraphicsMgr.VertexBatch.BlendState = BlendState.Opaque;

                GraphicsMgr.VertexBatch.Texture = renderTarget;
                GraphicsMgr.VertexBatch.Effect = _shaderEffect;
                _shaderEffect.Parameters["World"].SetValue(GraphicsMgr.VertexBatch.World);
                _shaderEffect.Parameters["View"].SetValue(GraphicsMgr.VertexBatch.View);
                _shaderEffect.Parameters["Projection"].SetValue(GraphicsMgr.VertexBatch.Projection);

                //EXTRACT
                //We extract the bright values which are above the Threshold and save them to Mip0
                Surface.SetTarget(_bloomSurfaceMip0);
                GraphicsMgr.Device.Clear(Color.Transparent);

                GraphicsMgr.VertexBatch.Texture = renderTarget;
                ScreenTexture = renderTarget;
                InverseResolution = new Vector2(1.0f / Surface.Size.X, 1.0f / Surface.Size.Y);

                if (UseLuminance) _shaderEffect.CurrentTechnique = _bloomPassExtractLuminance;
                else _shaderEffect.CurrentTechnique = _bloomPassExtract;

                GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);
                Surface.ResetTarget();

                //Now downsample to the next lower mip texture
                if (DownsamplePasses > 0)
                {
                    //DOWNSAMPLE TO MIP1
                    Surface.SetTarget(_bloomSurfaceMip1);
                    GraphicsMgr.Device.Clear(Color.Transparent);

                    GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip0.RenderTarget;
                    ScreenTexture = _bloomSurfaceMip0.RenderTarget;
                    _shaderEffect.CurrentTechnique = _bloomPassDownsample;

                    GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);
                    Surface.ResetTarget();

                    if (DownsamplePasses > 1)
                    {
                        //Our input resolution is halfed, so our inverse 1/res. must be doubled
                        InverseResolution *= 2;

                        //DOWNSAMPLE TO MIP2
                        Surface.SetTarget(_bloomSurfaceMip2);
                        GraphicsMgr.Device.Clear(Color.Transparent);

                        GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip1.RenderTarget;
                        ScreenTexture = _bloomSurfaceMip1.RenderTarget;
                        _shaderEffect.CurrentTechnique = _bloomPassDownsample;

                        GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                        Surface.ResetTarget();

                        if (DownsamplePasses > 2)
                        {
                            InverseResolution *= 2;

                            //DOWNSAMPLE TO MIP3
                            Surface.SetTarget(_bloomSurfaceMip3);
                            GraphicsMgr.Device.Clear(Color.Transparent);

                            GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip2.RenderTarget;
                            ScreenTexture = _bloomSurfaceMip2.RenderTarget;
                            _shaderEffect.CurrentTechnique = _bloomPassDownsample;

                            GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                            Surface.ResetTarget();

                            if (DownsamplePasses > 3)
                            {
                                InverseResolution *= 2;

                                //DOWNSAMPLE TO MIP4
                                Surface.SetTarget(_bloomSurfaceMip4);
                                GraphicsMgr.Device.Clear(Color.Transparent);

                                GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip3.RenderTarget;
                                ScreenTexture = _bloomSurfaceMip3.RenderTarget;
                                _shaderEffect.CurrentTechnique = _bloomPassDownsample;

                                GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                                Surface.ResetTarget();

                                if (DownsamplePasses > 4)
                                {
                                    InverseResolution *= 2;

                                    //DOWNSAMPLE TO MIP5
                                    Surface.SetTarget(_bloomSurfaceMip5);
                                    GraphicsMgr.Device.Clear(Color.Transparent);

                                    GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip4.RenderTarget;
                                    ScreenTexture = _bloomSurfaceMip4.RenderTarget;
                                    _shaderEffect.CurrentTechnique = _bloomPassDownsample;

                                    GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                                    Surface.ResetTarget();

                                    ChangeBlendState();

                                    //UPSAMPLE TO MIP4
                                    Surface.SetTarget(_bloomSurfaceMip4);

                                    GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip5.RenderTarget;
                                    ScreenTexture = _bloomSurfaceMip5.RenderTarget;

                                    BloomStrength = _bloomStrength5;
                                    BloomRadius = _bloomRadius5;

                                    if (UseLuminance) _shaderEffect.CurrentTechnique = _bloomPassUpsampleLuminance;
                                    else _shaderEffect.CurrentTechnique = _bloomPassUpsample;

                                    GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                                    Surface.ResetTarget();

                                    InverseResolution /= 2;
                                }

                                ChangeBlendState();

                                //UPSAMPLE TO MIP3
                                Surface.SetTarget(_bloomSurfaceMip3);

                                GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip4.RenderTarget;
                                ScreenTexture = _bloomSurfaceMip4.RenderTarget;

                                BloomStrength = _bloomStrength4;
                                BloomRadius = _bloomRadius4;

                                if (UseLuminance) _shaderEffect.CurrentTechnique = _bloomPassUpsampleLuminance;
                                else _shaderEffect.CurrentTechnique = _bloomPassUpsample;

                                GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                                Surface.ResetTarget();

                                InverseResolution /= 2;

                            }

                            ChangeBlendState();

                            //UPSAMPLE TO MIP2
                            Surface.SetTarget(_bloomSurfaceMip2);

                            GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip3.RenderTarget;
                            ScreenTexture = _bloomSurfaceMip3.RenderTarget;

                            BloomStrength = _bloomStrength3;
                            BloomRadius = _bloomRadius3;

                            if (UseLuminance) _shaderEffect.CurrentTechnique = _bloomPassUpsampleLuminance;
                            else _shaderEffect.CurrentTechnique = _bloomPassUpsample;

                            GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                            Surface.ResetTarget();

                            InverseResolution /= 2;
                        }

                        ChangeBlendState();

                        //UPSAMPLE TO MIP1
                        Surface.SetTarget(_bloomSurfaceMip1);

                        GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip2.RenderTarget;
                        ScreenTexture = _bloomSurfaceMip2.RenderTarget;

                        BloomStrength = _bloomStrength2;
                        BloomRadius = _bloomRadius2;

                        if (UseLuminance) _shaderEffect.CurrentTechnique = _bloomPassUpsampleLuminance;
                        else _shaderEffect.CurrentTechnique = _bloomPassUpsample;

                        GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                        Surface.ResetTarget();

                        InverseResolution /= 2;
                    }

                    ChangeBlendState();

                    //UPSAMPLE TO MIP0
                    Surface.SetTarget(_bloomSurfaceMip0);

                    GraphicsMgr.VertexBatch.Texture = _bloomSurfaceMip1.RenderTarget;
                    ScreenTexture = _bloomSurfaceMip1.RenderTarget;

                    BloomStrength = _bloomStrength1;
                    BloomRadius = _bloomRadius1;

                    if (UseLuminance) _shaderEffect.CurrentTechnique = _bloomPassUpsampleLuminance;
                    else _shaderEffect.CurrentTechnique = _bloomPassUpsample;

                    GraphicsMgr.VertexBatch.AddQuad(Vector2.Zero, Color.White);

                    Surface.ResetTarget();
                }

                GraphicsMgr.VertexBatch.Effect = null;
                GraphicsMgr.VertexBatch.Texture = null;

                Surface.SetTarget(Surface);
                GraphicsMgr.Device.Clear(Color.Transparent);
                _bloomSurfaceMip0.Draw();
                Surface.ResetTarget();
            }
        }

        private static void ChangeBlendState()
        {
            GraphicsMgr.VertexBatch.BlendState = BlendState.AlphaBlend;
        }

        private static void UpdateResolution()
        {
            if (Surface == null || Surface.Size != GameMgr.WindowManager.CanvasSize)
            {
                Surface = new Surface(new Vector2(GameMgr.WindowManager.CanvasSize.X, GameMgr.WindowManager.CanvasSize.Y));

                var bloomRenderTarget2DMip0 = new RenderTarget2D(GraphicsMgr.Device, (int)Surface.Size.X, (int)Surface.Size.Y, false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
                var bloomRenderTarget2DMip1 = new RenderTarget2D(GraphicsMgr.Device, (int)Surface.Size.X, (int)Surface.Size.Y, false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                var bloomRenderTarget2DMip2 = new RenderTarget2D(GraphicsMgr.Device, (int)Surface.Size.X, (int)Surface.Size.Y, false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                var bloomRenderTarget2DMip3 = new RenderTarget2D(GraphicsMgr.Device, (int)Surface.Size.X, (int)Surface.Size.Y, false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                var bloomRenderTarget2DMip4 = new RenderTarget2D(GraphicsMgr.Device, (int)Surface.Size.X, (int)Surface.Size.Y, false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                var bloomRenderTarget2DMip5 = new RenderTarget2D(GraphicsMgr.Device, (int)Surface.Size.X, (int)Surface.Size.Y, false, _renderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                _bloomSurfaceMip0 = new Surface(bloomRenderTarget2DMip0);
                _bloomSurfaceMip1 = new Surface(bloomRenderTarget2DMip1);
                _bloomSurfaceMip2 = new Surface(bloomRenderTarget2DMip2);
                _bloomSurfaceMip3 = new Surface(bloomRenderTarget2DMip3);
                _bloomSurfaceMip4 = new Surface(bloomRenderTarget2DMip4);
                _bloomSurfaceMip5 = new Surface(bloomRenderTarget2DMip5);
            }
        }

        internal static void Dispose()
        {
            Surface?.Dispose();
            _shaderEffect?.Dispose();
            _bloomSurfaceMip0.Dispose();
            _bloomSurfaceMip1.Dispose();
            _bloomSurfaceMip2.Dispose();
            _bloomSurfaceMip3.Dispose();
            _bloomSurfaceMip4.Dispose();
            _bloomSurfaceMip5.Dispose();
        }
    }
}

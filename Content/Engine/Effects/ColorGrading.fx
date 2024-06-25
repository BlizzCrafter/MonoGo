#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

//Color Grading Shader with LUTs, TheKosmonaut 2017 ( kosmonaut3d@googlemail.com )

#define Size 16
#define SizeRoot 4

float4x4 World;
float4x4 View;
float4x4 Projection;

Texture2D Input;
Texture2D LUT;

SamplerState Sampler;

SamplerState InputSampler
{
    Texture = ( Input );
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};
SamplerState LutSampler
{
    Texture = ( LUT );
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
}; 

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TexCoords = input.TexCoords;
	output.Color = input.Color;

	return output;
}

float4 PixelShaderApplyLUT(VertexShaderOutput input) : COLOR0
{
    //Our input
    float4 baseTexture =  tex2D(InputSampler, input.TexCoords.xy);

    int width = SizeRoot * Size;
    int height = (Size / SizeRoot) * Size;
    float2 wh = float2(width, height);

    //Manual trilinear interpolation

    //We need to clamp since our values go, for example, from 0 to 15. But with a red value of 1.0 we would get 16, which is on the next table already.

    //OBSOLETE: We also need to shift half a pixel to the left, since our sampling locations do not match the storage location (see CreateLUT)
    //float halfOffset = 0.5f;

    float red = baseTexture.r * (Size - 1);

    float redinterpol = frac(red);

    float green = baseTexture.g * (Size - 1);
    float greeninterpol = frac(green);

    float blue = baseTexture.b * (Size - 1);
    float blueinterpol = frac(blue);

    //Blue base value

    float row = trunc(blue / SizeRoot);
    float col = trunc(blue % SizeRoot);

    float2 blueBaseTable = float2(trunc(col * Size), trunc(row * Size));

    float4 b0r1g0;
    float4 b0r0g1;
    float4 b0r1g1;
    float4 b1r0g0;
    float4 b1r1g0;
    float4 b1r0g1;
    float4 b1r1g1;

    //We need to read 8 values (like in a 3d LUT) and interpolate between them.
    //This cannot be done with default hardware filtering so I am doing it manually.
    //Note that we must not interpolate when on the borders of tables!

    //Red 0 and 1, Green 0

    float4 b0r0g0 = tex2D(LutSampler, float2(blueBaseTable.x + red, blueBaseTable.y + green) / wh);

    if (red < Size - 1)
        b0r1g0 = tex2D(LutSampler, float2(blueBaseTable.x + red + 1, blueBaseTable.y + green) / wh);
    else
        b0r1g0 = b0r0g0;

    // Green 1

    if (green < Size - 1)
    {
        //Red 0 and 1

        b0r0g1 = tex2D(LutSampler, float2(blueBaseTable.x + red, blueBaseTable.y + green + 1) / wh);

        if (red < Size - 1)
            b0r1g1 = tex2D(LutSampler, float2(blueBaseTable.x + red + 1, blueBaseTable.y + green + 1) / wh);
        else
            b0r1g1 = b0r0g1;
    }
    else
    {
        b0r0g1 = b0r0g0;
        b0r1g1 = b0r0g1;
    }

    if (blue < Size - 1)
    {
        blue += 1;
        row = trunc(blue / SizeRoot);
        col = trunc(blue % SizeRoot);

        blueBaseTable = float2(trunc(col * Size), trunc(row * Size));

        b1r0g0 = tex2D(LutSampler, float2(blueBaseTable.x + red, blueBaseTable.y + green) / wh);

        if (red < Size - 1)
            b1r1g0 = tex2D(LutSampler, float2(blueBaseTable.x + red + 1, blueBaseTable.y + green) / wh);
        else
            b1r1g0 = b0r0g0;

        // Green 1

        if (green < Size - 1)
        {
            //Red 0 and 1

            b1r0g1 = tex2D(LutSampler, float2(blueBaseTable.x + red, blueBaseTable.y + green + 1) / wh);

            if (red < Size - 1)
                b1r1g1 = tex2D(LutSampler, float2(blueBaseTable.x + red + 1, blueBaseTable.y + green + 1) / wh);
            else
                b1r1g1 = b0r0g1;
        }
        else
        {
            b1r0g1 = b0r0g0;
            b1r1g1 = b0r0g1;
        }
    }
    else
    {
        b1r0g0 = b0r0g0;
        b1r1g0 = b0r1g0;
        b1r0g1 = b0r0g0;
        b1r1g1 = b0r1g1;
    }

    float4 result = lerp(lerp(b0r0g0, b0r1g0, redinterpol), lerp(b0r0g1, b0r1g1, redinterpol), greeninterpol);
    float4 result2 = lerp(lerp(b1r0g0, b1r1g0, redinterpol), lerp(b1r0g1, b1r1g1, redinterpol), greeninterpol);

    result = lerp(result, result2, blueinterpol);

    return result;
}

technique ApplyLUT
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderApplyLUT();
    }
}
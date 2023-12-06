#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 view_projection;
float CoreBlendBegin;
float CoreBlendEnd;
float SinTime;
float Time;
sampler Shape : register(s0);
sampler Core;
sampler Ramp;

struct v2f
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 uv : TEXCOORD0;
};

v2f SpriteVertexShader(v2f v)
{
    v.Position = mul(v.Position, view_projection);
    v.Color = v.Color;
    return v;
}

float4 SpritePixelShader(v2f i) : SV_TARGET
{
    float4 colShape = tex2D(Shape, i.uv);
    float4 colShape2 = tex2D(Shape, float2(i.uv.x, 1 - i.uv.y));
    colShape = lerp(colShape, colShape2, (SinTime + 1) * 0.5);
    clip(colShape.r - 0.01);

    float4 colRamped = tex2D(Ramp, float2(colShape.r, 0.5));
    colRamped.rgb /= colRamped.a;

    float2 uvTex = float2(i.uv.x, i.uv.y + Time);
    float4 colTex = tex2D(Core, uvTex);

    uvTex = float2(i.uv.x + 0.5, -i.uv.y + Time);
    float4 colTex2 = tex2D(Core, uvTex);

    colTex = lerp(colTex, colTex2, 0.5);
    colTex.rgb /= colTex.a;

    float ratio = smoothstep(CoreBlendBegin, CoreBlendEnd, colShape.r);
    float4 col = lerp(colRamped, colTex, ratio);
    col.a = colRamped.a;

    return col;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}

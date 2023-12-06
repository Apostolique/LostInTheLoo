#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 view_projection;
float SinTime;
float Time;
sampler Shape : register(s0);
sampler Core;
sampler Ramp;

struct VertexInput {
    float4 Position : POSITION0;
    float4 UV : TEXCOORD0;
    float4 UV2 : TEXCOORD1;
    float4 Color : COLOR0;
    float4 Meta1 : TEXCOORD2;
    float4 Meta2 : TEXCOORD3;
};
struct PixelInput {
    float4 Position : SV_Position0;
    float4 UV : TEXCOORD0;
    float4 UV2 : TEXCOORD1;
    float4 Color : COLOR0;
    float4 Meta1 : TEXCOORD2;
    float4 Meta2 : TEXCOORD3;
};

PixelInput SpriteVertexShader(VertexInput v) {
    PixelInput output;

    output.Position = mul(v.Position, view_projection);
    output.UV = v.UV;
    output.UV2 = v.UV2;
    output.Color = v.Color;
    output.Meta1 = v.Meta1;
    output.Meta2 = v.Meta2;
    return output;
}

float4 SpritePixelShader(PixelInput p) : SV_TARGET {
    float4 colShape = tex2D(Shape, p.UV.xy);
    float4 colShape2 = tex2D(Shape, float2(p.UV.x, 1 - p.UV.y));
    colShape = lerp(colShape, colShape2, (SinTime + 1) * 0.5);
    clip(colShape.r - 0.01);

    float4 colRamped = tex2D(Ramp, float2(colShape.r, p.Meta1.x));
    colRamped.rgb /= colRamped.a;

    float2 uvTex = p.UV2.xy + p.Meta2.xy * Time;
    float4 colTex = tex2D(Core, uvTex);

    uvTex = float2(p.UV2.x + 0.5, p.UV2.y) - p.Meta2.xy * Time;
    float4 colTex2 = tex2D(Core, uvTex);

    colTex = lerp(colTex, colTex2, 0.5);
    colTex.rgb /= colTex.a;

    float ratio = smoothstep(p.Meta1.y, p.Meta1.z, colShape.r);
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

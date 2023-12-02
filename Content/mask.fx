#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

sampler TextureSampler : register(s0);
Texture2D mask_texture;
sampler MaskSampler {
    Texture = ( mask_texture );
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct VertexToPixel {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

float4 PS(VertexToPixel p): SV_TARGET {
    float4 diffuse = tex2D(TextureSampler, p.TexCoord.xy);
    float4 mask = tex2D(MaskSampler, p.TexCoord.xy);

    return diffuse * float4(mask.r, mask.r, mask.r, mask.r);
}

technique SpriteBatch {
    pass {
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 view_projection;
float4x4 uv_transform;
sampler TextureSampler : register(s0);

struct VertexInput {
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};
struct PixelInput {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

PixelInput VS(VertexInput v) {
    PixelInput output;

    output.Position = mul(v.Position, view_projection);
    output.Color = v.Color;
    output.TexCoord = mul(v.TexCoord, uv_transform);
    return output;
}
float4 PS(PixelInput p) : SV_TARGET {
    float4 diffuse = tex2D(TextureSampler, p.TexCoord.xy);

    // if (p.TexCoord.x < 0 || p.TexCoord.x > 1) {
    //     discard;
    // }

    // if (p.TexCoord.y < 0 || p.TexCoord.y > 1) {
    //     discard;
    // }

    return diffuse * p.Color;
}

technique SpriteBatch {
    pass {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

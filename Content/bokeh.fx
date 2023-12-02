#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

#define pi 4.0 * atan(1.0)
#define ang (3.0 - sqrt(5.0)) * pi
#define gamma 1.8
#define samples 150.0

float2 unit;
float r;

sampler TextureSampler : register(s0);

struct VertexToPixel {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

float4 bokeh(float2 uv, float2 radius) {
    float4 col = float4(0.0, 0.0, 0.0, 0.0);
    for (float i = 0.0; i < samples; i++) {
        float d = i / samples;
        float2 p = float2(sin(ang * i), cos(ang * i)) * sqrt(d) * radius;
        col += pow(tex2D(TextureSampler, uv + p), float4(gamma, gamma, gamma, gamma));
    }
    return pow(col / samples, 1.0 / gamma);
}

float4 PS(VertexToPixel p) : SV_TARGET {
    float4 c = bokeh(p.TexCoord.xy, r * unit);

    return c * p.Color;
}

technique BokehBlur {
    pass P0 {
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

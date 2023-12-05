#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

// https://github.com/Sergio0694/ComputeSharp/blob/main/samples/ComputeSharp.ImageProcessing/Processors/HlslBokehBlurProcessor.Implementation.cs

#define pi 3.14159265
#define e 2.71828183

#define RADIUS 16
#define KERNEL_SIZE (RADIUS * 2 + 1)

float2 unit;
float kernelSize;
float2 kernel[KERNEL_SIZE];
float radius;

sampler TextureSampler : register(s0);

struct VertexToPixel {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

struct PixelShaderOutput {
    float4 Color0 : COLOR0;
    float4 Color1 : COLOR1;
};

PixelShaderOutput PS(VertexToPixel p) {
    float4 c0 = float4(0.0, 0.0, 0.0, 0.0);
    float4 c1 = float4(0.0, 0.0, 0.0, 0.0);

    for (int i = 0; i < KERNEL_SIZE; i++) {
        float offsetY = p.TexCoord.y + (i - RADIUS) * unit.y * radius;
        float offsetX = p.TexCoord.x;
        float4 color = tex2D(TextureSampler, float2(offsetX, offsetY));

        float factorX = kernel[i].x;
        float factorY = kernel[i].y;

        c0 += factorX * color;
        c1 += factorY * color;
    }

    PixelShaderOutput output;
    output.Color0 = c0;
    output.Color1 = c1;

    return output;
}

technique VerticalBokeh {
    pass P0 {
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

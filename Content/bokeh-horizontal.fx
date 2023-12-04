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
float z;
float w;

sampler RealSampler : register(s0);

Texture2D imaginary;
sampler ImaginarySampler {
    Texture = ( imaginary );
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

float4 weighted(float4 r, float4 i, float a, float b) {
    return r * a + i * b;
}

float4 PS(VertexToPixel p) : SV_TARGET {
    float offsetY = p.TexCoord.y;
    float4 resultReal = float4(0.0, 0.0, 0.0, 0.0);
    float4 resultImaginary = float4(0.0, 0.0, 0.0, 0.0);

    for (int i = 0; i < KERNEL_SIZE; ++i) {
        float offsetX = p.TexCoord.x + (i - RADIUS) * unit.x * radius;
        float4 sourceReal = tex2D(RealSampler, float2(offsetX, offsetY));
        float4 sourceImaginary = tex2D(ImaginarySampler, float2(offsetX, offsetY));
        float factorX = kernel[i].x;
        float factorY = kernel[i].y;

        resultReal += (factorX * sourceReal) - (factorY * sourceImaginary);
        resultImaginary += (factorX * sourceImaginary) - (factorY * sourceReal);
    }

    return weighted(resultReal, resultImaginary, z, w);
}

technique HorizontalBokeh {
    pass P0 {
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

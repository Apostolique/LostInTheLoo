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

#define RADIUS 5
#define KERNEL_SIZE (RADIUS * 2 + 1)

float2 unit;
float kernelSize;
float2 kernel[KERNEL_SIZE];
float radius;
float z;
float w;

sampler TextureSampler : register(s0);

Texture2D real;
sampler RealSampler {
    Texture = ( real );
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

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

struct PixelShaderOutput {
    float4 Color0 : COLOR0;
    float4 Color1 : COLOR1;
};

float2 c_add(float2 c1, float2 c2)
{
    float a = c1.x;
    float b = c1.y;
    float c = c2.x;
    float d = c2.y;
    return float2(a + c, b + d);
}
float2 c_sub(float2 c1, float2 c2)
{
    float a = c1.x;
    float b = c1.y;
    float c = c2.x;
    float d = c2.y;
    return float2(a - c, b - d);
}
float2 c_mul(float2 c1, float2 c2)
{
    float a = c1.x;
    float b = c1.y;
    float c = c2.x;
    float d = c2.y;
    return float2(a*c - b*d, b*c + a*d);
}
float2 c_div(float2 c1, float2 c2)
{
    float a = c1.x;
    float b = c1.y;
    float c = c2.x;
    float d = c2.y;
    float real = (a*c + b*d) / (c*c + d*d);
    float imag = (b*c - a*d) / (c*c + d*d);
    return float2(real, imag);
}
float c_abs(float2 c)
{
    return sqrt(c.x*c.x + c.y*c.y);
}
float2 c_pol(float2 c)
{
    float a = c.x;
    float b = c.y;
    float z = c_abs(c);
    float f = atan2(b, a);
    return float2(z, f);
}
float2 c_rec(float2 c)
{
    float z = abs(c.x);
    float f = c.y;
    float a = z * cos(f);
    float b = z * sin(f);
    return float2(a, b);
}
float2 c_pow(float2 base, float2 exp)
{
    float2 b = c_pol(base);
    float r = b.x;
    float f = b.y;
    float c = exp.x;
    float d = exp.y;
    float z = pow(r, c) * pow(e, -d * f);
    float fi = d * log(r) + c * f;
    float2 rpol = float2(z, fi);
    return c_rec(rpol);
}
float4 weighted(float4 r, float4 i, float a, float b) {
    return r * a + i * b;
}

PixelShaderOutput PS1(VertexToPixel p) : SV_TARGET {
    float4 c0 = float4(0.0, 0.0, 0.0, 0.0);
    float4 c1 = float4(0.0, 0.0, 0.0, 0.0);

    for (int i = 0; i < kernelSize; ++i) {
        float offsetY = clamp(p.TexCoord.y + (i - RADIUS) * unit.y, 0.0, 1.0);
        float offsetX = clamp(p.TexCoord.x, 0.0, 1.0);
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
float4 PS2(VertexToPixel p) : SV_TARGET {
    float offsetY = clamp(p.TexCoord.y, 0.0, 1.0);
    float4 resultReal = float4(0.0, 0.0, 0.0, 0.0);
    float4 resultImaginary = float4(0.0, 0.0, 0.0, 0.0);

    for (int i = 0; i < kernelSize; ++i) {
        float offsetX = clamp(p.TexCoord.x + (i - RADIUS) * unit.x, 0.0, 1.0);
        float4 sourceReal = tex2D(RealSampler, float2(offsetX, offsetY));
        float4 sourceImaginary = tex2D(ImaginarySampler, float2(offsetX, offsetY));
        float factorX = kernel[i].x;
        float factorY = kernel[i].y;

        resultReal += (factorX * sourceReal) - (factorY * sourceImaginary);
        resultImaginary += (factorX * sourceImaginary) - (factorY * sourceReal);
    }

    return weighted(resultReal, resultImaginary, z, w);
}

technique VerticalBokeh {
    pass P0 {
        PixelShader = compile PS_SHADERMODEL PS1();
    }
}
technique HorizontalBokeh {
    pass P0 {
        PixelShader = compile PS_SHADERMODEL PS2();
    }
}

Shader "Hidden/OilPaintEffect"
{
    Properties
    {
        _MainTex("Screen Texture",       2D) = "white" {}
        _BrushSize("Brush Size",           Float) = 3
        _ColorSteps("Color Steps",          Float) = 8
        _PainterNoise("Painterly Noise",      Float) = 0.15

        _EnableQuantize("Enable Color Steps",   Float) = 1.0

        _BumpMap("Canvas Bump Map",      2D) = "gray" {}
        _BumpTiling("Bump Tiling",          Float) = 10
        _BumpInfluence("Bump Influence",       Float) = 0.5
        _NoiseFreq("Grain Noise Freq",     Float) = 8192
        _NoiseInfluence("Noise Influence",      Float) = 0.05
        _GrainStrength("Final Grain Strength", Float) = 0.08

        _Contrast("Contrast",             Float) = 1.0
        _ReflectThreshold("Reflection Threshold",  Float) = 0.9
        _ReflectAttenuation("Reflect Attenuation",   Float) = 0.2

            // Lift-Gamma-Gain controls:
            _Lift("Lift (shadows up)",         Range(0,0.5)) = 0.05
            _Gamma("Gamma (midtones)",         Range(0.5,2.0)) = 1.0
            _Gain("Gain (highlights down)",    Range(0.5,1.5)) = 1.0

            // Saturation control
            _Saturation("Saturation",          Range(0,2)) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                ZTest Always
                Cull Off
                ZWrite Off

                HLSLPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                sampler2D _MainTex;
                float4   _MainTex_TexelSize;
                float    _BrushSize;
                float    _ColorSteps;
                float    _PainterNoise;
                float    _EnableQuantize;

                sampler2D _BumpMap;
                float    _BumpTiling;
                float    _BumpInfluence;
                float    _NoiseFreq;
                float    _NoiseInfluence;
                float    _GrainStrength;

                float    _Contrast;
                float    _ReflectThreshold;
                float    _ReflectAttenuation;

                float    _Lift;
                float    _Gamma;
                float    _Gain;

                float    _Saturation;

                struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
                struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    o.uv = v.uv;
                    return o;
                }

                float hash21(float2 p)
                {
                    p = frac(p * float2(123.34, 456.21));
                    p += dot(p, p + 45.32);
                    return frac(p.x * p.y);
                }

                float4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;
                    float2 step = _MainTex_TexelSize.xy * _BrushSize;

                    // 1) Brush-style 5x5 blur
                    float4 sum = 0;
                    float wsum = 0;
                    [unroll(5)] for (int x = -2; x <= 2; x++)
                    [unroll(5)] for (int y = -2; y <= 2; y++)
                    {
                        float2 off = step * float2(x, y);
                        float w = saturate(1 - length(off) / (length(step) * 2));
                        sum += tex2D(_MainTex, uv + off) * w;
                        wsum += w;
                    }

                    // 2) Base color + painterly noise
                    float4 baseCol = sum / wsum;
                    baseCol.rgb += hash21(uv * 1024) * _PainterNoise;

                    // 3) Quantize (toggleable)
                    float3 q = floor(baseCol.rgb * _ColorSteps) / _ColorSteps;
                    float4 quantCol = float4(q, baseCol.a);
                    float4 color = lerp(baseCol, quantCol, _EnableQuantize);

                    // 4) Canvas grain (height only, no color)
    float3 bumpCol = tex2D(_BumpMap, uv * _BumpTiling).rgb;
    float bump = dot(bumpCol, float3(0.299,0.587,0.114));
    float noise = hash21(uv * _NoiseFreq);
    float grain = bump * _BumpInfluence + noise * _NoiseInfluence;
    color.rgb += (grain - 0.5) * _GrainStrength;

    // 5) Contrast boost boost
                    color.rgb = (color.rgb - 0.5) * _Contrast + 0.5;

                    // 6) Reflection attenuation
                    float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
                    float t = smoothstep(_ReflectThreshold, 1, luminance);
                    color.rgb *= lerp(1, _ReflectAttenuation, t);

                    // 7) Lift-Gamma-Gain for shadows/highlights
                    color.rgb += _Lift;
                    color.rgb = pow(color.rgb, 1.0 / _Gamma);
                    color.rgb *= _Gain;

                    // 8) Saturation
                    float lum = dot(color.rgb, float3(0.299, 0.587, 0.114));
                    color.rgb = lerp(float3(lum, lum, lum), color.rgb, _Saturation);

                    return saturate(color);
                }
                ENDHLSL
            }
        }
}

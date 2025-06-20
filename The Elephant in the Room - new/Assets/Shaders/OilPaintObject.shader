Shader "Custom/OilPaintObject"
{
    Properties
    {
        _MainTex("Base Texture",           2D) = "white" {}
        _GrainMap("Grain Map (height)",   2D) = "gray"  {}
        _GrainScale("Grain UV Scale",      Float) = 10.0
        _DisplacementStrength("Grain Displacement", Float) = 0.05
        _ColorSteps("Color Steps",         Float) = 8
        _NoiseStrength("Painterly Noise",  Float) = 0.15
        _BumpMap("Bump Map",               2D) = "bump" {}
        _BumpStrength("Bump Strength",     Float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                HLSLPROGRAM
                #pragma target 3.0               // still SM3.0!
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                TEXTURE2D(_MainTex);
                TEXTURE2D(_GrainMap);
                TEXTURE2D(_BumpMap);
                SAMPLER(sampler_MainTex);
                SAMPLER(sampler_GrainMap);
                SAMPLER(sampler_BumpMap);

                float _GrainScale;
                float _DisplacementStrength;
                float _ColorSteps;
                float _NoiseStrength;
                float _BumpStrength;

                struct Attributes
                {
                    float4 position : POSITION;
                    float3 normal   : NORMAL;
                    float4 tangent  : TANGENT;
                    float2 uv       : TEXCOORD0;
                };

                struct Varyings
                {
                    float4 pos       : SV_POSITION;
                    float2 uv        : TEXCOORD0;
                    float3 worldNorm : TEXCOORD1;
                    float3 worldPos  : TEXCOORD2;
                    float3x3 tbn     : TEXCOORD3;
                };

                // cheap per-vertex noise
                float hash21(float2 p)
                {
                    p = frac(p * float2(123.34, 456.21));
                    p += dot(p, p + 45.32);
                    return frac(p.x * p.y);
                }

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    // build TBN
                    float3 worldNormal = TransformObjectToWorldNormal(v.normal);
                    float3 worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
                    float3 worldBitan = cross(worldNormal, worldTangent) * v.tangent.w;
                    o.tbn = float3x3(worldTangent, worldBitan, worldNormal);

                    // procedural height instead of real grain lookup
                    float height = hash21(v.uv * _GrainScale);

                    // displace along normal
                    float3 worldPos = TransformObjectToWorld(v.position.xyz);
                    worldPos += worldNormal * (height * _DisplacementStrength);

                    o.pos = TransformWorldToHClip(worldPos);
                    o.uv = v.uv;
                    o.worldNorm = worldNormal;
                    o.worldPos = worldPos;
                    return o;
                }

                float hash(float2 p)
                {
                    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
                }

                float4 frag(Varyings i) : SV_Target
                {
                    // now do real grain©\map sample here
                    float height = SAMPLE_TEXTURE2D(_GrainMap, sampler_GrainMap, i.uv * _GrainScale).r;

                // bump normal
                float3 bumpN = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv)).xyz;
                bumpN.xy *= _BumpStrength;
                bumpN = normalize(bumpN);
                float3 worldBumpN = normalize(mul(bumpN, i.tbn));
                float3 finalN = normalize(lerp(i.worldNorm, worldBumpN, 0.5));

                // UV warp by height
                float2 dispUV = i.uv + finalN.xy * (height * _DisplacementStrength * 0.1);

                // base color
                float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, dispUV).rgb;

                // lighting
                Light main = GetMainLight();
                float3 L = main.direction.xyz;
                float  nl = saturate(dot(finalN, -L));
                col *= nl * main.color.rgb;

                // quantize & painterly noise
                col = floor(col * _ColorSteps) / _ColorSteps;
                col += hash21(i.uv * 1024) * _NoiseStrength;

                return float4(col,1);
            }
            ENDHLSL
        }
        }
}

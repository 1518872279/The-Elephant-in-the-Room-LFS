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
                #pragma target 3.0
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
                
                float    _GrainScale;
                float    _DisplacementStrength;
                float    _ColorSteps;
                float    _NoiseStrength;
                float    _BumpStrength;

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

                // Simple hash function for vertex displacement
                float hash(float2 p)
                {
                    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
                }

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    
                    // Calculate TBN matrix
                    float3 worldNormal = TransformObjectToWorldNormal(v.normal);
                    float3 worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
                    float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w;
                    o.tbn = float3x3(worldTangent, worldBitangent, worldNormal);
                    
                    // Sample grain map using procedural noise as fallback
                    float2 grainUV = v.uv * _GrainScale;
                    float height = hash(grainUV) * 0.5 + 0.5; // Convert to 0-1 range
                    
                    // Apply displacement along normal
                    float3 worldPos = TransformObjectToWorld(v.position.xyz);
                    worldPos += worldNormal * (height * _DisplacementStrength);
                    
                    o.pos = TransformWorldToHClip(worldPos);
                    o.uv = v.uv;
                    o.worldNorm = worldNormal;
                    o.worldPos = worldPos;
                    return o;
                }

                float hash21(float2 p)
                {
                    p = frac(p * float2(123.34, 456.21));
                    p += dot(p, p + 45.32);
                    return frac(p.x * p.y);
                }

                float4 frag(Varyings i) : SV_Target
                {
                    // Sample grain map for additional displacement
                    float height = SAMPLE_TEXTURE2D(_GrainMap, sampler_GrainMap, i.uv * _GrainScale).r;
                    
                    // Sample bump map
                    float3 bumpNormal = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv));
                    bumpNormal.xy *= _BumpStrength;
                    bumpNormal = normalize(bumpNormal);
                    
                    // Transform bump normal to world space
                    float3 worldBumpNormal = normalize(mul(bumpNormal, i.tbn));
                    
                    // Blend original normal with bump normal
                    float3 finalNormal = normalize(lerp(i.worldNorm, worldBumpNormal, 0.5));
                    
                    // Apply displacement to UV coordinates for texture sampling
                    float2 displacedUV = i.uv + finalNormal.xy * (height * _DisplacementStrength * 0.1);
                    
                    // Sample base texture with displaced UV
                    float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, displacedUV).rgb;

                    // Directional lighting via URP Main Light
                    Light mainLight = GetMainLight();
                    float3 lightDir = mainLight.direction.xyz;
                    float  nl = saturate(dot(finalNormal, -lightDir));
                    float3 lc = mainLight.color.rgb;
                    col *= nl * lc;

                    // Color quantization
                    col = floor(col * _ColorSteps) / _ColorSteps;
                    // Painterly noise overlay
                    col += hash21(i.uv * 1024) * _NoiseStrength;

                    return float4(col,1);
                }
                ENDHLSL
            }
        }
}

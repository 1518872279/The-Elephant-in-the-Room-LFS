Shader "Custom/BumpSubtleOverlay_NoReflection"
{
    Properties
    {
        _BaseMap("Base Albedo",             2D) = "white" {}
        _BumpMap("Normal Map",              2D) = "bump"  {}
        _BumpScale("Bump Strength",           Range(0,3)) = 1.0
        _OverlayStrength("Overlay Strength",        Range(0,3)) = 0.3
        _Transparency("Overlay Transparency",    Range(0,1)) = 1.0
        _LightDir("Light Direction",         Vector) = (0,0,1,0)
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            Pass
            {
                HLSLPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                TEXTURE2D(_BaseMap);     SAMPLER(sampler_BaseMap);
                TEXTURE2D(_BumpMap);     SAMPLER(sampler_BumpMap);
                float    _BumpScale;
                float    _OverlayStrength;
                float    _Transparency;
                float4   _LightDir;

                struct Attributes
                {
                    float4 position : POSITION;
                    float2 uv       : TEXCOORD0;
                    float3 normal   : NORMAL;
                    float4 tangent  : TANGENT;
                };

                struct Varyings
                {
                    float4 pos     : SV_POSITION;
                    float2 uv      : TEXCOORD0;
                    float3x3 tbn   : TEXCOORD1;
                };

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    // Transform to clip
                    o.pos = TransformObjectToHClip(v.position.xyz);
                    o.uv = v.uv;
                    // Build TBN for normal map
                    float3 N = TransformObjectToWorldNormal(v.normal);
                    float3 T = TransformObjectToWorldDir(v.tangent.xyz);
                    float3 B = cross(N, T) * v.tangent.w;
                    o.tbn = float3x3(T, B, N);
                    return o;
                }

                float4 frag(Varyings i) : SV_Target
                {
                    // 1) Sample base albedo
                    float3 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb;

                    // 2) Unpack normal map, apply scale, rotate into world-space
                    float3 nT = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv));
                    nT.xy *= _BumpScale;
                    float3 worldN = normalize(mul(nT, i.tbn));

                    // 3) Simple diffuse lighting (no specular)
                    float3 L = normalize(_LightDir.xyz);
                    float  NdotL = saturate(dot(worldN, L));

                    // 4) Blend the lighting into base color
                    float blend = lerp(1.0, NdotL, _OverlayStrength);
                    float3 result = baseCol * blend;

                    // 5) Output with user©\controlled transparency
                    return float4(result, _Transparency);
                }
                ENDHLSL
            }
        }
}

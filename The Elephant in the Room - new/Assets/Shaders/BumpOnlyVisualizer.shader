Shader "Custom/BumpSubtleOverlay_Transparent"
{
    Properties
    {
        _BaseMap("Base Albedo",         2D) = "white" {}
        _BumpMap("Normal Map",          2D) = "bump"  {}
        _BumpScale("Bump Strength",       Range(0,100)) = 1.0
        _OverlayStrength("Overlay Strength",    Range(0,100)) = 0.3
        _Transparency("Overlay Transparency",Range(0,1)) = 1.0
        _LightDir("Light Direction",     Vector) = (0,0,1,0)
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

                TEXTURE2D(_BaseMap);    SAMPLER(sampler_BaseMap);
                TEXTURE2D(_BumpMap);    SAMPLER(sampler_BumpMap);
                float    _BumpScale;
                float    _OverlayStrength;
                float    _Transparency;
                float4   _LightDir;

                struct Attributes
                {
                    float4 position : POSITION;
                    float2 uv       : TEXCOORD0;
                };

                struct Varyings
                {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                };

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    o.pos = TransformObjectToHClip(v.position.xyz);
                    o.uv = v.uv;
                    return o;
                }

                float4 frag(Varyings i) : SV_Target
                {
                    // sample your real albedo
                    float4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                    float3 baseCol = baseSample.rgb;

                    // unpack & scale bump normal
                    float3 n = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv));
                    n.xy *= _BumpScale;
                    n = normalize(n);

                    // simple N¡¤L diffuse
                    float3 L = normalize(_LightDir.xyz);
                    float NdotL = saturate(dot(n, L));

                    // blend bump-lighting onto base
                    float shade = lerp(1.0, NdotL, _OverlayStrength);
                    float3 col = baseCol * shade;

                    // return with controlled transparency
                    // you can also multiply by baseSample.a if your base map has alpha
                    return float4(col, _Transparency);
                }
                ENDHLSL
            }
        }
}

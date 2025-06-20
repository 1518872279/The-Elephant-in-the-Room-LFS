Shader "Custom/SimpleImageOverlay"
{
    Properties
    {
        _MainTex("Image", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZTest LEqual
            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

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
                // Transform to clip©\space
                o.pos = TransformObjectToHClip(v.position.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                // Sample the image and output it directly
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return color;
            }
            ENDHLSL
        }
    }
}

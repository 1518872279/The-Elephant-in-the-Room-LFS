Shader "Custom/CameraProjectorURP"
{
    Properties
    {
        _MainTex("Scene Texture",    2D) = "white" {}
        _ProjectorTex("Projector Texture",2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        // declare both textures & samplers
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_ProjectorTex);
        SAMPLER(sampler_ProjectorTex);

        // projector VP matrix (set from C#)
        float4x4 _ProjectorVP;

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 uv  : TEXCOORD0;
        };

        v2f vert(uint id : SV_VertexID)
        {
            v2f o;
            // full-screen triangle trick
            o.uv = float2((id << 1) & 2, id & 2);
            o.pos = float4(o.uv * 2 - 1, 0, 1);
            return o;
        }

        float4 frag(v2f IN) : SV_Target
        {
            // sample the current scene
            float4 scene = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

            // compute projector uv
            float4 projCS = mul(_ProjectorVP, float4(IN.uv * 2 - 1, 0, 1));
            projCS.xy /= projCS.w;
            float2 puv = projCS.xy * 0.5 + 0.5;

            // sample projector image
            float4 proj = SAMPLE_TEXTURE2D(_ProjectorTex, sampler_ProjectorTex, puv);

            // blend projector over scene
            return lerp(scene, proj, proj.a);
        }
        ENDHLSL
    }
    }
}

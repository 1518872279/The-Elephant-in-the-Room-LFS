Shader "Custom/WetLensShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RaindropTex ("Raindrop Normal Map", 2D) = "bump" {}
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _RaindropScale ("Raindrop Scale", Range(0.1, 10)) = 1
        _RaindropSpeed ("Raindrop Speed", Range(0, 5)) = 1
        _Wetness ("Wetness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _RaindropTex;
            float4 _MainTex_ST;
            float _DistortionStrength;
            float _RaindropScale;
            float _RaindropSpeed;
            float _Wetness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample raindrop normal map with scrolling
                float2 raindropUV = i.uv * _RaindropScale + _Time.y * _RaindropSpeed * float2(0.1, 0.05);
                float3 raindropNormal = UnpackNormal(tex2D(_RaindropTex, raindropUV));
                
                // Apply distortion based on raindrop normal
                float2 distortion = raindropNormal.xy * _DistortionStrength * _Wetness;
                float2 distortedUV = i.uv + distortion;
                
                // Sample main texture with distortion
                fixed4 col = tex2D(_MainTex, distortedUV);
                
                // Add subtle darkening for wetness effect
                col.rgb *= (1 - _Wetness * 0.1);
                
                return col;
            }
            ENDCG
        }
    }
} 
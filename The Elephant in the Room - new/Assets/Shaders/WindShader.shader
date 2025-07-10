Shader "Custom/WindShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WindDirection ("Wind Direction (xyz = direction, w = speed)", Vector) = (1,0,0,1)
        _WindStrength ("Wind Strength", Range(0, 2)) = 0.5
        _WindFrequency ("Wind Frequency", Range(0, 5)) = 1
        _WindAmplitude ("Wind Amplitude", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _WindDirection;
            float _WindStrength;
            float _WindFrequency;
            float _WindAmplitude;

            v2f vert (appdata v)
            {
                v2f o;
                
                // Calculate wind effect
                float3 windDir = normalize(_WindDirection.xyz);
                float phase = dot(v.vertex.xyz, windDir) + _Time.y * _WindDirection.w * _WindFrequency;
                float3 windOffset = windDir * sin(phase) * _WindStrength * _WindAmplitude;
                
                // Apply wind offset to vertex position
                float3 windedVertex = v.vertex.xyz + windOffset;
                
                o.vertex = UnityObjectToClipPos(windedVertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
} 
Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Blur Size", Range(0,10)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Size;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * 0.227027;
                col += tex2D(_MainTex, i.uv + float2(_Size, 0) * _MainTex_TexelSize.xy) * 0.316216;
                col += tex2D(_MainTex, i.uv - float2(_Size, 0) * _MainTex_TexelSize.xy) * 0.316216;
                col += tex2D(_MainTex, i.uv + float2(0, _Size) * _MainTex_TexelSize.xy) * 0.070270;
                col += tex2D(_MainTex, i.uv - float2(0, _Size) * _MainTex_TexelSize.xy) * 0.070270;
                return col;
            }
            ENDCG
        }
    }
} 
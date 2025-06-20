Shader "Hidden/KuwaharaFilter"
{
    Properties
    {
        _MainTex("Source Texture",  2D) = "white" {}
        _Radius("Radius",          Range(1,10)) = 3
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }
            Pass
            {
                ZTest Always Cull Off ZWrite Off

                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4   _MainTex_ST;        // <<< Declare this for TRANSFORM_TEX
                float4   _MainTex_TexelSize; // x = 1/width, y = 1/height
                int      _Radius;

                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                };

                v2f vert(appdata_base v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    int   r = _Radius;
                    float n = (r + 1) * (r + 1);
                    float minSigma = 1e20;
                    float3 bestMean = float3(0,0,0);

                    for (int k = 0; k < 4; k++)
                    {
                        float3 mean = float3(0,0,0);
                        float3 sigma = float3(0,0,0);

                        int xOff = (k == 0 || k == 1) ? -r : 0;
                        int yOff = (k < 2) ? -r : 0;

                        for (int x = 0; x <= r; x++)
                            for (int y = 0; y <= r; y++)
                            {
                                float2 offsetUV = float2(xOff + x, yOff + y) * _MainTex_TexelSize.xy;
                                float3 col = tex2Dlod(_MainTex, float4(i.uv + offsetUV, 0, 0)).rgb;
                                mean += col;
                                sigma += col * col;
                            }

                        mean /= n;
                        sigma = abs(sigma / n - mean * mean);
                        float varianceSum = sigma.r + sigma.g + sigma.b;

                        if (varianceSum < minSigma)
                        {
                            minSigma = varianceSum;
                            bestMean = mean;
                        }
                    }

                    return float4(bestMean, 1);
                }
                ENDCG
            }
        }
}

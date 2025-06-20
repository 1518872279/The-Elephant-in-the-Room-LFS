# Oil Painting Camera Shader for URP

## Overview
This guide outlines how to implement an **Oil Painting** post-processing effect in Unity’s Universal Render Pipeline using:
- **Brush stroke sampling** for painterly averaging  
- **Color posterization** for discrete tonal steps  
- **Canvas noise** for subtle grain  

You’ll use a custom HLSL shader and a Scriptable Render Feature.

---

## 1. Blit Shader (`OilPaintEffect.shader`)
```hlsl
Shader "Hidden/OilPaintEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BrushSize ("Brush Size", Float) = 3
        _ColorSteps ("Color Steps", Float) = 8
        _NoiseStrength ("Noise Strength", Float) = 0.15
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BrushSize;
            float _ColorSteps;
            float _NoiseStrength;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Simple hash for noise
            float hash21(float2 p) {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;
                float2 offsetStep = _MainTex_TexelSize.xy * _BrushSize;

                float4 colorSum = 0;
                float weightSum = 0;
                [unroll(5)]
                for (int x = -2; x <= 2; x++) {
                    [unroll(5)]
                    for (int y = -2; y <= 2; y++) {
                        float2 off = offsetStep * float2(x, y);
                        float4 c = tex2D(_MainTex, uv + off);
                        float w = 1.0 - length(off);
                        colorSum += c * w;
                        weightSum += w;
                    }
                }

                float4 avgColor = colorSum / weightSum;
                avgColor.rgb = floor(avgColor.rgb * _ColorSteps) / _ColorSteps;
                float noise = hash21(uv * 1024) * _NoiseStrength;
                avgColor.rgb += noise;

                return avgColor;
            }
            ENDHLSL
        }
    }
}
```

---

## 2. Render Feature Script (`OilPaintRenderFeature.cs`)
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OilPaintRenderFeature : ScriptableRendererFeature {
    class CustomRenderPass : ScriptableRenderPass {
        Material mat;
        RenderTargetHandle tempRT;
        RenderTargetIdentifier src;

        public CustomRenderPass(Material m) {
            mat = m;
            tempRT.Init("_TempOilPaintTex");
        }

        public void Setup(RenderTargetIdentifier source) {
            src = source;
        }

        public override void Execute(ScriptableRenderContext ctx, ref RenderingData data) {
            var cmd = CommandBufferPool.Get("OilPaintEffect");
            var desc = data.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempRT.id, desc);
            Blit(cmd, src, tempRT.Identifier(), mat);
            Blit(cmd, tempRT.Identifier(), src);
            ctx.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Shader shader;
    public float brushSize = 3f;
    public float colorSteps = 8f;
    public float noiseStrength = 0.15f;

    CustomRenderPass pass;
    Material material;

    public override void Create() {
        if (shader == null) return;
        material = CoreUtils.CreateEngineMaterial(shader);
        pass = new CustomRenderPass(material) {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data) {
        pass.Setup(renderer.cameraColorTarget);
        material.SetFloat("_BrushSize", brushSize);
        material.SetFloat("_ColorSteps", colorSteps);
        material.SetFloat("_NoiseStrength", noiseStrength);
        renderer.EnqueuePass(pass);
    }
}
```

---

## 3. Usage
1. **Import** both `OilPaintEffect.shader` and `OilPaintRenderFeature.cs` into your Unity project.  
2. **Assign** the shader in the Render Feature component on your URP asset.  
3. **Tweak** the parameters:
   - **Brush Size** (`_BrushSize`) for stroke width  
   - **Color Steps** (`_ColorSteps`) for posterization level  
   - **Noise Strength** (`_NoiseStrength`) for canvas grain  

---

*Optional:* If you prefer **Shader Graph**, you can recreate the same logic using:
- **Scene Color** node  
- **Tile Sampler** or multiple Scene Color samples for brush stroke  
- **Quantize** step for color posterization  
- **Custom Function** for noise  

---

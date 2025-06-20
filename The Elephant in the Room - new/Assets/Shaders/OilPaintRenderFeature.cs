using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OilPaintRenderFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        Material mat;
#pragma warning disable 618
        RenderTargetHandle tempRT;
#pragma warning restore 618

        public CustomRenderPass(Material material)
        {
            mat = material;
#pragma warning disable 618
            tempRT.Init("_TempOilPaintTex");
#pragma warning restore 618
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (mat == null)
                return;

            var cmd = CommandBufferPool.Get("OilPaintEffect");
            var desc = renderingData.cameraData.cameraTargetDescriptor;
#pragma warning disable 618
            cmd.GetTemporaryRT(tempRT.id, desc);
#pragma warning restore 618

            var source = renderingData.cameraData.renderer.cameraColorTarget;

            // Run effect
            Blit(cmd, source, tempRT.Identifier(), mat);
            // Copy back
            cmd.CopyTexture(tempRT.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
#pragma warning disable 618
            cmd.ReleaseTemporaryRT(tempRT.id);
#pragma warning restore 618
        }
    }

    [Header("Feature Toggles")]
    public bool enableOilPaint = true;
    public bool enableColorQuantize = true;
    public bool enableCanvasGrain = true; // now only using bump, color ignored
    public bool enableBumpAndNoise = true;
    public bool enableContrast = true;
    public bool enableReflection = true;

    [Header("Oil Paint Settings")]
    public Shader shader;
    [Range(1, 10)] public float brushSize = 3f;
    public float colorSteps = 8f;
    [Range(0, .5f)] public float noiseStrength = 0.15f;

    [Header("Canvas Bump & Procedural Noise")]
    public Texture2D bumpMap;
    [Range(1, 50)] public float bumpTiling = 10f;
    [Range(0, 1)] public float bumpInfluence = 0.5f;
    [Range(1, 16384)] public float grainNoiseFreq = 8192f;
    [Range(0, 1)] public float noiseInfluence = 0.05f;
    [Range(0, .2f)] public float grainStrength = 0.08f; // final strength of bump-based grain

    [Header("Color & Reflection Controls")]
    [Range(0.5f, 2f)] public float contrast = 1f;
    [Range(0f, 1f)] public float reflectThreshold = 0.9f;
    [Range(0f, 1f)] public float reflectAttenuation = 0.2f;

    [Header("Lift-Gamma-Gain Controls")]
    [Range(0f, 0.5f)] public float lift = 0.05f;
    [Range(0.5f, 2f)] public float gamma = 1f;
    [Range(0.5f, 1.5f)] public float gain = 1f;

    [Header("Saturation Control")]
    [Range(0f, 2f)] public float saturation = 1f;

    CustomRenderPass pass;
    Material material;

    public override void Create()
    {
        if (shader == null)
        {
            Debug.LogWarning("OilPaint shader not assigned.");
            return;
        }
        material = CoreUtils.CreateEngineMaterial(shader);
        pass = new CustomRenderPass(material)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null) return;

        // Toggles
        material.SetFloat("_EnableOilPaint", enableOilPaint ? 1f : 0f);
        material.SetFloat("_EnableQuantize", enableColorQuantize ? 1f : 0f);
        material.SetFloat("_EnableGrain", enableCanvasGrain ? 1f : 0f);
        material.SetFloat("_EnableBumpNoise", enableBumpAndNoise ? 1f : 0f);
        material.SetFloat("_EnableContrast", enableContrast ? 1f : 0f);
        material.SetFloat("_EnableReflection", enableReflection ? 1f : 0f);

        // Paint params
        if (enableOilPaint)
        {
            material.SetFloat("_BrushSize", brushSize);
            material.SetFloat("_ColorSteps", colorSteps);
            material.SetFloat("_PainterNoise", noiseStrength);
        }

        // Bump + procedural noise as grain (color ignored)
        if (enableCanvasGrain && bumpMap != null)
        {
            material.SetTexture("_BumpMap", bumpMap);
            material.SetFloat("_BumpTiling", bumpTiling);
            material.SetFloat("_BumpInfluence", bumpInfluence);
            material.SetFloat("_NoiseFreq", grainNoiseFreq);
            material.SetFloat("_NoiseInfluence", noiseInfluence);
            material.SetFloat("_GrainStrength", grainStrength);
        }

        // Contrast & reflection
        material.SetFloat("_Contrast", contrast);
        material.SetFloat("_ReflectThreshold", reflectThreshold);
        material.SetFloat("_ReflectAttenuation", reflectAttenuation);

        // Lift-Gamma-Gain
        material.SetFloat("_Lift", lift);
        material.SetFloat("_Gamma", gamma);
        material.SetFloat("_Gain", gain);

        // Saturation
        material.SetFloat("_Saturation", saturation);

        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && material != null)
        {
            CoreUtils.Destroy(material);
            material = null;
        }
    }
}

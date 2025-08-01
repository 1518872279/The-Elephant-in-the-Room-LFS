using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OilPaintRenderFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        Material mat;
        RenderTargetHandle tempRT;

        public CustomRenderPass(Material material)
        {
            mat = material;
            tempRT.Init("_TempOilPaintTex");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Only run on Game and SceneView cameras
            var camType = renderingData.cameraData.cameraType;
            if (camType != CameraType.Game && camType != CameraType.SceneView)
                return;

            if (mat == null)
                return;

            var cmd = CommandBufferPool.Get("OilPaintEffect");

            // 1) Grab a copy of the camera's descriptor, zero out its depth buffer
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            // 2) Allocate a temp RT matching that descriptor, with bilinear filtering
            cmd.GetTemporaryRT(tempRT.id, desc, FilterMode.Bilinear);

            // 3) Run the two-pass blit
            var source = renderingData.cameraData.renderer.cameraColorTarget;
            Blit(cmd, source, tempRT.Identifier(), mat);
            Blit(cmd, tempRT.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
            cmd.ReleaseTemporaryRT(tempRT.id);
        }
    }

    [Header("Feature Toggles")]
    public bool enableOilPaint = true;
    public bool enableColorQuantize = true;
    public bool enableCanvasGrain = true;
    public bool enableBumpAndNoise = false;
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
    [Range(0, .2f)] public float grainStrength = 0.08f;

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
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null || !enableOilPaint)
            return;

        // Update shader keywords & properties:
        material.SetFloat("_EnableQuantize", enableColorQuantize ? 1f : 0f);
        material.SetFloat("_EnableGrain", enableCanvasGrain ? 1f : 0f);
        material.SetFloat("_EnableBumpNoise", enableBumpAndNoise ? 1f : 0f);
        material.SetFloat("_EnableContrast", enableContrast ? 1f : 0f);
        material.SetFloat("_EnableReflection", enableReflection ? 1f : 0f);

        material.SetFloat("_BrushSize", brushSize);
        material.SetFloat("_ColorSteps", colorSteps);
        material.SetFloat("_PainterNoise", noiseStrength);

        if (bumpMap != null)
        {
            material.SetTexture("_BumpMap", bumpMap);
            material.SetFloat("_BumpTiling", bumpTiling);
            material.SetFloat("_BumpInfluence", bumpInfluence);
            material.SetFloat("_NoiseFreq", grainNoiseFreq);
            material.SetFloat("_NoiseInfluence", noiseInfluence);
            material.SetFloat("_GrainStrength", grainStrength);
        }

        material.SetFloat("_Contrast", contrast);
        material.SetFloat("_ReflectThreshold", reflectThreshold);
        material.SetFloat("_ReflectAttenuation", reflectAttenuation);

        material.SetFloat("_Lift", lift);
        material.SetFloat("_Gamma", gamma);
        material.SetFloat("_Gain", gain);

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

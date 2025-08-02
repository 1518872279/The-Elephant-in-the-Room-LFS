using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuwaharaRenderFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        const string k_ProfilerTag = "Kuwahara Filter";
        RenderTargetHandle m_TempRT;
        Material m_Material;
        int m_Radius;
        float m_Strength;

        public CustomRenderPass(Material mat)
        {
            m_Material = mat;
            m_TempRT.Init("_TempKuwaharaTex");
            profilingSampler = new ProfilingSampler(k_ProfilerTag);
        }

        public void SetParameters(int radius, float strength)
        {
            m_Radius = radius;
            m_Strength = strength;
            m_Material.SetInt("_Radius", m_Radius);
            m_Material.SetFloat("_Strength", m_Strength);
        }

        // Called once per camera before Execute
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Copy the camera descriptor, but zero out depth bits
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            // Allocate a temporary RT matching the camera's color buffer
            cmd.GetTemporaryRT(m_TempRT.id, desc, FilterMode.Bilinear);

            // Tell URP that this pass will render into our temp RT
            ConfigureTarget(m_TempRT.Identifier());
            ConfigureClear(ClearFlag.None, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_Material == null)
                return;

            var cmd = CommandBufferPool.Get(k_ProfilerTag);
            
            // Set the material's keywords to match the current rendering state
            var cameraData = renderingData.cameraData;
            if (cameraData.xrRendering)
            {
                m_Material.EnableKeyword("UNITY_SINGLE_PASS_STEREO");
                m_Material.EnableKeyword("STEREO_INSTANCING_ON");
            }
            else
            {
                m_Material.DisableKeyword("UNITY_SINGLE_PASS_STEREO");
                m_Material.DisableKeyword("STEREO_INSTANCING_ON");
            }

            var cameraColor = renderingData.cameraData.renderer.cameraColorTarget;

            // 1) Blit from the camera's color buffer into our temp RT with the filter
            Blit(cmd, cameraColor, m_TempRT.Identifier(), m_Material);
            // 2) Blit back from temp RT into the camera's color buffer
            Blit(cmd, m_TempRT.Identifier(), cameraColor);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Release the RT we allocated
            cmd.ReleaseTemporaryRT(m_TempRT.id);
        }
    }

    [Header("Kuwahara Filter Settings")]
    public Shader shader;
    [Range(1, 10)] public int radius = 3;
    [Range(0.001f, 0.05f)] public float strength = 0.01f;

    CustomRenderPass m_ScriptablePass;
    Material m_Material;

    public override void Create()
    {
        if (shader == null)
        {
            Debug.LogWarning("Kuwahara shader not assigned in RenderFeature.");
            return;
        }

        m_Material = CoreUtils.CreateEngineMaterial(shader);
        m_ScriptablePass = new CustomRenderPass(m_Material)
        {
            // Run just before URP's post-processing kicks in
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    // Here we pass in the current radius each frame
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_Material == null)
            return;

        m_ScriptablePass.SetParameters(radius, strength);
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && m_Material != null)
        {
            CoreUtils.Destroy(m_Material);
            m_Material = null;
        }
    }
}

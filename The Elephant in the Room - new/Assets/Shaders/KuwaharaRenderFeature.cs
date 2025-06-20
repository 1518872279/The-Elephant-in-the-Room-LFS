using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuwaharaRenderFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material material;
#pragma warning disable 618
        private RenderTargetHandle tempRT;
#pragma warning restore 618

        public CustomRenderPass(Material mat)
        {
            this.material = mat;
#pragma warning disable 618
            tempRT.Init("_TempKuwaharaTex");
#pragma warning restore 618
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;

            var cmd = CommandBufferPool.Get("KuwaharaFilter");
            var desc = renderingData.cameraData.cameraTargetDescriptor;
#pragma warning disable 618
            cmd.GetTemporaryRT(tempRT.id, desc);
#pragma warning restore 618

            // Source and destination RT
            var source = renderingData.cameraData.renderer.cameraColorTarget;

            // 1) Apply Kuwahara filter into tempRT
            Blit(cmd, source, tempRT.Identifier(), material);
            // 2) Copy back to camera target
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

    [Header("Kuwahara Filter Settings")]
    public Shader shader;
    [Tooltip("Radius of the Kuwahara filter sectors.")]
    [Range(1, 10)] public int radius = 3;

    private CustomRenderPass pass;
    private Material material;

    public override void Create()
    {
        if (shader == null)
        {
            Debug.LogWarning("Kuwahara shader not assigned in Render Feature.");
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
        if (material == null)
            return;

        // Set the radius property on the material
        material.SetInt("_Radius", radius);

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

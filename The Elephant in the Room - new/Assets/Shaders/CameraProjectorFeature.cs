using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraProjectorFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Shader projectorShader = null;
        public Texture2D projectorTexture = null;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public Settings settings = new Settings();

    class ProjectorPass : ScriptableRenderPass
    {
        Material mat;
        RenderTargetHandle tempRT;
        Texture projTex;

        public ProjectorPass(Material material, Texture texture)
        {
            mat = material;
            projTex = texture;
            tempRT.Init("_TempProjectorRT");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // no Configure target here; we'll just Blit directly
        }

        public override void Execute(ScriptableRenderContext ctx, ref RenderingData data)
        {
            if (mat == null || projTex == null)
                return;

            // The camera URP is currently rendering
            Camera cam = data.cameraData.camera;

            // Compute projector VP
            Matrix4x4 vp = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false)
                         * cam.worldToCameraMatrix;
            mat.SetMatrix("_ProjectorVP", vp);
            mat.SetTexture("_ProjectorTex", projTex);

            var cmd = CommandBufferPool.Get("CameraProjectorURP");
            var desc = data.cameraData.cameraTargetDescriptor;

            // Grab a temp RT
            cmd.GetTemporaryRT(tempRT.id, desc, FilterMode.Bilinear);

            // The source is the camera¡¯s color target
            var src = data.cameraData.renderer.cameraColorTarget;

            // Draw full-screen triangle
            Blit(cmd, src, tempRT.Identifier(), mat, 0);
            Blit(cmd, tempRT.Identifier(), src);

            ctx.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempRT.id);
        }
    }

    ProjectorPass _pass;
    Material _material;

    public override void Create()
    {
        if (settings.projectorShader == null)
        {
            Debug.LogWarning("CameraProjectorFeature: missing shader.");
            return;
        }

        _material = CoreUtils.CreateEngineMaterial(settings.projectorShader);
        _pass = new ProjectorPass(_material, settings.projectorTexture)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    // Inject into the renderer
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (_material != null && settings.projectorTexture != null)
            renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && _material != null)
            CoreUtils.Destroy(_material);
    }
}

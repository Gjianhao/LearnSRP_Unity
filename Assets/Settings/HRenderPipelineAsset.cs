using UnityEngine;
using UnityEngine.Rendering;

namespace Settings {
    [CreateAssetMenu(menuName = "LearnSRP_Unity/HRenderPipelineAsset")]
    public class HRenderPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline() {
            return new HRenderPipeline();
        }
    }

    class HRenderPipeline : RenderPipeline {
        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            foreach (var camera in cameras) {
                RenderPerCamera(context, camera);
            }
            context.Submit();
        }

        private void RenderPerCamera(ScriptableRenderContext context, Camera camera) {
            // 将camera相关参数，设置到渲染管线中
            context.SetupCameraProperties(camera);
            // 对场景进行裁剪
            camera.TryGetCullingParameters(out var cullingParameters);
            var cullingResults = context.Cull(ref cullingParameters);
        }
    }
}
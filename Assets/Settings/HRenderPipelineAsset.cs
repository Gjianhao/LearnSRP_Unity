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
        private ShaderTagId _shaderTag = new ShaderTagId("ForwardBase");
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

            var drawSetting = CreateDrawingSettings(camera);
            var filterSetting = new FilteringSettings(RenderQueueRange.all);
            // 绘制物体
            context.DrawRenderers(cullingResults, ref drawSetting, ref filterSetting);
        }

        private DrawingSettings CreateDrawingSettings(Camera camera) {
            // 创建一个排序设置结构体
            var sortingSetting = new SortingSettings(camera);
            // 创建一个绘制设置结构体
            var drawSetting = new DrawingSettings(_shaderTag, sortingSetting);
            return drawSetting;
        }
    }
}
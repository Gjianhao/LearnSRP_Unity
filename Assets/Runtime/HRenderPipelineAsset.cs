using Runtime.Pass;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime {
    [CreateAssetMenu(menuName = "LearnSRP_Unity/HRenderPipelineAsset")]
    public class HRenderPipelineAsset : RenderPipelineAsset {
        private bool _srpBatcher = true;

        public bool SrpBatcher => _srpBatcher;
        protected override RenderPipeline CreatePipeline() {
            return new HRenderPipeline();
        }
    }

    class HRenderPipeline : RenderPipeline {
        // 这里的name值对应Shader里的Tag，可以使用内置管线里的 “ForwardBase”
        private ShaderTagId _shaderTag = new ShaderTagId("HForwardBase");
        private LightConfigurator _lightConfigurator = new LightConfigurator();
        private ShadowCasterPass _shadowCasterPass = new ShadowCasterPass();
        private CommandBuffer _command = new CommandBuffer();

        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            foreach (var camera in cameras) {
                RenderPerCamera(context, camera);
                // 设置绘制天空盒的指令，这里只是将指令进行了缓冲
                context.DrawSkybox(camera);
            }

            context.Submit();
        }

        private void RenderPerCamera(ScriptableRenderContext context, Camera camera) {
            // 将camera相关参数，设置到渲染管线中
            context.SetupCameraProperties(camera);
            // 对场景进行裁剪 获取用于相机视锥体剔除相关的数据
            camera.TryGetCullingParameters(out var cullingParameters);
            var cullingResults = context.Cull(ref cullingParameters);
            _lightConfigurator.SetupShaderLightingParams(context, ref cullingResults);

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
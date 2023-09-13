using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Pass {
    public class ShadowCasterPass {
        private CommandBuffer _commandBuffer = new CommandBuffer();
        private ShadowMapTextureHandler _shadowMapTextureHandler = new ShadowMapTextureHandler();

        public ShadowCasterPass() {
            _commandBuffer.name = "HShadowCaster";
        }

        // 灯光的阴影分辨率
        private static int GetShadowMapResolution(Light light) {
            switch (light.shadowResolution) {
                case LightShadowResolution.VeryHigh:
                    return 2048;
                case LightShadowResolution.High:
                    return 1024;
                case LightShadowResolution.Medium:
                    return 512;
                case LightShadowResolution.Low:
                    return 256;
            }

            return 256;
        }

        private void SetupShadowCasterView(ScriptableRenderContext context, int shadowMapResolution, ref Matrix4x4 matrixView, ref Matrix4x4 matrixProj) {
            _commandBuffer.Clear();
            _commandBuffer.SetViewport(new Rect(0, 0, shadowMapResolution, shadowMapResolution));
            // 设置观察和投影矩阵
            
            // 设置渲染目标
            
            // Clear贴图
            
            // 执行命令缓冲
            
        }

        public class ShadowMapTextureHandler {
            private RenderTargetIdentifier _renderTargetIdentifier = "_HMainShadowMap";
            private int _shadowmapId = Shader.PropertyToID("_HMainShadowMap");
            private RenderTexture _shadowmapTexture;

            public RenderTargetIdentifier RenderTargetIdentifier {
                get { return _renderTargetIdentifier; }
            }

            public void AcquireRenderTextureIfNot(int resolution) {
                if (_shadowmapTexture && _shadowmapTexture.width != resolution) {
                    RenderTexture.ReleaseTemporary(_shadowmapTexture);
                    _shadowmapTexture = null;
                }

                if (!_shadowmapTexture) {
                    _shadowmapTexture = RenderTexture.GetTemporary(resolution, resolution, 16, RenderTextureFormat.Shadowmap);
                    Shader.SetGlobalTexture(ShaderProperties.MainShadowMap, _shadowmapTexture);
                    _renderTargetIdentifier = new RenderTargetIdentifier(_shadowmapTexture);
                }
            }
        }

        public static class ShaderProperties {
            public static readonly int MainLightMatrixWorldToShadowSpace = Shader.PropertyToID("_HMainLightMatrixWorldToShadowMap");
            public static readonly int ShadowParams = Shader.PropertyToID("_ShadowParams");
            public static readonly int MainShadowMap = Shader.PropertyToID("_HMainShadowMap");
        }
    }
}
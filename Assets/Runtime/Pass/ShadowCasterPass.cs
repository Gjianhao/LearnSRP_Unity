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
            _commandBuffer.SetViewProjectionMatrices(matrixView, matrixProj);
            // 设置渲染目标
            _commandBuffer.SetRenderTarget(_shadowMapTextureHandler.RenderTargetIdentifier, _shadowMapTextureHandler.RenderTargetIdentifier);
            // Clear贴图
            _commandBuffer.ClearRenderTarget(true, true, Color.black, 1);
            // 执行命令缓冲
            context.ExecuteCommandBuffer(_commandBuffer);
        }

        /// 得到光线空间下的投影矩阵，其对应的x,y,z范围均为(-1,1)
        /// 因此我们需要构建坐标变换矩阵，可以将世界坐标转换到ShadowMap齐次坐标空间。对应的xy范围为(0,1),z的范围为(1,0)
        static Matrix4x4 GetWorldToShadowMapSpaceMatrix(Matrix4x4 proj, Matrix4x4 view) {
            // 检查平台是否zBuffer反转，一般情况下，z轴方向是朝屏幕内，近小远大，但是在zBuffer反转的情况下，z轴是朝屏幕外，近大远小
            if (SystemInfo.usesReversedZBuffer) {
                proj.m20 = -proj.m20;
                proj.m21 = -proj.m21;
                proj.m22 = -proj.m22;
                proj.m23 = -proj.m23;
            }

            Matrix4x4 worldToShadow = proj * view;
            var textureScaleAndBias = Matrix4x4.identity;
            textureScaleAndBias.m00 = 0.5f;
            textureScaleAndBias.m11 = 0.5f;
            textureScaleAndBias.m22 = 0.5f;
            textureScaleAndBias.m03 = 0.5f;
            textureScaleAndBias.m23 = 0.5f;
            textureScaleAndBias.m13 = 0.5f;

            return textureScaleAndBias * worldToShadow;
        }

        public void Execute(ScriptableRenderContext context, Camera camera, ref CullingResults cullingResults, ref LightConfigurator.LightData lightData) {
            
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
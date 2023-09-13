using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Pass {
    public class ShadowCasterPass {
        private CommandBuffer _commandBuffer = new CommandBuffer();
        private ShadowMapTextureHandler _shadowMapTextureHandler = new ShadowMapTextureHandler();
        
        public class ShadowMapTextureHandler {
            private RenderTargetIdentifier _renderTargetIdentifier = "_HMainShadowMap";
            private int _shadowmapId = Shader.PropertyToID("_HMainShadowMap");
            private RenderTexture _shadowmapTexture;

            public RenderTargetIdentifier RenderTargetIdentifier {
                get {
                    return _renderTargetIdentifier;
                }
            }
        }
    }

    
}

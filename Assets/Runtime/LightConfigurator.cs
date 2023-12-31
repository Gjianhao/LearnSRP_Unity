﻿using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime {
    public class LightConfigurator {
        private static int CompareLightRenderMode(LightRenderMode mode1, LightRenderMode mode2) {
            if (mode1 == mode2) {
                return 0;
            }

            if (mode1 == LightRenderMode.ForcePixel) {
                return -1;
            }

            if (mode2 == LightRenderMode.ForcePixel) {
                return 1;
            }

            if (mode1 == LightRenderMode.Auto) {
                return -1;
            }

            if (mode2 == LightRenderMode.Auto) {
                return 1;
            }

            return 0;
        }

        // 如果有多个平行光，按照LightRenderMode、intensity对齐排序
        private static int CompareLight(Light light1, Light light2) {
            // LightRenderMode相同，则按照intensity(强度)优先级排序
            if (light1.renderMode == light2.renderMode) {
                return (int)Mathf.Sign(light2.intensity - light1.intensity);
            }

            var ret = CompareLightRenderMode(light1.renderMode, light2.renderMode);
            if (ret == 0) {
                ret = (int)Mathf.Sign(light2.intensity - light1.intensity);
            }

            return ret;
        }

        private static int GetMainLightIndex(NativeArray<VisibleLight> lights) {
            Light mainLight = null;
            var mainLightIndex = -1;
            var index = 0;
            foreach (var light in lights) {
                if (light.lightType == LightType.Directional) {
                    var lightComp = light.light;
                    if (lightComp.renderMode == LightRenderMode.ForceVertex) {
                        continue;
                    }

                    if (!mainLight) {
                        mainLight = lightComp;
                        mainLightIndex = index;
                    }
                    else {
                        if (CompareLight(mainLight, lightComp) > 0) {
                            mainLight = lightComp;
                            mainLightIndex = index;
                        }
                    }
                }

                index++;
            }

            return mainLightIndex;
        }

        private int _mainLightIndex = -1;
        public LightData SetupShaderLightingParams(ScriptableRenderContext context, ref CullingResults cullingResults) {
            var visibleLights = cullingResults.visibleLights;
            var mainLightIndex = GetMainLightIndex(visibleLights);
            if (mainLightIndex >= 0) {
                var mainLight = visibleLights[mainLightIndex];
                // 注意这里的光源方向是负的
                var forward = - (Vector4)mainLight.light.gameObject.transform.forward;
                Shader.SetGlobalVector(ShaderProperties.MainLightDirection, forward);
                Shader.SetGlobalColor(ShaderProperties.MainLightColor, mainLight.finalColor);
            }
            else {
                Shader.SetGlobalColor(ShaderProperties.MainLightColor, Color.black);
            }

            Shader.SetGlobalColor(ShaderProperties.AmbientColor, RenderSettings.ambientLight);
            _mainLightIndex = mainLightIndex;
            return new LightData() {
                mainLightIndex = mainLightIndex,
                mainLight = mainLightIndex >= 0 && mainLightIndex < visibleLights.Length ? visibleLights[mainLightIndex] : default(VisibleLight)
            };
        }

        public class ShaderProperties {
            public static int MainLightDirection = Shader.PropertyToID("_XMainLightDirection");
            public static int MainLightColor = Shader.PropertyToID("_XMainLightColor");
            public static int AmbientColor = Shader.PropertyToID("_XAmbientColor");
        }
        public struct LightData {
            public int mainLightIndex;
            public VisibleLight mainLight;

            public bool HasMainLight() {
                return mainLightIndex >= 0;
            }
        }
    }
}
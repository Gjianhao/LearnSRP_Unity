#include "HLSLSupport.cginc"
CBUFFER_START(UnityLighting)
// 环境光
half4 _XAmbientColor;
// 主灯光方向
float4 _XMainLightDirection;
// 主灯光颜色
half4 _XMainLightColor;
CBUFFER_END

// 漫反射部分
half4 LambertDiffuse(float3 normal) {
    return max(0, dot(normal, _XMainLightDirection.xyz)) * _XMainLightColor;
}

// 高光部分
half4 BlinnPhongSpecular(float3 viewDir, float3 normal, float shininess) {
    float3 H = normalize(viewDir + _XMainLightDirection);
    float NH = max(0, dot(H, normal));
    return pow(NH, shininess) * _XMainLightColor;
}
// BlinnPhong光照模型
half4 BlinnPhongLight(float3 positionWS, float3 normalWS, float shininess, half4 diffuseColor, half4 specular) {
    float3 V = normalize(_WorldSpaceCameraPos - positionWS);
    return _XAmbientColor + LambertDiffuse(normalWS) * diffuseColor + BlinnPhongSpecular(V, normalWS, shininess) * specular;
}

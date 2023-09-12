Shader "BlinnPhongSpecular" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Shininess ("Shininess", Range(10, 128)) = 50
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1)
    }
    SubShader {
        Tags {
            "RenderType" = "Opaque"
            "LightMode" = "HForwardBase"
        }
        LOD 100

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_cbuffer

            #include "UnityCG.cginc"
            #include "BlinnPhone.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS: NORMAL;
            };

            struct Varyings {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            UNITY_DECLARE_TEX2D(_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _Shininess;
            CBUFFER_END

            Varyings vert(Attributes v) {
                Varyings o;
                o.positionCS = UnityObjectToClipPos(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = mul(unity_ObjectToWorld, float4(v.normalOS, 0.0)).xyz;
                o.positionWS = mul(unity_ObjectToWorld, v.positionOS).xyz;
                return o;
            }

            half4 frag(Varyings i) : SV_Target {
                half4 diffuseColor = UNITY_SAMPLE_TEX2D(_MainTex, i.uv);
                float3 positionWS = i.positionWS;
                float3 normalWS = i.normalWS;
                half4 color = 1;
                return color;
            }
            ENDHLSL
        }
    }
}
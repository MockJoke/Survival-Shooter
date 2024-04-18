Shader "Custom/Rim Lit Bumped Specular Dissolve URP" {
    
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ActualMainTex("Main Texture", 2D) = "white" {}
        _MainTex("Dissolve Shape", 2D) = "white" {}
        _Cutoff("Dissolve Value", Range(0, 1)) = 0
        _LineWidth("Line Width", Range(0, 0.2)) = 0
        _LineColor("Line Color", Color) = (1, 1, 1, 1)
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range(0.03,1)) = 0.078125
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _RimColor ("Rim Color", Color) = (0.2, 0.2, 0.2, 0.0)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
    }
    
    SubShader {
        Tags {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="TransparentCutout"
        }
        
        Pass {
            Name "UniversalForward"
            Tags {
                "LightMode"="UniversalForward"
            }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float2 uv_ActualMainTex : TEXCOORD0;
                float2 uv_MainTex : TEXCOORD1;
                float2 uv_BumpMap : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };
            
            fixed4 _Color;
            sampler2D _ActualMainTex;
            sampler2D _MainTex;
            float _Cutoff;
            float _LineWidth;
            fixed4 _LineColor;
            fixed4 _SpecColor;
            float _Shininess;
            sampler2D _BumpMap;
            float4 _RimColor;
            float _RimPower;
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_ActualMainTex = v.uv;
                o.uv_MainTex = v.uv;
                o.uv_BumpMap = v.uv;
                o.viewDir = normalize(UnityWorldSpaceViewDir(v.vertex));
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target {
                fixed4 mainTexColor = tex2D(_ActualMainTex, IN.uv_ActualMainTex);
                fixed4 baseColor = mainTexColor * _Color;

                half4 dissolve = tex2D(_MainTex, IN.uv_MainTex);
                float isAtLeastLine = 1.0 - (dissolve.a - _Cutoff) + _LineWidth;
                baseColor.rgb = lerp(baseColor.rgb, _LineColor.rgb, isAtLeastLine);

                half rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.normal));
                baseColor.rgb += _RimColor.rgb * pow(rim, _RimPower) + lerp(half4(0, 0, 0, 0), _LineColor, isAtLeastLine);

                clip(dissolve.a - _Cutoff);

                return baseColor;
            }
            ENDCG
        }
    }

    Fallback "Transparent/Cutout/VertexLit"
}
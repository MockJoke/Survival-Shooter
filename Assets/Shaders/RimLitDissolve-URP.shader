Shader "Custom/Rim Lit Dissolve URP" {
    
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex("Dissolve Shape", 2D) = "white" {}
        _Cutoff("Dissolve Value", Range(0, 1)) = 0
        _LineWidth("Line Width", Range(0, 0.2)) = 0
        _LineColor("Line Color", Color) = (1, 1, 1, 1)
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
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };
            
            fixed4 _Color;
            sampler2D _MainTex;
            float _Cutoff;
            float _LineWidth;
            fixed4 _LineColor;
            float4 _RimColor;
            float _RimPower;
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.viewDir = normalize(UnityWorldSpaceViewDir(v.vertex));
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target {
                fixed4 baseColor = tex2D(_MainTex, IN.uv) * _Color;
                float rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.normal));
                fixed4 rimColor = _RimColor * pow(rim, _RimPower);
                baseColor.rgb += rimColor.rgb;

                half4 dissolve = tex2D(_MainTex, IN.uv);
                float isAtLeastLine = 1.0 - (dissolve.a - _Cutoff) + _LineWidth;
                baseColor.rgb = lerp(baseColor.rgb, _LineColor.rgb, isAtLeastLine);

                clip(dissolve.a - _Cutoff);

                return baseColor;
            }
            ENDCG
        }
    }

    Fallback "Transparent/Cutout/VertexLit"
}
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LD CrewBoom/Environment"
{
    Properties
    {
        [HideInInspector] [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Blend mode Source", Int) = 1
        [HideInInspector] [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend mode Destination", Int) = 0
        [HideInInspector] _ZWrite ("ZWrite", Float) = 1.0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2.0
        [HideInInspector] [KeywordEnum(Opaque, Cutout, Transparent)] _Transparency ("Transparency", Float) = 0
        [HideInInspector] _CutOut("Alpha Cutout", Range(0,1)) = 0.1
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Emission ("Emission", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend [_BlendSrc] [_BlendDst]
        ZWrite [_ZWrite]
        Cull [_Cull]

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma shader_feature _TRANSPARENCY_OPAQUE _TRANSPARENCY_CUTOUT _TRANSPARENCY_TRANSPARENT
            #pragma vertex vert
            #pragma fragment frag
            

            #include "BRCCommon.cginc"
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
                SHADOW_COORDS(2) // put shadows data into TEXCOORD1
                float2 uv2 : TEXCOORD3;
                float4 color : COLOR0;
            };

            BRC_LIGHTING_PROPERTIES;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Emission;
            float4 _Emission_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _Emission);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.color = v.color * _Color;
                TRANSFER_SHADOW(o)
                return o;
            }

            float _CutOut;

            fixed4 frag(v2f i) : SV_Target
            {
                BRC_LIGHTING_FRAGMENT;
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * BRCLighting;
                fixed3 emissionCol = tex2D(_Emission, i.uv2).rgb;
                col.rgb += emissionCol.rgb;
                #if _TRANSPARENCY_CUTOUT
                    clip(col.a - _CutOut);
                #endif
                return col;
            }
            ENDCG
        }
        Pass
        {
            Tags {"LightMode" = "ShadowCaster"}

            CGPROGRAM
            #pragma shader_feature _TRANSPARENCY_OPAQUE _TRANSPARENCY_CUTOUT _TRANSPARENCY_TRANSPARENT
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

                struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            sampler2D _MainTex;
            float _CutOut;

            float4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                #if _TRANSPARENCY_CUTOUT
                    clip(col.a - _CutOut);
                #endif
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    CustomEditor "EnvironmentMaterialEditor"
}
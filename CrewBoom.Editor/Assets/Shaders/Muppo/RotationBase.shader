// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Muppo/RotationBase"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.1
		[NoScaleOffset][SingleLineTexture]_MainTex("Base Color", 2D) = "gray" {}
		[NoScaleOffset][SingleLineTexture]_Emission("Emission", 2D) = "black" {}
		_MaxOutlineWidth("Max Outline Width", Float) = 0.03
		_MaxDistanceFromCamera("Max Distance From Camera", Float) = 20
		[Toggle(_MULTIPLYEMISSIVEWITHNDOTL_ON)] _MultiplyEmissiveWithNdotL("MultiplyEmissiveWithNdotL", Float) = 0
		_RotationCenter("RotationCenter", Vector) = (0.5,0.5,0,0)
		_RotationSpeed("RotationSpeed", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0"}
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float clampResult271 = clamp( distance( _WorldSpaceCameraPos , ase_worldPos ) , 0.1 , _MaxDistanceFromCamera );
			float lerpResult269 = lerp( 0.0025 , _MaxOutlineWidth , (0.0 + (clampResult271 - 0.1) * (1.0 - 0.0) / (_MaxDistanceFromCamera - 0.1)));
			float outlineVar = lerpResult269;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float4 color264 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			float mulTime358 = _Time.y * _RotationSpeed;
			float cos356 = cos( mulTime358 );
			float sin356 = sin( mulTime358 );
			float2 rotator356 = mul( i.uv_texcoord - _RotationCenter , float2x2( cos356 , -sin356 , sin356 , cos356 )) + _RotationCenter;
			float4 tex2DNode76 = tex2D( _MainTex, rotator356 );
			o.Emission = (color264).rgb;
			clip( tex2DNode76.a - _Cutoff );
		}
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma shader_feature_local _MULTIPLYEMISSIVEWITHNDOTL_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _MainTex;
		uniform float2 _RotationCenter;
		uniform float _RotationSpeed;
		uniform sampler2D _Emission;
		uniform float _Cutoff = 0.1;
		uniform float _MaxOutlineWidth;
		uniform float _MaxDistanceFromCamera;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float mulTime358 = _Time.y * _RotationSpeed;
			float cos356 = cos( mulTime358 );
			float sin356 = sin( mulTime358 );
			float2 rotator356 = mul( i.uv_texcoord - _RotationCenter , float2x2( cos356 , -sin356 , sin356 , cos356 )) + _RotationCenter;
			float4 tex2DNode76 = tex2D( _MainTex, rotator356 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 normalizeResult170 = normalize( ase_worldNormal );
			UnityGI gi115 = gi;
			float3 diffNorm115 = normalizeResult170;
			gi115 = UnityGI_Base( data, 1, diffNorm115 );
			float3 indirectDiffuse115 = gi115.indirect.diffuse + diffNorm115 * 0.0001;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult54 = dot( normalizeResult170 , ase_worldlightDir );
			float3 temp_output_158_0 = ( ( ( indirectDiffuse115 * ase_lightColor.a ) + ( ase_lightColor.rgb * saturate( ( ( dotResult54 + 0.0 ) / 0.01 ) ) ) ) * (tex2DNode76).rgb );
			c.rgb = temp_output_158_0;
			c.a = 1;
			clip( tex2DNode76.a - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 normalizeResult170 = normalize( ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult54 = dot( normalizeResult170 , ase_worldlightDir );
			float mulTime358 = _Time.y * _RotationSpeed;
			float cos356 = cos( mulTime358 );
			float sin356 = sin( mulTime358 );
			float2 rotator356 = mul( i.uv_texcoord - _RotationCenter , float2x2( cos356 , -sin356 , sin356 , cos356 )) + _RotationCenter;
			float4 tex2DNode76 = tex2D( _MainTex, rotator356 );
			float3 temp_output_158_0 = ( ( ( float3(0,0,0) * ase_lightColor.a ) + ( ase_lightColor.rgb * saturate( ( ( dotResult54 + 0.0 ) / 0.01 ) ) ) ) * (tex2DNode76).rgb );
			o.Albedo = temp_output_158_0;
			float2 uv_Emission278 = i.uv_texcoord;
			float3 temp_output_279_0 = (tex2D( _Emission, uv_Emission278 )).rgb;
			#ifdef _MULTIPLYEMISSIVEWITHNDOTL_ON
				float3 staticSwitch354 = ( saturate( dotResult54 ) * temp_output_279_0 );
			#else
				float3 staticSwitch354 = temp_output_279_0;
			#endif
			o.Emission = staticSwitch354;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.CommentaryNode;361;1079.904,975.2173;Inherit;False;658;428.9999;;5;356;359;358;360;357;Rotation;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;226;1062.684,477.0481;Inherit;False;686.2737;430.6522;;5;54;53;170;52;115;Normals, Lighting and N.L;0.5220588,0.6044625,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;242;1828.317,453.9826;Inherit;False;1563.762;533.8073;;12;76;235;58;60;59;158;130;182;133;57;107;74;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;219;1931.6,1032.39;Inherit;False;1455.298;588.3884;;13;269;342;264;200;270;271;275;267;272;274;277;266;83;Outline;1,0.6029412,0.7097364,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;281;2175.242,36.0609;Inherit;False;1205.185;407.0481;;5;352;279;351;278;354;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;2152.846,675.2479;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;2282.911,762.0916;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;74;2435.365,711.1209;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;274;2234.42,1404.567;Float;False;Constant;_MinDistanceFromCamera;Min Distance From Camera;5;0;Create;True;0;0;0;False;0;False;0.1;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;267;2240.506,1288.796;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;271;2536.097,1337.584;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;270;2701.196,1336.397;Float;False;Property;_MaxOutlineWidth;Max Outline Width;3;0;Create;True;0;0;0;False;0;False;0.03;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;2696.034,1260.386;Float;False;Constant;_MinOutlineWidth;Min Outline Width;5;0;Create;True;0;0;0;False;0;False;0.0025;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;269;2974.445,1309.963;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;264;2569.858,1077.365;Inherit;False;Constant;_Black;Black;19;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;59;1854.399,765.0732;Float;False;Constant;_BaseCellOffset;Base Cell Offset;2;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;130;2873.839,618.2346;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;2681.994,641.4904;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;2677.692,537.1094;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;76;2453.128,789.8848;Inherit;True;Property;_MainTex;Base Color;0;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;acaf960ab70cd0d4e98880cb0e37cda0;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;235;2758.312,790.259;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;3065.895,617.4326;Inherit;True;2;2;0;FLOAT3;1,1,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;272;2239.24,1483.234;Float;False;Property;_MaxDistanceFromCamera;Max Distance From Camera;4;0;Create;True;0;0;0;False;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;342;2818.758,1106.016;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;278;2232.739,223.4655;Inherit;True;Property;_Emission;Emission;2;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;351;2250.45,128.2331;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;279;2554.668,218.5811;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;352;2786.45,126.2331;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;354;2983.859,220.0686;Inherit;False;Property;_MultiplyEmissiveWithNdotL;MultiplyEmissiveWithNdotL;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;107;2430.96,575.9693;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;1099.793,729.0509;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;52;1087.586,556.6165;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;170;1274.678,557.2952;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;115;1487.659,558.6013;Inherit;False;World;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;54;1482.084,702.6164;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3557.821,677.3474;Float;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;CrewBoom/RotationBase;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.1;True;True;0;True;Transparent;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;0;4;10;25;True;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0.03;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;;-1;0;False;_MinOutlineWidth;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TFHCRemapNode;275;2697.29,1437.256;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;277;2020.826,1437.123;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;266;1953.448,1289.038;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OutlineNode;83;3158.182,1171.678;Inherit;False;0;True;Masked;0;0;Front;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1921.473,856.7595;Float;False;Constant;_BaseCellSharpness;Base Cell Sharpness;1;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;356;1527.904,1150.217;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;359;1276.904,1153.217;Inherit;False;Property;_RotationCenter;RotationCenter;6;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;358;1313.904,1291.217;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;360;1129.904,1290.217;Inherit;False;Property;_RotationSpeed;RotationSpeed;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;1246.904,1025.217;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;362;2071.719,379.2131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;58;0;54;0
WireConnection;58;1;59;0
WireConnection;57;0;58;0
WireConnection;57;1;60;0
WireConnection;74;0;57;0
WireConnection;267;0;266;0
WireConnection;267;1;277;0
WireConnection;271;0;267;0
WireConnection;271;1;274;0
WireConnection;271;2;272;0
WireConnection;269;0;200;0
WireConnection;269;1;270;0
WireConnection;269;2;275;0
WireConnection;130;0;182;0
WireConnection;130;1;133;0
WireConnection;133;0;107;1
WireConnection;133;1;74;0
WireConnection;182;0;115;0
WireConnection;182;1;107;2
WireConnection;76;1;356;0
WireConnection;235;0;76;0
WireConnection;158;0;130;0
WireConnection;158;1;235;0
WireConnection;342;0;264;0
WireConnection;351;0;362;0
WireConnection;279;0;278;0
WireConnection;352;0;351;0
WireConnection;352;1;279;0
WireConnection;354;1;279;0
WireConnection;354;0;352;0
WireConnection;170;0;52;0
WireConnection;115;0;170;0
WireConnection;54;0;170;0
WireConnection;54;1;53;0
WireConnection;0;0;158;0
WireConnection;0;2;354;0
WireConnection;0;10;76;4
WireConnection;0;13;158;0
WireConnection;0;11;83;0
WireConnection;275;0;271;0
WireConnection;275;1;274;0
WireConnection;275;2;272;0
WireConnection;83;0;342;0
WireConnection;83;2;76;4
WireConnection;83;1;269;0
WireConnection;356;0;357;0
WireConnection;356;1;359;0
WireConnection;356;2;358;0
WireConnection;358;0;360;0
WireConnection;362;0;54;0
ASEEND*/
//CHKSM=576E024A2A94C1811EA93863861FE0E406ADCDDD
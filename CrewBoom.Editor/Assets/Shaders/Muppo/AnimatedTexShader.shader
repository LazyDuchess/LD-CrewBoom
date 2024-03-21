// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Muppo/AnimatedTexShader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.1
		[NoScaleOffset][SingleLineTexture]_MainTex("Base Color", 2D) = "gray" {}
		_MaxOutlineWidth("Max Outline Width", Float) = 0.03
		_HDR("HDR", Float) = 1
		_FlipBookTexture("FlipBookTexture", 2D) = "white" {}
		_FlipBookMask("FlipBookMask", 2D) = "white" {}
		_TexColumns("TexColumns", Float) = 8
		_TexRows("TexRows", Float) = 8
		_TexSpeed("TexSpeed", Float) = 1
		_TexTiling("TexTiling", Vector) = (1,1,0,0)
		_TexOffset("TexOffset", Vector) = (0,0,0,0)
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
			float clampResult271 = clamp( distance( _WorldSpaceCameraPos , ase_worldPos ) , 0.1 , 20.0 );
			float lerpResult269 = lerp( 0.0025 , _MaxOutlineWidth , (0.0 + (clampResult271 - 0.1) * (1.0 - 0.0) / (20.0 - 0.1)));
			float outlineVar = lerpResult269;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float4 color264 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			float2 uv_MainTex76 = i.uv_texcoord;
			float4 tex2DNode76 = tex2D( _MainTex, uv_MainTex76 );
			o.Emission = (color264).rgb;
			clip( tex2DNode76.a - _Cutoff );
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
		uniform sampler2D _FlipBookTexture;
		uniform float2 _TexTiling;
		uniform float2 _TexOffset;
		uniform float _TexColumns;
		uniform float _TexRows;
		uniform float _TexSpeed;
		uniform sampler2D _FlipBookMask;
		uniform float4 _FlipBookMask_ST;
		uniform float _HDR;
		uniform float _Cutoff = 0.1;
		uniform float _MaxOutlineWidth;

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
			float2 uv_MainTex76 = i.uv_texcoord;
			float4 tex2DNode76 = tex2D( _MainTex, uv_MainTex76 );
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
			float3 temp_output_158_0 = ( ( ( indirectDiffuse115 * ase_lightColor.a ) + ( ase_lightColor.rgb * saturate( ( ( dotResult54 + 0.0 ) / 0.1 ) ) ) ) * (tex2DNode76).rgb );
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
			float2 uv_MainTex76 = i.uv_texcoord;
			float4 tex2DNode76 = tex2D( _MainTex, uv_MainTex76 );
			float3 temp_output_158_0 = ( ( ( float3(0,0,0) * ase_lightColor.a ) + ( ase_lightColor.rgb * saturate( ( ( dotResult54 + 0.0 ) / 0.1 ) ) ) ) * (tex2DNode76).rgb );
			o.Albedo = temp_output_158_0;
			float2 uv_TexCoord353 = i.uv_texcoord * _TexTiling + _TexOffset;
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles351 = _TexColumns * _TexRows;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset351 = 1.0f / _TexColumns;
			float fbrowsoffset351 = 1.0f / _TexRows;
			// Speed of animation
			float fbspeed351 = _Time[ 1 ] * _TexSpeed;
			// UV Tiling (col and row offset)
			float2 fbtiling351 = float2(fbcolsoffset351, fbrowsoffset351);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex351 = round( fmod( fbspeed351 + 0.0, fbtotaltiles351) );
			fbcurrenttileindex351 += ( fbcurrenttileindex351 < 0) ? fbtotaltiles351 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox351 = round ( fmod ( fbcurrenttileindex351, _TexColumns ) );
			// Multiply Offset X by coloffset
			float fboffsetx351 = fblinearindextox351 * fbcolsoffset351;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy351 = round( fmod( ( fbcurrenttileindex351 - fblinearindextox351 ) / _TexColumns, _TexRows ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy351 = (int)(_TexRows-1) - fblinearindextoy351;
			// Multiply Offset Y by rowoffset
			float fboffsety351 = fblinearindextoy351 * fbrowsoffset351;
			// UV Offset
			float2 fboffset351 = float2(fboffsetx351, fboffsety351);
			// Flipbook UV
			half2 fbuv351 = uv_TexCoord353 * fbtiling351 + fboffset351;
			// *** END Flipbook UV Animation vars ***
			float2 uv_FlipBookMask = i.uv_texcoord * _FlipBookMask_ST.xy + _FlipBookMask_ST.zw;
			o.Emission = ( (tex2D( _FlipBookTexture, fbuv351 )).rgb * tex2D( _FlipBookMask, uv_FlipBookMask ).r * _HDR );
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
Node;AmplifyShaderEditor.CommentaryNode;368;1818.921,-38.70154;Inherit;False;1554.1;476.6765;;12;361;362;360;358;351;366;365;353;355;356;357;367;AnimatedTexture;0.4997601,0.4206123,0.6415094,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;226;1097.396,466.9239;Inherit;False;686.2737;430.6522;;5;54;53;170;52;115;Normals;0.5220588,0.6044625,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;242;1821.172,459.9366;Inherit;False;1563.762;533.8073;;12;76;235;58;60;59;158;130;182;133;57;107;74;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;219;1931.6,1032.39;Inherit;False;1455.298;588.3884;;14;269;342;264;200;270;271;275;267;272;274;277;266;83;350;Outline;1,0.6029412,0.7097364,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;170;1309.39,547.1711;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;54;1516.796,692.4922;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;2145.701,681.202;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;2275.766,768.0457;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;74;2428.22,717.075;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;266;1953.448,1289.038;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;277;2020.826,1437.123;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;274;2234.42,1404.567;Float;False;Constant;_MinDistanceFromCamera;Min Distance From Camera;5;0;Create;True;0;0;0;False;0;False;0.1;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;272;2239.24,1483.234;Float;False;Constant;_MaxDistanceFromCamera;Max Distance From Camera;5;0;Create;True;0;0;0;False;0;False;20;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;267;2240.506,1288.796;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;275;2697.29,1437.256;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;271;2536.097,1337.584;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;270;2701.196,1336.397;Float;False;Property;_MaxOutlineWidth;Max Outline Width;2;0;Create;True;0;0;0;False;0;False;0.03;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;2696.034,1260.386;Float;False;Constant;_MinOutlineWidth;Min Outline Width;5;0;Create;True;0;0;0;False;0;False;0.0025;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;269;2974.445,1309.963;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;264;2569.858,1077.365;Inherit;False;Constant;_Black;Black;19;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;350;3074.518,1083.5;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;83;3156.182,1170.678;Inherit;False;0;True;Masked;0;0;Front;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;59;1847.254,771.0273;Float;False;Constant;_BaseCellOffset;Base Cell Offset;2;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1897.328,868.7136;Float;False;Constant;_BaseCellSharpness;Base Cell Sharpness;1;0;Create;True;0;0;0;False;0;False;0.1;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;52;1122.298,546.4924;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;1134.505,718.9267;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;342;2818.758,1106.016;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;130;2866.694,624.1887;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;2674.849,647.4445;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;2670.547,543.0635;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;76;2445.983,795.8389;Inherit;True;Property;_MainTex;Base Color;1;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;1c0b4963b77499d4089a53ab9bba1bdc;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;235;2751.167,796.2131;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;107;2423.815,581.9234;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.IndirectDiffuseLighting;115;1522.371,548.4772;Inherit;False;World;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3549.821,676.3474;Float;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;CrewBoom/AnimatedTexShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Masked;0.1;True;True;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;0;4;10;25;True;0.5;True;0;1;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0.03;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;_MinOutlineWidth;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;3033.168,666.7146;Inherit;True;2;2;0;FLOAT3;1,1,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;357;2108.922,347.2986;Inherit;False;Property;_TexSpeed;TexSpeed;8;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;356;2108.922,267.2986;Inherit;False;Property;_TexRows;TexRows;7;0;Create;True;0;0;0;False;0;False;8;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;355;2076.922,187.2985;Inherit;False;Property;_TexColumns;TexColumns;6;0;Create;True;0;0;0;False;0;False;8;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;353;2044.922,43.29845;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;365;1868.921,11.29847;Inherit;False;Property;_TexTiling;TexTiling;9;0;Create;True;0;0;0;False;0;False;1,1;4,4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;366;1868.921,155.2985;Inherit;False;Property;_TexOffset;TexOffset;10;0;Create;True;0;0;0;False;0;False;0,0;0.06,-0.16;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;351;2310.921,163.2985;Inherit;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ComponentMaskNode;360;2853.628,139.2606;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;362;3211.622,140.2985;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;361;2849.921,216.2985;Inherit;True;Property;_FlipBookMask;FlipBookMask;5;0;Create;True;0;0;0;False;0;False;-1;None;5881b79413fd5a64591b8ac393db4f6c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;358;2566.406,139.7134;Inherit;True;Property;_FlipBookTexture;FlipBookTexture;4;0;Create;True;0;0;0;False;0;False;-1;None;8c3e49b474c5b8b489305ddb7770cc2e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;367;2966.868,43.07492;Inherit;False;Property;_HDR;HDR;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
WireConnection;170;0;52;0
WireConnection;54;0;170;0
WireConnection;54;1;53;0
WireConnection;58;0;54;0
WireConnection;58;1;59;0
WireConnection;57;0;58;0
WireConnection;57;1;60;0
WireConnection;74;0;57;0
WireConnection;267;0;266;0
WireConnection;267;1;277;0
WireConnection;275;0;271;0
WireConnection;275;1;274;0
WireConnection;275;2;272;0
WireConnection;271;0;267;0
WireConnection;271;1;274;0
WireConnection;271;2;272;0
WireConnection;269;0;200;0
WireConnection;269;1;270;0
WireConnection;269;2;275;0
WireConnection;350;0;76;4
WireConnection;83;0;342;0
WireConnection;83;2;350;0
WireConnection;83;1;269;0
WireConnection;342;0;264;0
WireConnection;130;0;182;0
WireConnection;130;1;133;0
WireConnection;133;0;107;1
WireConnection;133;1;74;0
WireConnection;182;0;115;0
WireConnection;182;1;107;2
WireConnection;235;0;76;0
WireConnection;115;0;170;0
WireConnection;0;0;158;0
WireConnection;0;2;362;0
WireConnection;0;10;76;4
WireConnection;0;13;158;0
WireConnection;0;11;83;0
WireConnection;158;0;130;0
WireConnection;158;1;235;0
WireConnection;353;0;365;0
WireConnection;353;1;366;0
WireConnection;351;0;353;0
WireConnection;351;1;355;0
WireConnection;351;2;356;0
WireConnection;351;3;357;0
WireConnection;360;0;358;0
WireConnection;362;0;360;0
WireConnection;362;1;361;1
WireConnection;362;2;367;0
WireConnection;358;1;351;0
ASEEND*/
//CHKSM=4B2389BB31718CA07BE14DDADCD80E4D0854E2F2
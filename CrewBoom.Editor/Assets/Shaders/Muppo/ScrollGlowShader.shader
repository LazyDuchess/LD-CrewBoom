// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Muppo/ScrollGlowShader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.1
		[NoScaleOffset][SingleLineTexture]_MainTex("MainTex", 2D) = "gray" {}
		_MaxOutlineWidth("Max Outline Width", Float) = 0.03
		[NoScaleOffset][SingleLineTexture]_Emission("EmissiveTex", 2D) = "black" {}
		_EmissiveHue("EmissiveHue", Range( 0 , 1)) = 0
		_EmissiveHDR("EmissiveHDR", Float) = 1
		[Toggle(_GLOWTOGGLE_ON)] _GlowToggle("GlowToggle", Float) = 1
		_GlowMask("GlowMask", 2D) = "white" {}
		_GlowColor("GlowColor", Color) = (1,0,0,1)
		[Toggle]_GlowCycle("GlowCycle", Float) = 1
		_GlowSpeed("GlowSpeed", Float) = 10
		_GlowHDR("GlowHDR", Float) = 1
		[Toggle(_SCROLLTOGGLE_ON)] _ScrollToggle("ScrollToggle", Float) = 1
		_ScrollMask("ScrollMask", 2D) = "white" {}
		_ScrollTex("ScrollTex", 2D) = "white" {}
		_ScrollSize("ScrollSize", Vector) = (1,1,0,0)
		_ScrollOffset("ScrollOffset", Vector) = (0,0,0,0)
		_ScrollRotation("ScrollRotation", Range( 0 , 360)) = 0
		_ScrollSpeed("ScrollSpeed", Vector) = (0,1,0,0)
		_ScrollHue("ScrollHue", Range( 0 , 1)) = 0
		_ScrollHDR("ScrollHDR", Float) = 1
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
			o.Emission = color264.rgb;
			clip( tex2DNode76.a - _Cutoff );
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma shader_feature_local _SCROLLTOGGLE_ON
		#pragma shader_feature_local _GLOWTOGGLE_ON
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
		uniform sampler2D _ScrollMask;
		uniform float4 _ScrollMask_ST;
		uniform float _ScrollHue;
		uniform sampler2D _ScrollTex;
		uniform float2 _ScrollSpeed;
		uniform float2 _ScrollSize;
		uniform float2 _ScrollOffset;
		uniform float _ScrollRotation;
		uniform float _ScrollHDR;
		uniform float _EmissiveHue;
		uniform sampler2D _Emission;
		uniform float _EmissiveHDR;
		uniform float _GlowCycle;
		uniform float4 _GlowColor;
		uniform float _GlowSpeed;
		uniform sampler2D _GlowMask;
		uniform float4 _GlowMask_ST;
		uniform float _GlowHDR;
		uniform float _Cutoff = 0.1;
		uniform float _MaxOutlineWidth;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

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
			float2 uv_MainTex76 = i.uv_texcoord;
			float4 tex2DNode76 = tex2D( _MainTex, uv_MainTex76 );
			float3 temp_output_158_0 = ( ( ( float3(0,0,0) * ase_lightColor.a ) + ( ase_lightColor.rgb * saturate( ( ( dotResult54 + 0.0 ) / 0.01 ) ) ) ) * (tex2DNode76).rgb );
			o.Albedo = temp_output_158_0;
			float2 uv_ScrollMask = i.uv_texcoord * _ScrollMask_ST.xy + _ScrollMask_ST.zw;
			float2 uv_TexCoord313 = i.uv_texcoord * _ScrollSize + _ScrollOffset;
			float cos405 = cos( radians( _ScrollRotation ) );
			float sin405 = sin( radians( _ScrollRotation ) );
			float2 rotator405 = mul( uv_TexCoord313 - float2( 0.5,0.5 ) , float2x2( cos405 , -sin405 , sin405 , cos405 )) + float2( 0.5,0.5 );
			float2 panner311 = ( 1.0 * _Time.y * _ScrollSpeed + rotator405);
			float3 hsvTorgb348 = RGBToHSV( tex2D( _ScrollTex, panner311 ).rgb );
			float3 hsvTorgb349 = HSVToRGB( float3(( _ScrollHue + hsvTorgb348.x ),hsvTorgb348.y,hsvTorgb348.z) );
			#ifdef _SCROLLTOGGLE_ON
				float3 staticSwitch399 = ( tex2D( _ScrollMask, uv_ScrollMask ).r * hsvTorgb349 * _ScrollHDR );
			#else
				float3 staticSwitch399 = float3( 0,0,0 );
			#endif
			float2 uv_Emission278 = i.uv_texcoord;
			float3 hsvTorgb381 = RGBToHSV( tex2D( _Emission, uv_Emission278 ).rgb );
			float3 hsvTorgb383 = HSVToRGB( float3(( _EmissiveHue + hsvTorgb381.x ),hsvTorgb381.y,hsvTorgb381.z) );
			float mulTime367 = _Time.y * _GlowSpeed;
			float2 uv_GlowMask = i.uv_texcoord * _GlowMask_ST.xy + _GlowMask_ST.zw;
			float4 tex2DNode400 = tex2D( _GlowMask, uv_GlowMask );
			float3 hsvTorgb375 = HSVToRGB( float3(( mulTime367 * 0.1 ),1.0,1.0) );
			#ifdef _GLOWTOGGLE_ON
				float3 staticSwitch393 = ( (( _GlowCycle )?( ( hsvTorgb375 * tex2DNode400.r ) ):( ( (_GlowColor).rgb * (0.0 + (sin( mulTime367 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * tex2DNode400.r ) )) * _GlowHDR );
			#else
				float3 staticSwitch393 = float3( 0,0,0 );
			#endif
			o.Emission = ( staticSwitch399 + ( hsvTorgb383 * _EmissiveHDR ) + staticSwitch393 );
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
Node;AmplifyShaderEditor.CommentaryNode;385;827.894,-1022.978;Inherit;False;1880.365;759.4941;;14;378;375;400;377;372;380;379;370;376;371;373;368;367;374;Glowing;0.4481132,0.6304269,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;333;-382.6957,-188.4153;Inherit;False;2086.27;579.8313;;16;407;406;315;365;366;313;405;311;329;331;349;350;286;351;348;330;ScrollingMasked;0.9008027,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;332;1068.097,463.5504;Inherit;False;726.0759;361.555;Normals and light;5;54;53;52;170;115;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;242;1821.172,459.9366;Inherit;False;1567.393;543.9291;Comment;12;182;107;74;57;58;59;60;76;235;158;130;133;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;219;1906.017,1062.833;Inherit;False;1483.964;616.3492;Comment;12;83;269;264;200;270;267;271;275;272;274;277;266;Outline;1,0.6029412,0.7097364,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;281;2003.35,-194.0628;Inherit;False;1382.785;524.939;;8;278;303;384;382;383;381;388;389;Emissive;0.6419381,0.9433962,0.4850481,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;54;1638.598,615.6709;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;52;1118.097,513.5504;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;170;1305.189,514.2291;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;115;1520.173,516.6558;Inherit;False;World;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RGBToHSVNode;381;2363.332,15.68647;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;383;2725.066,37.38776;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;382;2600.146,14.36399;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;384;2294.812,-83.56843;Inherit;False;Property;_EmissiveHue;EmissiveHue;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;371;1822.202,-835.5906;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;376;2058.142,-551.6061;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;370;2057.349,-802.6263;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;379;2361.259,-543.7358;Inherit;False;Property;_GlowHDR;GlowHDR;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;380;2524.259,-647.7358;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;266;1969.309,1330.389;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;277;2036.686,1478.474;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;275;2728.145,1480.272;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;271;2566.952,1380.6;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;267;2256.366,1330.146;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;264;2728.17,1127.385;Inherit;False;Constant;_Black;Black;19;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;269;2986.3,1354.979;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;83;3142.381,1286.035;Inherit;False;0;True;Masked;0;0;Front;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;2134.701,612.202;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;1855.254,701.0273;Float;False;Constant;_BaseCellOffset;Base Cell Offset;2;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1923.328,773.7136;Float;False;Constant;_BaseCellSharpness;Base Cell Sharpness;1;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;274;2250.281,1445.917;Float;False;Constant;_MinDistanceFromCamera;Min Distance From Camera;5;0;Create;True;0;0;0;False;0;False;0.1;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;272;2255.102,1524.584;Float;False;Constant;_MaxDistanceFromCamera;Max Distance From Camera;5;0;Create;True;0;0;0;False;0;False;20;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;2726.889,1303.402;Float;False;Constant;_MinOutlineWidth;Min Outline Width;5;0;Create;True;0;0;0;False;0;False;0.0025;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;270;2732.051,1379.414;Float;False;Property;_MaxOutlineWidth;Max Outline Width;2;0;Create;True;0;0;0;False;0;False;0.03;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;372;1564.958,-963.9778;Inherit;False;Property;_GlowColor;GlowColor;8;0;Create;True;0;0;0;False;0;False;1,0,0,1;0,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;1476.436,-40.90237;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;303;3171.723,-60.98732;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;393;2809.62,-347.3479;Inherit;False;Property;_GlowToggle;GlowToggle;6;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;377;2299.471,-648.8658;Inherit;False;Property;_GlowCycle;GlowCycle;9;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;399;1753.732,-63.96207;Inherit;False;Property;_ScrollToggle;ScrollToggle;12;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RGBToHSVNode;348;809.5272,148.9758;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;286;529.3575,150.028;Inherit;True;Property;_ScrollTex;ScrollTex;14;0;Create;True;0;0;0;False;0;False;-1;255f3d3f8d683944fb5bec074d87c28d;d4adf2f3c8be3a946ab2cb31d431852b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;350;1036.341,79.65336;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;349;1169.261,167.6771;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;331;1228.855,90.40508;Inherit;False;Property;_ScrollHDR;ScrollHDR;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;375;1658.987,-455.8453;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SinOpNode;374;1267.92,-781.891;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;367;1073.249,-693.5796;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;368;898.2481,-692.5591;Inherit;False;Property;_GlowSpeed;GlowSpeed;10;0;Create;True;0;0;0;False;0;False;10;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;373;1418.945,-780.7051;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;311;350.3695,173.3851;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;405;147.9258,5.676575;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;313;-99.19067,4.994461;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;366;-313.9239,97.69247;Inherit;False;Property;_ScrollOffset;ScrollOffset;16;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;365;-284.5576,-25.99193;Inherit;False;Property;_ScrollSize;ScrollSize;15;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;74;2409.22,750.075;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;2287.766,750.0457;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;3197.75,628.3867;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;1334.307,640.1054;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;76;2626.22,761.8605;Inherit;True;Property;_MainTex;MainTex;1;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;05b84be0185414844a4633df57a28cc2;03e438ecbde033e46830b5b338d68750;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;107;2408.815,584.9234;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;2672.547,513.0635;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;130;2947.694,628.1887;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;2668.849,652.4445;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;235;2937.97,760.0801;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;388;2758.764,184.5177;Inherit;False;Property;_EmissiveHDR;EmissiveHDR;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;389;2967.064,100.9178;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RadiansOpNode;406;-44.2389,217.8208;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;315;139.7813,199.3584;Inherit;False;Property;_ScrollSpeed;ScrollSpeed;18;0;Create;True;0;0;0;False;0;False;0,1;0,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;407;-329.3793,218.8215;Inherit;False;Property;_ScrollRotation;ScrollRotation;17;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3675.622,633.8889;Float;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;CrewBoom/ScrollGlowShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Masked;0.1;True;True;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;0;4;10;25;True;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0.03;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;400;1650.414,-687.3216;Inherit;True;Property;_GlowMask;GlowMask;7;0;Create;True;0;0;0;False;0;False;-1;ad344710178f9484a8fee39e5cbd3724;c6b591ac7b14d1749926ed8e634f4af5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;378;1452.629,-591.3272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;329;1101.079,-115.2053;Inherit;True;Property;_ScrollMask;ScrollMask;13;0;Create;True;0;0;0;False;0;False;-1;544d1f4eafd55424394b5172b0d2bd56;90b2669df48da4547acd173b4ca139dc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;351;745.1238,77.96898;Inherit;False;Property;_ScrollHue;ScrollHue;19;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;278;2075.419,15.95095;Inherit;True;Property;_Emission;EmissiveTex;3;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;54;0;170;0
WireConnection;54;1;53;0
WireConnection;170;0;52;0
WireConnection;115;0;170;0
WireConnection;381;0;278;0
WireConnection;383;0;382;0
WireConnection;383;1;381;2
WireConnection;383;2;381;3
WireConnection;382;0;384;0
WireConnection;382;1;381;1
WireConnection;371;0;372;0
WireConnection;376;0;375;0
WireConnection;376;1;400;1
WireConnection;370;0;371;0
WireConnection;370;1;373;0
WireConnection;370;2;400;1
WireConnection;380;0;377;0
WireConnection;380;1;379;0
WireConnection;275;0;271;0
WireConnection;275;1;274;0
WireConnection;275;2;272;0
WireConnection;271;0;267;0
WireConnection;271;1;274;0
WireConnection;271;2;272;0
WireConnection;267;0;266;0
WireConnection;267;1;277;0
WireConnection;269;0;200;0
WireConnection;269;1;270;0
WireConnection;269;2;275;0
WireConnection;83;0;264;0
WireConnection;83;2;76;4
WireConnection;83;1;269;0
WireConnection;58;0;54;0
WireConnection;58;1;59;0
WireConnection;330;0;329;1
WireConnection;330;1;349;0
WireConnection;330;2;331;0
WireConnection;303;0;399;0
WireConnection;303;1;389;0
WireConnection;303;2;393;0
WireConnection;393;0;380;0
WireConnection;377;0;370;0
WireConnection;377;1;376;0
WireConnection;399;0;330;0
WireConnection;348;0;286;0
WireConnection;286;1;311;0
WireConnection;350;0;351;0
WireConnection;350;1;348;1
WireConnection;349;0;350;0
WireConnection;349;1;348;2
WireConnection;349;2;348;3
WireConnection;375;0;378;0
WireConnection;374;0;367;0
WireConnection;367;0;368;0
WireConnection;373;0;374;0
WireConnection;311;0;405;0
WireConnection;311;2;315;0
WireConnection;405;0;313;0
WireConnection;405;2;406;0
WireConnection;313;0;365;0
WireConnection;313;1;366;0
WireConnection;74;0;57;0
WireConnection;57;0;58;0
WireConnection;57;1;60;0
WireConnection;158;0;130;0
WireConnection;158;1;235;0
WireConnection;182;0;115;0
WireConnection;182;1;107;2
WireConnection;130;0;182;0
WireConnection;130;1;133;0
WireConnection;133;0;107;1
WireConnection;133;1;74;0
WireConnection;235;0;76;0
WireConnection;389;0;383;0
WireConnection;389;1;388;0
WireConnection;406;0;407;0
WireConnection;0;0;158;0
WireConnection;0;2;303;0
WireConnection;0;10;76;4
WireConnection;0;13;158;0
WireConnection;0;11;83;0
WireConnection;378;0;367;0
ASEEND*/
//CHKSM=15884100EF948516DE06B1B34C893C086A854836
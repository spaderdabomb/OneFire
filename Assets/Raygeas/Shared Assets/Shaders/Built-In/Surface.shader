// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Built-In/Surface"
{
	Properties
	{
		[Header(Main Maps)][Space(5)]_Tiling("Tiling", Float) = 1
		[NoScaleOffset][Space(5)]_MainTex("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("Normal", 2D) = "bump" {}
		[NoScaleOffset]_MetallicGlossMap("Metallic/Smoothness", 2D) = "white" {}
		[NoScaleOffset]_OcclusionMap("Occlusion", 2D) = "white" {}
		[Header(Main Settings)][Space(5)]_Color("Color", Color) = (1,1,1,0)
		_BumpScale("Normal", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Glossiness("Smoothness", Range( 0 , 1)) = 0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)]_SmoothnessTextureChannel("Smoothness Source", Float) = 0
		_OcclusionStrength("Occlusion", Range( 0 , 1)) = 1
		[Space(15)][Header(Coverage Maps)][Space(5)]_CovTiling("Tiling", Float) = 1
		[NoScaleOffset][Space(5)]_CovMainTex("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_CovBumpMap("Normal", 2D) = "bump" {}
		[NoScaleOffset]_CoverageMetallicSmoothness("Metallic/Smoothness", 2D) = "white" {}
		[NoScaleOffset]_CovMask("Coverage Mask", 2D) = "white" {}
		[Header(Coverage Settings)][Space(5)][Toggle(_COVERAGEON_ON)] _Coverageon("Enable", Float) = 0
		[Space(15)]_CovColor("Color", Color) = (1,1,1,0)
		_CovBumpScale("Normal", Float) = 1
		_CovMetallic("Metallic", Range( 0 , 1)) = 0
		_CovGlossiness("Smoothness", Range( 0 , 1)) = 0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)]_CovSmoothnessTextureChannel("Smoothness Source", Float) = 0
		[Enum(World Normal,0,Vertex Position,1)][Space(15)]_CovOverlayMethod("Overlay Method", Float) = 0
		_CovOffset("Offset", Float) = 1
		_CovBalance("Balance", Float) = -1
		_MaskContrast("Mask Contrast", Float) = 1
		_NormalBlending("Normal Blending", Range( 0 , 1)) = 1
		[Space(15)]_MaskTilingX("Mask Tiling X", Float) = 1
		_MaskTilingY("Mask Tiling Y", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _COVERAGEON_ON
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _BumpMap;
		uniform float _Tiling;
		uniform float _BumpScale;
		uniform sampler2D _CovBumpMap;
		uniform float _CovTiling;
		uniform float _CovBumpScale;
		uniform float _NormalBlending;
		uniform float _CovOverlayMethod;
		uniform float _CovOffset;
		uniform float _CovBalance;
		uniform sampler2D _CovMask;
		uniform float _MaskTilingX;
		uniform float _MaskTilingY;
		uniform float _MaskContrast;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _CovColor;
		uniform sampler2D _CovMainTex;
		uniform float _Metallic;
		uniform sampler2D _MetallicGlossMap;
		uniform float _CovMetallic;
		uniform sampler2D _CoverageMetallicSmoothness;
		uniform float _SmoothnessTextureChannel;
		uniform float _Glossiness;
		uniform float _CovSmoothnessTextureChannel;
		uniform float _CovGlossiness;
		uniform sampler2D _OcclusionMap;
		uniform float _OcclusionStrength;


		inline float3 TriplanarSampling432( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			xNorm.xyz  = half3( UnpackScaleNormal( xNorm, normalScale.y ).xy * float2(  nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz  = half3( UnpackScaleNormal( yNorm, normalScale.x ).xy * float2(  nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz  = half3( UnpackScaleNormal( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSampling421( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling430( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_Tiling).xx;
			float2 uv_TexCoord472 = i.uv_texcoord * temp_cast_0;
			float2 Tiling462 = uv_TexCoord472;
			float3 tex2DNode6 = UnpackScaleNormal( tex2D( _BumpMap, Tiling462 ), _BumpScale );
			float CoverageTiling463 = _CovTiling;
			float2 temp_cast_1 = (CoverageTiling463).xx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar432 = TriplanarSampling432( _CovBumpMap, ase_worldPos, ase_worldNormal, 10.0, temp_cast_1, _CovBumpScale, 0 );
			float3 tanTriplanarNormal432 = mul( ase_worldToTangent, triplanar432 );
			float3 lerpResult515 = lerp( BlendNormals( tex2DNode6 , tanTriplanarNormal432 ) , tanTriplanarNormal432 , ( 1.0 - _NormalBlending ));
			float3 MainNormalMap454 = tex2DNode6;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float lerpResult457 = lerp( (WorldNormalVector( i , MainNormalMap454 )).y , ase_vertex3Pos.y , _CovOverlayMethod);
			float temp_output_289_0 = ( lerpResult457 + ( 1.0 - _CovOffset ) );
			float lerpResult541 = lerp( temp_output_289_0 , ( 1.0 - temp_output_289_0 ) , _CovBalance);
			float2 appendResult550 = (float2(_MaskTilingX , _MaskTilingY));
			float2 uv_TexCoord545 = i.uv_texcoord * appendResult550;
			float2 MaskTiling551 = uv_TexCoord545;
			float4 tex2DNode507 = tex2D( _CovMask, MaskTiling551 );
			float lerpResult524 = lerp( ( 1.0 - tex2DNode507.g ) , tex2DNode507.g , _MaskContrast);
			float CoverageMask297 = saturate( ( lerpResult541 * saturate( lerpResult524 ) ) );
			float3 lerpResult309 = lerp( tex2DNode6 , lerpResult515 , CoverageMask297);
			#ifdef _COVERAGEON_ON
				float3 staticSwitch308 = lerpResult309;
			#else
				float3 staticSwitch308 = tex2DNode6;
			#endif
			float3 Normal75 = staticSwitch308;
			o.Normal = Normal75;
			float4 tex2DNode2 = tex2D( _MainTex, Tiling462 );
			float4 temp_output_3_0 = ( _Color * tex2DNode2 );
			float2 temp_cast_2 = (CoverageTiling463).xx;
			float4 triplanar421 = TriplanarSampling421( _CovMainTex, ase_worldPos, ase_worldNormal, 10.0, temp_cast_2, 1.0, 0 );
			float4 lerpResult302 = lerp( temp_output_3_0 , ( _CovColor * triplanar421 ) , CoverageMask297);
			#ifdef _COVERAGEON_ON
				float4 staticSwitch304 = lerpResult302;
			#else
				float4 staticSwitch304 = temp_output_3_0;
			#endif
			float4 Albedo19 = staticSwitch304;
			o.Albedo = Albedo19.rgb;
			float4 tex2DNode239 = tex2D( _MetallicGlossMap, Tiling462 );
			float temp_output_241_0 = ( _Metallic * tex2DNode239.r );
			float2 temp_cast_5 = (CoverageTiling463).xx;
			float4 triplanar430 = TriplanarSampling430( _CoverageMetallicSmoothness, ase_worldPos, ase_worldNormal, 10.0, temp_cast_5, 1.0, 0 );
			float lerpResult335 = lerp( temp_output_241_0 , ( _CovMetallic * triplanar430.x ) , CoverageMask297);
			#ifdef _COVERAGEON_ON
				float staticSwitch340 = lerpResult335;
			#else
				float staticSwitch340 = temp_output_241_0;
			#endif
			float Metallic262 = staticSwitch340;
			o.Metallic = Metallic262;
			float AlbedoAlpha434 = tex2DNode2.a;
			float lerpResult441 = lerp( tex2DNode239.a , AlbedoAlpha434 , _SmoothnessTextureChannel);
			float temp_output_240_0 = ( lerpResult441 * _Glossiness );
			float CoverageAlbedoAlpha369 = triplanar421.a;
			float lerpResult451 = lerp( triplanar430.a , CoverageAlbedoAlpha369 , _CovSmoothnessTextureChannel);
			float lerpResult333 = lerp( temp_output_240_0 , ( lerpResult451 * _CovGlossiness ) , CoverageMask297);
			#ifdef _COVERAGEON_ON
				float staticSwitch339 = lerpResult333;
			#else
				float staticSwitch339 = temp_output_240_0;
			#endif
			float Smoothness263 = staticSwitch339;
			o.Smoothness = Smoothness263;
			float lerpResult410 = lerp( 1.0 , tex2D( _OcclusionMap, Tiling462 ).g , _OcclusionStrength);
			float Ambient_Occlusion415 = lerpResult410;
			o.Occlusion = Ambient_Occlusion415;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows dithercrossfade 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
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
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;473;-5238.548,719.7208;Inherit;False;985.5457;524.7683;;10;460;463;461;472;462;551;546;547;550;545;Tiling;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;461;-4965.548,808.65;Inherit;False;Property;_Tiling;Tiling;0;0;Create;False;0;0;0;False;2;Header(Main Maps);Space(5);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;472;-4760.245,784.6017;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;80;-5246.29,-134.5053;Inherit;False;2077.593;772.498;;18;517;516;310;515;75;308;311;312;309;432;175;361;6;454;306;468;471;433;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;462;-4496.549,784.6498;Inherit;False;Tiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;468;-5152.193,-11.93699;Inherit;False;462;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-5120.167,85.10455;Inherit;False;Property;_BumpScale;Normal;6;0;Create;False;0;1;Option1;0;0;False;0;False;1;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;547;-5196.686,1143.18;Inherit;False;Property;_MaskTilingY;Mask Tiling Y;28;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;546;-5196.686,1056.18;Inherit;False;Property;_MaskTilingX;Mask Tiling X;27;0;Create;True;0;0;0;False;1;Space(15);False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-4834.046,-5.186795;Inherit;True;Property;_BumpMap;Normal;2;2;[NoScaleOffset];[Normal];Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;550;-4964.165,1087.874;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;298;-3118.266,-139.3249;Inherit;False;2076.209;781.7334;;20;297;487;360;531;525;523;524;507;527;287;289;541;286;544;456;455;457;283;357;552;Coverage Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;454;-4421.551,74.17956;Inherit;False;MainNormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;545;-4768.686,1064.18;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;455;-3078.327,-55.61077;Inherit;False;454;MainNormalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;551;-4497.429,1064.103;Inherit;False;MaskTiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;357;-2849.578,102.7021;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;283;-2850.429,-55.41725;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;552;-2847.237,414.4482;Inherit;False;551;MaskTiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;456;-2847.994,260.5911;Inherit;False;Property;_CovOverlayMethod;Overlay Method;22;1;[Enum];Create;False;0;2;World Normal;0;Vertex Position;1;0;False;1;Space(15);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;286;-2630.466,259.0982;Inherit;False;Property;_CovOffset;Offset;23;0;Create;False;0;0;0;False;0;False;1;-1.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;41;-2788.599,-1251.465;Inherit;False;1918.847;1025.771;;15;421;300;1;2;422;470;467;19;304;434;369;303;302;301;3;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;457;-2576.994,82.59109;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;544;-2459.106,259.2239;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;460;-4692.275,945.233;Inherit;False;Property;_CovTiling;Tiling;11;0;Create;False;0;0;0;False;3;Space(15);Header(Coverage Maps);Space(5);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;507;-2606.656,389.4706;Inherit;True;Property;_CovMask;Coverage Mask;15;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;cb16297235b7b1f4cbac6703cb5b9678;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;289;-2253.86,83.08209;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;523;-2283.287,381.6895;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;467;-2582.252,-933.5354;Inherit;False;462;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;525;-2309.054,516.6456;Inherit;False;Property;_MaskContrast;Mask Contrast;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;463;-4497.549,944.6509;Inherit;False;CoverageTiling;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;79;-5245.993,-1253.395;Inherit;False;2409.134;1027.341;;30;465;431;466;430;242;439;54;435;450;332;340;339;239;334;241;240;262;363;365;366;364;333;335;263;437;436;370;451;438;441;Metallic/Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;527;-2103.648,162.7202;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;524;-2090.287,414.6894;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;470;-2707.409,-393.6988;Inherit;False;463;CoverageTiling;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;422;-2714.46,-596.0686;Inherit;True;Property;_CovMainTex;Albedo;12;1;[NoScaleOffset];Create;False;0;0;0;False;1;Space(5);False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;2;-2329.908,-957.6903;Inherit;True;Property;_MainTex;Albedo;1;1;[NoScaleOffset];Create;False;0;0;0;False;1;Space(5);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;287;-2101.583,258.3081;Inherit;False;Property;_CovBalance;Balance;24;0;Create;False;0;0;0;False;0;False;-1;-0.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;466;-5011.479,-1063.774;Inherit;False;462;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;541;-1914.835,83.98171;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;531;-1913.686,414.4957;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;434;-1961.05,-877.3463;Inherit;False;AlbedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;421;-2413.572,-519.6924;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;10;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;437;-4674.257,-889.3327;Inherit;False;434;AlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;239;-4777.259,-1087.302;Inherit;True;Property;_MetallicGlossMap;Metallic/Smoothness;3;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;431;-5185.024,-722.3202;Inherit;True;Property;_CoverageMetallicSmoothness;Metallic/Smoothness;14;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;465;-5176.347,-522.9006;Inherit;False;463;CoverageTiling;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;439;-4704.184,-793.5801;Inherit;False;Property;_SmoothnessTextureChannel;Smoothness Source;9;1;[Enum];Create;False;0;2;Metallic Alpha;0;Albedo Alpha;1;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;360;-1666.661,228.7535;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;471;-5177.73,518.2753;Inherit;False;463;CoverageTiling;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;361;-5115.832,423.2877;Inherit;False;Property;_CovBumpScale;Normal;18;0;Create;False;0;1;Option1;0;0;False;0;False;1;2.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;369;-1994.562,-423.8585;Inherit;False;CoverageAlbedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;433;-5188.57,221.3949;Inherit;True;Property;_CovBumpMap;Normal;13;2;[NoScaleOffset];[Normal];Create;False;0;0;0;False;0;False;None;None;False;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.LerpOp;441;-4410.865,-913.1537;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;438;-4754.427,-443.8916;Inherit;False;369;CoverageAlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-4406.52,-1163.217;Inherit;False;Property;_Metallic;Metallic;7;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-4410.217,-789.8232;Inherit;False;Property;_Glossiness;Smoothness;8;0;Create;False;0;0;0;False;0;False;0;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;450;-4719.256,-354.9561;Inherit;False;Property;_CovSmoothnessTextureChannel;Smoothness Source;21;1;[Enum];Create;False;0;2;Metallic Alpha;0;Albedo Alpha;1;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;487;-1478.098,229.2119;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;516;-4556.359,446.4276;Inherit;False;Property;_NormalBlending;Normal Blending;26;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;430;-4879.729,-642.3202;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;10;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;432;-4906.278,332.9303;Inherit;True;Spherical;World;True;Top Texture 2;_TopTexture2;bump;0;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;10;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;416;-4193.45,721.0216;Inherit;False;1113.388;393.1936;;5;469;408;410;406;415;Ambient Occlusion;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;451;-4411.137,-467.4431;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-4062.507,-913.7997;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-4062.128,-1086.816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;435;-4410.078,-697.0166;Inherit;False;Property;_CovMetallic;Metallic;19;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;332;-4413.566,-343.2623;Inherit;False;Property;_CovGlossiness;Smoothness;20;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;297;-1291.606,229.7733;Inherit;False;CoverageMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;306;-4422.68,163.9788;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;517;-4279.708,446.5965;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-2261.156,-1141.94;Inherit;False;Property;_Color;Color;5;0;Create;False;0;0;0;False;2;Header(Main Settings);Space(5);False;1,1,1,0;0.668,0.5182759,0.4990805,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;300;-2259.796,-726.5112;Inherit;False;Property;_CovColor;Color;17;0;Create;False;0;0;0;False;1;Space(15);False;1,1,1,0;0.2901961,0.6344857,0.6901961,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;370;-4058.444,-465.2663;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;436;-4059.553,-640.2471;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;365;-3607.754,-943.4928;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;363;-3607.993,-1118.355;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;334;-3934.646,-989.0728;Inherit;False;297;CoverageMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;469;-4164.05,810.6268;Inherit;False;462;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;312;-3908.813,-39.02818;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;515;-4099.537,306.9151;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;310;-4166.596,71.59686;Inherit;False;297;CoverageMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-1916.578,-1057.209;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-1918.98,-630.9773;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;303;-1959.508,-799.1876;Inherit;False;297;CoverageMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;335;-3593.652,-1087.556;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;333;-3594.719,-914.7487;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;364;-3483.303,-1120.154;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;366;-3479.754,-945.4928;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;406;-3939.215,787.0216;Inherit;True;Property;_OcclusionMap;Occlusion;4;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;408;-3933.847,1017.323;Inherit;False;Property;_OcclusionStrength;Occlusion;10;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;309;-3891.998,-11.46561;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;311;-3777.262,-38.99716;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;302;-1651.226,-848.3546;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;410;-3544.308,885.0372;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;339;-3371.297,-938.7908;Inherit;False;Property;_Coverageon2;Coverage;16;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;304;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;340;-3371.892,-1110.276;Inherit;False;Property;_Coverageon3;Coverage;16;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;304;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;308;-3682.621,-34.47466;Inherit;False;Property;_Coverageon1;Coverage;16;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;304;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;304;-1404.458,-1055.96;Inherit;False;Property;_Coverageon;Enable;16;0;Create;False;0;0;0;False;2;Header(Coverage Settings);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-3099.97,-938.7877;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;262;-3101.251,-1110.798;Inherit;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;415;-3349.893,884.5079;Inherit;False;Ambient Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-3413.293,-34.25854;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1108.726,-1055.769;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-632.4727,-966.7545;Inherit;False;19;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-632.3541,-885.075;Inherit;False;75;Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-630.7629,-712.2136;Inherit;False;263;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;264;-632.6641,-795.9246;Inherit;False;262;Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;452;-694.8375,-627.148;Inherit;False;415;Ambient Occlusion;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-342.2618,-860.8546;Float;False;True;-1;2;;0;0;Standard;Raygeas/Built-In/Surface;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;0;15;10;25;False;0.5;True;0;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;_CullMode;-1;0;True;_AlphaCutoff;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;472;0;461;0
WireConnection;462;0;472;0
WireConnection;6;1;468;0
WireConnection;6;5;175;0
WireConnection;550;0;546;0
WireConnection;550;1;547;0
WireConnection;454;0;6;0
WireConnection;545;0;550;0
WireConnection;551;0;545;0
WireConnection;283;0;455;0
WireConnection;457;0;283;2
WireConnection;457;1;357;2
WireConnection;457;2;456;0
WireConnection;544;0;286;0
WireConnection;507;1;552;0
WireConnection;289;0;457;0
WireConnection;289;1;544;0
WireConnection;523;0;507;2
WireConnection;463;0;460;0
WireConnection;527;0;289;0
WireConnection;524;0;523;0
WireConnection;524;1;507;2
WireConnection;524;2;525;0
WireConnection;2;1;467;0
WireConnection;541;0;289;0
WireConnection;541;1;527;0
WireConnection;541;2;287;0
WireConnection;531;0;524;0
WireConnection;434;0;2;4
WireConnection;421;0;422;0
WireConnection;421;3;470;0
WireConnection;239;1;466;0
WireConnection;360;0;541;0
WireConnection;360;1;531;0
WireConnection;369;0;421;4
WireConnection;441;0;239;4
WireConnection;441;1;437;0
WireConnection;441;2;439;0
WireConnection;487;0;360;0
WireConnection;430;0;431;0
WireConnection;430;3;465;0
WireConnection;432;0;433;0
WireConnection;432;8;361;0
WireConnection;432;3;471;0
WireConnection;451;0;430;4
WireConnection;451;1;438;0
WireConnection;451;2;450;0
WireConnection;240;0;441;0
WireConnection;240;1;54;0
WireConnection;241;0;242;0
WireConnection;241;1;239;1
WireConnection;297;0;487;0
WireConnection;306;0;6;0
WireConnection;306;1;432;0
WireConnection;517;0;516;0
WireConnection;370;0;451;0
WireConnection;370;1;332;0
WireConnection;436;0;435;0
WireConnection;436;1;430;1
WireConnection;365;0;240;0
WireConnection;363;0;241;0
WireConnection;312;0;6;0
WireConnection;515;0;306;0
WireConnection;515;1;432;0
WireConnection;515;2;517;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;301;0;300;0
WireConnection;301;1;421;0
WireConnection;335;0;241;0
WireConnection;335;1;436;0
WireConnection;335;2;334;0
WireConnection;333;0;240;0
WireConnection;333;1;370;0
WireConnection;333;2;334;0
WireConnection;364;0;363;0
WireConnection;366;0;365;0
WireConnection;406;1;469;0
WireConnection;309;0;6;0
WireConnection;309;1;515;0
WireConnection;309;2;310;0
WireConnection;311;0;312;0
WireConnection;302;0;3;0
WireConnection;302;1;301;0
WireConnection;302;2;303;0
WireConnection;410;1;406;2
WireConnection;410;2;408;0
WireConnection;339;1;366;0
WireConnection;339;0;333;0
WireConnection;340;1;364;0
WireConnection;340;0;335;0
WireConnection;308;1;311;0
WireConnection;308;0;309;0
WireConnection;304;1;3;0
WireConnection;304;0;302;0
WireConnection;263;0;339;0
WireConnection;262;0;340;0
WireConnection;415;0;410;0
WireConnection;75;0;308;0
WireConnection;19;0;304;0
WireConnection;0;0;20;0
WireConnection;0;1;77;0
WireConnection;0;3;264;0
WireConnection;0;4;265;0
WireConnection;0;5;452;0
ASEEND*/
//CHKSM=01018BB846358A0E24A074F58D0DDCAD7DC8FBAC
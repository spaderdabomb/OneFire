// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Built-In/Water"
{
	Properties
	{
		[NoScaleOffset][Normal]_WavesNormal("Waves Normal", 2D) = "bump" {}
		[Header(Water)][Space(5)]_WaterColor("Water Color", Color) = (0.2705882,0.4823529,0.5372549,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.96
		_Tiling("Tiling", Float) = 0.15
		_WavesSpeed("Waves Speed", Range( 0.1 , 1)) = 0.3
		_NormalIntensity("Normal Intensity", Range( 0 , 2)) = 1
		_Transparency("Transparency", Range( 0 , 10)) = 0
		_TransparencyFade("Transparency Fade", Range( 0 , 2)) = 0
		_CoastalBlending("Coastal Blending", Range( 0 , 1)) = 1
		_RefractionFactor("Refraction Factor", Range( 0 , 1)) = 0.5
		[Header(Foam)][Space(5)][Toggle(_ENABLEFOAM_ON)] _EnableFoam("Enable", Float) = 1
		_FoamTiling("Tiling", Float) = 50
		_FoamOpacity("Opacity", Range( 0 , 1)) = 0
		_FoamDistance("Distance", Range( 0.01 , 1)) = 0.07
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ENABLEFOAM_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard alpha:fade keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float eyeDepth;
		};

		uniform sampler2D _WavesNormal;
		uniform float _WavesSpeed;
		uniform float _Tiling;
		uniform float _NormalIntensity;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _RefractionFactor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float4 _WaterColor;
		uniform float _Transparency;
		uniform float _TransparencyFade;
		uniform float _FoamTiling;
		uniform float _FoamDistance;
		uniform float _FoamOpacity;
		uniform float _Smoothness;
		uniform float _CoastalBlending;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float2 voronoihash61( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi61( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash61( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.707 * sqrt(dot( r, r ));
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime524 = _Time.y * _WavesSpeed;
			float Time525 = mulTime524;
			float temp_output_533_0 = ( Time525 * 0.1 );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult106 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 WorldSpaceTile68 = ( appendResult106 * _Tiling );
			float2 panner112 = ( temp_output_533_0 * float2( 1,1 ) + WorldSpaceTile68);
			float2 panner114 = ( ( 1.0 - temp_output_533_0 ) * float2( 1,1 ) + WorldSpaceTile68);
			float3 Normal89 = BlendNormals( UnpackScaleNormal( tex2D( _WavesNormal, panner112 ), _NormalIntensity ) , UnpackScaleNormal( tex2D( _WavesNormal, ( 1.0 - panner114 ) ), _NormalIntensity ) );
			o.Normal = Normal89;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor341 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPosNorm.xy);
			float4 temp_output_277_0 = ( ase_grabScreenPosNorm + float4( ( Normal89 * ( _RefractionFactor * 0.1 ) ) , 0.0 ) );
			float4 screenColor274 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,temp_output_277_0.xy);
			float eyeDepth337 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, temp_output_277_0.xy ));
			float ifLocalVar336 = 0;
			if( eyeDepth337 > i.eyeDepth )
				ifLocalVar336 = 1.0;
			else if( eyeDepth337 < i.eyeDepth )
				ifLocalVar336 = 0.0;
			float4 lerpResult342 = lerp( screenColor341 , screenColor274 , ifLocalVar336);
			float4 Refractions282 = saturate( lerpResult342 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth384 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth384 = saturate( abs( ( screenDepth384 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Transparency ) ) );
			float saferPower405 = abs( distanceDepth384 );
			float4 lerpResult389 = lerp( Refractions282 , _WaterColor , pow( saferPower405 , _TransparencyFade ));
			float time61 = ( Time525 * 5 );
			float2 voronoiSmoothId61 = 0;
			float2 coords61 = WorldSpaceTile68 * _FoamTiling;
			float2 id61 = 0;
			float2 uv61 = 0;
			float fade61 = 0.5;
			float voroi61 = 0;
			float rest61 = 0;
			for( int it61 = 0; it61 <3; it61++ ){
			voroi61 += fade61 * voronoi61( coords61, time61, id61, uv61, 0,voronoiSmoothId61 );
			rest61 += fade61;
			coords61 *= 2;
			fade61 *= 0.5;
			}//Voronoi61
			voroi61 /= rest61;
			float saferPower557 = abs( ( 1.0 - voroi61 ) );
			float screenDepth17 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth17 = abs( ( screenDepth17 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( ( _FoamDistance * 0.3 ) ) );
			#ifdef _ENABLEFOAM_ON
				float staticSwitch538 = ( saturate( ( pow( saferPower557 , -1.5 ) + ( 1.0 - distanceDepth17 ) ) ) * _FoamOpacity );
			#else
				float staticSwitch538 = 0.0;
			#endif
			float Foam183 = staticSwitch538;
			float4 Color502 = ( lerpResult389 + Foam183 );
			o.Albedo = Color502.rgb;
			o.Smoothness = ( _Smoothness - Foam183 );
			float screenDepth390 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth390 = saturate( abs( ( screenDepth390 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( ( _CoastalBlending * 0.5 ) ) ) );
			o.Alpha = distanceDepth390;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;529;-2584.429,-805.9885;Inherit;False;832;251;;1;524;Time;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-2519.143,-706.5082;Inherit;False;Property;_WavesSpeed;Waves Speed;4;0;Create;True;0;0;0;False;0;False;0.3;0.15;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;95;-2583.085,-471.5587;Inherit;False;914.8306;366.1572;;3;68;66;106;World Space Tile;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;524;-2216.503,-706.0734;Inherit;False;1;0;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;94;-4846.841,-813.0936;Inherit;False;2186.94;615.6711;;2;547;532;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;525;-1997.5,-706.0734;Inherit;False;Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;66;-2535.119,-371.4225;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;100;-2275.704,-212.4924;Inherit;False;Property;_Tiling;Tiling;3;0;Create;False;0;0;0;False;0;False;0.15;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;106;-2274.506,-336.3506;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;532;-4811.517,-526.2394;Inherit;False;525;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;533;-4628.842,-525.9211;Inherit;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;564;-2058.308,-281.5123;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;546;-4424.781,-349.8685;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-1882.602,-282.2153;Inherit;False;WorldSpaceTile;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;53;-4194.032,-342.7578;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;-4389.567,-528.772;Inherit;False;68;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;114;-4002.688,-390.5459;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-4051.327,-538.0314;Inherit;False;Property;_NormalIntensity;Normal Intensity;5;0;Create;True;0;0;0;True;0;False;1;0.65;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;547;-4430.116,-615.9222;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;112;-4000.802,-722.5079;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;475;-3714.394,-319.2642;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;88;-3746.536,-577.0309;Inherit;True;Property;_WavesNormal;Waves Normal;0;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;0;False;None;None;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.WireNode;562;-3742.636,-581.6293;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;563;-3743.636,-356.6293;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;-3452.905,-435.7638;Inherit;True;Property;_TextureSample1;Texture Sample 1;9;1;[Normal];Create;True;0;0;0;False;0;False;88;None;None;True;0;True;bump;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;38;-3453.7,-751.3057;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;1;[Normal];Create;True;0;0;0;False;0;False;88;None;None;True;0;True;bump;Auto;True;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;179;-4846.876,-1485.19;Inherit;False;2192.071;602.2827;;4;18;531;183;538;Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.BlendNormalsNode;366;-3088.602,-589.6835;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;89;-2868.122,-593.9658;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;276;-4813.861,364.0716;Inherit;False;Property;_RefractionFactor;Refraction Factor;9;0;Create;True;0;0;0;False;0;False;0.5;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;531;-4797.42,-1299.021;Inherit;False;525;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;130;-4638.324,-1404.5;Inherit;False;68;WorldSpaceTile;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleNode;560;-4581.165,-1298.973;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;344;-4514.776,368.3385;Inherit;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-4547.08,239.1602;Inherit;False;89;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-4581.932,-1196.854;Inherit;False;Property;_FoamTiling;Tiling;11;0;Create;False;0;0;0;False;0;False;50;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-4693.312,-1019.369;Inherit;False;Property;_FoamDistance;Distance;13;0;Create;False;0;0;0;False;0;False;0.07;0.207;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;561;-4369.381,-1018.894;Inherit;False;0.3;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;-4298.193,286.221;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GrabScreenPosition;273;-4391.817,-15.88059;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VoronoiNode;61;-4356.62,-1323.371;Inherit;False;0;1;1;0;3;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT;1;FLOAT2;2
Node;AmplifyShaderEditor.OneMinusNode;559;-4075.166,-1323.973;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;558;-4077.166,-1164.973;Inherit;False;Constant;_Float0;Float 0;20;0;Create;True;0;0;0;False;0;False;-1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;17;-4168.696,-1042.887;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;277;-4107.907,168.8873;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;343;-4846.966,-109.4729;Inherit;False;2184.978;814.9983;;5;342;302;282;341;274;Refraction;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;340;-3868.313,612.9766;Inherit;False;Constant;_Float2;Float 2;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;557;-3889.166,-1254.973;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;19;-3888.096,-1043.147;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;338;-3980.032,418.3849;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;337;-3925.868,322.9119;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;339;-3868.641,518.1597;Inherit;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;186;-3679.546,-1165.326;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;336;-3655.318,430.938;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;341;-3713.221,-16.95279;Inherit;False;Global;_GrabScreen1;Grab Screen 1;12;0;Create;True;0;0;0;False;0;False;Instance;274;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;274;-3661.647,169.8504;Inherit;False;Global;_GrabScreen0;Grab Screen 0;12;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;504;-2587.859,-1490.296;Inherit;False;1538.612;598.8039;;1;385;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;188;-3528.267,-1166.05;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;410;-3660.923,-1027.311;Inherit;False;Property;_FoamOpacity;Opacity;12;0;Create;False;0;0;0;False;0;False;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;342;-3346.864,171.2634;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;411;-3327.13,-1105.829;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;302;-3145.958,171.6933;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;385;-2557.504,-1130.779;Inherit;False;Property;_Transparency;Transparency;6;0;Create;True;0;0;0;False;0;False;0;1.99;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;384;-2258.301,-1153.979;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;406;-2291.112,-1034.65;Inherit;False;Property;_TransparencyFade;Transparency Fade;7;0;Create;True;0;0;0;False;0;False;0;0.4;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;282;-2955.281,171.0323;Inherit;False;Refractions;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;538;-3160.494,-1130.239;Inherit;False;Property;_EnableFoam;Enable;10;0;Create;False;0;0;0;False;2;Header(Foam);Space(5);False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;381;-1970.564,-1406.371;Inherit;False;282;Refractions;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;85;-2007.613,-1301.857;Inherit;False;Property;_WaterColor;Water Color;1;0;Create;True;0;0;0;False;2;Header(Water);Space(5);False;0.2705882,0.4823529,0.5372549,0;0.2716558,0.4827171,0.539,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;405;-1938.112,-1100.649;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-2905.787,-1130.36;Inherit;False;Foam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;408;-1708.558,-1085.917;Inherit;False;183;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;389;-1695.62,-1257.237;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;407;-1469.256,-1176.618;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;391;-1555.582,-306.6877;Inherit;False;Property;_CoastalBlending;Coastal Blending;8;0;Create;True;0;0;0;False;0;False;1;0.17;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;502;-1273.207,-1182.801;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;392;-1263.46,-305.5437;Inherit;False;0.5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1311.092,-538.3046;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;0;False;0;False;0.96;0.917;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;498;-1214.798,-434.9816;Inherit;False;183;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;390;-1094.584,-329.6877;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;505;-1025.322,-703.5594;Inherit;False;502;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;-1024.467,-604.3355;Inherit;False;89;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;499;-994.7999,-498.9826;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-770.1937,-607.7742;Float;False;True;-1;2;;0;0;Standard;Raygeas/Built-In/Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;0;15;10;70;False;0;True;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;524;0;58;0
WireConnection;525;0;524;0
WireConnection;106;0;66;1
WireConnection;106;1;66;3
WireConnection;533;0;532;0
WireConnection;564;0;106;0
WireConnection;564;1;100;0
WireConnection;546;0;533;0
WireConnection;68;0;564;0
WireConnection;53;0;546;0
WireConnection;114;0;101;0
WireConnection;114;1;53;0
WireConnection;547;0;533;0
WireConnection;112;0;101;0
WireConnection;112;1;547;0
WireConnection;475;0;114;0
WireConnection;562;0;127;0
WireConnection;563;0;127;0
WireConnection;46;0;88;0
WireConnection;46;1;475;0
WireConnection;46;5;563;0
WireConnection;38;0;88;0
WireConnection;38;1;112;0
WireConnection;38;5;562;0
WireConnection;366;0;38;0
WireConnection;366;1;46;0
WireConnection;89;0;366;0
WireConnection;560;0;531;0
WireConnection;344;0;276;0
WireConnection;561;0;18;0
WireConnection;278;0;281;0
WireConnection;278;1;344;0
WireConnection;61;0;130;0
WireConnection;61;1;560;0
WireConnection;61;2;5;0
WireConnection;559;0;61;0
WireConnection;17;0;561;0
WireConnection;277;0;273;0
WireConnection;277;1;278;0
WireConnection;557;0;559;0
WireConnection;557;1;558;0
WireConnection;19;0;17;0
WireConnection;337;0;277;0
WireConnection;186;0;557;0
WireConnection;186;1;19;0
WireConnection;336;0;337;0
WireConnection;336;1;338;0
WireConnection;336;2;339;0
WireConnection;336;4;340;0
WireConnection;341;0;273;0
WireConnection;274;0;277;0
WireConnection;188;0;186;0
WireConnection;342;0;341;0
WireConnection;342;1;274;0
WireConnection;342;2;336;0
WireConnection;411;0;188;0
WireConnection;411;1;410;0
WireConnection;302;0;342;0
WireConnection;384;0;385;0
WireConnection;282;0;302;0
WireConnection;538;0;411;0
WireConnection;405;0;384;0
WireConnection;405;1;406;0
WireConnection;183;0;538;0
WireConnection;389;0;381;0
WireConnection;389;1;85;0
WireConnection;389;2;405;0
WireConnection;407;0;389;0
WireConnection;407;1;408;0
WireConnection;502;0;407;0
WireConnection;392;0;391;0
WireConnection;390;0;392;0
WireConnection;499;0;16;0
WireConnection;499;1;498;0
WireConnection;0;0;505;0
WireConnection;0;1;103;0
WireConnection;0;4;499;0
WireConnection;0;9;390;0
ASEEND*/
//CHKSM=B86DD471777676042FF5E6D82541DC92E107FA33
// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Built-In/Waterfall"
{
	Properties
	{
		[NoScaleOffset][Normal][Header(Maps)][Space(10)]_NormalMap("Normal", 2D) = "bump" {}
		_NormalTilingX("Normal Tiling X", Float) = 1
		_NormalTilingY("Normal Tiling Y", Float) = 1
		[Header(Settings)][Space(5)]_MainColor("Main Color", Color) = (1,1,1,0)
		_FlowSpeed("Flow Speed", Float) = 1
		_NormalScale("Normal", Float) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_RefractionFactor("Refraction Factor", Range( 0 , 1)) = 0.5
		[Header(Foam)][Space(5)]_FoamColor("Color", Color) = (1,1,1,0)
		_FoamTilingX("Tiling X", Float) = 1
		_FoamTilingY("Tiling Y", Float) = 1
		_FoamVoronoiSpeed("Voronoi Speed", Float) = 1
		_FoamLevel("Level", Float) = 0
		_FoamFade("Fade", Float) = 1
		_FoamScale("Scale", Float) = 1
		_FoamOffset("Offset", Float) = 1
		[Header(Gradient)][Space(5)]_GradientColor("Color", Color) = (1,1,1,0)
		_GradientLevel("Level", Float) = 0
		_GradientFade("Fade", Float) = 1
		[Header(Vertex Offset)][Space(5)]_VOIntensity("Intensity", Float) = 1
		_VOScale("Scale", Float) = 5
		[Header(Opacity)][Space(5)][Toggle(_OPACITYENABLE_ON)] _OpacityEnable("Enable", Float) = 0
		_OpacityLevel("Level", Float) = 0
		_OpacityFade("Fade", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _OPACITYENABLE_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float eyeDepth;
		};

		uniform float _FlowSpeed;
		uniform float _VOScale;
		uniform float _VOIntensity;
		uniform float _GradientLevel;
		uniform float _GradientFade;
		uniform sampler2D _NormalMap;
		uniform float _NormalTilingX;
		uniform float _NormalTilingY;
		uniform float _NormalScale;
		uniform float4 _MainColor;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _RefractionFactor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float4 _FoamColor;
		uniform float _FoamTilingX;
		uniform float _FoamTilingY;
		uniform float _FoamVoronoiSpeed;
		uniform float _FoamScale;
		uniform float _FoamOffset;
		uniform float _FoamLevel;
		uniform float _FoamFade;
		uniform float4 _GradientColor;
		uniform float _Smoothness;
		uniform float _OpacityLevel;
		uniform float _OpacityFade;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


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


		inline float2 UnityVoronoiRandomVector( float2 UV, float offset )
		{
			float2x2 m = float2x2( 15.27, 47.63, 99.41, 89.98 );
			UV = frac( sin(mul(UV, m) ) * 46839.32 );
			return float2( sin(UV.y* +offset ) * 0.5 + 0.5, cos( UV.x* offset ) * 0.5 + 0.5 );
		}
		
		//x - Out y - Cells
		float3 UnityVoronoi( float2 UV, float AngleOffset, float CellDensity, inout float2 mr )
		{
			float2 g = floor( UV * CellDensity );
			float2 f = frac( UV * CellDensity );
			float t = 8.0;
			float3 res = float3( 8.0, 0.0, 0.0 );
		
			for( int y = -1; y <= 1; y++ )
			{
				for( int x = -1; x <= 1; x++ )
				{
					float2 lattice = float2( x, y );
					float2 offset = UnityVoronoiRandomVector( lattice + g, AngleOffset );
					float d = distance( lattice + offset, f );
		
					if( d < res.x )
					{
						mr = f - lattice - offset;
						res = float3( d, offset.x, offset.y );
					}
				}
			}
			return res;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float Time98 = _Time.y;
			float Flow_Speed156 = ( _FlowSpeed * 0.1 );
			float2 uv_TexCoord109 = v.texcoord.xy + ( float2( 0,1 ) * ( Time98 * ( Flow_Speed156 * 7 ) ) );
			float simplePerlin2D100 = snoise( uv_TexCoord109*_VOScale );
			simplePerlin2D100 = simplePerlin2D100*0.5 + 0.5;
			float Gradient74 = saturate( ( ( v.texcoord.xy.y + _GradientLevel ) * _GradientFade ) );
			float3 lerpResult105 = lerp( float3( 0,0,0 ) , ( ase_vertexNormal * ( simplePerlin2D100 * _VOIntensity ) ) , Gradient74);
			float3 Vertex_Offset144 = lerpResult105;
			v.vertex.xyz += Vertex_Offset144;
			v.vertex.w = 1;
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult165 = (float2(_NormalTilingX , _NormalTilingY));
			float Flow_Speed156 = ( _FlowSpeed * 0.1 );
			float Time98 = _Time.y;
			float2 uv_TexCoord158 = i.uv_texcoord * appendResult165 + ( float2( 0,1 ) * Flow_Speed156 * ( Time98 * 1.3 ) );
			float3 Normal148 = UnpackScaleNormal( tex2D( _NormalMap, uv_TexCoord158 ), _NormalScale );
			o.Normal = Normal148;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor126 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPosNorm.xy);
			float4 temp_output_121_0 = ( ase_grabScreenPosNorm + float4( ( Normal148 * ( _RefractionFactor * 0.1 ) ) , 0.0 ) );
			float4 screenColor131 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,temp_output_121_0.xy);
			float eyeDepth123 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, temp_output_121_0.xy ));
			float ifLocalVar125 = 0;
			if( eyeDepth123 > i.eyeDepth )
				ifLocalVar125 = 1.0;
			else if( eyeDepth123 < i.eyeDepth )
				ifLocalVar125 = 0.0;
			float4 lerpResult127 = lerp( screenColor126 , screenColor131 , ifLocalVar125);
			float4 Refractions132 = saturate( lerpResult127 );
			float4 lerpResult137 = lerp( _MainColor , Refractions132 , ( 1.0 - _MainColor.a ));
			float2 appendResult59 = (float2(_FoamTilingX , _FoamTilingY));
			float2 uv_TexCoord21 = i.uv_texcoord * appendResult59 + ( ( float2( 0,1 ) * Flow_Speed156 ) * Time98 );
			float2 uv17 = 0;
			float3 unityVoronoy17 = UnityVoronoi(uv_TexCoord21,( Time98 * _FoamVoronoiSpeed ),15.0,uv17);
			float saferPower45 = abs( unityVoronoy17.x );
			float lerpResult53 = lerp( 0.0 , saturate( (pow( saferPower45 , 3.0 )*_FoamScale + ( 1.0 - _FoamOffset )) ) , ( ( i.uv_texcoord.y + _FoamLevel ) * _FoamFade ));
			float Foam54 = saturate( lerpResult53 );
			float Gradient74 = saturate( ( ( i.uv_texcoord.y + _GradientLevel ) * _GradientFade ) );
			float4 lerpResult77 = lerp( ( _FoamColor * Foam54 ) , ( _GradientColor * Gradient74 ) , Gradient74);
			float4 Color8 = ( lerpResult137 + lerpResult77 );
			o.Emission = Color8.rgb;
			float Smoothness146 = ( saturate( ( 1.0 - ( Foam54 + Gradient74 ) ) ) * _Smoothness );
			o.Smoothness = Smoothness146;
			#ifdef _OPACITYENABLE_ON
				float staticSwitch180 = saturate( ( ( i.uv_texcoord.y + _OpacityLevel ) * _OpacityFade ) );
			#else
				float staticSwitch180 = 1.0;
			#endif
			float Opacity177 = staticSwitch180;
			o.Alpha = Opacity177;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float4 tSpace0 : TEXCOORD4;
				float4 tSpace1 : TEXCOORD5;
				float4 tSpace2 : TEXCOORD6;
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
				o.customPack1.z = customInputData.eyeDepth;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.eyeDepth = IN.customPack1.z;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;149;-2070.646,-825.3487;Inherit;False;767.8995;353.2796;;5;156;169;39;98;18;Time & Flow Speed;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;18;-1988.522,-729.5422;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1985.24,-618.2674;Inherit;False;Property;_FlowSpeed;Flow Speed;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;150;-1761.65,1561.575;Inherit;False;1702.514;853.0475;;12;5;165;160;148;163;164;159;162;161;158;4;193;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-1751.591,-729.8784;Inherit;False;Time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;169;-1753.094,-618.6454;Inherit;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;141;-4048.598,362.6495;Inherit;False;2746.4;1079.211;;27;2;3;13;12;54;15;53;90;86;91;88;87;45;46;17;19;20;21;59;60;62;38;33;99;36;157;197;Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-1550.714,-619.2427;Inherit;False;Flow Speed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-1707.118,2216.833;Inherit;False;98;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;-3967.272,777.5289;Inherit;False;156;Flow Speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;33;-3937.658,616.2988;Inherit;False;Constant;_Vector0;Vector 0;9;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;161;-1595.78,2082.724;Inherit;False;156;Flow Speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;159;-1563.684,1937.63;Inherit;False;Constant;_Vector2;Vector 0;9;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;164;-1598.33,1760.563;Inherit;False;Property;_NormalTilingX;Normal Tiling X;1;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-1597.767,1852.017;Inherit;False;Property;_NormalTilingY;Normal Tiling Y;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;193;-1488.302,2188.627;Inherit;False;1.3;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3701.976,680.6547;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;-3750.777,881.267;Inherit;False;98;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;-1302.516,2041.55;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;165;-1305.103,1801.784;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-3756.249,482.8224;Inherit;False;Property;_FoamTilingX;Tiling X;9;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-3754.52,574.2764;Inherit;False;Property;_FoamTilingY;Tiling Y;10;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-3483.792,772.5659;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;-3484.854,516.8194;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;158;-1019.616,1881.472;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-3488.845,978.9445;Inherit;False;Property;_FoamVoronoiSpeed;Voronoi Speed;11;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-987.5886,2046.724;Inherit;False;Property;_NormalScale;Normal;5;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;151;-4053.852,1558.449;Inherit;False;2173.808;857.8049;;16;132;128;130;129;117;124;131;127;126;125;123;122;121;120;119;118;Refraction;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-3193.739,880.8233;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-3256.813,609.87;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-698.8611,1927.684;Inherit;True;Property;_NormalMap;Normal;0;2;[NoScaleOffset];[Normal];Create;False;0;0;0;False;2;Header(Maps);Space(10);False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VoronoiNode;17;-2947.212,733.5634;Inherit;False;0;0;1;0;1;False;1;True;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;15;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;129;-3981.626,2048.358;Inherit;False;Property;_RefractionFactor;Refraction Factor;7;0;Create;True;0;0;0;False;0;False;0.5;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;148;-357.4237,1927.829;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2946.132,885.292;Inherit;False;Constant;_FoamPower;Foam Power;8;0;Create;True;0;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-2858.115,1014.777;Inherit;False;Property;_FoamOffset;Offset;15;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;167;-1203.314,954.9199;Inherit;False;1316.54;479.3688;;7;74;71;68;69;67;66;70;Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;45;-2654.832,797.2023;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;11.79;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;91;-2652.347,1015.261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-2862.289,1126.505;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;118;-3667.485,2047.608;Inherit;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;130;-3699.789,1918.431;Inherit;False;148;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-2654.474,910.0485;Inherit;False;Property;_FoamScale;Scale;14;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-2800.704,1295.569;Inherit;False;Property;_FoamLevel;Level;12;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;142;-4045.229,-342.6966;Inherit;False;2742.297;574.4365;;16;168;155;114;108;144;105;104;113;112;107;100;109;111;110;115;102;Vertex Offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;86;-2405.237,886.8078;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-2541.194,1176.364;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-3450.903,1965.491;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GrabScreenPosition;120;-3544.527,1663.389;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;66;-1077.536,1055.978;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;70;-1014.176,1216.043;Inherit;False;Property;_GradientLevel;Level;17;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2509.372,1289.13;Inherit;False;Property;_FoamFade;Fade;13;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;90;-2123.45,886.5231;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;121;-3260.617,1848.158;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;-3920.55,-7.882363;Inherit;False;156;Flow Speed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-768.1765,1144.044;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-798.1559,1277.727;Inherit;False;Property;_GradientFade;Fade;18;0;Create;False;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;197;-2293.494,1177.631;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;53;-1882.326,861.9011;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;122;-3132.742,2097.654;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;123;-3078.578,2002.182;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-3021.351,2197.429;Inherit;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-3021.023,2292.246;Inherit;False;Constant;_Float2;Float 2;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;102;-3736.485,-103.6164;Inherit;False;98;Time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;155;-3703.074,-8.522358;Inherit;False;7;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-568.5619,1193.855;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;15;-1722.93,861.4791;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;125;-2808.028,2110.207;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;126;-2836.061,1660.895;Inherit;False;Global;_GrabScreen1;Grab Screen 1;12;0;Create;True;0;0;0;False;0;False;Instance;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;131;-2814.356,1840.587;Inherit;False;Global;_GrabScreen0;Grab Screen 0;12;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-3484.85,-74.408;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;110;-3485.007,-217.5462;Inherit;False;Constant;_Vector1;Vector 1;18;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;71;-368.5375,1193.109;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;178;-1200.71,-156.1436;Inherit;False;1572.483;387.1183;;10;177;180;174;172;176;175;170;181;192;191;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;147;-1202.725,372.5199;Inherit;False;1573.828;472.3382;;8;93;92;146;96;1;97;95;94;Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-1548.367,861.9572;Inherit;False;Foam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;127;-2529.876,1870.736;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-3244.876,-166.0151;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-162.6226,1193.708;Inherit;False;Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;143;-4043.123,-1388.131;Inherit;False;1871.227;917.9144;;15;166;75;154;8;7;133;140;10;137;77;79;72;41;55;78;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;128;-2328.968,1871.166;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1043.969,497.1916;Inherit;False;54;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-1047.453,581.9653;Inherit;False;74;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;109;-3031.099,-213.8909;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;114;-2966.781,-41.42624;Inherit;False;Property;_VOScale;Scale;20;0;Create;False;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;170;-1148.81,-94.1507;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;174;-1081.955,78.23698;Inherit;False;Property;_OpacityLevel;Level;22;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;132;-2132.29,1870.505;Inherit;False;Refractions;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-3893.622,-1080.051;Inherit;False;54;Foam;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-3393.447,-1299.432;Inherit;False;Property;_MainColor;Main Color;3;0;Create;True;0;0;0;False;2;Header(Settings);Space(5);False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;75;-3890.999,-747.2593;Inherit;False;74;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;-793.7116,525.5784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;100;-2726.221,-138.5516;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;-3925.924,-1256.769;Inherit;False;Property;_FoamColor;Color;8;0;Create;False;0;0;0;False;2;Header(Foam);Space(5);False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;72;-3924.587,-943.8919;Inherit;False;Property;_GradientColor;Color;16;0;Create;False;0;0;0;False;2;Header(Gradient);Space(5);False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;108;-2644.398,95.51114;Inherit;False;Property;_VOIntensity;Intensity;19;0;Create;False;0;0;0;False;2;Header(Vertex Offset);Space(5);False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;192;-863.3352,2.585304;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-894.1249,149.6954;Inherit;False;Property;_OpacityFade;Fade;23;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-3612.787,-863.8994;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-3610.15,-1183.665;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;140;-3157.788,-1204.06;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;133;-3181.871,-1094.214;Inherit;False;132;Refractions;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;154;-3027.039,-1242.473;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;-3638.955,-632.738;Inherit;False;74;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-2391.296,-35.49033;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;112;-2425.479,-201.1879;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;95;-638.4185,526.2563;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;-666.3352,60.58531;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;77;-3302.107,-887.8298;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;137;-2930.177,-1215.13;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-2143.376,-137.0861;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-2173.161,1.964592;Inherit;False;74;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-448.4816,526.9014;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-572.5314,681.0917;Inherit;False;Property;_Smoothness;Smoothness;6;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;176;-294.5294,60.14187;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;-293.8599,-37.72567;Inherit;False;Constant;_Float0;Float 0;24;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-2626.28,-913.5861;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;105;-1888.654,-108.2344;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-215.4814,587.9324;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;180;-120.342,35.95567;Inherit;False;Property;_OpacityEnable;Enable;21;0;Create;False;0;0;0;False;2;Header(Opacity);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-2440.759,-913.2786;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-1664.327,-110.0565;Inherit;False;Vertex Offset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;146;-11.73541,587.1765;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;177;133.5994,35.66837;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;145;-850.7821,-528.0842;Inherit;False;144;Vertex Offset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-819.0582,-621.1563;Inherit;False;177;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;9;-819.1131,-809.8279;Inherit;False;8;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-819.8272,-720.3153;Inherit;False;146;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-820.1661,-896.693;Inherit;False;148;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;175;-474.2018,60.59329;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-541.1989,-879.769;Float;False;True;-1;2;;0;0;Standard;Raygeas/Built-In/Waterfall;False;False;False;False;False;False;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;5;True;2;5;False;;10;False;;2;5;False;;10;False;;0;False;;2;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;98;0;18;0
WireConnection;169;0;39;0
WireConnection;156;0;169;0
WireConnection;193;0;162;0
WireConnection;38;0;33;0
WireConnection;38;1;157;0
WireConnection;160;0;159;0
WireConnection;160;1;161;0
WireConnection;160;2;193;0
WireConnection;165;0;164;0
WireConnection;165;1;163;0
WireConnection;36;0;38;0
WireConnection;36;1;99;0
WireConnection;59;0;60;0
WireConnection;59;1;62;0
WireConnection;158;0;165;0
WireConnection;158;1;160;0
WireConnection;20;0;99;0
WireConnection;20;1;19;0
WireConnection;21;0;59;0
WireConnection;21;1;36;0
WireConnection;5;1;158;0
WireConnection;5;5;4;0
WireConnection;17;0;21;0
WireConnection;17;1;20;0
WireConnection;148;0;5;0
WireConnection;45;0;17;0
WireConnection;45;1;46;0
WireConnection;91;0;88;0
WireConnection;118;0;129;0
WireConnection;86;0;45;0
WireConnection;86;1;87;0
WireConnection;86;2;91;0
WireConnection;12;0;2;2
WireConnection;12;1;3;0
WireConnection;119;0;130;0
WireConnection;119;1;118;0
WireConnection;90;0;86;0
WireConnection;121;0;120;0
WireConnection;121;1;119;0
WireConnection;67;0;66;2
WireConnection;67;1;70;0
WireConnection;197;0;12;0
WireConnection;197;1;13;0
WireConnection;53;1;90;0
WireConnection;53;2;197;0
WireConnection;123;0;121;0
WireConnection;155;0;168;0
WireConnection;68;0;67;0
WireConnection;68;1;69;0
WireConnection;15;0;53;0
WireConnection;125;0;123;0
WireConnection;125;1;122;0
WireConnection;125;2;124;0
WireConnection;125;4;117;0
WireConnection;126;0;120;0
WireConnection;131;0;121;0
WireConnection;115;0;102;0
WireConnection;115;1;155;0
WireConnection;71;0;68;0
WireConnection;54;0;15;0
WireConnection;127;0;126;0
WireConnection;127;1;131;0
WireConnection;127;2;125;0
WireConnection;111;0;110;0
WireConnection;111;1;115;0
WireConnection;74;0;71;0
WireConnection;128;0;127;0
WireConnection;109;1;111;0
WireConnection;132;0;128;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;100;0;109;0
WireConnection;100;1;114;0
WireConnection;192;0;170;2
WireConnection;192;1;174;0
WireConnection;78;0;72;0
WireConnection;78;1;75;0
WireConnection;79;0;41;0
WireConnection;79;1;55;0
WireConnection;140;0;10;4
WireConnection;154;0;10;0
WireConnection;107;0;100;0
WireConnection;107;1;108;0
WireConnection;95;0;94;0
WireConnection;191;0;192;0
WireConnection;191;1;172;0
WireConnection;77;0;79;0
WireConnection;77;1;78;0
WireConnection;77;2;166;0
WireConnection;137;0;154;0
WireConnection;137;1;133;0
WireConnection;137;2;140;0
WireConnection;113;0;112;0
WireConnection;113;1;107;0
WireConnection;97;0;95;0
WireConnection;176;0;191;0
WireConnection;7;0;137;0
WireConnection;7;1;77;0
WireConnection;105;1;113;0
WireConnection;105;2;104;0
WireConnection;96;0;97;0
WireConnection;96;1;1;0
WireConnection;180;1;181;0
WireConnection;180;0;176;0
WireConnection;8;0;7;0
WireConnection;144;0;105;0
WireConnection;146;0;96;0
WireConnection;177;0;180;0
WireConnection;0;1;153;0
WireConnection;0;2;9;0
WireConnection;0;4;152;0
WireConnection;0;9;179;0
WireConnection;0;11;145;0
ASEEND*/
//CHKSM=0DE9D95905B26CD6EA0FA268ADB88ADC56BB886B
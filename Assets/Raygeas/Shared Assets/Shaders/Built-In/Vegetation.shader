// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Built-In/Vegetation"
{
	Properties
	{
		[Header(Maps)][Space(5)]_Tiling("Tiling", Float) = 1
		[NoScaleOffset][Space(5)]_Albedo("Base", 2D) = "white" {}
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		[NoScaleOffset]_SmoothnessTexture("Smoothness", 2D) = "white" {}
		_AlphaCutoff("Opacity Cutoff", Range( 0 , 1)) = 0.35
		[Header(Settings)][Space(5)]_MainColor("Main Color", Color) = (1,1,1,0)
		_NormalScale("Normal", Float) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[Header(Second Color)][Space(5)][Toggle(_COLOR2ENABLE_ON)] _Color2Enable("Enable", Float) = 0
		[Space(10)]_SecondColor("Second Color", Color) = (0,0,0,0)
		[KeywordEnum(World_Noise_2D,World_Noise_3D,Vertex_Gradient,UV_Gradient)] _SecondColorOverlayType("Overlay Method", Float) = 0
		_SecondColorOffset("Offset", Float) = 1
		_SecondColorFade("Balance", Float) = 1
		_WorldNoiseScale("World Noise Scale", Float) = 1
		[Header(Wind)][Space(5)][Toggle(_ENABLEWIND_ON)] _EnableWind("Enable", Float) = 1
		[Space(10)]_WindForce("Force", Range( 0 , 1)) = 0.6696684
		_WindWavesScale("Waves Scale", Range( 0 , 1)) = 0.25
		_WindFlowDensity("Flow Density", Range( 0.5 , 5)) = 1
		[Toggle]_UVBaseLock("UV Base Lock", Float) = 0
		[Toggle][Header(Grass Distance Fade)][Space(5)]_GrassDistanceFadeEnable("Enable", Float) = 0
		[Space(10)]_GrassFadeDistance("Distance", Float) = 30
		_GrassFalloff("Falloff", Range( 0 , 1)) = 0.7
		[Header(Lighting)][Space(5)]_LightingFlatness("Lighting Flatness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ENABLEWIND_ON
		#pragma shader_feature_local _COLOR2ENABLE_ON
		#pragma shader_feature_local _SECONDCOLOROVERLAYTYPE_WORLD_NOISE_2D _SECONDCOLOROVERLAYTYPE_WORLD_NOISE_3D _SECONDCOLOROVERLAYTYPE_VERTEX_GRADIENT _SECONDCOLOROVERLAYTYPE_UV_GRADIENT
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float3 RAYGlobalDirection;
		uniform float _WindForce;
		uniform float _WindWavesScale;
		uniform float RAYGlobalWavesScale;
		uniform float _WindFlowDensity;
		uniform float RAYGlobalFlowDensity;
		uniform float _UVBaseLock;
		uniform float RAYGlobalWindForce;
		uniform float _GrassFadeDistance;
		uniform float _GrassFalloff;
		uniform float _GrassDistanceFadeEnable;
		uniform float _LightingFlatness;
		uniform sampler2D _Normal;
		uniform float _Tiling;
		uniform float _NormalScale;
		uniform float4 _MainColor;
		uniform sampler2D _Albedo;
		uniform float4 _SecondColor;
		uniform float _WorldNoiseScale;
		uniform float _SecondColorOffset;
		uniform float _SecondColorFade;
		uniform sampler2D _SmoothnessTexture;
		uniform float _Smoothness;
		uniform float _AlphaCutoff;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime34 = _Time.y * ( RAYGlobalWindForce * ( _WindForce * 5 ) );
			float simplePerlin3D35 = snoise( ( ase_worldPos + mulTime34 )*( ( 1.0 - _WindWavesScale ) * RAYGlobalWavesScale ) );
			simplePerlin3D35 = simplePerlin3D35*0.5 + 0.5;
			float temp_output_231_0 = ( pow( simplePerlin3D35 , ( _WindFlowDensity * RAYGlobalFlowDensity ) ) * 0.01 );
			float lerpResult1020 = lerp( temp_output_231_0 , ( temp_output_231_0 * pow( v.texcoord.xy.y , 1.5 ) ) , _UVBaseLock);
			float4 transform916 = mul(unity_WorldToObject,float4( ( RAYGlobalDirection * ( lerpResult1020 * v.color.r * ( ( _WindForce * 100 ) * RAYGlobalWindForce ) ) ) , 0.0 ));
			#ifdef _ENABLEWIND_ON
				float4 staticSwitch341 = transform916;
			#else
				float4 staticSwitch341 = float4( 0,0,0,0 );
			#endif
			float4 Wind191 = staticSwitch341;
			float cameraDepthFade958 = (( -UnityObjectToViewPos( v.vertex.xyz ).z -_ProjectionParams.y - _GrassFadeDistance ) / 5.0);
			float lerpResult1039 = lerp( ( 1.0 - cameraDepthFade958 ) , cameraDepthFade958 , ( _GrassFalloff * 0.5 ));
			float lerpResult1023 = lerp( 1.0 , saturate( lerpResult1039 ) , _GrassDistanceFadeEnable);
			float GrassDistanceFadeMask982 = lerpResult1023;
			float4 lerpResult1065 = lerp( float4( ( ase_vertex3Pos * -1 ) , 0.0 ) , Wind191 , GrassDistanceFadeMask982);
			v.vertex.xyz += lerpResult1065.xyz;
			v.vertex.w = 1;
			float3 ase_vertexNormal = v.normal.xyz;
			float3 lerpResult1096 = lerp( ase_vertexNormal , float3(0,1,0) , _LightingFlatness);
			float3 LightingFlatness1101 = lerpResult1096;
			v.normal = LightingFlatness1101;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_Tiling).xx;
			float2 uv_TexCoord1028 = i.uv_texcoord * temp_cast_0;
			float2 Tiling1032 = uv_TexCoord1028;
			float3 Normal888 = UnpackScaleNormal( tex2D( _Normal, Tiling1032 ), _NormalScale );
			o.Normal = Normal888;
			float4 tex2DNode1 = tex2D( _Albedo, Tiling1032 );
			float4 temp_output_10_0 = ( _MainColor * tex2DNode1 );
			float3 ase_worldPos = i.worldPos;
			float simplePerlin2D742 = snoise( (ase_worldPos).xz*_WorldNoiseScale );
			simplePerlin2D742 = simplePerlin2D742*0.5 + 0.5;
			float simplePerlin3D1015 = snoise( ase_worldPos*_WorldNoiseScale );
			simplePerlin3D1015 = simplePerlin3D1015*0.5 + 0.5;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			#if defined(_SECONDCOLOROVERLAYTYPE_WORLD_NOISE_2D)
				float staticSwitch360 = simplePerlin2D742;
			#elif defined(_SECONDCOLOROVERLAYTYPE_WORLD_NOISE_3D)
				float staticSwitch360 = simplePerlin3D1015;
			#elif defined(_SECONDCOLOROVERLAYTYPE_VERTEX_GRADIENT)
				float staticSwitch360 = ase_vertex3Pos.y;
			#elif defined(_SECONDCOLOROVERLAYTYPE_UV_GRADIENT)
				float staticSwitch360 = i.uv_texcoord.y;
			#else
				float staticSwitch360 = simplePerlin2D742;
			#endif
			float temp_output_875_0 = ( staticSwitch360 + ( 1.0 - _SecondColorOffset ) );
			float lerpResult1025 = lerp( temp_output_875_0 , ( 1.0 - temp_output_875_0 ) , ( _SecondColorFade - -0.5 ));
			float SecondColorMask335 = saturate( lerpResult1025 );
			float4 lerpResult332 = lerp( temp_output_10_0 , ( _SecondColor * tex2D( _Albedo, Tiling1032 ) ) , SecondColorMask335);
			#ifdef _COLOR2ENABLE_ON
				float4 staticSwitch1022 = lerpResult332;
			#else
				float4 staticSwitch1022 = temp_output_10_0;
			#endif
			float4 Albedo259 = staticSwitch1022;
			o.Albedo = Albedo259.rgb;
			float Smoothness734 = saturate( ( tex2D( _SmoothnessTexture, Tiling1032 ).g * _Smoothness ) );
			o.Smoothness = Smoothness734;
			o.Alpha = 1;
			float AlbedoAlpha263 = tex2DNode1.a;
			clip( AlbedoAlpha263 - _AlphaCutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.CommentaryNode;66;-5631.718,-260.3194;Inherit;False;3919.657;965.9094;;34;1066;35;1067;941;779;945;190;182;228;947;344;780;803;34;777;1021;915;775;188;359;358;341;1020;191;345;776;916;914;769;357;356;231;940;56;Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-5592.354,490.0497;Inherit;False;Property;_WindForce;Force;15;0;Create;False;0;0;0;False;1;Space(10);False;0.6696684;0.27;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;947;-5249.32,236.4667;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;344;-5177.039,43.42474;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;945;-5270.597,-71.62838;Inherit;False;Global;RAYGlobalWindForce;RAYGlobalWindForce;20;0;Fetch;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;336;-5630.573,772.877;Inherit;False;2182.876;716.7827;;18;334;335;1025;248;1026;360;743;310;745;1027;875;1017;871;361;1015;939;742;1048;Second Color Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;777;-4953.549,-29.21493;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-5016.729,122.1841;Inherit;False;Property;_WindWavesScale;Waves Scale;16;0;Create;False;0;0;0;False;0;False;0.25;0.47;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;745;-5577.272,891.5659;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;34;-4789.77,-29.02846;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;803;-4723.601,122.0921;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;228;-4788.961,-197.7769;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;779;-4819.625,244.3927;Float;False;Global;RAYGlobalWavesScale;RAYGlobalWavesScale;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;939;-5322.134,890.9437;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1017;-5352.771,1093.87;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;743;-5384.468,1002.72;Inherit;False;Property;_WorldNoiseScale;World Noise Scale;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;780;-4531.625,169.3926;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;182;-4498.28,-131.7945;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;941;-4532.451,286.8994;Inherit;False;Property;_WindFlowDensity;Flow Density;17;0;Create;False;0;0;0;False;0;False;1;1;0.5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1067;-4499.72,378.3408;Float;False;Global;RAYGlobalFlowDensity;RAYGlobalFlowDensity;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1033;-3398.161,1244.392;Inherit;False;788.1025;243.0056;;3;1031;1032;1028;Tiling;1,1,1,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;742;-5092.825,933.2988;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1015;-5094.324,1048.603;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;361;-5095.055,1322.688;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;871;-5061.946,1153.86;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;310;-4702.921,1260.388;Inherit;False;Property;_SecondColorOffset;Offset;11;0;Create;False;0;0;0;False;0;False;1;-0.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1066;-4191.94,315.3503;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;35;-4276.886,-6.457231;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1027;-4541.701,1250.711;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;360;-4766.671,1082.927;Inherit;False;Property;_SecondColorOverlayType;Overlay Method;10;0;Create;False;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;4;World_Noise_2D;World_Noise_3D;Vertex_Gradient;UV_Gradient;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1031;-3343.456,1346.191;Inherit;False;Property;_Tiling;Tiling;0;0;Create;True;0;0;0;False;2;Header(Maps);Space(5);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;940;-3910.744,140.0812;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;358;-3906.87,437.1895;Inherit;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;0;False;0;False;1.5;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-3972.463,307.049;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;981;-3453.76,-794.0844;Inherit;False;1738.585;437.0968;;10;982;1023;1024;959;960;1039;1046;958;1073;1095;Grass Distance Fade Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;875;-4392.424,1150.373;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1028;-3121.726,1322.873;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;231;-3710.768,139.7341;Inherit;False;0.01;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;356;-3708.32,384.8364;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;248;-4434.502,1334.592;Inherit;False;Property;_SecondColorFade;Balance;12;0;Create;False;0;0;0;False;0;False;1;0.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;262;-5626.675,-1242.181;Inherit;False;2115.644;893.7669;;15;1034;1035;1036;368;247;3;1022;337;259;263;332;367;10;1;366;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;1026;-4242.623,1224.432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1032;-2856.821,1321.974;Inherit;False;Tiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleNode;345;-3388.276,490.2618;Inherit;False;100;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;359;-3507.656,244.4772;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;775;-3481.333,577.2404;Float;False;Global;RAYGlobalWindForce;RAYGlobalWindForce;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1021;-3525.292,359.9566;Inherit;False;Property;_UVBaseLock;UV Base Lock;18;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;960;-3389.769,-590.0407;Inherit;False;Property;_GrassFadeDistance;Distance;20;0;Create;False;0;0;0;False;1;Space(10);False;30;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1048;-4244.53,1333.965;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1034;-5568.744,-763.387;Inherit;False;1032;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;1025;-4055.623,1149.432;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;769;-3203.848,273.3365;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;776;-3172.288,519.1628;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;958;-3206.669,-638.0401;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;959;-3236.969,-513.1837;Inherit;False;Property;_GrassFalloff;Falloff;21;0;Create;False;0;0;0;False;0;False;0.7;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1020;-3273.709,141.7515;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;733;-3401.677,782.6193;Inherit;False;1241.814;395.4615;;6;1038;681;708;734;893;709;Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;1036;-5327.594,-570.4702;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;1035;-5328.594,-883.5371;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;368;-5310.445,-786.1342;Inherit;True;Property;_Albedo;Base;1;1;[NoScaleOffset];Create;False;0;0;0;False;1;Space(5);False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SaturateNode;334;-3870.359,1149.847;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-2920.548,274.0343;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;915;-2986.027,101.7881;Inherit;False;Global;RAYGlobalDirection;RAYGlobalDirection;25;0;Create;True;0;0;0;False;0;False;0,0,0;-0.7886131,0,0.6148897;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScaleNode;1046;-2916.545,-513.7789;Inherit;False;0.5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1073;-2919.637,-693.6653;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;366;-5030.424,-590.7482;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-5031.256,-981.5227;Inherit;True;Property;_LeavesTexture;Leaves Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1038;-3350.293,875.436;Inherit;False;1032;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;247;-4946.804,-777.2593;Inherit;False;Property;_SecondColor;Second Color;9;0;Create;True;0;0;0;False;1;Space(10);False;0,0,0,0;0.09481568,0.6156863,0.3505847,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;335;-3688.51,1149.484;Inherit;False;SecondColorMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;914;-2691.047,180.5614;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;1039;-2721.529,-661.1333;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-4949.521,-1171.078;Inherit;False;Property;_MainColor;Main Color;5;0;Create;True;0;0;0;False;2;Header(Settings);Space(5);False;1,1,1,0;0.263263,0.4790527,0.574,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;892;-3447.783,-1235.904;Inherit;False;939.5;345.4;;4;887;1037;886;888;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-4626.809,-1081.864;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;367;-4629.995,-688.4203;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;337;-4656.514,-853.1598;Inherit;False;335;SecondColorMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;708;-3109.486,851.5749;Inherit;True;Property;_SmoothnessTexture;Smoothness;3;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;681;-3103.936,1059.007;Inherit;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;916;-2498.345,180.6679;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1024;-2483.135,-540.2987;Inherit;False;Property;_GrassDistanceFadeEnable;Enable;19;1;[Toggle];Create;False;0;0;0;False;2;Header(Grass Distance Fade);Space(5);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1095;-2489.531,-660.9793;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1102;-2096,784;Inherit;False;900;483;;5;1097;1098;1099;1096;1101;Lighting Flatness;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;332;-4360.349,-901.5045;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;709;-2752.487,976.5747;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;341;-2236.624,154.9358;Inherit;False;Property;_EnableWind;Enable;14;0;Create;False;0;0;0;False;2;Header(Wind);Space(5);False;0;1;1;True;;Toggle;2;;;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;1037;-3382.031,-1151.313;Inherit;False;1032;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;887;-3348.085,-1042.008;Inherit;False;Property;_NormalScale;Normal;6;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1023;-2234.135,-644.2987;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1097;-1952,992;Inherit;False;Constant;_Vector0;Vector 0;22;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;1099;-1952,832;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1098;-2048,1152;Inherit;False;Property;_LightingFlatness;Lighting Flatness;22;0;Create;True;0;0;0;False;2;Header(Lighting);Space(5);False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;893;-2580.616,976.8357;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-1950.11,154.1265;Inherit;False;Wind;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;886;-3107.923,-1134.495;Inherit;True;Property;_Normal;Normal;2;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;982;-2032.846,-644.1286;Inherit;False;GrassDistanceFadeMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1022;-4034.959,-1081.689;Inherit;False;Property;_Color2Enable;Enable;8;0;Create;False;0;0;0;False;2;Header(Second Color);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1096;-1664,960;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;1058;-1456,64;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-4658.396,-940.4174;Inherit;False;AlbedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;734;-2406.864,976.4997;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-3757.885,-1082.028;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;888;-2772.554,-1133.705;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1101;-1440,960;Inherit;False;LightingFlatness;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;917;-1232,160;Inherit;False;191;Wind;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScaleNode;1060;-1200,64;Inherit;False;-1;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1064;-1296,256;Inherit;False;982;GrassDistanceFadeMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;889;-992,-144;Inherit;False;888;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;806;-992,-48;Inherit;False;734;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-1088,-336;Inherit;False;Property;_AlphaCutoff;Opacity Cutoff;4;0;Create;False;0;0;0;False;0;False;0.35;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;804;-992,-224;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;774;-992,32;Inherit;False;263;AlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1065;-960,128;Inherit;False;3;0;FLOAT4;0,-3,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;1103;-1024,320;Inherit;False;1101;LightingFlatness;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;992;-736,-192;Float;False;True;-1;2;;0;0;Standard;Raygeas/Built-In/Vegetation;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;1;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;_CullMode;-1;0;True;_AlphaCutoff;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;947;0;56;0
WireConnection;344;0;947;0
WireConnection;777;0;945;0
WireConnection;777;1;344;0
WireConnection;34;0;777;0
WireConnection;803;0;190;0
WireConnection;939;0;745;0
WireConnection;780;0;803;0
WireConnection;780;1;779;0
WireConnection;182;0;228;0
WireConnection;182;1;34;0
WireConnection;742;0;939;0
WireConnection;742;1;743;0
WireConnection;1015;0;1017;0
WireConnection;1015;1;743;0
WireConnection;1066;0;941;0
WireConnection;1066;1;1067;0
WireConnection;35;0;182;0
WireConnection;35;1;780;0
WireConnection;1027;0;310;0
WireConnection;360;1;742;0
WireConnection;360;0;1015;0
WireConnection;360;2;871;2
WireConnection;360;3;361;2
WireConnection;940;0;35;0
WireConnection;940;1;1066;0
WireConnection;875;0;360;0
WireConnection;875;1;1027;0
WireConnection;1028;0;1031;0
WireConnection;231;0;940;0
WireConnection;356;0;357;2
WireConnection;356;1;358;0
WireConnection;1026;0;875;0
WireConnection;1032;0;1028;0
WireConnection;345;0;56;0
WireConnection;359;0;231;0
WireConnection;359;1;356;0
WireConnection;1048;0;248;0
WireConnection;1025;0;875;0
WireConnection;1025;1;1026;0
WireConnection;1025;2;1048;0
WireConnection;776;0;345;0
WireConnection;776;1;775;0
WireConnection;958;1;960;0
WireConnection;1020;0;231;0
WireConnection;1020;1;359;0
WireConnection;1020;2;1021;0
WireConnection;1036;0;1034;0
WireConnection;1035;0;1034;0
WireConnection;334;0;1025;0
WireConnection;188;0;1020;0
WireConnection;188;1;769;1
WireConnection;188;2;776;0
WireConnection;1046;0;959;0
WireConnection;1073;0;958;0
WireConnection;366;0;368;0
WireConnection;366;1;1036;0
WireConnection;1;0;368;0
WireConnection;1;1;1035;0
WireConnection;335;0;334;0
WireConnection;914;0;915;0
WireConnection;914;1;188;0
WireConnection;1039;0;1073;0
WireConnection;1039;1;958;0
WireConnection;1039;2;1046;0
WireConnection;10;0;3;0
WireConnection;10;1;1;0
WireConnection;367;0;247;0
WireConnection;367;1;366;0
WireConnection;708;1;1038;0
WireConnection;916;0;914;0
WireConnection;1095;0;1039;0
WireConnection;332;0;10;0
WireConnection;332;1;367;0
WireConnection;332;2;337;0
WireConnection;709;0;708;2
WireConnection;709;1;681;0
WireConnection;341;0;916;0
WireConnection;1023;1;1095;0
WireConnection;1023;2;1024;0
WireConnection;893;0;709;0
WireConnection;191;0;341;0
WireConnection;886;1;1037;0
WireConnection;886;5;887;0
WireConnection;982;0;1023;0
WireConnection;1022;1;10;0
WireConnection;1022;0;332;0
WireConnection;1096;0;1099;0
WireConnection;1096;1;1097;0
WireConnection;1096;2;1098;0
WireConnection;263;0;1;4
WireConnection;734;0;893;0
WireConnection;259;0;1022;0
WireConnection;888;0;886;0
WireConnection;1101;0;1096;0
WireConnection;1060;0;1058;0
WireConnection;1065;0;1060;0
WireConnection;1065;1;917;0
WireConnection;1065;2;1064;0
WireConnection;992;0;804;0
WireConnection;992;1;889;0
WireConnection;992;4;806;0
WireConnection;992;10;774;0
WireConnection;992;11;1065;0
WireConnection;992;12;1103;0
ASEEND*/
//CHKSM=9733D3CDDC0C117D1308B9E8BBD24765456FCDCD
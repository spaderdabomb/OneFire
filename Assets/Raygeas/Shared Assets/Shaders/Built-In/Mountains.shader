// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Built-In/Mountains"
{
	Properties
	{
		[NoScaleOffset][Header(Maps)][Space(5)]_Albedo("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		[Space(10)]_Color("Color", Color) = (0.8,0.8,0.8,0)
		_NormalScale("Normal", Float) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[Header(Fog)][Space(5)][Toggle(_ENABLEFOG_ON)] _EnableFog("Enable", Float) = 1
		[Space(10)]_FogColor("Fog Color", Color) = (1,1,1,0)
		_Height("Height", Float) = 1
		_Density("Density", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#if defined(DIRECTIONAL_COOKIE) && defined(UNITY_LIGHT_ATTENUATION)
		#undef UNITY_LIGHT_ATTENUATION
		#define UNITY_LIGHT_ATTENUATION(destName, input, worldPos) fixed destName = UNITY_SHADOW_ATTENUATION(input, worldPos);
		#endif
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ENABLEFOG_ON
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred nofog 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float _NormalScale;
		uniform sampler2D _Albedo;
		uniform float4 _Color;
		uniform float4 _FogColor;
		uniform float _Height;
		uniform float _Density;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal3 = i.uv_texcoord;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal3 ), _NormalScale );
			float2 uv_Albedo2 = i.uv_texcoord;
			o.Albedo = ( tex2D( _Albedo, uv_Albedo2 ) * _Color ).rgb;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			#ifdef _ENABLEFOG_ON
				float4 staticSwitch35 = ( _FogColor * saturate( ( ( ( ( 1.0 - ase_vertex3Pos.y ) + _Height ) * 0.1 ) * ( _Density * 0.1 ) ) ) );
			#else
				float4 staticSwitch35 = float4( 0,0,0,0 );
			#endif
			o.Emission = staticSwitch35.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.PosVertexDataNode;26;-1966.443,378.2545;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;-1754.796,556.2723;Inherit;False;Property;_Height;Height;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;43;-1754.443,426.293;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1542.796,480.2723;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1572.796,635.2722;Inherit;False;Property;_Density;Density;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;33;-1400.796,480.2723;Inherit;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;41;-1401.443,635.293;Inherit;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1172.794,545.2723;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;30;-991.0875,545.4656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-1053.551,335.6638;Inherit;False;Property;_FogColor;Fog Color;6;0;Create;True;0;0;0;False;1;Space(10);False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-752.1368,430.2618;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;38;-893.8272,-230.1334;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;1;Space(10);False;0.8,0.8,0.8,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-862.2328,76.02168;Inherit;False;Property;_NormalScale;Normal;3;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-963.3433,-446.6574;Inherit;True;Property;_Albedo;Albedo;0;1;[NoScaleOffset];Create;True;0;0;0;False;2;Header(Maps);Space(5);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-511.1983,-348.084;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;35;-543.7928,405.784;Inherit;False;Property;_EnableFog;Enable;5;0;Create;False;0;0;0;False;2;Header(Fog);Space(5);False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-649.1184,27.27813;Inherit;True;Property;_Normal;Normal;1;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;44;-640,256;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-95.15202,4.839249;Float;False;True;-1;2;;0;0;Standard;Raygeas/Built-In/Mountains;False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;True;_CustomCutoff;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;43;0;26;2
WireConnection;31;0;43;0
WireConnection;31;1;32;0
WireConnection;33;0;31;0
WireConnection;41;0;28;0
WireConnection;27;0;33;0
WireConnection;27;1;41;0
WireConnection;30;0;27;0
WireConnection;29;0;11;0
WireConnection;29;1;30;0
WireConnection;37;0;2;0
WireConnection;37;1;38;0
WireConnection;35;0;29;0
WireConnection;3;5;34;0
WireConnection;0;0;37;0
WireConnection;0;1;3;0
WireConnection;0;2;35;0
WireConnection;0;4;44;0
ASEEND*/
//CHKSM=F1EFAACFF4D5FEE97ACA1C12B5E90FD47FAED853
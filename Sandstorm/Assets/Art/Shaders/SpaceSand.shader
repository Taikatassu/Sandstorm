// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SpaceSand"
{
	Properties
	{
		_Emission("Emission", 2D) = "white" {}
		[HDR]_Emissioncolor("Emission color", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		[HDR]_Albedocolor("Albedo color", Color) = (0,0,0,0)
		_AlbedoFadeStart("AlbedoFadeStart", Float) = 100
		_AlbedoFadeDist("AlbedoFadeDist", Range( 0 , 0.2)) = 1
		_EmissionFadeStart("EmissionFadeStart", Float) = 100
		_EmissionFadeDist("EmissionFadeDist", Range( 0 , 0.2)) = 1
		_EmissionFadeScale("EmissionFadeScale", Range( 0 , 5)) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Albedocolor;
		uniform float _AlbedoFadeStart;
		uniform float _AlbedoFadeDist;
		uniform float _EmissionFadeStart;
		uniform float _EmissionFadeScale;
		uniform float _EmissionFadeDist;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _Emissioncolor;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv0_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float temp_output_10_0 = distance( ase_worldPos , _WorldSpaceCameraPos );
			o.Albedo = ( ( tex2D( _Albedo, uv0_Albedo ) * _Albedocolor ) * min( ( 1.0 / ( max( ( temp_output_10_0 - _AlbedoFadeStart ) , 1.0 ) * _AlbedoFadeDist ) ) , 1.0 ) ).rgb;
			float2 uv0_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			o.Emission = ( min( ( 1.0 / ( pow( max( ( temp_output_10_0 - _EmissionFadeStart ) , 1.0 ) , _EmissionFadeScale ) * _EmissionFadeDist ) ) , 1.0 ) * ( tex2D( _Emission, uv0_Emission ) * _Emissioncolor ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
429;73;1550;1042;1410.553;434.9079;1;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;26;-643.5177,351.2494;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;16;-1055.729,120.9863;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;50;-240.8847,336.8308;Float;False;Property;_EmissionFadeStart;EmissionFadeStart;6;0;Create;True;0;0;False;0;100;6.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;10;-209.3587,211.5208;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-231.2223,41.92171;Float;False;Property;_AlbedoFadeStart;AlbedoFadeStart;4;0;Create;True;0;0;False;0;100;74.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;18;79.25333,299.5563;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;20;271.9507,298.3082;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;116.4465,454.0921;Float;False;Property;_EmissionFadeScale;EmissionFadeScale;8;0;Create;True;0;0;False;0;2;2.61;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;52;196.3026,2.29942;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;53;454.4465,295.0921;Float;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;44;-105.6644,-698.91;Float;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;None;c0c6af1ef1fe1cc44a5adabd6756a3d5;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-145.7138,-54.00292;Float;False;Property;_AlbedoFadeDist;AlbedoFadeDist;5;0;Create;True;0;0;False;0;1;0.0248;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;141.7492,160.8281;Float;False;Property;_EmissionFadeDist;EmissionFadeDist;7;0;Create;True;0;0;False;0;1;0.0298;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-707.753,-237.2535;Float;True;Property;_Emission;Emission;0;0;Create;True;0;0;False;0;None;e436f00cac8cc7048abce9efdebe26c9;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;51;464.2727,3.550012;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-400.5,-105;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;544.4004,141.3402;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;652.3595,-74.75061;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;201.5885,-566.6564;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;47;846.7186,-96.28793;Float;False;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;42;458.5887,-707.6566;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-89.5,-449;Float;False;Property;_Emissioncolor;Emission color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,6.634696,7.999999,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-143.5,-246;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;43;511.2142,-910.6564;Float;False;Property;_Albedocolor;Albedo color;3;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0.397784,0.9716981,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;11;835.7595,102.8029;Float;False;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;48;1015.594,-98.5489;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;814.5886,-699.6566;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;30;1009.634,102.5419;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;212.5,-238;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;9;-906.4747,337.8028;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;1323.34,104.2796;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1381.212,-205.3347;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;55;-966.5535,-52.9079;Float;False;Global;Position;Position;9;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;5;1622.797,-108.3836;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;SpaceSand;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;26;0
WireConnection;10;1;16;0
WireConnection;18;0;10;0
WireConnection;18;1;50;0
WireConnection;20;0;18;0
WireConnection;52;0;10;0
WireConnection;52;1;17;0
WireConnection;53;0;20;0
WireConnection;53;1;54;0
WireConnection;51;0;52;0
WireConnection;4;2;1;0
WireConnection;28;0;53;0
WireConnection;28;1;46;0
WireConnection;49;0;51;0
WireConnection;49;1;27;0
WireConnection;41;2;44;0
WireConnection;47;1;49;0
WireConnection;42;0;44;0
WireConnection;42;1;41;0
WireConnection;3;0;1;0
WireConnection;3;1;4;0
WireConnection;11;1;28;0
WireConnection;48;0;47;0
WireConnection;40;0;42;0
WireConnection;40;1;43;0
WireConnection;30;0;11;0
WireConnection;7;0;3;0
WireConnection;7;1;6;0
WireConnection;12;0;30;0
WireConnection;12;1;7;0
WireConnection;45;0;40;0
WireConnection;45;1;48;0
WireConnection;5;0;45;0
WireConnection;5;2;12;0
ASEEND*/
//CHKSM=EB7FF6375E5F01B184F8636B179C3233E603A0CD
// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CrystalSand"
{
	Properties
	{
		_Tiling("Tiling", Float) = 130
		_DistModifier("DistModifier", Range( 0.001 , 10)) = 0.2
		_MinDist("MinDist", Float) = 10
		_Normal("Normal", 2D) = "white" {}
		_AlbedoColor("AlbedoColor", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		[HDR]_GlitterColor("GlitterColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
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
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _AlbedoColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _GlitterColor;
		uniform float _Tiling;
		uniform float _MinDist;
		uniform float _DistModifier;


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


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float2 temp_output_2_0_g1 = uv0_Normal;
			float2 break6_g1 = temp_output_2_0_g1;
			float temp_output_25_0_g1 = ( pow( 0.5 , 3.0 ) * 0.1 );
			float2 appendResult8_g1 = (float2(( break6_g1.x + temp_output_25_0_g1 ) , break6_g1.y));
			float4 tex2DNode14_g1 = tex2D( _Normal, temp_output_2_0_g1 );
			float temp_output_4_0_g1 = 2.0;
			float3 appendResult13_g1 = (float3(1.0 , 0.0 , ( ( tex2D( _Normal, appendResult8_g1 ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float2 appendResult9_g1 = (float2(break6_g1.x , ( break6_g1.y + temp_output_25_0_g1 )));
			float3 appendResult16_g1 = (float3(0.0 , 1.0 , ( ( tex2D( _Normal, appendResult9_g1 ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float3 normalizeResult22_g1 = normalize( cross( appendResult13_g1 , appendResult16_g1 ) );
			o.Normal = normalizeResult22_g1;
			float2 uv0_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( _AlbedoColor * tex2D( _Albedo, uv0_Albedo ) ).rgb;
			float2 temp_cast_1 = (_Tiling).xx;
			float2 uv_TexCoord3 = i.uv_texcoord * temp_cast_1;
			float simplePerlin2D1 = snoise( uv_TexCoord3 );
			float2 temp_cast_2 = (_Tiling).xx;
			float2 temp_cast_3 = (131.94).xx;
			float2 uv_TexCoord19 = i.uv_texcoord * temp_cast_2 + temp_cast_3;
			float simplePerlin2D21 = snoise( uv_TexCoord19 );
			float2 temp_cast_4 = (_Tiling).xx;
			float2 temp_cast_5 = (177.48).xx;
			float2 uv_TexCoord22 = i.uv_texcoord * temp_cast_4 + temp_cast_5;
			float simplePerlin2D23 = snoise( uv_TexCoord22 );
			float4 appendResult18 = (float4(simplePerlin2D1 , simplePerlin2D21 , simplePerlin2D23 , 0.0));
			float3 newWorldNormal7 = (WorldNormalVector( i , appendResult18.xyz ));
			float3 ase_worldPos = i.worldPos;
			o.Emission = ( _GlitterColor * ( (float4( 0,0,0,0 ) + (max( ( (newWorldNormal7).xxxz * (newWorldNormal7).yyyz * (newWorldNormal7).zzzz * 5.94 ) , float4( 0.5,0.5,0.5,0 ) ) - float4( 0.5,0.5,0.5,0 )) * (float4( 0.5,0.5,0.5,0 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,0 ) - float4( 0.5,0.5,0.5,0 ))) / pow( ( max( distance( _WorldSpaceCameraPos , ase_worldPos ) , _MinDist ) / _DistModifier ) , 2.0 ) ) ).rgb;
			o.Smoothness = 0.2;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
429;73;1550;1042;87.27643;511.7655;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;24;-1938.28,-144.5805;Float;False;Constant;_Float3;Float 3;0;0;Create;True;0;0;False;0;177.48;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1938.279,-342.9138;Float;False;Constant;_Float2;Float 2;0;0;Create;True;0;0;False;0;131.94;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1913,-472.5;Float;False;Property;_Tiling;Tiling;2;0;Create;True;0;0;False;0;130;2000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-1683.28,-183.5805;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1690,-575.5;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1687.279,-353.9138;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-1407,-594.5;Float;True;Simplex2D;1;0;FLOAT2;100,100;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;21;-1417.279,-352.9138;Float;True;Simplex2D;1;0;FLOAT2;100,100;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-1414.279,-110.5805;Float;True;Simplex2D;1;0;FLOAT2;100,100;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;-1045.946,-441.5805;Float;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;56;-515.5293,603.1086;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;49;-641.6282,383.4076;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;7;-1066.045,35.01943;Float;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;59;-310.1315,799.4077;Float;False;Property;_MinDist;MinDist;4;0;Create;True;0;0;False;0;10;5.29;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;52;-290.6298,441.9086;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;26;-769.7771,85.32684;Float;False;FLOAT4;1;1;1;2;1;0;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SwizzleNode;27;-770.7771,163.3268;Float;False;FLOAT4;2;2;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SwizzleNode;25;-768.7771,8.326843;Float;False;FLOAT4;0;0;0;2;1;0;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-787.7771,264.3268;Float;False;Constant;_Float4;Float 4;0;0;Create;True;0;0;False;0;5.94;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;60;-69.63153,456.2082;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-539.7771,30.32684;Float;True;4;4;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;57;79.86808,727.907;Float;False;Property;_DistModifier;DistModifier;3;0;Create;True;0;0;False;0;0.2;6.86;0.001;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;62;-4.4646,-349.5073;Float;True;Property;_Albedo;Albedo;7;0;Create;True;0;0;False;0;None;662d72b6ec210cf4cbeec2b4d3cb8b2a;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;47;-168.8187,28.93125;Float;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.5,0.5,0.5,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;58;183.8681,393.809;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;66;-27.13129,-625.8406;Float;True;Property;_Normal;Normal;5;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;236.5354,-483.5073;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;48;124.1813,39.93125;Float;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.5,0.5,0.5,0;False;2;FLOAT4;1,1,1,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0.5,0.5,0.5,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;54;360.6702,393.8089;Float;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;50;498.4705,44.10887;Float;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;63;515.5354,-364.5073;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;65;213.8687,-759.8406;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;71;992.7236,-505.2655;Float;False;Property;_AlbedoColor;AlbedoColor;6;0;Create;True;0;0;False;0;0,0,0,0;0.1626468,0.3200631,0.8018868,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;73;635.7236,251.2345;Float;False;Property;_GlitterColor;GlitterColor;8;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;68;535.5354,-661.5073;Float;True;NormalCreate;0;;1;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;42;1310.305,-2514.421;Float;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;2,2,1,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;69;581.7236,-75.2655;Float;False;Constant;_Smoothness;Smoothness;6;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;953.7236,-226.2655;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-380.9456,-167.5805;Float;False;Constant;_Color0;Color 0;0;0;Create;True;0;0;False;0;0.8113208,0.6823558,0.4707191,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;867.7236,72.2345;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;61;1216.099,28.09991;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;CrystalSand;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;2;0
WireConnection;22;1;24;0
WireConnection;3;0;2;0
WireConnection;19;0;2;0
WireConnection;19;1;20;0
WireConnection;1;0;3;0
WireConnection;21;0;19;0
WireConnection;23;0;22;0
WireConnection;18;0;1;0
WireConnection;18;1;21;0
WireConnection;18;2;23;0
WireConnection;7;0;18;0
WireConnection;52;0;49;0
WireConnection;52;1;56;0
WireConnection;26;0;7;0
WireConnection;27;0;7;0
WireConnection;25;0;7;0
WireConnection;60;0;52;0
WireConnection;60;1;59;0
WireConnection;31;0;25;0
WireConnection;31;1;26;0
WireConnection;31;2;27;0
WireConnection;31;3;30;0
WireConnection;47;0;31;0
WireConnection;58;0;60;0
WireConnection;58;1;57;0
WireConnection;64;2;62;0
WireConnection;48;0;47;0
WireConnection;54;0;58;0
WireConnection;50;0;48;0
WireConnection;50;1;54;0
WireConnection;63;0;62;0
WireConnection;63;1;64;0
WireConnection;65;2;66;0
WireConnection;68;1;66;0
WireConnection;68;2;65;0
WireConnection;72;0;71;0
WireConnection;72;1;63;0
WireConnection;74;0;73;0
WireConnection;74;1;50;0
WireConnection;61;0;72;0
WireConnection;61;1;68;0
WireConnection;61;2;74;0
WireConnection;61;4;69;0
ASEEND*/
//CHKSM=E38E89A20C0D4E34982511FFF0B677F13960EF12
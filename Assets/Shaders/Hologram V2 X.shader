// Upgrade NOTE: upgraded instancing buffer 'HologramV2X' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hologram V2 X"
{
	Properties
	{
		_Scanline1Amount("Scanline 1 Amount", Float) = 10
		_Scanline1Speed("Scanline 1 Speed", Float) = 1
		_Scnaline2Amount("Scnaline 2 Amount", Float) = 1.25
		_Scanline2Speed("Scanline 2 Speed", Float) = 0.19
		_Color("Color", Color) = (0.6179246,1,1,1)
		[HDR]_FresnelColor("Fresnel Color", Color) = (0.8588235,0.3529412,0,1)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		UNITY_INSTANCING_BUFFER_START(HologramV2X)
			UNITY_DEFINE_INSTANCED_PROP(float4, _FresnelColor)
#define _FresnelColor_arr HologramV2X
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr HologramV2X
			UNITY_DEFINE_INSTANCED_PROP(float, _Scanline2Speed)
#define _Scanline2Speed_arr HologramV2X
			UNITY_DEFINE_INSTANCED_PROP(float, _Scnaline2Amount)
#define _Scnaline2Amount_arr HologramV2X
			UNITY_DEFINE_INSTANCED_PROP(float, _Scanline1Amount)
#define _Scanline1Amount_arr HologramV2X
			UNITY_DEFINE_INSTANCED_PROP(float, _Scanline1Speed)
#define _Scanline1Speed_arr HologramV2X
		UNITY_INSTANCING_BUFFER_END(HologramV2X)

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_7_0 = ( ase_vertex3Pos.x + _SinTime.y );
			float mulTime19 = _Time.y * 3.0;
			float3 appendResult37 = (float3(0.0 , ( 0.0 + ( ( ( ( ( step( 0.0 , temp_output_7_0 ) * step( temp_output_7_0 , 0.2 ) ) * 0.1 ) * step( 0.98 , sin( mulTime19 ) ) ) * _SinTime.w ) * 1.5 ) ) , 0.0));
			v.vertex.xyz += appendResult37;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Color_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float _Scanline1Amount_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scanline1Amount_arr, _Scanline1Amount);
			float _Scanline1Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scanline1Speed_arr, _Scanline1Speed);
			float mulTime43 = _Time.y * _Scanline1Speed_Instance;
			float temp_output_45_0 = frac( ( ( ase_vertex3Pos.x * _Scanline1Amount_Instance ) + mulTime43 ) );
			float _Scnaline2Amount_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scnaline2Amount_arr, _Scnaline2Amount);
			float _Scanline2Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scanline2Speed_arr, _Scanline2Speed);
			float temp_output_52_0 = frac( ( ( ase_vertex3Pos.x * _Scnaline2Amount_Instance ) - ( _Time.y * _Scanline2Speed_Instance ) ) );
			float2 temp_cast_0 = (unity_DeltaTime.x).xx;
			float dotResult4_g1 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
			float lerpResult10_g1 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g1 ) * 43758.55 ) ));
			float temp_output_70_0 = lerpResult10_g1;
			float temp_output_71_0 = (( temp_output_70_0 > 0.8 ) ? temp_output_70_0 :  0.8 );
			o.Albedo = ( ( _Color_Instance * ( temp_output_45_0 + temp_output_52_0 ) ) * temp_output_71_0 ).rgb;
			float4 _FresnelColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_FresnelColor_arr, _FresnelColor);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV64 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode64 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV64, 3.0 ) );
			o.Emission = ( ( ( _FresnelColor_Instance * fresnelNode64 ) + ( _Color_Instance * temp_output_45_0 ) ) * temp_output_71_0 ).rgb;
			o.Alpha = ( ( ( temp_output_45_0 * temp_output_52_0 ) * temp_output_71_0 ) * _Color_Instance.a );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

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
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
0;90.2;1536;710;708.6384;616.767;1.315668;False;False
Node;AmplifyShaderEditor.SinTimeNode;14;-1890.545,-521.2711;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;6;-1904.995,-760.1989;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-1338.78,-874.7296;Float;False;Constant;_Float2;Float 2;0;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-1561.193,-671.3273;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1715.783,-46.08331;Float;False;Constant;_Float3;Float 3;0;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1359.691,-422.3021;Float;False;Constant;_Float1;Float 1;0;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;19;-1450.903,-76.75359;Float;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;10;-1110.666,-889.9373;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;8;-1116.369,-532.5575;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-966.7194,-214.789;Float;False;Constant;_Float4;Float 4;0;0;Create;True;0;0;False;0;0.98;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-666.6382,-634.5916;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-552.8076,-378.3368;Float;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;21;-1144.461,-140.795;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;22;-782.5369,-144.1617;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1334.795,1690.529;Float;False;InstancedProperty;_Scnaline2Amount;Scnaline 2 Amount;2;0;Create;True;0;0;False;0;1.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-822.2693,2201.427;Float;False;InstancedProperty;_Scanline2Speed;Scanline 2 Speed;3;0;Create;True;0;0;False;0;0.19;0.19;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1590.852,1023.442;Float;False;InstancedProperty;_Scanline1Amount;Scanline 1 Amount;0;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;39;-1813.533,834.276;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;53;-941.2549,2074.846;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-357.8497,-629.7775;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;46;-1603.644,1517.993;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;44;-1502.741,1215.212;Float;False;InstancedProperty;_Scanline1Speed;Scanline 1 Speed;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-619.7405,2090.036;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;27;-68.73175,-373.4622;Float;True;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-78.14026,-625.2785;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;43;-1278.145,1191.025;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-1324.387,902.5524;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1066.396,1469.667;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;-696.3049,1483.984;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DeltaTime;69;-99.55591,1766.838;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-956.8003,916.3269;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;273.4023,-226.3208;Float;False;Constant;_Float9;Float 9;5;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-511.2105,495.2776;Float;False;Constant;_Float7;Float 7;5;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;229.6349,-614.0865;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;59;-459.8393,734.5204;Float;False;InstancedProperty;_Color;Color;4;0;Create;True;0;0;False;0;0.6179246,1,1,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;70;200.028,1779.452;Float;True;Random Number Generator;-1;;1;8b5b5636ce6c76a4788b64be633aea66;0;3;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;45;-561.8978,944.1525;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;481.9877,-610.1877;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;67;-256.3584,193.4789;Float;False;InstancedProperty;_FresnelColor;Fresnel Color;5;1;[HDR];Create;True;0;0;False;0;0.8588235,0.3529412,0,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FractNode;52;-354.427,1510.826;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;64;-358.5969,401.4014;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;475.1619,2001.522;Float;False;Constant;_Float8;Float 8;6;0;Create;True;0;0;False;0;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-86.67532,1299.818;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;764.3264,-677.1054;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;71;714.051,1833.062;Float;True;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;111.1136,890.7079;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;148.2753,432.6823;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;635.3372,1377.586;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;1015.474,1408.36;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;718.9033,597.0738;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;332.74,1238.706;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;36;1087.607,-589.0161;Float;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;37;1471.791,-588.8301;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;1357.829,1307.444;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;961.0837,1126.16;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;1121.195,641.8613;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1793.543,-462.4417;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Hologram V2 X;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;6;1
WireConnection;7;1;14;2
WireConnection;19;0;20;0
WireConnection;10;0;11;0
WireConnection;10;1;7;0
WireConnection;8;0;7;0
WireConnection;8;1;9;0
WireConnection;13;0;10;0
WireConnection;13;1;8;0
WireConnection;21;0;19;0
WireConnection;22;0;24;0
WireConnection;22;1;21;0
WireConnection;16;0;13;0
WireConnection;16;1;17;0
WireConnection;54;0;53;0
WireConnection;54;1;55;0
WireConnection;25;0;16;0
WireConnection;25;1;22;0
WireConnection;43;0;44;0
WireConnection;40;0;39;1
WireConnection;40;1;41;0
WireConnection;47;0;46;1
WireConnection;47;1;48;0
WireConnection;49;0;47;0
WireConnection;49;1;54;0
WireConnection;42;0;40;0
WireConnection;42;1;43;0
WireConnection;26;0;25;0
WireConnection;26;1;27;4
WireConnection;70;1;69;1
WireConnection;45;0;42;0
WireConnection;75;0;26;0
WireConnection;75;1;76;0
WireConnection;52;0;49;0
WireConnection;64;3;65;0
WireConnection;57;0;45;0
WireConnection;57;1;52;0
WireConnection;28;1;75;0
WireConnection;71;0;70;0
WireConnection;71;1;72;0
WireConnection;71;2;70;0
WireConnection;71;3;72;0
WireConnection;58;0;59;0
WireConnection;58;1;45;0
WireConnection;66;0;67;0
WireConnection;66;1;64;0
WireConnection;61;0;45;0
WireConnection;61;1;52;0
WireConnection;73;0;61;0
WireConnection;73;1;71;0
WireConnection;63;0;66;0
WireConnection;63;1;58;0
WireConnection;60;0;59;0
WireConnection;60;1;57;0
WireConnection;36;0;28;0
WireConnection;37;1;36;0
WireConnection;74;0;73;0
WireConnection;74;1;59;4
WireConnection;62;0;60;0
WireConnection;62;1;71;0
WireConnection;68;0;63;0
WireConnection;68;1;71;0
WireConnection;0;0;62;0
WireConnection;0;2;68;0
WireConnection;0;9;74;0
WireConnection;0;11;37;0
ASEEND*/
//CHKSM=2CFC951379B1B50500D0E7E9AE351C39DF232824
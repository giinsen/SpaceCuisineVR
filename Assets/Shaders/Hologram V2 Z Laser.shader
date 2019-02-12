// Upgrade NOTE: upgraded instancing buffer 'HologramV2ZLaser' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hologram V2 Z Laser"
{
	Properties
	{
		_Scanline1Amount("Scanline 1 Amount", Float) = 1.56
		_Scanline1Speed("Scanline 1 Speed", Float) = 1
		[HDR]_PulseColor("Pulse Color", Color) = (2.680933,2.79544,0,1)
		_Color("Color", Color) = (0.6179246,1,1,1)
		[HDR]_FresnelColor("Fresnel Color", Color) = (0.8588235,0.3529412,0,1)
		_PulseDirection("Pulse Direction", Float) = 1
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

		UNITY_INSTANCING_BUFFER_START(HologramV2ZLaser)
			UNITY_DEFINE_INSTANCED_PROP(float4, _PulseColor)
#define _PulseColor_arr HologramV2ZLaser
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr HologramV2ZLaser
			UNITY_DEFINE_INSTANCED_PROP(float4, _FresnelColor)
#define _FresnelColor_arr HologramV2ZLaser
			UNITY_DEFINE_INSTANCED_PROP(float, _Scanline1Speed)
#define _Scanline1Speed_arr HologramV2ZLaser
			UNITY_DEFINE_INSTANCED_PROP(float, _PulseDirection)
#define _PulseDirection_arr HologramV2ZLaser
			UNITY_DEFINE_INSTANCED_PROP(float, _Scanline1Amount)
#define _Scanline1Amount_arr HologramV2ZLaser
		UNITY_INSTANCING_BUFFER_END(HologramV2ZLaser)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float _PulseDirection_Instance = UNITY_ACCESS_INSTANCED_PROP(_PulseDirection_arr, _PulseDirection);
			float4 _Color_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float _Scanline1Amount_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scanline1Amount_arr, _Scanline1Amount);
			float temp_output_40_0 = ( ase_vertex3Pos.z * _Scanline1Amount_Instance );
			float _Scanline1Speed_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scanline1Speed_arr, _Scanline1Speed);
			float mulTime43 = _Time.y * _Scanline1Speed_Instance;
			float temp_output_286_0 = (( _PulseDirection_Instance > 0.0 ) ? frac( ( ( 1.0 - temp_output_40_0 ) - mulTime43 ) ) :  frac( ( temp_output_40_0 + mulTime43 ) ) );
			float4 _PulseColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_PulseColor_arr, _PulseColor);
			float temp_output_303_0 = ( ase_vertex3Pos.z + 1.0 );
			float4 temp_output_207_0 = ( _PulseColor_Instance * temp_output_303_0 );
			o.Albedo = ( ( _Color_Instance * temp_output_286_0 ) + temp_output_207_0 ).rgb;
			float4 _FresnelColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_FresnelColor_arr, _FresnelColor);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV64 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode64 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV64, 3.0 ) );
			o.Emission = ( ( _FresnelColor_Instance * fresnelNode64 ) + ( ( _Color_Instance * temp_output_286_0 ) + temp_output_207_0 ) ).rgb;
			o.Alpha = ( ( temp_output_286_0 * _Color_Instance.a ) + ( _PulseColor_Instance.a * temp_output_303_0 ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

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
0;90.2;1536;710;2098.419;-789.129;2.812343;False;False
Node;AmplifyShaderEditor.PosVertexDataNode;39;-2118.912,733.5325;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;-1896.23,922.6985;Float;False;InstancedProperty;_Scanline1Amount;Scanline 1 Amount;0;0;Create;True;0;0;False;0;1.56;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-1629.765,804.4552;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1750.007,1201.432;Float;False;InstancedProperty;_Scanline1Speed;Scanline 1 Speed;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;43;-1525.411,1177.245;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;285;-1410.049,504.0767;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;289;-1233.771,1142.011;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-1249.21,888.5414;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;257;-3567.061,1616.964;Float;False;InstancedProperty;_PulseDirection;Pulse Direction;5;0;Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;302;-359.2765,2223.928;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;304;-367.7094,2460.897;Float;False;Constant;_Float4;Float 4;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;45;-873.3276,884.6759;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;290;-944.0068,1160.535;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;284;-793.9218,1120.142;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;303;67.67471,2289.104;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;59;-459.8393,732.1082;Float;False;InstancedProperty;_Color;Color;3;0;Create;True;0;0;False;0;0.6179246,1,1,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;65;-511.2105,495.2776;Float;False;Constant;_Float7;Float 7;5;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;206;-943.4493,1420.965;Float;False;InstancedProperty;_PulseColor;Pulse Color;2;1;[HDR];Create;True;0;0;False;0;2.680933,2.79544,0,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;286;-643.8654,1065.087;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;64;-353.5969,398.4014;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;67;-256.3584,193.4789;Float;False;InstancedProperty;_FresnelColor;Fresnel Color;4;1;[HDR];Create;True;0;0;False;0;0.8588235,0.3529412,0,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;-159.825,1472.851;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;111.1136,890.7079;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;326.2731,1217.15;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;623.1726,1757.294;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;148.2753,432.6823;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;209;436.6763,763.5827;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;1179.533,1212.329;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;208;727.836,1225.012;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-1312.941,1740.941;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;274;-1328.134,1486.825;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;272;-845.777,2449.912;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;278;-1401.602,2092.382;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;718.9033,597.0738;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-1084.049,1930.102;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;270;-621.0851,2187.851;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;-436.2343,1817.128;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-1547.919,1789.77;Float;False;Constant;_Float13;Float 13;7;0;Create;True;0;0;False;0;5.43;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;1548.53,1233.525;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;-2384.878,2325.731;Float;False;Constant;_Float8;Float 8;7;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;199;-2450.134,2072.289;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;193;-1999.589,2590.31;Float;False;Constant;_Float18;Float 18;7;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;194;-1393.365,2382.461;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;-1893.913,1792.432;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;277;-1973.869,1326.393;Float;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;300;-1903.455,1681.93;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-2132.455,1799.93;Float;False;Constant;_Float3;Float 3;7;0;Create;True;0;0;False;0;2.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-2315.289,1690.064;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;252;-1746.049,2413.723;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;172;-1995.992,2454.853;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-2094.358,1699.736;Float;False;Constant;_Intensity;Intensity;7;0;Create;True;0;0;False;0;0.72;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;147;-2334.991,1874.003;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-2188.302,2116.375;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-2155.019,1578.748;Float;False;InstancedProperty;_PulseLength;Pulse Length;6;0;Create;True;0;0;False;0;0.34;-0.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;196;-2958.107,2002.391;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;280;-1687.553,1846.533;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;152;-1662.35,2165.933;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;273;-1520.068,1524.965;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;269;-859.6741,2175.514;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;197;-3195.134,1931.151;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;279;-1634.136,2061.526;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;276;-1770.445,1308.392;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;-2804.825,2127.531;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-1972.603,2261.742;Float;False;Constant;_Float9;Float 9;0;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;149;-2032.881,1966.041;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;298;-1939.757,1541.449;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;179;-1128.018,2267.688;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;159;-1748.905,1578.133;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-3627.511,2330.367;Float;False;Constant;_Float16;Float 16;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1985.271,977.3984;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Hologram V2 Z Laser;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;39;3
WireConnection;40;1;41;0
WireConnection;43;0;44;0
WireConnection;285;0;40;0
WireConnection;289;0;285;0
WireConnection;289;1;43;0
WireConnection;42;0;40;0
WireConnection;42;1;43;0
WireConnection;45;0;42;0
WireConnection;290;0;289;0
WireConnection;284;0;257;0
WireConnection;303;0;302;3
WireConnection;303;1;304;0
WireConnection;286;0;284;0
WireConnection;286;2;290;0
WireConnection;286;3;45;0
WireConnection;64;3;65;0
WireConnection;207;0;206;0
WireConnection;207;1;303;0
WireConnection;58;0;59;0
WireConnection;58;1;286;0
WireConnection;60;0;59;0
WireConnection;60;1;286;0
WireConnection;212;0;206;4
WireConnection;212;1;303;0
WireConnection;66;0;67;0
WireConnection;66;1;64;0
WireConnection;209;0;58;0
WireConnection;209;1;207;0
WireConnection;74;0;286;0
WireConnection;74;1;59;4
WireConnection;208;0;60;0
WireConnection;208;1;207;0
WireConnection;162;1;163;0
WireConnection;274;0;273;0
WireConnection;274;2;276;0
WireConnection;274;3;159;0
WireConnection;272;0;257;0
WireConnection;278;0;279;0
WireConnection;278;2;280;0
WireConnection;278;3;152;0
WireConnection;63;0;66;0
WireConnection;63;1;209;0
WireConnection;153;0;274;0
WireConnection;153;1;278;0
WireConnection;270;0;272;0
WireConnection;270;2;179;0
WireConnection;270;3;269;0
WireConnection;182;0;153;0
WireConnection;182;1;270;0
WireConnection;213;0;74;0
WireConnection;213;1;212;0
WireConnection;199;0;200;0
WireConnection;194;0;252;0
WireConnection;299;0;161;0
WireConnection;299;1;297;0
WireConnection;300;0;160;0
WireConnection;300;1;301;0
WireConnection;252;0;172;0
WireConnection;252;1;193;0
WireConnection;189;0;199;0
WireConnection;189;1;188;0
WireConnection;196;0;197;0
WireConnection;280;0;149;0
WireConnection;280;1;300;0
WireConnection;280;2;299;0
WireConnection;152;0;149;0
WireConnection;152;1;150;0
WireConnection;273;0;257;0
WireConnection;269;0;179;0
WireConnection;279;0;257;0
WireConnection;276;0;277;0
WireConnection;276;1;149;0
WireConnection;200;0;196;0
WireConnection;200;1;195;0
WireConnection;149;0;147;2
WireConnection;149;1;189;0
WireConnection;298;0;160;0
WireConnection;298;1;297;0
WireConnection;179;1;194;0
WireConnection;159;0;149;0
WireConnection;159;1;298;0
WireConnection;159;2;161;0
WireConnection;0;0;208;0
WireConnection;0;2;63;0
WireConnection;0;9;213;0
ASEEND*/
//CHKSM=9FD90E8E297064BF968CD7C4FB67434A46C7DE72
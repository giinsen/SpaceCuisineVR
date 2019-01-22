Shader "Custom/Glitch" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_EmissiveMap("Emissive", 2D) = "white" {}
		_Amount("Glitch", Range(0,2)) = 0.5
		_Speed("GlitchSpeed",Range(0,15)) = 7
		_Granularity("Granularity", Range(0, 200)) = 50
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#include "Utils.cginc"

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_EmissiveMap;
		};
		
		float _Amount;
		float _Speed;
		float _Granularity;

		float _var;

		void vert(inout appdata_full v)
		{
			float t = float2(_Time.y, 0.);
			float noise = rand(t);
			fixed oscillation = cos(_Time.y*_Speed+noise*50.);

			v.vertex.x += cos(v.vertex.z * _Granularity) * oscillation * _Amount;
			v.vertex.y += sin(v.vertex.x * _Granularity) * oscillation * _Amount;
			v.vertex.z += cos(v.vertex.y * _Granularity) * oscillation * _Amount;

			// displacement perlin noise
			float3 offset = float3(0, 0, 0);
			float3 seed = v.vertex.xyz;
			seed = rotateY(seed, _Time.y);
			float3 seedScale = float3(1, 2, 3);
			offset.x = noiseIQ(seed * seedScale.x);
			offset.y = noiseIQ(seed * seedScale.y)*4.;
			offset.z = noiseIQ(seed * seedScale.z);
			//v.vertex.xyz += offset * 0.1;
			
			// rotate
			//v.vertex.xyz = rotateY(v.vertex.xyz, _Time.y + length(v.vertex.xyz)*4.);

			/*
			// voxelisation
			float lod = 256.-_Amount*50;
			//float lod = 1. + length(v.vertex.xyz) * 0.1;
			float3 grid = ceil(v.vertex.xyz * lod) / lod;
			v.vertex.xyz = grid;
			//v.vertex.xyz = lerp(v.vertex.xyz, grid, clamp(_Amount,0,1));
			*/
		}

		sampler2D _MainTex;
		sampler2D _EmissiveMap;

		void surf(Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Emission = tex2D(_EmissiveMap, IN.uv_EmissiveMap).rgb;
		}
	ENDCG
	}
Fallback "Diffuse"
}
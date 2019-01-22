Shader "Custom/GlitchEnvironment" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NormalMap("Normal", 2D) = "white"{}
		_HeightMap("Height", 2D) = "white"{}
		_Emission("Emission", Range(0,1)) = 0
		//_EmissiveMap("Emissive", 2D) = "white" {}
		_DisplacementAmount("Displacement Amount", Range(0,2)) = 0.5
		_Speed("Displacement Speed",Range(0,15)) = 7
		_Granularity("Voxellisation", Range(0, 16)) = 50
		_ColorSwapAmount("Color Swap Amount", Range(0,1)) = 0
		_ColorSpeed("Color Swap Speed", Range(0, 15)) = 0
		_UVSwapAmount("UV Swap Amount", Range(0, 1)) = 0
		_UVSwapSpeed("UV Swap Speed", Range(0, 15)) = 0
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
		float2 uv_NormalMap;
		float2 uv_HeightMap;
		//float2 uv_EmissiveMap;
	};

	float _DisplacementAmount;
	float _Speed;
	float _Granularity;

	float _var;

	void vert(inout appdata_full v)
	{
		// displacement perlin noise
		float3 offset = float3(0, 0, 0);
		float3 seed = v.vertex.xyz;
		seed = rotateY(seed, _Time.y*_Speed);
		float3 seedScale = float3(1, 2, 3);
		offset.x = noiseIQ(seed * seedScale.x)*15;
		//offset.y = noiseIQ(seed * seedScale.y)*15.;
		offset.z = noiseIQ(seed * seedScale.z);
		v.vertex.xyz += offset * 0.1* _DisplacementAmount;

		
		// voxelisation
		float lod = 16 - _Granularity;
		//float lod = 1. + length(v.vertex.xyz) * 0.1;
		float3 grid = ceil(v.vertex.xyz * lod) / lod;
		v.vertex.xyz = grid;
		v.vertex.xyz = lerp(v.vertex.xyz, grid, clamp(_Granularity,0,1));

	}

	sampler2D _MainTex;
	sampler2D _NormalMap;
	sampler2D _HeightMap;
	//sampler2D _EmissiveMap;
	float _ColorSwapAmount;
	float _ColorSpeed;
	float _Emission;
	float _UVSwapAmount;
	float _UVSwapSpeed;

	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed time = ceil(_Time.y*_ColorSpeed*0.5);
		fixed uvtime = ceil(_Time.y *_UVSwapSpeed*0.5);

		//Determining color swap UV
		float factorX = ceil(IN.uv_MainTex.x*15.*rand(time / 8));
		float factorY = ceil(IN.uv_MainTex.y*15.*rand(time / 16));

		float3 seed = float3(factorX, factorY, factorX);
		float thresh = 1.001 - _ColorSwapAmount * 1.001;

		float w_c = step(thresh, pow(rand(seed*150.*time), 1.5));

		//Texture swap
		factorX = ceil(IN.uv_MainTex.x*15.*rand(uvtime / 32));
		factorY = ceil(IN.uv_MainTex.y*15.*rand(uvtime / 64));
		seed = float3(factorX, factorY, factorX);
		float uvthresh = 1.001 - _UVSwapAmount * 1.001;
		float yolo = step(uvthresh, pow(rand(seed*150.*uvtime), 1.5));
		float3 transformation = float3(rand(seed+uvtime)*factorX, rand(seed + uvtime), rand(seed + uvtime))*_UVSwapAmount;
		transformation = lerp(float3(0, 0, 0), transformation, yolo);

		float3 color = tex2D(_MainTex, IN.uv_MainTex-transformation).rgb;
		//float3 neg = saturate(color.bgr + (1 - dot(color, 0.95)) * 0.25);
		float3 neg = float3(rand(color.r+time+factorX), rand(color.g+time + factorY), rand(color.b+time + factorX)) * 1.5;
		color = lerp(color, neg, w_c);

		o.Albedo = color;
		o.Normal = tex2D(_NormalMap, IN.uv_NormalMap - transformation).rgb;
		//o.Height = tex2D(_NormalMap, IN.uv_NormalMap).rgb;
		o.Emission = color*_Emission;
	}
	ENDCG
	}
		Fallback "Diffuse"
}

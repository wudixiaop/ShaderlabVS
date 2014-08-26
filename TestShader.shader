Shader "ShaderStudy/01ColorTint" {
	Properties 
	{
		_ColorTint ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_BumpMap ("Bump Map", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range(0.5, 10)) = 3.0
		_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
		_Shinness ("Shinness", Range(-1, 2.0)) = 0.5
	}
	
	SubShader
    {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong

		float4 _ColorTint;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _RimColor;
		float _RimPower;
		float _Shinness;

		struct Input 
		{
			float4 color : COLOR;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			IN.color = _ColorTint;
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			
			half rim = 1 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			
			o.Specular = _Shinness;
			o.Gloss = c.a;

		}
		ENDCG
	} 
	FallBack "Diffuse"
}

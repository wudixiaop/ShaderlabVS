Shader "ShaderStudy/01ColorTint" {
	Properties 
	{
		_ColorTint ("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_BumpMap ("Bump Map", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range (0.5 , +10)) = 3.0
		_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
		_Shinness ("Shinness", Range(-1, 2.0)) = 0.5
		_Vector ("Vector", Vector) = (1,1,1,1)
	}
	
	// Comment test
	//
	/* Comment test*/
	/* Comment
	//  test
	**
	*/
	SubShader // Comment test
    {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass{
			Lighting Off
			SetTexture [_MainTex] {}
			Cull Back
			Cull Front
			Cull Off
			AlphaTest Greater
			Color 3412341234
		}

		Pass {
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Shininess [_Shininess]
                Specular [_SpecColor]
                Emission [_Emission]
            }
            Lighting On
            SeparateSpecular On
            SetTexture [_MainTex] {
                Combine texture * primary DOUBLE, texture * primary
            }
        }

		Stencil {
                Ref 2
                Comp always
                Pass replace
                ZFail decrWrap
            }

		// We use the material in many passes by defining them in the subshader.
        // Anything defined here becomes default values for all contained passes.
        Material {
            Diffuse [_Color]
            Ambient [_Color]
            Shininess [_Shininess]
            Specular [_SpecColor]
            Emission [_Emission]
        }

        Lighting On
        SeparateSpecular On

        // Set up alpha blending
        Blend SrcAlpha OneMinusSrcAlpha


		CGPROGRAM
		#pragma surface surf BlinnPhong

		float4 _ColorTint;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler3D _sampler3DValue;
		samplerCUBE _samplerCUBEValue;
		float4 _RimColor;
		float _RimPower;
		float _Shinness;
		fixed _FxiedValue;
		half _HalfValue;

		struct Input 
		{
			float4 color :COLOR;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			IN.color = _ColorTint;
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = mul(UNITY_MATRIX_MVP,c.a);
			o.Normal = UnpackNormal( tex2D (_BumpMap, IN.uv_BumpMap));
			tex2D f = tex2D(_MainTex, IN.uv_MainTex);

			half rim = 1 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);

			o.Specular = _Shinness;
			o.Gloss = mul(UNITY_MATRIX_MVP,c.a);

			if (true)
			{

			}
			else
			{

			}
			
			do
			{
			}
			while(false)

			for(int i=0; i < 100; i++)
			{

			}
		}

		ENDCG
	} 

	SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+2"}

        ColorMask RGB
        Cull Front
        ZTest Always

        Stencil {
            Ref 1
            Comp notequal 
        }

        CGPROGRAM
        #pragma surface surf Lambert
        float4 _Color;
        struct Input {
            float4 color : COLOR;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = _Color.rgb;
            o.Normal = half3(0,0,-1);
            o.Alpha = 1;
        }

        ENDCG
    } 

	FallBack "Diffuse"
}

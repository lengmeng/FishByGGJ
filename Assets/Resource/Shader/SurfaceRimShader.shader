Shader "Custom/SurfaceRimShader" {
	Properties
	{
		_MainColor("【主颜色】Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("【纹理】Texture", 2D) = "white" {}
		_BumpMap("【凹凸纹理】Bumpmap", 2D) = "bump" {}
		_RimColor("【边缘发光颜色】Rim Color", Color) = (0.17,0.36,0.81,0.0)
		_RimPower("【边缘颜色强度】Rim Power", Range(0.6,36.0)) = 8.0
		_RimIntensity("【边缘颜色强度系数】Rim Intensity", Range(0.0,100.0)) = 1.0
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque" 
		}
		CGPROGRAM
			#pragma surface surf Lambert  
			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 viewDir;  
			};
			//边缘颜色
			float4 _MainColor;
			//主纹理
			sampler2D _MainTex;  
			//凹凸纹理  
			sampler2D _BumpMap;
			//边缘颜色
			float4 _RimColor;
			//边缘颜色强度
			float _RimPower;
			//边缘颜色强度
			float _RimIntensity;
			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb*_MainColor.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
				o.Emission = _RimColor.rgb * pow(rim, _RimPower)*_RimIntensity;
			}

		ENDCG
	}
}
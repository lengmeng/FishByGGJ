Shader "Custom/AlphaBlendShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
		_HorizontalAmount("HA", Float) = 4
        _VerticalAmount ("Vertical Amount", Float) = 4
        _Speed ("Speed", Range(1, 100)) = 30
	}
	SubShader {
		// 通常使用了透明度混合都会使用之这三个标签
		Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		Pass{
			Tags {"LightMode" = "ForwardBase"}
			// 关闭剔除功能
			Cull Off
			// 关闭深度写入 指定混合指令
			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _HorizontalAmount;
            float _VerticalAmount;
            float _Speed;
			fixed _AlphaScale;

			struct a2v{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float4 texcoord: TEXCOORD0;
			};

			struct v2f{
				float4 pos: SV_POSITION;
				float3 worldPos: TEXCOORD0;
				float3 worldNormal: TEXCOORD1;
				float2 uv: TEXCOORD2;
			};

			v2f vert(a2v v){
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			fixed4 frag(v2f i): SV_Target{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				//fixed4 texColor = tex2D(_MainTex, i.uv);
				// _Time.y是加载场景后所经过的时间 
				float time = floor(_Time.y * _Speed);
				// 简写了计算过程 可以自己写一套
				float row = floor(time / _HorizontalAmount);
				float colum = time - row * _VerticalAmount;
				// -row因为Unity纹理坐标是从下到上 而纹理中是图片位置是从上到下
				half2 uv = i.uv + half2(colum, -row);
				uv.x /= _HorizontalAmount;
				uv.y /= _VerticalAmount;
				fixed4 texColor = tex2D(_MainTex, uv);

				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo *
					max(0, dot(worldNormal, worldLightDir));
				
				return fixed4(ambient + diffuse, texColor.a * _AlphaScale);
			}
			ENDCG
		}
	}
	FallBack "Transparent/VertexLit"
}

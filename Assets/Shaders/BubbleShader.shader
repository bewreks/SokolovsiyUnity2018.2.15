Shader "Unlit/BubbleShader"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Color2 ("Color2", Color) = (1, 1, 1, 1)
		_MinBlend ("MinBlend", Range(0, .5)) = 0
		_MaxBlend ("MaxBlend", Range(.5, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				fixed4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

            fixed  _MaxBlend;
            fixed  _MinBlend;
			fixed4 _Color;
			fixed4 _Color2;
			
			v2f vert (appdata v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = lerp(_Color, _Color2, 
				          lerp(_MinBlend, _MaxBlend, o.vertex.y));
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				return col;
			}
			ENDCG
		}
	}
}

Shader "Custom/WaterShake"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color Tint",Color) = (1,1,1,1)
		_Magnitude("Distortion Magnitude" ,Float) = 1
		_Frequency("Distortion Frequency", Float) = 1
		_InvWaveLength("Distortion Inverse Wave Length", Float)=10
		_Speed("Speed",Float) = 0.5
	}
	SubShader
	{
	 // Need to disable batching because of the vertex animation 
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "DisableBatching"="True" }

		Pass
		{
		 Tags{ "LightMode"="ForwardBase"}
		 ZWrite Off
		 Blend SrcAlpha OneMinusSrcAlpha
		 Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _Magnitude;
			float _Frequency;
			float _InvWaveLength;
			float _Speed;
			
			
			v2f vert (a2v v)
			{
				v2f o;
				
				float4 offset;
				offset.yzw = float3(0.0 , 0.0, 0.0);

				//利用一个sin三角函数对ugui面片的X坐标和Y坐标进行曲线的波动，最后产生了Ugui随着时间不停的扭曲的效果
				offset.x = sin(_Frequency * _Time.y + v.vertex.x * _InvWaveLength + v.vertex.y * _InvWaveLength 
				+v.vertex.z * _InvWaveLength) * _Magnitude;
				offset.y = sin(_Frequency * _Time.y + v.vertex.x * _InvWaveLength + v.vertex.y * _InvWaveLength
				+v.vertex.z * _InvWaveLength) * _Magnitude;
				
				o.pos = UnityObjectToClipPos(v.vertex + offset);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb *= _Color.rgb;
				return col;
			}
			ENDCG
		}
	}
	FallBack "Transparent/VertexLit"
}

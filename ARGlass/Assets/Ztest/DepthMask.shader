Shader "DepthMask"
{
	SubShader{
		Tags{ "Queue" = "Geometry-10" }

		Lighting Off

		ZTest Lequal
		ZWrite On


		ColorMask 0

		Pass{}

	}
}

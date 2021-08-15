Shader "Custom/MapVertexAlpha"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Emission("Emission", Color) = (0,0,0,0)
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent"  "Queue" = "Transparent"}

		CGPROGRAM
		#pragma surface surf Lambert alpha:blend

		struct Input
		{
			float2 uv_MainTex;
			half4 color : COLOR;
		};

		sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _Emission;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = IN.color.a;
			o.Emission = _Emission;
		}
		ENDCG
	}
		Fallback "Diffuse"
}
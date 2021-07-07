Shader "Custom/MapCaustics"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_1Tex("Layer 1", 2D) = "white" {}
		_2Tex("Layer 2", 2D) = "white" {}
    }
    SubShader
    {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        //Blend Ond One

		ZWrite Off
		Blend DstColor One

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows Alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _1Tex;
		sampler2D _2Tex;

        struct Input
        {
			float2 uv_1Tex;
			float2 uv_2Tex;
			half4 color : COLOR;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_1Tex, IN.uv_1Tex) * _Color;
			fixed4 b = tex2D(_2Tex, IN.uv_2Tex) * _Color;

			o.Albedo = (c.rgb * b.rgb)  * IN.color.a;

			o.Alpha = c.a * IN.color.a;
			o.Alpha = IN.color.a;
		}
        ENDCG
    }
    FallBack "Diffuse"
}

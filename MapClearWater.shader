Shader "Custom/MapClearWater"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _1Tex ("Layer 1", 2D) = "white" {}
		_2Tex("Layer 2", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_AlphaStart ("Alpha Cutoff Start", Range(0,1)) = 0.3
		_AlphaEnd("Alpha Cutoff End", Range(0,1)) = 0.9
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue" = "Transparent"}
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:blend

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
		half _AlphaStart;
		half _AlphaEnd;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_1Tex, IN.uv_1Tex) * _Color;
			fixed4 b = tex2D(_2Tex, IN.uv_2Tex) * _Color;

			o.Albedo = (c.rgb + b.rgb) * IN.color.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			
			if(c.a + b.a > _AlphaStart && c.a + b.a < _AlphaEnd)
			{
				o.Alpha = 0;
			}
			else
			{
				o.Alpha = (c.a + b.a) * IN.color.a;
			}
		}
        ENDCG
    }
    FallBack "Diffuse"
}

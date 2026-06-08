Shader "Custom/GrassWind"
{
    Properties
    {
        _MainTex ("Grass Texture", 2D) = "white" {}
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.2
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1.5
        _WindFrequency ("Wind Frequency", Range(0, 10)) = 2.0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        Cull Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        float _WindStrength;
        float _WindSpeed;
        float _WindFrequency;
        float _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert(inout appdata_full v)
        {
            float timeFactor = _Time.y * _WindSpeed;
            float wind = sin(timeFactor + v.vertex.x * _WindFrequency) * _WindStrength;

            float heightFactor = saturate(v.vertex.y); // More sway at the top
            v.vertex.xz += wind * heightFactor;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = texColor.rgb; // Keep the original texture color
            o.Alpha = texColor.a;
            clip(o.Alpha - _Cutoff);
        }
        ENDCG
    }

    FallBack "Transparent/Cutout/Diffuse"
}

Shader "Custom/MergerShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2DArray) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _BumpScale("Scale", Float) = 1.0
        [Normal] _BumpMap("Normal Map", 2DArray) = "bump" {}

        _ParallaxMap("Height Map", 2DArray) = "black" {}
        _Parallax("Height Scale", Range(0.005, 0.08)) = 0.02

        _OcclusionMap("Occlusion", 2DArray) = "white" {}
        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0

        _DetailMask("Detail Mask", 2DArray) = "white" {}

        [HideInInspector] _ZOffset("Z Buffer Offset", int) = 0
        //_ArrayIndex("2DArray Index", int) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Offset[_ZOffset],[_ZOffset]

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows vertex:vert

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.5

            #pragma require 2darray

        UNITY_DECLARE_TEX2DARRAY(_MainTex);
        UNITY_DECLARE_TEX2DARRAY(_BumpMap);

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_ParallaxMap;
            float arrayIndex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        //int _ArrayIndex;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            /*UNITY_DEFINE_INSTANCED_PROP(2DArray, _MainTex)*/
            //UNITY_DEFINE_INSTANCED_PROP(2DArray, _BumpMap)
            /*UNITY_DEFINE_INSTANCED_PROP(float, arrayIndex)*/
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o)
        {
            o.uv_MainTex = v.texcoord.xy;
            o.uv_BumpMap = v.texcoord.xy;
            o.uv_ParallaxMap = v.texcoord.xy;

            o.arrayIndex = v.texcoord.z;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            int texIndex = 0;
            //IN.arrayIndex = _ArrayIndex;
            fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3 (IN.uv_MainTex, IN.arrayIndex));
            o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3 (IN.uv_BumpMap, IN.arrayIndex)));
            // Albedo comes from a texture tinted by color
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

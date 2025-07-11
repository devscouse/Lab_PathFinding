Shader "Unlit/NodeShader"
{
    Properties
    {
        _GradientTex ("Gradient Texture", 2D) = "white" {}
        _NodeInfoTex ("_NodeInfoTex", 2D) = "black" {}
        _CurrTime ("Global Time", Float) = 0
        _FadeTime ("Fade Time", Float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _GradientTex;
            sampler2D _NodeInfoTex;
            float4 _NodeInfoTex_ST;
            float _CurrTime;
            float _FadeTime;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NodeInfoTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 nodeData = tex2D(_NodeInfoTex, i.uv);
                float exploredTime = nodeData.a * 255.0;

                if (exploredTime <= 0.001) {
                    discard;
                }
                float age = _CurrTime - exploredTime;
                float fade = clamp(1.0 - (age / _FadeTime), 0.1, 1);

                float gradientIndex = saturate(age / 255.0);
                float3 baseColor = tex2D(_GradientTex, float2(gradientIndex, 0.5));
                return float4(baseColor * fade, fade);
            }
            ENDCG
        }
    }
}

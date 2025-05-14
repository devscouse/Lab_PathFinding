Shader "Unlit/DebugNodeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Channel ("Channel to show (0=R, 1=G, 2=B, 3=A)", Range(0, 3)) = 0
        _MaxValue ("Maximum expected value in the channel", float) = 1
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Channel;
            float _MaxValue;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                int channel = (int)(_Channel + 0.5);
                float4 col = tex2D(_MainTex, i.uv);
                float value = channel == 0 ? col.r :
                              channel == 1 ? col.g :
                              channel == 2 ? col.b : col.a;
                value = saturate(value / (_MaxValue + 1e-5));
                return fixed4(value, value, value, 1);
            }
            ENDCG
        }
    }
}

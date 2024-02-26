Shader "Hidden/UvColor"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Cull Off
        ZWrite On
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 color : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float uvx = frac(v.uv.x);
                float uvy = frac(v.uv.y);
                o.vertex = float4(float2(uvx, 1 - uvy) * 2 - 1, 0, 1);
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(i.color, 1);
            }
            ENDCG
        }
    }
}

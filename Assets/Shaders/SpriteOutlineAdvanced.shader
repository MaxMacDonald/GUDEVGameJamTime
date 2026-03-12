Shader "Custom/SpriteOutlineExpanded"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0,10)) = 1
        _Expand ("Mesh Expand", Range(0,0.1)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 _OutlineColor;
            float4 _Color;
            float _OutlineSize;
            float _Expand;

            v2f vert (appdata v)
            {
                v2f o;

                // Expand sprite mesh outward
                float2 dir = sign(v.vertex.xy);
                v.vertex.xy += dir * _Expand;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;

                return o;
            }

            float SampleAlpha(float2 uv)
            {
                return tex2D(_MainTex, uv).a;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float outline = 0;

                float2 texel = _MainTex_TexelSize.xy * _OutlineSize;

                outline += SampleAlpha(i.uv + float2(texel.x,0));
                outline += SampleAlpha(i.uv + float2(-texel.x,0));
                outline += SampleAlpha(i.uv + float2(0,texel.y));
                outline += SampleAlpha(i.uv + float2(0,-texel.y));

                outline += SampleAlpha(i.uv + float2(texel.x,texel.y));
                outline += SampleAlpha(i.uv + float2(-texel.x,texel.y));
                outline += SampleAlpha(i.uv + float2(texel.x,-texel.y));
                outline += SampleAlpha(i.uv + float2(-texel.x,-texel.y));

                outline = saturate(outline);

                if (col.a == 0 && outline > 0)
                    return _OutlineColor;

                return col;
            }

            ENDCG
        }
    }
}
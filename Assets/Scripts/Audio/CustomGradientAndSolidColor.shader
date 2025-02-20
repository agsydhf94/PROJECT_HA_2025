Shader "Custom/CustomGradientAndSolidColor"
{
    Properties
    {
        _GradientColor1 ("Gradient Start Color", Color) = (0, 1, 0, 1)
        _GradientColor2 ("Gradient End Color", Color) = (0, 0, 1, 1)
        _SolidColor ("Solid Color", Color) = (1, 0, 0, 1)
        _GradientStart ("Gradient Start Y Position", Float) = 0.0
        _GradientEnd ("Gradient End Y Position", Float) = 7.0
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

            #include "UnityCG.cginc"

            struct vertInput
            {
                float4 vertex : POSITION;
            };

            struct vertOutput
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _GradientColor1;
            fixed4 _GradientColor2;
            fixed4 _SolidColor;
            float _GradientStart;
            float _GradientEnd;

            vertOutput vert (vertInput v)
            {
                vertOutput o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // 월드 좌표 계산
                return o;
            }

            fixed4 frag (vertOutput i) : SV_Target
            {
                float gradientPosition = i.worldPos.y; // 월드 공간 Y 좌표 사용
                fixed4 finalColor;

                if (gradientPosition <= _GradientEnd)
                {
                    // A에서 B로의 그래디언트 계산
                    float gradientFactor = saturate((gradientPosition - _GradientStart) / (_GradientEnd - _GradientStart));
                    finalColor = lerp(_GradientColor1, _GradientColor2, gradientFactor);
                }
                else
                {
                    // 단색 영역
                    finalColor = _SolidColor;
                }

                return finalColor;
            }
            ENDCG
        }
    }
}

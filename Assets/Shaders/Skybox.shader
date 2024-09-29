Shader "Unlit/Skybox"
{
    SubShader
    {
        Tags
        {
            "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"
        }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            // Uniform variables for sky colors
            uniform fixed4 _SkyColorHorizon, _SkyColorTop, _SkyColorBottom;
            uniform float _GlobalLightIntensity;

            // Function to calculate sky color based on view direction
            fixed4 GetSkyColor(float3 viewDir)
            {
                // Latitude based on the view direction's y-axis (vertical angle)
                float2 lat = atan2((abs(viewDir.y)), sqrt(viewDir.x * viewDir.x + viewDir.z * viewDir.z));
                float height = pow(2 * lat / 3.141592, 1);

                // Adjust colors based on global light intensity (day to night transition)
                fixed4 horizonColor = lerp(fixed4(0,0,0,1), _SkyColorHorizon, _GlobalLightIntensity); // Darker at night
                fixed4 topColor = lerp(fixed4(0,0,0,1), _SkyColorTop, _GlobalLightIntensity);         // Darker at night
                fixed4 bottomColor = lerp(fixed4(0,0,0,1), _SkyColorBottom, _GlobalLightIntensity);   // Darker at night

                return lerp(horizonColor, lerp(bottomColor, topColor, saturate(sign(viewDir.y))), height);
            }

            // Fragment shader function
            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.texcoord);
                return GetSkyColor(viewDir);
            }
            ENDCG
        }
    }
    Fallback Off
}
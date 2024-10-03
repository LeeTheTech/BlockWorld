Shader "Unlit/BlockTransparentLiquid"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent+1"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment pixel_shader
            #pragma exclude_renderers nomrt // Excludes this shader from rendering in render-to-texture modes.
            #pragma multi_compile ___ UNITY_HDR_ON // Allows multiple shader variants depending on HDR settings.
            #pragma target 3.0 // Specifies the minimum shader model version required.

            #include "UnityCG.cginc"

            struct structureVS
            {
                float4 screen_vertex : SV_POSITION; // Position of the vertex in screen space.
                float4 world_vertex : TEXCOORD0; // Position of the vertex in world space.
                float3 normal : TEXCOORD1; // Normal vector at the vertex.
                float2 uv : TEXCOORD2; // UV coordinates for texturing.
                float4 color : COLOR; // Color data from the vertex color.
            };
            
            uniform sampler2D _AnimatedBlockTextures;
            uniform float _GlobalLightIntensity;

            structureVS vertex_shader(float4 vertex : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0, float4 color : COLOR)
            {
                structureVS vs;
                vs.screen_vertex = UnityObjectToClipPos(vertex);
                vs.world_vertex = mul(unity_ObjectToWorld, vertex);
                vs.normal = UnityObjectToWorldNormal(normal);

                // Calculate the number of frames in the atlas (8x8 grid in a 128x128 texture with 16x16 frames)
                float frameCount = 128 / 16; // Total frames per row or column (8)

                // Calculate which frame we should be displaying based on time and speed
                float currentFrame = floor(fmod(_Time * 100, frameCount));

                // Calculate UV offsets to move through the frames vertically (from top to bottom)
                float2 uvOffset = float2(0, currentFrame / frameCount); // Vertical shift based on the current frame

                // Adjust UV coordinates (texture atlas)
                vs.uv = uv / 128 * 16;
                vs.uv.y = 1 - vs.uv.y - uvOffset.y; // Flip UV vertically and apply animation offset

                vs.color = color; // Pass through vertex color
                return vs;
            }

            half4 pixel_shader(structureVS vs) : SV_Target
            {
                half4 texColor = tex2D(_AnimatedBlockTextures, vs.uv);
                texColor.a = 0.5;
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
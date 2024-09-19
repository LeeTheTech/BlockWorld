Shader "Unlit/BlockTransparent"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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

            uniform sampler2D _BlockTextures;
            float4 _Color;

           structureVS vertex_shader(float4 vertex : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0, float4 color : COLOR)
            {
                structureVS vs;
                vs.screen_vertex = UnityObjectToClipPos(vertex); // Convert object space position to screen space.
                vs.world_vertex = mul(unity_ObjectToWorld, vertex); // Convert object space position to world space.
                vs.normal = UnityObjectToWorldNormal(normal); // Convert object space normal to world space.
                vs.uv = uv / 128 * 16; // Adjust UV coordinates (texture atlas).
                vs.uv.y = 1 - vs.uv.y; // Flip UV vertically.
                vs.color = color; // Pass through vertex color.
                return vs;
            }

            half4 pixel_shader(structureVS i) : SV_Target
            {
                half4 texColor = tex2D(_BlockTextures, i.uv);
                texColor.a = 0.5; // Set alpha to 50% transparency
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
﻿Shader "Unlit/AnimatedNoCullBlock"
{
    SubShader
    {
        Tags
        {
            // Specifies that this shader is for opaque objects.
            "RenderType"="Opaque"
        }
        LOD 100 // Level of Detail for the shader, affects its rendering performance.
        Cull Off

        Pass
        {
            Tags
            {
                // Specifies that this pass is intended for deferred rendering.
                "LightMode" = "Deferred"
            }

            CGPROGRAM
            #pragma vertex vertex_shader // Indicates the function for the vertex shader.
            #pragma fragment pixel_shader // Indicates the function for the fragment shader.
            #pragma exclude_renderers nomrt // Excludes this shader from rendering in render-to-texture modes.
            #pragma multi_compile ___ UNITY_HDR_ON // Allows multiple shader variants depending on HDR settings.
            #pragma target 3.0 // Specifies the minimum shader model version required.

            #include "UnityPBSLighting.cginc" // Includes common lighting calculations for Physically Based Rendering.

            // Shader properties
            float4 _Color; // Base color of the object.
            float _Metallic; // Metallic property used in the shading model.
            float _Gloss; // Glossiness property used in the shading model.

            // Structure for vertex shader output.
            struct structureVS
            {
                float4 screen_vertex : SV_POSITION; // Position of the vertex in screen space.
                float4 world_vertex : TEXCOORD0; // Position of the vertex in world space.
                float3 normal : TEXCOORD1; // Normal vector at the vertex.
                float2 uv : TEXCOORD2; // UV coordinates for texturing.
                float4 color : COLOR; // Color data from the vertex color.
            };

            // Structure for fragment shader output.
            struct structurePS
            {
                half4 albedo : SV_Target0; // Albedo color output for the fragment.
                half4 specular : SV_Target1; // Specular reflection output for the fragment.
                half4 normal : SV_Target2; // Normal vector output for the fragment.
                half4 emission : SV_Target3; // Emission color output for the fragment.
            };

            // Vertex shader function.
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

            // Sky color parameters.
            uniform fixed4 _SkyColorHorizon, _SkyColorTop, _SkyColorBottom;

            // Function to calculate sky color based on view direction.
            fixed4 GetSkyColor(float3 viewDir)
            {
                float2 lat = atan2((abs(viewDir.y)), sqrt(viewDir.x * viewDir.x + viewDir.z * viewDir.z));
                // Latitude based on view direction.
                float height = pow(2 * lat / 3.141592, 1); // Normalize latitude to height.
                return lerp(_SkyColorHorizon, lerp(_SkyColorBottom, _SkyColorTop, saturate(sign(viewDir.y))), height);
                // Interpolate sky color based on height.
            }
            
            uniform sampler2D _AnimatedBlockTextures;
            uniform float _MinLightLevel;
            uniform float _GlobalLightIntensity;
            
            structurePS pixel_shader(structureVS vs)
            {
                structurePS ps;
                float3 normalDirection = normalize(vs.normal); // Normalize the normal vector.
                half3 specular;
                half specularMonochrome;
                half3 diffuseColor =DiffuseAndSpecularFromMetallic(_Color.rgb, _Metallic, specular, specularMonochrome);
                // Compute diffuse and specular colors.

                fixed4 c = tex2D(_AnimatedBlockTextures, vs.uv); // Sample the texture color at the given UV coordinates.
                clip(c.a - 0.1); // Clip pixels with alpha less than 0.1 (create transparency).

                fixed4 sky = GetSkyColor(vs.world_vertex - _WorldSpaceCameraPos);
                // Get sky color based on world vertex position.
                float fade = saturate(pow(distance(_WorldSpaceCameraPos.xz, vs.world_vertex.xz) / (16.0 - 1.0) / 16.0, 12)); // Compute fade factor based on distance from the camera.

                float sunLight = vs.color.g * 16; // Extract sunlight (green channel)
                float blockLight = vs.color.b * 16; // Extract block light (blue channel)
                
                float blockLightWeight = 1.0 - _GlobalLightIntensity;
                float sunLightWeight = _GlobalLightIntensity;

                // Blend the two lights based on the weights
                float light = (blockLight * blockLightWeight) + (sunLight * sunLightWeight);

                // Ensure there's always a minimum light contribution to avoid complete darkness at night
                float minLightContribution = 0.05;
                light = max(light, minLightContribution); // Ensure minimum light contribution

                // Apply the blended light to the texture color
                c.rgb *= light;
                c.rgb += diffuseColor; // Add diffuse color to the texture color.
                c.a = 1; // Set alpha to 1 (fully opaque).
                
                ps.albedo = 0; // Set albedo (color) to zero (not used).
                ps.specular = half4(specular, 0); // Set specular reflection.
                ps.normal = half4(normalDirection * 0.5 + 0.5, 1.0); // Convert normal vector to [0,1] range.
                ps.emission = lerp(c, sky, fade); // Compute emission color based on texture color and sky color.
                #ifndef UNITY_HDR_ON
                ps.emission.rgb = exp2(-ps.emission.rgb); // Convert emission color for non-HDR rendering.
                #endif
                return ps;
            }
            ENDCG
        }
    }
}
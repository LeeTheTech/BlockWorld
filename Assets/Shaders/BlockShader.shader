Shader "Custom/DefaultMat"
{
    SubShader
    {
        Tags
        {
            // Specifies that this shader is for opaque objects.
            "RenderType"="Opaque"
        }
        LOD 200 // Level of Detail for the shader, affects its rendering performance.

        CGPROGRAM
        #pragma surface surf Block // Uses the 'Block' surface shader model.

        // Custom lighting function for the surface shader.
        half4 LightingBlock(SurfaceOutput s, half3 lightDir, half atten)
        {
            half NdotL = dot(s.Normal, lightDir); // Calculate the dot product between the normal and light direction.
            half4 c;
            NdotL = max(NdotL, 0.1); // Clamp the dot product value to a minimum of 0.1 for better shading.
            c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten); // Calculate the diffuse color contribution based on albedo, light color, and attenuation.

            c.a = 1; // Set alpha to 1 (fully opaque).
            return c;
        }

        // Shader properties.
        sampler2D _MainTex; // Main texture (not used in this shader).
        uniform sampler2D _BlockTextures; // Texture sampler for block textures.

        // Input structure for the surface function.
        struct Input
        {
            float2 uv_BlockTextures; // UV coordinates for block textures.
            float4 color : COLOR; // Color data from the vertex color.
            float3 worldPos; // World position of the fragment.
        };

        // Surface function for the shader.
        void surf(Input IN, inout SurfaceOutput o)
        {
            float2 uv = IN.uv_BlockTextures; // Get UV coordinates from input.
            uv = uv / 512.0; // Scale UV coordinates 
            uv.y = 1 - uv.y; // Flip UV vertically.
            fixed4 c = tex2D(_BlockTextures, uv); // Sample the block texture at the given UV coordinates.
            clip(c.a - 0.1); // Clip pixels with alpha less than 0.1 (create transparency).

            // Calculate fade based on distance from the camera.
            const float fade = pow(distance(_WorldSpaceCameraPos.xz, IN.worldPos.xz) / (16.0 - 1.0) / 16.0, 2);

            // Set the surface output properties.
            o.Albedo = c.rgb * IN.color.rgb; // Calculate albedo color based on texture color and vertex color.
            o.Emission = fade; // Set emission color based on fade factor.
            o.Gloss = 0; // Set glossiness to 0 (non-shiny surface).
            o.Alpha = c.a; // Set alpha based on texture alpha.
        }
        ENDCG
    }
    FallBack "Diffuse" // Fallback shader if this shader is not supported.
}
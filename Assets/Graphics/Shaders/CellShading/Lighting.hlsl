#ifndef CUSTOM_CEL_SHADING_INCLUDED
#define CUSTOM_CEL_SHADING_INCLUDED

#if !SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#endif

struct SurfaceInfos
{
    float3 normal;
    float3 view;
    
    float smoothness;
    float shininess;
    float rim_threshold;
    float specular_multiplier;
    
    float diffuse_cell_threshold;
    float specular_cell_threshold;
    float rim_cell_threshold;
    
    float shadow_attenuation_threshold;
    float distance_attenuation_threshold;

    float3 texture_color;
    float3 shadow_color;
};

#if !SHADERGRAPH_PREVIEW
float3 CalculateCelShading(const Light light, const SurfaceInfos surface_infos)
{
    const float shadow_attenuation_smooth =
        smoothstep(0.0f, surface_infos.shadow_attenuation_threshold, light.shadowAttenuation);
    const float distance_attenuation_smooth =
        smoothstep(0.0f, surface_infos.distance_attenuation_threshold, light.distanceAttenuation);
    const float attenuation = shadow_attenuation_smooth * distance_attenuation_smooth;
    
    float diffuse = saturate(dot(surface_infos.normal, light.direction));
    diffuse *= attenuation;
    
    const float3 half_vector = SafeNormalize(light.direction + surface_infos.view);
    float specular = saturate(dot(surface_infos.normal, half_vector));
    specular = pow(specular, surface_infos.shininess);
    specular *= diffuse * surface_infos.smoothness;
    specular = specular > surface_infos.specular_cell_threshold ? 1 : 0;

    float rim = 1 - dot(surface_infos.view, surface_infos.normal);
    rim *= pow(diffuse, surface_infos.rim_threshold);
    rim *= pow(rim, surface_infos.smoothness);
    rim = rim > surface_infos.rim_cell_threshold ? 1 : 0;

    
    float3 color = length(light.color * diffuse) > surface_infos.diffuse_cell_threshold ?
        surface_infos.texture_color * light.color : surface_infos.texture_color * surface_infos.shadow_color ;
    color += max(specular, rim) * surface_infos.specular_multiplier;
    
    return color;
    
    /*
    specular = surface_infos.smoothness * smoothstep(
        (1 - surface_infos.smoothness) * surface_infos.specular_cell_threshold + surface_infos.specular_cell_offset,
        surface_infos.specular_cell_threshold + surface_infos.specular_cell_offset,
        specular);
        
    rim = surface_infos.smoothness * smoothstep(
        surface_infos.rim_cell_threshold - 0.5f * surface_infos.rim_cell_offset,
        surface_infos.rim_cell_threshold - 0.5f * surface_infos.rim_cell_offset,
        rim);
    */
    
    return light.color * diffuse;//(diffuse + max(specular, rim));
}
#endif

void GetCelShaded_float(const float3 normal, const float3 view, const float3 position,
    const float smoothness, const float rim_threshold, const float specular_multiplier,
    const float diffuse_cell_threshold, const float specular_cell_threshold, const float rim_cell_threshold,
    const float shadow_attenuation_threshold, const float distance_attenuation_threshold,
    float3 shadow_color, float3 texture_color,
    out float3 color)
{
    color = 0.5;
    
    #if !SHADERGRAPH_PREVIEW

    SurfaceInfos surface_infos;
    surface_infos.normal = normalize(normal);
    surface_infos.view = view;
    
    surface_infos.smoothness = smoothness;
    surface_infos.shininess = exp2(10 * smoothness + 1);
    surface_infos.rim_threshold = rim_threshold;
    surface_infos.specular_multiplier = specular_multiplier;
    
    surface_infos.diffuse_cell_threshold = diffuse_cell_threshold;
    surface_infos.specular_cell_threshold = specular_cell_threshold;
    surface_infos.rim_cell_threshold = rim_cell_threshold;
    
    surface_infos.shadow_attenuation_threshold = shadow_attenuation_threshold;
    surface_infos.distance_attenuation_threshold = distance_attenuation_threshold;

    surface_infos.texture_color = texture_color;
    surface_infos.shadow_color = shadow_color;
    
    #if SHADOWS_SCREEN
    const float4 clip_pos = TransformWorldToHClip(position);
    const float4 shadow_coord = ComputeScreenPos(clip_pos);
    #else
    const float4 shadow_coord = TransformWorldToShadowCoord(position);
    #endif
    
    Light light = GetMainLight(shadow_coord);
    color = CalculateCelShading(light, surface_infos);

    const int light_count_on_pixel = GetAdditionalLightsCount();
    
    for (int i = 0; i < light_count_on_pixel; i++)
    {
        light = GetAdditionalLight(i, position, 1);
        color += CalculateCelShading(light, surface_infos);
    }
   
    #endif
}
#endif
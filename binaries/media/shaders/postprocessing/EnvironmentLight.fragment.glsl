#version 430 core

out vec4 outColor;
uniform int DisablePostEffects;
uniform float VDAOGlobalMultiplier;

#include LogDepth.glsl

FragmentData currentFragment;

#include Lighting.glsl
vec2 UV = gl_FragCoord.xy / textureSize(deferredTex, 0);
#include UsefulIncludes.glsl
#include Shade.glsl
#include EnvironmentLight.glsl


void main()
{	
	vec4 albedoRoughnessData = textureMSAAFull(albedoRoughnessTex, UV);
	vec4 normalsDistanceData = textureMSAAFull(normalsDistancetex, UV);
	vec4 specularBumpData = textureMSAAFull(specularBumpTex, UV);
	vec3 camSpacePos = reconstructCameraSpaceDistance(UV, normalsDistanceData.a);
	vec3 worldPos = FromCameraSpace(camSpacePos);
	
	currentFragment = FragmentData(
		albedoRoughnessData.rgb,
		specularBumpData.rgb,
		normalsDistanceData.rgb,
		vec3(1,0,0),
		worldPos,
		camSpacePos,
		normalsDistanceData.a,
		1.0,
		albedoRoughnessData.a,
		specularBumpData.a
	);	
	
	vec4 color = EnvironmentLight(currentFragment).rgbb;
    outColor = clamp(color, 0.0, 10000.0);
}
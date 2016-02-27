//uniform mat4 ViewMatrix;
//uniform mat4 ProjectionMatrix;
uniform mat4 VPMatrix;

const int MAX_LIGHTS = 6;
uniform int LightsCount;
uniform mat4 LightsPs[MAX_LIGHTS];
uniform mat4 LightsVs[MAX_LIGHTS];
uniform int LightsShadowMapsLayer[MAX_LIGHTS];
uniform vec3 LightsPos[MAX_LIGHTS];
uniform float LightsFarPlane[MAX_LIGHTS];
uniform vec4 LightsColors[MAX_LIGHTS];
uniform float LightsBlurFactors[MAX_LIGHTS];
uniform int LightsExclusionGroups[MAX_LIGHTS];

uniform vec4 LightsConeLB[MAX_LIGHTS];
uniform vec4 LightsConeLB2BR[MAX_LIGHTS];
uniform vec4 LightsConeLB2TL[MAX_LIGHTS];


uniform int Instances;

struct Material{
	vec4 diffuseColor;
	vec4 specularColor;
	vec4 roughnessAndParallaxHeight;
	
	uvec2 diffuseAddr;
	uvec2 specularAddr;
	uvec2 alphaAddr;
	uvec2 roughnessAddr;
	uvec2 bumpAddr;
	uvec2 normalAddr;
	
	vec4 alignment3;
	vec4 alignment4;
};

layout (std430, binding = 7) buffer MatBuffer
{
  Material Materials[]; 
};
uniform int MaterialIndex;

Material getCurrentMaterial(){
	//return Materials[MaterialIndex];
	Material mat = Material(
		Materials[MaterialIndex].diffuseColor,
		Materials[MaterialIndex].specularColor,
		Materials[MaterialIndex].roughnessAndParallaxHeight,
		
		Materials[MaterialIndex].diffuseAddr,
		Materials[MaterialIndex].specularAddr,
		Materials[MaterialIndex].alphaAddr,
		Materials[MaterialIndex].roughnessAddr,
		Materials[MaterialIndex].bumpAddr,
		Materials[MaterialIndex].normalAddr,
		
		Materials[MaterialIndex].alignment3,
		Materials[MaterialIndex].alignment4
	);
	return mat;
}

Material currentMaterial = getCurrentMaterial();

layout (std430, binding = 0) buffer MMBuffer
{
  mat4 ModelMatrixes[]; 
}; 
layout (std430, binding = 1) buffer RMBuffer
{
  mat4 RotationMatrixes[]; 
}; 
layout (std430, binding = 2) buffer IDMBuffer
{
  uint InstancedIds[]; 
}; 
uniform vec3 CameraPosition;
//uniform vec3 CameraDirection;
uniform float Time;

#define SpecularColor currentMaterial.specularColor.xyz
#define DiffuseColor currentMaterial.diffuseColor.xyz

uniform float Brightness;

#define UseNormalsTex (currentMaterial.normalAddr.x > 0)
#define UseBumpTex (currentMaterial.bumpAddr.x > 0)
#define UseAlphaTex (currentMaterial.alphaAddr.x > 0)
#define UseRoughnessTex (currentMaterial.roughnessAddr.x > 0)
#define UseDiffuseTex (currentMaterial.diffuseAddr.x > 0)
#define UseSpecularTex (currentMaterial.specularAddr.x > 0)


#extension GL_ARB_bindless_texture : require
#define bumpTex sampler2D(currentMaterial.bumpAddr)
#define alphaTex sampler2D(currentMaterial.alphaAddr)
#define diffuseTex sampler2D(currentMaterial.diffuseAddr)
#define normalsTex sampler2D(currentMaterial.normalAddr)
#define specularTex sampler2D(currentMaterial.specularAddr)
#define roughnessTex sampler2D(currentMaterial.roughnessAddr)


#define Roughness currentMaterial.roughnessAndParallaxHeight.x
#define ParallaxHeightMultiplier currentMaterial.roughnessAndParallaxHeight.y
//uniform float Metalness;

uniform vec2 resolution;
float ratio = resolution.y/resolution.x;

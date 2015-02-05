#include LightingSamplers.glsl
#include Mesh3dUniforms.glsl

in vec2 LightScreenSpaceFromGeo[MAX_LIGHTS];
smooth in vec3 positionModelSpace;
smooth in vec3 positionWorldSpace;
smooth in vec3 normal;

vec2 hash2x2(vec2 co) {
	return vec2(
		fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453),
		fract(sin(dot(co.yx ,vec2(12.9898,78.233))) * 43758.5453));
}

float specular(vec3 normal){
	vec3 lightRelativeToVPos = LightsPos[0] - positionWorldSpace.xyz;
	vec3 cameraRelativeToVPos = CameraPosition - positionWorldSpace.xyz;
	vec3 R = reflect(lightRelativeToVPos, normal);
	float cosAlpha = dot(normalize(cameraRelativeToVPos), normalize(R));
	float clamped = clamp(-cosAlpha, 0.0, 1.0);
	return clamp(pow(clamped, 10.0), 0.0, 1.0);
}

float diffuse(vec3 normal){
	vec3 lightRelativeToVPos = LightsPos[0] - positionWorldSpace.xyz;
	float dotdiffuse = dot(normalize(lightRelativeToVPos), normalize (normal));
	float angle = clamp(dotdiffuse, 0.0, 1.0);
	return (angle)*2;
}

float getShadowPercent(vec2 uv, uint i){
	float accum = 1.0;
	for(float g = 0.0f; g < 32.0f; g += 2.0){ 
		float distance2 = distance(positionWorldSpace.xyz, LightsPos[i]);
		vec2 offsetDistance = hash2x2(vec2(1.0f * g,1.0f * i)) * (0.0001 * g);
		vec2 fakeUV = uv + offsetDistance;
		float distance1 = 0.0;
		if(i==0)distance1 = texture(lightDepth0, fakeUV).r;
		else if(i==1)distance1 = texture(lightDepth1, fakeUV).r;
		else if(i==2)distance1 = texture(lightDepth2, fakeUV).r;
		else if(i==3)distance1 = texture(lightDepth3, fakeUV).r;
		else if(i==4)distance1 = texture(lightDepth4, fakeUV).r;
		else if(i==5)distance1 = texture(lightDepth5, fakeUV).r;
		else if(i==6)distance1 = texture(lightDepth6, fakeUV).r;
		else if(i==7)distance1 = texture(lightDepth7, fakeUV).r;
		float badass_depth = log(LogEnchacer*distance2 + 1.0) / log(LogEnchacer*LightsFarPlane[i] + 1.0);
		float diff = abs(distance1 -  badass_depth);
		if(diff > (0.0008)) accum *= 0.96f;
	}
	return accum;
}

vec3 processLighting(vec3 color){
	bool shadow = false;
	//for(uint i = 0; i < LightsCount; i++)if(LightScreenSpace[i].x < 0.0 || LightScreenSpace.x > 1.0) {shadow = true;}
	//for(uint i = 0; i < LightsCount; i++)if(LightScreenSpace[i].y < 0.0 || LightScreenSpace.y > 1.0) {shadow = true;}
	
	float diffuseComponent = diffuse(normal);
	float specularComponent = specular(normal);
	if(!shadow){
		for(uint i = 0; i < LightsCount; i++){

			float percent = getShadowPercent(LightScreenSpaceFromGeo[i], i);
			color *= percent;
			diffuseComponent *= percent;
			specularComponent *= percent;
		}
		//color = vec3(diff);
	} else {
		color *= 0.2;
	}
	//float diffuse = 1.0;
	return (color * diffuseComponent + specularComponent).xyz;
}
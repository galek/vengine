#version 430 core
uniform vec3 CameraPosition;

in vec4 positionWorldSpace;
in vec3 positionModelSpace;
in vec3 normal;


out vec4 outColor;
/*
float hash3(vec3 uv) {
	return fract(sin(uv.x * 15.78 + uv.y * 35.14 + uv.z * 26.1134) * 43758.23);
}*/

void main()
{
	vec4 color = vec4(0.3, 0.4, 0.7, 1.0);
	
	color += clamp((positionWorldSpace.y + 1900.0)/670.0, 0.0, 1.0) * 0.2;

	//float d = clamp(380.0f / length(CameraPosition - positionWorldSpace.xyz), 0.2, 1.0);
	//color *= d;
	float diffuse = clamp(dot(vec3(1.0, 1.0, 1.0), normalize(normal)), 0.2, 1.0);
	color *= diffuse * normalize(positionModelSpace).xyzz;
	
    outColor = color;
}
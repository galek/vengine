#include_once LightingSamplers.glsl

float lookupDepthFromLight(uint i, vec2 uvi, float comparison){
    float distance1 = 0.0;
    vec3 uv = vec3(uvi, comparison);
    if(i==0)distance1 = texture(lightDepth0, uv);
    else if(i==1)distance1 = texture(lightDepth1, uv);
    else if(i==2)distance1 = texture(lightDepth2, uv);
    else if(i==3)distance1 = texture(lightDepth3, uv);
    else if(i==4)distance1 = texture(lightDepth4, uv);
    else if(i==5)distance1 = texture(lightDepth5, uv);
    else if(i==6)distance1 = texture(lightDepth6, uv);
    else if(i==7)distance1 = texture(lightDepth7, uv);
    else if(i==8)distance1 = texture(lightDepth8, uv);
    else if(i==9)distance1 = texture(lightDepth9, uv);
    else if(i==10)distance1 = texture(lightDepth10, uv);
    else if(i==11)distance1 = texture(lightDepth11, uv);
    else if(i==12)distance1 = texture(lightDepth12, uv);
    else if(i==13)distance1 = texture(lightDepth13, uv);
    else if(i==14)distance1 = texture(lightDepth14, uv);
    else if(i==15)distance1 = texture(lightDepth15, uv);
    else if(i==16)distance1 = texture(lightDepth16, uv);
    else if(i==17)distance1 = texture(lightDepth17, uv);
    else if(i==18)distance1 = texture(lightDepth18, uv);
    else if(i==19)distance1 = texture(lightDepth19, uv);
    else if(i==20)distance1 = texture(lightDepth20, uv);
    return distance1;
}
#define mPI (3.14159265)
#define mPI2 (2.0*3.14159265)
#define GOLDEN_RATIO (1.6180339)

float rand2s(vec2 co){
        return fract(sin(dot(co.xy,vec2(12.9898,78.233))) * 43758.5453);
}
float ArandsPointer = 0;
float LightingGetRand(){
    float r = rand2s(vec2(ArandsPointer, ArandsPointer*2.42354));
    ArandsPointer+=0.5432;
    return r;
}/*
float getBlurAmount(vec2 uv, uint i, float ainvd, float distance2){
    float AInv = 1.0 / ((ainvd) + 1.0);
    float average = 0.0;
    float counter = 0;
    float minval = 999;
    float maxval = 0;
    for(float x = 0; x < mPI2; x+=0.5){ 
        for(float y=0.01;y<1.0;y+= 0.3){  
            vec2 crd = vec2(sin(x + y), cos(x + y)) * rand2s(uv - vec2(x,y)) * AInv * 0.4;
            vec2 fakeUV = uv + crd;
            float bval = (lookupDepthFromLight(i, fakeUV, distance2));
            average += bval;
            counter+=1;
        }
    }
    if(counter == 0) return 0.0;
    float bbb = average/counter;
    return clamp(bbb, 0, 12);
}
*/
float LastProbeDistance = 0.0;
float rand2d(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}
float getShadowPercent(vec2 uv, vec3 pos, uint i){
    float accum = 0.0;
    
    ArandsPointer = float(ArandsPointer + rand2s(uv+Time) * 113.86786 );
    float distance2 = distance(pos, LightsPos[i]) + 0.1;
    
    mat4 lightPV = (LightsPs[i] * LightsVs[i]);
    mat4 bias = mat4( 0.5, 0.0, 0.0, 0.0,
         0.0, 0.5, 0.0, 0.0,
         0.0, 0.0, 0.5, 0.0,
         0.5, 0.5, 0.5, 1.0);
    vec4 lightClipSpace = (lightPV) * vec4(pos, 1.0);
    vec3 lightScreenSpace = lightClipSpace.xyz / lightClipSpace.w;

    float distance1 = 0.0;
    vec2 fakeUV = vec2(0.0);
    
    float counter = 0;
    float distance3 = (lightScreenSpace.z * 0.5 + 0.5);
    //return lookupDepthFromLight(i, uv) - distance3 > 0.000015 ? 0.0 : 1.0;
    float pssblur = 0;//max(0, (getBlurAmount(uv, i, distance2, distance3)) - 0.1) * 1.1;
    //return lookupDepthFromLight(i, uv, distance3 - 0.0001);
    for(float x = 0; x < mPI2; x+=0.8){ 
        for(float y=0.05;y<1.0;y+= 0.2 ){  
            fakeUV = uv + vec2(sin(x+y), cos(x+y)) * rand2s(uv + vec2(x,y)) * distance2 * 0.0005;
            accum += 1.0 - lookupDepthFromLight(i, fakeUV, distance3 - 0.0003);

            //if(distance3 -  distance1 > 0.000015) accum += 1.0 ;
            counter+=1;
        }
    }

    
    //LastProbeDistance = LastProbeDistance / counter;
    float rs = 1.0 - (accum / counter);
    return rs;//return smoothstep(0.0, 0.9, rs);
}
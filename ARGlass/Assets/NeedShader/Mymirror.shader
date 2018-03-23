// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Glass_Mirror" {  
    Properties {  
        _cubemap ("cubemap", Cube) = "_Skybox" {}  
        _cubemap_slider ("cubemap_slider", Range(0, 1)) = 1  
        _color ("color", Color) = (0.5,0.5,0.5,1)  
        _texture_diffuse ("texture_diffuse", 2D) = "white" {}  
        _alpha_slider ("_alpha_slider", Range(0, 1)) = 1  
    }  
    SubShader {  
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}  
  
        Blend SrcAlpha OneMinusSrcAlpha   
  
        Pass {  
//            Name "FORWARD"  
//           Tags {  
//                "LightMode"="ForwardBase"  
//            }  
              
              
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #define UNITY_PASS_FORWARDBASE  
            #include "UnityCG.cginc"  
            #include "AutoLight.cginc"  
 
            #include "UnityPBSLighting.cginc"  
            #include "UnityStandardBRDF.cginc"  
            #pragma multi_compile_fwdbase_fullshadows  
            //#pragma multi_compile_fog  
            //#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2   
            #pragma target 3.0  
           // uniform float4 _LightColor0;  
            uniform samplerCUBE _cubemap;  
            uniform float _cubemap_slider;  
            uniform float4 _color;  
            uniform sampler2D _texture_diffuse; uniform float4 _texture_diffuse_ST;  
            uniform float _alpha_slider;  
            struct VertexInput {  
                float4 vertex : POSITION;  
                float3 normal : NORMAL;  
                float2 texcoord0 : TEXCOORD0;  
            };  
            struct VertexOutput {  
                float4 pos : SV_POSITION;  
                float2 uv0 : TEXCOORD0;  
                float4 posWorld : TEXCOORD1;  
                float3 normalDir : TEXCOORD2;  
                LIGHTING_COORDS(3,4)  
                UNITY_FOG_COORDS(5)  
            };  
            VertexOutput vert (VertexInput v) {  
                VertexOutput o = (VertexOutput)0;  
                o.uv0 = v.texcoord0;  
                o.normalDir = UnityObjectToWorldNormal(v.normal);  
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);  
                float3 lightColor = _LightColor0.rgb;  
                o.pos = UnityObjectToClipPos(v.vertex );  
                UNITY_TRANSFER_FOG(o,o.pos);  
                TRANSFER_VERTEX_TO_FRAGMENT(o)  
                return o;  
            }  
            float4 frag(VertexOutput i) : COLOR {  
                i.normalDir = normalize(i.normalDir);  
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);  
                float3 normalDirection = i.normalDir;  
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );  
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);  
                float3 lightColor = _LightColor0.rgb;  
                float3 halfDirection = normalize(viewDirection+lightDirection);  
////// Lighting:  
                float attenuation = LIGHT_ATTENUATION(i);  
                float3 attenColor = attenuation * _LightColor0.rgb;  
                ////  
                float Pi = 3.141592654;  
                float InvPi = 0.31830988618;  
                /////  
///////// Gloss:  
                float gloss = 0.8;  
                float specPow = exp2( gloss * 10.0+1.0);  
  
                /////// GI Data:  
                UnityLight light;  
                #ifdef LIGHTMAP_OFF  
                    light.color = lightColor;  
                    light.dir = lightDirection;  
                    light.ndotl = LambertTerm (normalDirection, light.dir);  
                #else  
                    light.color = half3(0.f, 0.f, 0.f);  
                    light.ndotl = 0.0f;  
                    light.dir = half3(0.f, 0.f, 0.f);  
                #endif  
                UnityGIInput d;  
                d.light = light;  
                d.worldPos = i.posWorld.xyz;  
                d.worldViewDir = viewDirection;  
                d.atten = attenuation;  
                d.boxMax[0] = unity_SpecCube0_BoxMax;  
                d.boxMin[0] = unity_SpecCube0_BoxMin;  
                d.probePosition[0] = unity_SpecCube0_ProbePosition;  
                d.probeHDR[0] = unity_SpecCube0_HDR;  
                d.boxMax[1] = unity_SpecCube1_BoxMax;  
                d.boxMin[1] = unity_SpecCube1_BoxMin;  
                d.probePosition[1] = unity_SpecCube1_ProbePosition;  
                d.probeHDR[1] = unity_SpecCube1_HDR;  
                Unity_GlossyEnvironmentData ugls_en_data;  
                ugls_en_data.roughness = 1.0 - gloss;  
                ugls_en_data.reflUVW = viewReflectDirection;  
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );  
                lightDirection = gi.light.dir;  
                lightColor = gi.light.color;  
////// Specular:  
                float NdotL = max(0, dot( normalDirection, lightDirection ));  
                float LdotH = max(0.0,dot(lightDirection, halfDirection));  
                float3 specularColor = 0.0;  
                float specularMonochrome;  
                float node_5610 = 0.2;  
                float3 diffuseColor = float3(node_5610,node_5610,node_5610); // Need this for specular when using metallic  
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );  
                specularMonochrome = 1.0-specularMonochrome;  
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));  
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));  
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));  
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );  
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));  
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);  
                if (IsGammaSpace())  
                    specularPBL = sqrt(max(1e-4h, specularPBL));  
                specularPBL = max(0, specularPBL * NdotL);  
                float3 directSpecular = (floor(attenuation) * _LightColor0.rgb)*specularPBL*FresnelTerm(specularColor, LdotH);  
                half grazingTerm = saturate( gloss + specularMonochrome );  
                float3 indirectSpecular = (gi.indirect.specular);  
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);  
                float3 specular = (directSpecular + indirectSpecular);  
  
/////// Diffuse:  
                //float NdotL = max(0.0,dot( normalDirection, lightDirection ));  
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;  
                 float3 indirectDiffuse = float3(0,0,0);  
                indirectDiffuse *= UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light  
                float3 node_307 = (texCUBE(_cubemap,viewReflectDirection).rgb*_cubemap_slider);  
  
                //indirectDiffuse += float3(node_307,node_307,node_307); // Diffuse Ambient Light  
                indirectDiffuse += node_307;  
                float4 _texture_diffuse_var = tex2D(_texture_diffuse,i.uv0);  
  
                 diffuseColor = (_texture_diffuse_var.rgb);  
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;  
                //float3 diffuse = node_307 + diffuseColor;  
/// Final Color:  
                float3 finalColor = diffuse + specular;  
                //float3 finalColor = diffuseColor;  
                fixed4 finalRGBA = fixed4(finalColor,_alpha_slider);  
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);  
                return finalRGBA;  
            }  
            ENDCG  
        }  
         
  
    }  
    FallBack "Diffuse"  
}  
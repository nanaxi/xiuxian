// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4013,x:33008,y:32501,varname:node_4013,prsc:2|diff-2946-OUT,spec-9045-OUT,difocc-544-OUT;n:type:ShaderForge.SFN_Tex2d,id:5562,x:32568,y:32246,ptovrint:False,ptlb:node_5562,ptin:_node_5562,varname:node_5562,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:81fddb621898a8145b90362373b4a588,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2946,x:32835,y:32237,varname:node_2946,prsc:2|A-108-OUT,B-5562-RGB;n:type:ShaderForge.SFN_Slider,id:108,x:32519,y:32145,ptovrint:False,ptlb:node_108,ptin:_node_108,varname:node_108,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.275554,max:50;n:type:ShaderForge.SFN_Slider,id:6400,x:32431,y:33014,ptovrint:False,ptlb:node_6400,ptin:_node_6400,varname:node_6400,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.282051,max:50;n:type:ShaderForge.SFN_Slider,id:9045,x:32394,y:32692,ptovrint:False,ptlb:GAOGUANG,ptin:_GAOGUANG,varname:_node_6400_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.9433218,max:20;n:type:ShaderForge.SFN_LightAttenuation,id:5904,x:31848,y:32480,varname:node_5904,prsc:2;n:type:ShaderForge.SFN_Dot,id:9133,x:31848,y:32302,varname:node_9133,prsc:2,dt:4|A-9729-OUT,B-3620-OUT;n:type:ShaderForge.SFN_Multiply,id:1483,x:32080,y:32344,varname:node_1483,prsc:2|A-9133-OUT,B-5904-OUT;n:type:ShaderForge.SFN_NormalVector,id:9729,x:31569,y:32215,prsc:2,pt:False;n:type:ShaderForge.SFN_LightVector,id:3620,x:31588,y:32382,varname:node_3620,prsc:2;n:type:ShaderForge.SFN_Dot,id:3333,x:31706,y:32630,varname:node_3333,prsc:2,dt:1|A-3620-OUT,B-3666-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:3666,x:31506,y:32697,varname:node_3666,prsc:2;n:type:ShaderForge.SFN_Slider,id:2905,x:31385,y:32985,ptovrint:False,ptlb:node_2905,ptin:_node_2905,varname:node_2905,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Exp,id:4232,x:31760,y:32980,varname:node_4232,prsc:2,et:1|IN-2905-OUT;n:type:ShaderForge.SFN_Power,id:1574,x:31904,y:32765,varname:node_1574,prsc:2|VAL-3333-OUT,EXP-4232-OUT;n:type:ShaderForge.SFN_Slider,id:9826,x:31910,y:33069,ptovrint:False,ptlb:node_9826,ptin:_node_9826,varname:node_9826,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Multiply,id:8974,x:32137,y:32799,varname:node_8974,prsc:2|A-1574-OUT,B-9826-OUT;n:type:ShaderForge.SFN_Add,id:544,x:32316,y:32381,varname:node_544,prsc:2|A-1483-OUT,B-8974-OUT;proporder:5562-108-6400-9045-2905-9826;pass:END;sub:END;*/

Shader "Shader Forge/MJ" {
    Properties {
        _node_5562 ("node_5562", 2D) = "white" {}
        _node_108 ("node_108", Range(0, 50)) = 4.275554
        _node_6400 ("node_6400", Range(0, 50)) = 1.282051
        _GAOGUANG ("GAOGUANG", Range(0, 20)) = 0.9433218
        _node_2905 ("node_2905", Range(0, 1)) = 1
        _node_9826 ("node_9826", Range(0, 100)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform sampler2D _node_5562; uniform float4 _node_5562_ST;
            uniform float _node_108;
            uniform float _GAOGUANG;
            uniform float _node_2905;
            uniform float _node_9826;
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
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
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
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_GAOGUANG,_GAOGUANG,_GAOGUANG);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                indirectDiffuse *= ((0.5*dot(i.normalDir,lightDirection)+0.5*attenuation)+(pow(max(0,dot(lightDirection,viewReflectDirection)),exp2(_node_2905))*_node_9826)); // Diffuse AO
                float4 _node_5562_var = tex2D(_node_5562,TRANSFORM_TEX(i.uv0, _node_5562));
                float3 diffuseColor = (_node_108*_node_5562_var.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_5562; uniform float4 _node_5562_ST;
            uniform float _node_108;
            uniform float _GAOGUANG;
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
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_GAOGUANG,_GAOGUANG,_GAOGUANG);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _node_5562_var = tex2D(_node_5562,TRANSFORM_TEX(i.uv0, _node_5562));
                float3 diffuseColor = (_node_108*_node_5562_var.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}

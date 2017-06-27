// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created by FedericoArroyo //

Shader "EDE/ClothMasked" {
    Properties {
        _COLOR_TEX ("COLOR_TEX", 2D) = "white" {}
        _BaseCOLOR ("BaseCOLOR", Color) = (1,0,0,1)
        _FrnlEXP ("FrnlEXP", Float ) = 2
        [MaterialToggle] _USENM ("USENM", Float ) = 0
        _NM_TEX ("NM_TEX", 2D) = "bump" {}
        [MaterialToggle] _MULTBASECOLOR ("MULTBASECOLOR", Float ) = 0.3647059
        _IrradianceMAP ("IrradianceMAP", Cube) = "_Skybox" {}
        _IrrINT ("IrrINT", Float ) = 0.5
        _FresnelINT ("FresnelINT", Float ) = 0.5
        _LightINT ("LightINT", Float ) = 1
        [MaterialToggle] _Mask ("Mask", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _COLOR_TEX; uniform float4 _COLOR_TEX_ST;
            uniform float4 _BaseCOLOR;
            uniform float _FrnlEXP;
            uniform fixed _USENM;
            uniform sampler2D _NM_TEX; uniform float4 _NM_TEX_ST;
            uniform fixed _MULTBASECOLOR;
            uniform samplerCUBE _IrradianceMAP;
            uniform float _IrrINT;
            uniform float _FresnelINT;
            uniform float _LightINT;
            uniform fixed _Mask;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_561 = i.uv0;
                float3 normalLocal = lerp( float3(0,0,1), UnpackNormal(tex2D(_NM_TEX,TRANSFORM_TEX(node_561.rg, _NM_TEX))).rgb, _USENM );
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = 1;
////// Emissive:
                float4 node_2 = tex2D(_COLOR_TEX,TRANSFORM_TEX(node_561.rg, _COLOR_TEX));
                float4 node_89 = texCUBE(_IrradianceMAP,viewReflectDirection);
                float3 emissive = ((((lerp( node_2.rgb, (node_2.rgb*_BaseCOLOR.rgb), _MULTBASECOLOR )+(node_89.rgb*_IrrINT))+(node_2.rgb*clamp((dot(lightDirection,i.normalDir)*0.2),0,1)))*((_LightColor0.rgb*_LightINT)*attenuation))+((node_89.rgb*_FresnelINT)*pow(1.0-max(0,dot(normalDirection, viewDirection)),_FrnlEXP)));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,lerp( float3(1,1,1), node_2.a, _Mask ).r);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

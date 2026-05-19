Shader "Custom/NeonBlob"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0.2, 0.5, 1, 1)
        _GlowColor ("Glow Color", Color) = (0.4, 0.7, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2
        _FresnelPower ("Fresnel Power", Range(0.1, 5)) = 2
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.2
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;
                float4 _GlowColor;
                float _GlowIntensity;
                float _FresnelPower;
                float _PulseSpeed;
                float _PulseAmount;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // Pulse animation
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float3 positionOS = IN.positionOS.xyz * pulse;
                
                OUT.positionHCS = TransformObjectToHClip(positionOS);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                
                float3 positionWS = TransformObjectToWorld(positionOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                // Normalize vectors
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);
                
                // Fresnel effect for glow
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                
                // Pulsing glow
                float glowPulse = 1.0 + sin(_Time.y * _PulseSpeed * 2.0) * 0.3;
                
                // Combine colors
                float4 finalColor = _MainColor;
                finalColor.rgb += _GlowColor.rgb * fresnel * _GlowIntensity * glowPulse;
                
                // Distance from center for additional glow
                float distFromCenter = length(IN.uv - 0.5) * 2.0;
                float centerGlow = 1.0 - saturate(distFromCenter);
                finalColor.rgb += _GlowColor.rgb * centerGlow * 0.5;
                
                // Alpha based on distance from center (soft edges)
                finalColor.a = _MainColor.a * (1.0 - pow(distFromCenter, 2.0));
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Sprites/Default"
}

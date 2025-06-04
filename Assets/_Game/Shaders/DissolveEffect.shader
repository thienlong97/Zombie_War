Shader "Custom/DissolveEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _EdgeColor1 ("Edge Color 1", Color) = (1,1,1,1)
        _EdgeColor2 ("Edge Color 2", Color) = (1,1,1,1)
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _EdgeWidth ("Edge Width", Range(0, 0.2)) = 0.1
        _VerticalBias ("Vertical Bias", Range(0, 2)) = 1
        _NoiseScale ("Noise Scale", Vector) = (1,1,0,0)
        _UpwardForce ("Upward Force", Range(0, 5)) = 1
        _SandNoiseScale ("Sand Noise Scale", Range(0, 10)) = 2
        _DisplacementStrength ("Displacement Strength", Range(0, 2)) = 0.5
        _DisplacementSpeed ("Displacement Speed", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 originalWorldPos : TEXCOORD2;
                float dissolveHeight : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _NoiseScale;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _EdgeColor1)
                UNITY_DEFINE_INSTANCED_PROP(float4, _EdgeColor2)
                UNITY_DEFINE_INSTANCED_PROP(float, _DissolveAmount)
                UNITY_DEFINE_INSTANCED_PROP(float, _EdgeWidth)
                UNITY_DEFINE_INSTANCED_PROP(float, _VerticalBias)
                UNITY_DEFINE_INSTANCED_PROP(float, _UpwardForce)
                UNITY_DEFINE_INSTANCED_PROP(float, _SandNoiseScale)
                UNITY_DEFINE_INSTANCED_PROP(float, _DisplacementStrength)
                UNITY_DEFINE_INSTANCED_PROP(float, _DisplacementSpeed)
            UNITY_INSTANCING_BUFFER_END(Props)

            // Improved noise function for more natural sand movement
            float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 mod289(float4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float4 permute(float4 x) { return mod289(((x * 34.0) + 1.0) * x); }
            float4 taylorInvSqrt(float4 r) { return 1.79284291400159 - 0.85373472095314 * r; }

            float snoise(float3 v)
            {
                const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);
                const float4 D = float4(0.0, 0.5, 1.0, 2.0);

                // First corner
                float3 i = floor(v + dot(v, C.yyy));
                float3 x0 = v - i + dot(i, C.xxx);

                // Other corners
                float3 g = step(x0.yzx, x0.xyz);
                float3 l = 1.0 - g;
                float3 i1 = min(g.xyz, l.zxy);
                float3 i2 = max(g.xyz, l.zxy);

                float3 x1 = x0 - i1 + C.xxx;
                float3 x2 = x0 - i2 + C.yyy;
                float3 x3 = x0 - D.yyy;

                // Permutations
                i = mod289(i);
                float4 p = permute(permute(permute(
                    i.z + float4(0.0, i1.z, i2.z, 1.0))
                    + i.y + float4(0.0, i1.y, i2.y, 1.0))
                    + i.x + float4(0.0, i1.x, i2.x, 1.0));

                // Gradients
                float n_ = 0.142857142857;
                float3 ns = n_ * D.wyz - D.xzx;

                float4 j = p - 49.0 * floor(p * ns.z * ns.z);

                float4 x_ = floor(j * ns.z);
                float4 y_ = floor(j - 7.0 * x_);

                float4 x = x_ * ns.x + ns.yyyy;
                float4 y = y_ * ns.x + ns.yyyy;
                float4 h = 1.0 - abs(x) - abs(y);

                float4 b0 = float4(x.xy, y.xy);
                float4 b1 = float4(x.zw, y.zw);

                float4 s0 = floor(b0) * 2.0 + 1.0;
                float4 s1 = floor(b1) * 2.0 + 1.0;
                float4 sh = -step(h, float4(0, 0, 0, 0));

                float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
                float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

                float3 p0 = float3(a0.xy, h.x);
                float3 p1 = float3(a0.zw, h.y);
                float3 p2 = float3(a1.xy, h.z);
                float3 p3 = float3(a1.zw, h.w);

                // Normalise gradients
                float4 norm = taylorInvSqrt(float4(dot(p0, p0), dot(p1, p1), dot(p2, p2), dot(p3, p3)));
                p0 *= norm.x;
                p1 *= norm.y;
                p2 *= norm.z;
                p3 *= norm.w;

                // Mix final noise value
                float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
                m = m * m;
                return 42.0 * dot(m * m, float4(dot(p0, x0), dot(p1, x1), dot(p2, x2), dot(p3, x3)));
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.originalWorldPos = worldPos;

                // Get dissolve parameters
                float dissolveAmount = UNITY_ACCESS_INSTANCED_PROP(Props, _DissolveAmount);
                float upwardForce = UNITY_ACCESS_INSTANCED_PROP(Props, _UpwardForce);
                float sandNoiseScale = UNITY_ACCESS_INSTANCED_PROP(Props, _SandNoiseScale);
                float displacementStrength = UNITY_ACCESS_INSTANCED_PROP(Props, _DisplacementStrength);
                float displacementSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _DisplacementSpeed);
                float verticalBias = UNITY_ACCESS_INSTANCED_PROP(Props, _VerticalBias);

                // Calculate vertex displacement
                float dissolveHeight = worldPos.y + verticalBias;
                float heightFactor = saturate((dissolveHeight - dissolveAmount * 2.0) * 2.0);
                
                // Create sand movement effect
                float3 noiseInput = float3(
                    worldPos.x * sandNoiseScale + _Time.y * displacementSpeed,
                    worldPos.y * sandNoiseScale,
                    worldPos.z * sandNoiseScale + _Time.y * displacementSpeed * 0.7
                );
                
                float noise = snoise(noiseInput) * 0.5 + 0.5;
                
                // Calculate displacement
                float3 displacement = float3(
                    sin(_Time.y * displacementSpeed + worldPos.x) * noise * 0.3,
                    upwardForce * (1.0 - heightFactor) * dissolveAmount,
                    cos(_Time.y * displacementSpeed + worldPos.z) * noise * 0.3
                );
                
                // Apply displacement based on dissolve state
                worldPos.xyz += displacement * displacementStrength * (1.0 - heightFactor) * dissolveAmount;
                
                o.worldPos = worldPos;
                o.dissolveHeight = dissolveHeight;
                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                float2 noiseUV = i.uv * _NoiseScale.xy;
                float baseNoise = tex2D(_NoiseTex, noiseUV).r;
                
                // Create more detailed noise for sand effect
                float3 detailNoisePos = i.worldPos * UNITY_ACCESS_INSTANCED_PROP(Props, _SandNoiseScale);
                float detailNoise = snoise(detailNoisePos) * 0.5 + 0.5;
                
                // Combine noises
                float noise = (baseNoise + detailNoise) * 0.5;
                
                float dissolveAmount = UNITY_ACCESS_INSTANCED_PROP(Props, _DissolveAmount);
                float edgeWidth = UNITY_ACCESS_INSTANCED_PROP(Props, _EdgeWidth);
                
                // Calculate dissolve value with height factor
                float heightFactor = saturate((i.dissolveHeight - dissolveAmount * 2.0) * 2.0);
                float dissolveValue = noise - dissolveAmount + (1.0 - heightFactor) * 0.1;
                
                // Calculate edge effect
                float edge = 1 - saturate(dissolveValue / edgeWidth);
                
                // Get colors
                float4 edgeColor1 = UNITY_ACCESS_INSTANCED_PROP(Props, _EdgeColor1);
                float4 edgeColor2 = UNITY_ACCESS_INSTANCED_PROP(Props, _EdgeColor2);
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Apply edge color and sand effect
                fixed4 edgeColor = lerp(edgeColor1, edgeColor2, edge);
                col.rgb = lerp(col.rgb, edgeColor.rgb, edge * step(dissolveValue, edgeWidth));
                
                // Apply transparency
                col.a *= step(0, dissolveValue);
                
                // Add sand particles effect
                float particleNoise = snoise(i.worldPos * 5.0 + _Time.yyy) * 0.5 + 0.5;
                float sandParticle = step(0.97, particleNoise) * (1.0 - heightFactor) * dissolveAmount;
                col.rgb = lerp(col.rgb, edgeColor1.rgb, sandParticle);
                
                return col;
            }
            ENDCG
        }
    }
} 
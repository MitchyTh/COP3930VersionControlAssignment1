Shader "Lit/FireInObjectLit1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",Float)=1
        _ScrollDirect("Scroll Direction",vector)=(1,1,0.5,0) // scroll direction A is (x,y) // scroll direction B is (z,w)
        _FlameHeight("Flame Height",Float)=1
        _FireShapeNoiseScale("Fire Shape Noise Scale",vector)=(10,10,0,0) // x is scale noise 1, y is scale noise 2

        _BGColor("Background Color",Color)=(1,1,1,1)
        _BaseColor("Base Color",Color)=(1,1,1,1)
        _HighlightColor("Highlight Color",Color)=(1,1,1,1)
        _Brightness("Brightness",Float)=2
        _Alpha("Alpha",Float)=1
        _AlphaClip("Alpha Clip Threshold",Float)=0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        // Cull Off

        Pass
        {
            Tags{ "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #include "Lighting.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal   : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            //Gradient noise
            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            {
                Out = unity_gradientNoise(UV * Scale) + 0.5;
            }
            // remap
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            // voronoi 
            inline float2 unity_voronoi_noise_randomVector (float2 UV, float offset)
            {
                float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
                UV = frac(sin(mul(UV, m)) * 46839.32);
                return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
            }

            void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
            {
                float2 g = floor(UV * CellDensity);
                float2 f = frac(UV * CellDensity);
                float t = 8.0;
                float3 res = float3(8.0, 0.0, 0.0);

                for(int y=-1; y<=1; y++)
                {
                    for(int x=-1; x<=1; x++)
                    {
                        float2 lattice = float2(x,y);
                        float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
                        float d = distance(lattice + offset, f);
                        if(d < res.x)
                        {
                            res = float3(d, offset.x, offset.y);
                            Out = res.x;
                            Cells = res.y;
                        }
                    }
                }
            }
            // twirl
            void Unity_Twirl_float(float2 UV, float2 Center, float Strength, float2 Offset, out float2 Out)
            {
                float2 delta = UV - Center;
                float angle = Strength * length(delta);
                float x = cos(angle) * delta.x - sin(angle) * delta.y;
                float y = sin(angle) * delta.x + cos(angle) * delta.y;
                Out = float2(x + Center.x + Offset.x, y + Center.y + Offset.y);
            }
            //

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FlameHeight;
            float _Speed;
            float4 _ScrollDirect;
            float4 _FireShapeNoiseScale;
            float4 _HighlightColor;
            float4 _BaseColor;
            float4 _BGColor;
            float _Brightness;
            float _Alpha;
            float _AlphaClip;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal= normalize(v.normal); // object space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal); // world space to calculate light
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                // region make fire shape - by noise
                // control height of fire
                float heightOut=pow(1-i.uv.y,(1-_FlameHeight));
                // first scroll vector for fire shape noise
                float2 vectorScrollOutShapeFire=_Time.y*_Speed*_ScrollDirect.xy;
                // second make fire shape
                float fireShapeNoiseOut;
                Unity_GradientNoise_float(i.uv.xy+vectorScrollOutShapeFire,_FireShapeNoiseScale.x,fireShapeNoiseOut);
                // third make scroll vector for fire direction
                // twirl uv
                float2 twirlOut;
                Unity_Twirl_float(i.uv,float2(0.5,0.5),1,float2(0,0),twirlOut);
                float2 vectorScrollOutFireDirect=vectorScrollOutShapeFire*_ScrollDirect.zw;
                //another noise for angleoffset - plus noise of fire shape
                float angleOffsetNoise;
                Unity_GradientNoise_float(i.uv,_FireShapeNoiseScale.y,angleOffsetNoise);
                // remap angle offset noise to custom range
                float angleOffsetNoiseAfterRemap;
                Unity_Remap_float(angleOffsetNoise,float2(0,1),float2(2,5),angleOffsetNoiseAfterRemap);
                // fourth voronoi 
                float voronoiOut;
                float voronoiCell;
                Unity_Voronoi_float(twirlOut+vectorScrollOutFireDirect,angleOffsetNoiseAfterRemap,5,voronoiOut,voronoiCell);
                //final multi all element to make final fire shape have moving
                float fireShape=heightOut*fireShapeNoiseOut*voronoiOut;
                // make color
                // highlight Color
                float4 smoothStepForHighlightColor=smoothstep(0.25, 1, fireShape);
                float4 highlightColor=smoothStepForHighlightColor*_HighlightColor;
                // base color
                float4 smoothStepForBaseColor=smoothstep(0.1, 1, fireShape);
                float4 baseColor=smoothStepForBaseColor*_BaseColor;
                // background color
                float4 backgroundColor=_BGColor*fireShape;

                // plus and bright ness fire // maybe need change this
                float4 fireColorShapeFinal= max(float4(0,0,0,0),(highlightColor+baseColor+backgroundColor)*_Brightness);


                
                // region control alpha and vertex
                float4 alphaCalculateByNormalVector=1-(smoothstep(0.5,1,-normalize(i.normal).y)+smoothstep(0.5,1,normalize(i.normal).y));
                float finalAlpha=alphaCalculateByNormalVector.w*fireColorShapeFinal.w*_Alpha-_AlphaClip;
                

                // final fire shape and movement
                // fixed4 col = fixed4(fireColorShapeFinal.rgb,finalAlpha*finalAlpha);

                
                // final add affect of light to shader
                // Normalize normal
                half3 normalWorldSpace = normalize(i.worldNormal);
                // Light direction (main directional light)
                half3 lightDir = _WorldSpaceLightPos0.xyz;
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                // Diffuse term (Lambert)
                half NdotL = saturate(dot(normalWorldSpace, lightDir));
                half3 diffuse =fireColorShapeFinal.rgb*_LightColor0.rgb * NdotL;

                half4 finalCol=half4(diffuse, finalAlpha);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalCol);
                return finalCol;
            }
            ENDCG
        }
    }
}

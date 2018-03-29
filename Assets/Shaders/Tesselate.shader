Shader "Custom/Tesselate" 
{
	Properties
	{
		_Tess("Tessellation", Range(1,32)) = 4
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DispTex("Disp Texture", 2D) = "gray" {}
		_NormalMap("Normalmap", 2D) = "bump" {}
		_Displacement("Displacement", Range(0, 1.0)) = 0.3
		_Color("Color", color) = (1,1,1,0)
		_SpecColor("Spec color", color) = (0.5,0.5,0.5,0.5)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" "LightMode" = "ForwardBase" }
		LOD 300

		CGPROGRAM
		#pragma fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
		#pragma target 4.6
		#include "Tessellation.cginc"
		#include "UnityCG.cginc"
		#pragma geometry geom
		#pragma fragment frag

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		float _Tess;

		float4 tessDistance(appdata v0, appdata v1, appdata v2)
		{
			float minDist = 10.0;
			float maxDist = 25.0;
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
		}

		sampler2D _DispTex;
		float _Displacement;

		/*void disp(inout appdata v)
		{
			float d = tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r * _Displacement;
			v.vertex.xyz += v.normal * d;
		}*/

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _NormalMap;
		fixed4 _Color;

		struct v2g
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 vertex : TEXCOORD1;
		};

		struct g2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float light : TEXCOORD1;
		};

		v2g disp(appdata_full v)
		{
			float d = tex2Dlod(_DispTex, float4(v.texcoord.xy, 0, 0)).r * _Displacement;
			v.vertex.xyz += v.normal * d;
			v2g o;
			o.vertex = v.vertex;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		[maxvertexcount(3)]
		void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
		{
			g2f o;

			// Compute the normal
			float3 vecA = IN[1].vertex - IN[0].vertex;
			float3 vecB = IN[2].vertex - IN[0].vertex;
			float3 normal = cross(vecA, vecB);
			normal = normalize(mul(normal, (float3x3) unity_WorldToObject));

			// Compute diffuse light
			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
			o.light = max(0., dot(normal, lightDir));

			// Compute barycentric uv
			o.uv = (IN[0].uv + IN[1].uv + IN[2].uv) / 3;

			for (int i = 0; i < 3; i++)
			{
				o.pos = IN[i].pos;
				triStream.Append(o);
			}
		}

		half4 frag(g2f i) : COLOR
		{
			float4 col = tex2D(_MainTex, i.uv);
			col.rgb *= i.light * _Color;
			return col;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
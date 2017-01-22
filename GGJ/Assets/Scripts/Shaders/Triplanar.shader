Shader "Unlit/Triplanar"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1.0
	}
		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc" // for _LightColor0

		struct v2f
	{
		half3 objNormal : TEXCOORD0;
		float3 coords : TEXCOORD1;
		float2 uv : TEXCOORD2;
		float4 pos : SV_POSITION;
		fixed4 diff : COLOR0; // diffuse lighting color
	};

	float _Tiling;

	v2f vert(float4 pos : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(pos);
		o.coords = pos.xyz * _Tiling;
		o.objNormal = normal;
		o.uv = uv;
		half3 worldNormal = UnityObjectToWorldNormal(o.objNormal);
		half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
		o.diff = nl * _LightColor0;
		return o;
	}

	sampler2D _MainTex;
	sampler2D _NormalMap;

	fixed4 frag(v2f i) : SV_Target
	{
		half3 tnormal = UnpackNormal(tex2D(_NormalMap, i.uv));
		// use absolute value of normal as texture weights
		half3 blend = abs(i.objNormal);
		// make sure the weights sum up to 1 (divide by sum of x+y+z)
		blend /= dot(blend,1.0);
		// read the three texture projections, for x,y,z axes
		fixed4 cx = tex2D(_MainTex, i.coords.yz);
		fixed4 cy = tex2D(_MainTex, i.coords.xz);
		fixed4 cz = tex2D(_MainTex, i.coords.xy);
		// blend the textures based on weights
		fixed4 c = cx * blend.x + cy * blend.y + cz * blend.z;
		//c *= i.diff;
		return c;
	}
		ENDCG
	}
	}
}
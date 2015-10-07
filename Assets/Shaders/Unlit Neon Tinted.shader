// Unlit opaque shader.
// - no lighting
// - no lightmap support
// - no alpha
// - supports tint color

Shader "Unlit/Neon Tinted" {
	Properties {
      	_MainTex ( "Main Texture (RGBA)", 2D ) = "white" {}
		_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	}
   	
	SubShader {
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }
		LOD 100
		ZWrite On Fog { Mode Off }	
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"	// --> /Applications/Unity/Unity.app/Contents/CGIncludes/UnityCG.cginc
	
			sampler2D _MainTex;
		    float4 _Color;
					
			v2f_img vert( appdata_img v ) {
				v2f_img o;
				o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
				o.uv = v.texcoord;
				return o;		    
			}
			
			float4 frag( v2f_img i ) : COLOR {
			    return 2.0 * tex2D( _MainTex, i.uv ) * _Color;
			}
			ENDCG
		}
	}	
	
}


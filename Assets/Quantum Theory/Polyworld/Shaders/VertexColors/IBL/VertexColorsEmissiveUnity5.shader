// Copyright © 2014 Laurens Mathot
// Code Animo™ http://codeanimo.com
// License terms can be found at the bottom of this file.

Shader "QuantumTheory/VertexColors/Unity5/Emissive" {
	Properties {		
		_Color ("Emissive Color", Color) = (1,1,1,0)
		_EMISSION("Emission Scale", Float) = 10
	}
		
	CGINCLUDE

	float _EMISSION;
	
	float4 _Color;
	
	ENDCG
	
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		// Surface Shaders Passes:
		CGPROGRAM
		#pragma surface surf Lambert addshadow
		#pragma target 3.0
		#pragma glsl
	
		
		struct Input {
			float4 color : COLOR;
			
		};
	
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.color;			
			float3 emission = _Color * _EMISSION*(IN.color.a);
			o.Emission = emission;
		}
		
		ENDCG
		
		
		// Vertex Lit Path Pass:
		Pass {
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 2.0
			//#pragma glsl
			
			#define QT_EMISSION_ENABLED
			
			#include "UnityCG.cginc"
			#include "VertexLitIBL.cginc"
			
			ENDCG
		}
		// Vertex Lit Lightmapped:
		Pass {
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0
			//#pragma glsl
			
			#define QT_EMISSION_ENABLED
			#define QT_VERTEX_LIT_LIGHTMAP
			
			#include "UnityCG.cginc"
			#include "VertexLitIBL.cginc"
			
			
			ENDCG
		}
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
			
			#define QT_EMISSION_ENABLED
			#define QT_VERTEX_LIT_LIGHTMAP
			
			#include "UnityCG.cginc"
			#include "VertexLitIBL.cginc"
			
			
			ENDCG
		}
	}
	Fallback "QuantumTheory/VertexColors/VertexLit"
	
}

// Copyright © 2014 Laurens Mathot
// Code Animo™ http://codeanimo.com
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
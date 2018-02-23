/**
*  Copyright (C) 2018 3D Repo Ltd
*
*  This program is free software: you can redistribute it and/or modify
*  it under the terms of the GNU Affero General Public License as
*  published by the Free Software Foundation, either version 3 of the
*  License, or (at your option) any later version.
*
*  This program is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU Affero General Public License for more details.
*
*  You should have received a copy of the GNU Affero General Public License
*  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

Shader "3DRepo/StandardTransparent" {
	Properties{
		_ColorMapTex("SubMesh Diffuse Colour Texture", 2D) = "Gray" {}
		_MatPropMapTex("SubMesh Properties Texture", 2D) = "black" {}
		_MapWidth("Multipart Mapping Width", Float) = 0
		_MapHeight("Multipart Mapping Height", Float) = 0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		//LOD 200			

		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MatPropMapTex;
		sampler2D _ColorMapTex;
		float _MapWidth;
		float _MapHeight;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
			float2 uv2_MpHLTex;
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {			
			int index = round(IN.uv2_MpHLTex.y);
			int height = round(_MapHeight);
			uint width = round(_MapWidth);
			float row = index / height;
			float col = index - (row * height);

			float2 uvMap = float2(row / _MapWidth + 0.5 / _MapWidth, col / _MapHeight + 0.5 / _MapHeight);
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * tex2D(_ColorMapTex, uvMap);

			o.Albedo = c.rgb;				
			o.Alpha = c.a;

			o.Smoothness = tex2D(_MatPropMapTex, uvMap).r;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;

		}
		ENDCG
		
	}
	//FallBack "Diffuse"
}

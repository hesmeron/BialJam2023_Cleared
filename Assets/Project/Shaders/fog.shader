// Made with Amplify Shader Editor v1.9.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "fog"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_MainColor("MainColor", Color) = (0,0.1115615,1,0)
		_FogColor("FogColor", Color) = (0,0,0,0)
		_Length("Length", Float) = 0
		_Offset("Offset", Float) = 0
		_Vertical("Vertical", Float) = 0
		_VerticalOffset("VerticalOffset", Float) = 0
		_MaxShadow("MaxShadow", Range( 0 , 1)) = 0.5
		_GrayOut("_GrayOut", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float customSurfaceDepth6;
			float3 worldPos;
		};

		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform float _GrayOut;
		uniform float4 _MainColor;
		uniform float _Length;
		uniform float _Offset;
		uniform float _VerticalOffset;
		uniform float _Vertical;
		uniform float _MaxShadow;
		uniform float4 _FogColor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 customSurfaceDepth6 = ase_vertex3Pos;
			o.customSurfaceDepth6 = -UnityObjectToViewPos( customSurfaceDepth6 ).z;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTexture, uv_MainTexture );
			float grayscale42 = Luminance(tex2DNode1.rgb);
			float cameraDepthFade6 = (( i.customSurfaceDepth6 -_ProjectionParams.y - _Offset ) / _Length);
			float temp_output_15_0 = saturate( cameraDepthFade6 );
			float3 ase_worldPos = i.worldPos;
			float4 matrixToPos28 = float4( UNITY_MATRIX_M[0][3],UNITY_MATRIX_M[1][3],UNITY_MATRIX_M[2][3],UNITY_MATRIX_M[3][3]);
			float temp_output_35_0 = ( ( ( _VerticalOffset + ( ase_worldPos.y - matrixToPos28.y ) ) / _Vertical ) + _MaxShadow );
			o.Emission = ( ( grayscale42 * _GrayOut ) + ( ( 1.0 - _GrayOut ) * ( ( ( ( tex2DNode1 * _MainColor ) * ( 1.0 - temp_output_15_0 ) ) * saturate( pow( temp_output_35_0 , 2.0 ) ) ) + ( _FogColor * temp_output_15_0 ) ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19100
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;646,-190;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;fog;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.PosVertexDataNode;7;-841.6001,380.9;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-436.6238,-290.8244;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-924.6238,-456.8244;Inherit;True;Property;_MainTexture;MainTexture;0;0;Create;True;0;0;0;False;0;False;-1;None;b2be9c55e7e7ba447967677c82b2cb23;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-878.6238,-237.8244;Inherit;False;Property;_MainColor;MainColor;1;0;Create;True;0;0;0;False;0;False;0,0.1115615,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-755.6238,-94.02444;Inherit;False;Property;_FogColor;FogColor;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;10;-408.6238,-181.0245;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;6;-700.0569,132.5535;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;100;False;1;FLOAT;0.38;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-888.0569,132.9535;Inherit;False;Property;_Length;Length;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-890.0569,227.9535;Inherit;False;Property;_Offset;Offset;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;15;-398.109,162.5164;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-240.0569,36.3535;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-337.0775,542.6954;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-442.6777,429.7354;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;26;-1092.99,292.321;Inherit;False;Property;_DarkeningVector;DarkeningVector;7;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;23;-629.8777,322.5353;Inherit;False;Property;_VerticalOffset;VerticalOffset;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosFromTransformMatrix;28;-1114.99,655.921;Inherit;False;1;0;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;22;-1016.878,498.1754;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.MMatrixNode;31;-1253.99,671.921;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;32;-687.99,534.921;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;18;259.8719,213.4038;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;25;-1.566034,475.8421;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-343.1094,673.2153;Inherit;False;Property;_Vertical;Vertical;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-602.285,843.1548;Inherit;False;Property;_MaxShadow;MaxShadow;8;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-77.92142,753.501;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;38;-82.88423,1095.577;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;37;-230.103,948.359;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;36;147.0418,913.6212;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;153.6595,723.3954;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;319.0736,935.1258;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-88.8382,-699.4973;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;42;-411.3961,-722.6554;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-449.4416,-590.3246;Inherit;False;Property;_GrayOut;_GrayOut;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-170.2857,-410.9824;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;113.8624,-296.7917;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;297.7991,-212.3692;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;44;83.19275,-512.5799;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;323.0448,-393.4815;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;516.5793,-539.0463;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;0;2;46;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;10;0;15;0
WireConnection;6;2;7;0
WireConnection;6;0;12;0
WireConnection;6;1;13;0
WireConnection;15;0;6;0
WireConnection;9;0;8;0
WireConnection;9;1;15;0
WireConnection;20;0;24;0
WireConnection;20;1;21;0
WireConnection;24;0;23;0
WireConnection;24;1;32;0
WireConnection;28;0;31;0
WireConnection;32;0;22;2
WireConnection;32;1;28;2
WireConnection;18;0;25;0
WireConnection;25;0;35;0
WireConnection;35;0;20;0
WireConnection;35;1;34;0
WireConnection;36;0;37;0
WireConnection;36;1;38;0
WireConnection;39;0;35;0
WireConnection;39;1;40;0
WireConnection;40;0;36;0
WireConnection;43;0;42;0
WireConnection;43;1;41;0
WireConnection;42;0;1;0
WireConnection;4;0;2;0
WireConnection;4;1;10;0
WireConnection;19;0;4;0
WireConnection;19;1;18;0
WireConnection;16;0;19;0
WireConnection;16;1;9;0
WireConnection;44;0;41;0
WireConnection;45;0;44;0
WireConnection;45;1;16;0
WireConnection;46;0;43;0
WireConnection;46;1;45;0
ASEEND*/
//CHKSM=E0AAA555414EA323C8FF3B884F6F054A290E914C
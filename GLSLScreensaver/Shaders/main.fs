#ifdef GL_ES
precision mediump float;
#endif

uniform float time;
uniform vec2 mouse;
uniform vec2 resolution;


void main() 
{
	vec2 uv = ( gl_FragCoord.xy / resolution.xy ) * 2.0 - 1.0;
	uv.x *= resolution.x/resolution.y;
	
	vec3 finalColor = vec3( 0.0, 0.0, 0.0 );
	
	float g = -mod( gl_FragCoord.y + time, sin( gl_FragCoord.x ) + 0.005 );
	g = g + clamp(uv.y, -0.3, 0.0);	
	
	finalColor = vec3(0, g, 0);
	
	gl_FragColor = vec4( finalColor, 1.0 );

}
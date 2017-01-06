#ifdef GL_ES
precision mediump float;
#endif

#extension GL_OES_standard_derivatives : enable

uniform float time;
uniform vec2 mouse;
uniform vec2 resolution;

#define iResolution resolution
#define iMouse mouse
#define iGlobalTime time
bool iChannel0 = false;
vec4 textureBox(bool iChannel0, vec3 pos, vec3 nor){return vec4(0.5);}

// Created by inigo quilez - iq/2016
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.


// Antialiasing level. Make it 1 if you have a slow machine
#define AA 2


vec3 map( vec3 p )
{
	float scale = 1.0;
    
    float orb = 10000.0;

    for( int i=0; i<6; i++ )
	{
		p = -1.0 + 2.0*fract(0.5*p+0.5);

        p -= sign(p)*0.04; // trick
        
        float r2 = dot(p,p);
		float k = 0.95/r2;
		p     *= k;
		scale *= k;

        orb = min( orb, r2);
	}

    float d1 = sqrt( min( min( dot(p.xy,p.xy), dot(p.yz,p.yz) ), dot(p.zx,p.zx) ) ) - 0.02;
    float d2 = abs(p.y);
    float dmi = d2;
    float adr = 0.7*floor((0.5*p.y+0.5)*8.0);
    if( d1<d2 )
    {
        dmi = d1;
        adr = 0.0;
    }
    return vec3( 0.5*dmi/scale, adr, orb );
}

vec3 trace( in vec3 ro, in vec3 rd )
{
	float maxd = 20.0;
    float t = 0.01;
    vec2  info = vec2(0.0);
    for( int i=0; i<256; i++ )
    {
	    float precis = 0.001*t;
        
        vec3  r = map( ro+rd*t );
	    float h = r.x;
        info = r.yz;
        if( h<precis||t>maxd ) break;
        t += h;
    }

    if( t>maxd ) t=-1.0;
    return vec3( t, info );
}

vec3 calcNormal( in vec3 pos, in float t )
{
    float precis = 0.0001 * t * 0.57;

    vec2 e = vec2(1.0,-1.0)*precis;
    return normalize( e.xyy*map( pos + e.xyy ).x + 
					  e.yyx*map( pos + e.yyx ).x + 
					  e.yxy*map( pos + e.yxy ).x + 
                      e.xxx*map( pos + e.xxx ).x );
}

vec3 forwardSF( float i, float n) 
{
    const float PI  = 3.141592653589793238;
    const float PHI = 1.618033988749894848;
    float phi = 2.0*PI*fract(i/PHI);
    float zi = 1.0 - (2.0*i+1.0)/n;
    float sinTheta = sqrt( 1.0 - zi*zi);
    return vec3( cos(phi)*sinTheta, sin(phi)*sinTheta, zi);
}

float calcAO( in vec3 pos, in vec3 nor )
{
	float ao = 0.0;
    for( int i=0; i<16; i++ )
    {
        vec3 w = forwardSF( float(i), 16.0 );
		w *= sign( dot(w,nor) );
        float h = float(i)/15.0;
        ao += clamp( map( pos + nor*0.01 + w*h*0.15 ).x*2.0, 0.0, 1.0 );
    }
	ao /= 16.0;
	
    return clamp( ao*16.0, 0.0, 1.0 );
}


vec3 textureBox( sampler2D sam, in vec3 pos, in vec3 nor )
{
    vec3 w = nor*nor;
    return (w.x*texture2D( sam, pos.yz ).xyz + 
            w.y*texture2D( sam, pos.zx ).xyz + 
            w.z*texture2D( sam, pos.xy ).xyz ) / (w.x+w.y+w.z);
}

vec3 render( in vec3 ro, in vec3 rd )
{
    vec3 col = vec3(0.0);
    vec3 res = trace( ro, rd );;
    float t = res.x;
    if( t>0.0 )
    {
        vec3  pos = ro + t*rd;
        vec3  nor = calcNormal( pos, t );
        float fre = clamp(1.0+dot(rd,nor),0.0,1.0);
        float occ = pow( clamp(res.z*2.0,0.0,1.0), 1.2 );
              occ = 1.5*(0.1+0.9*occ)*calcAO(pos,nor);        
        vec3  lin = vec3(1.0,1.0,1.5)*(2.0+fre*fre*vec3(1.8,1.0,1.0))*occ*(1.0-0.5*abs(nor.y));
        
      	col = 0.5 + 0.5*cos( 6.2831*res.y + vec3(0.0,1.0,2.0) );  
        col *= textureBox( iChannel0, pos, nor ).xyz;
        col = col*lin;
        col += 0.6*pow(1.0-fre,32.0)*occ*vec3(0.5,1.0,1.5);        
        col *= exp(-0.3*t);
    }
    col.z += 0.01;

    return sqrt(col);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    float time = iGlobalTime*0.15 + 0.005*iMouse.x;
    
    vec3 tot = vec3(0.0);
    #if AA>1
    for( int jj=0; jj<AA; jj++ )
    for( int ii=0; ii<AA; ii++ )
    #else
    int ii = 0, jj = 0;
    #endif
    {
        vec2 q = fragCoord.xy+vec2(float(ii),float(jj))/float(AA);
        vec2 p = (2.0*q-iResolution.xy)/iResolution.y;

        // camera
        vec3 ro = vec3( 2.8*cos(0.1+.33*time), 0.5 + 0.20*cos(0.37*time), 2.8*cos(0.5+0.35*time) );
        vec3 ta = vec3( 1.9*cos(1.2+.41*time), 0.5 + 0.10*cos(0.27*time), 1.9*cos(2.0+0.38*time) );
        float roll = 0.2*cos(0.1*time);
        vec3 cw = normalize(ta-ro);
        vec3 cp = vec3(sin(roll), cos(roll),0.0);
        vec3 cu = normalize(cross(cw,cp));
        vec3 cv = normalize(cross(cu,cw));
        vec3 rd = normalize( p.x*cu + p.y*cv + 2.0*cw );

        tot += render( ro, rd );
    }
    
    tot = tot/float(AA*AA);
    
	fragColor = vec4( tot, 1.0 );	

}

void mainVR( out vec4 fragColor, in vec2 fragCoord, in vec3 fragRayOri, in vec3 fragRayDir )
{
    vec3 col = render( fragRayOri + vec3(0.82,1.3,-0.3), fragRayDir );
    fragColor = vec4( col, 1.0 );
}




void main( void ) {
	gl_FragColor = vec4( .0 );
	//mainVR(gl_FragColor, gl_FragCoord.xy, vec3(1), vec3(1,1,0));
	mainImage(gl_FragColor, gl_FragCoord.xy);
}
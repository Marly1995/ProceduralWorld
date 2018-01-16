// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ShaderToy/Cloud"{
	Properties{
	}
	SubShader{

		Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float4 screenCoord : TEXCOORD1;
				};

				v2f vert(appdata v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.screenCoord.xy = ComputeScreenPos(o.vertex);
					return o;
				};

				fixed4 frag(v2f i) : SV_Target{
				}
	void mainImage(out(float4) fragColor, in(float2) fragCoord);
	float4 main(float4 uv : SV_Position) : SV_Target{ float4 col; mainImage(col, uv.xy); return col; }
				};

#if defined(__cplusplus) || defined(SHADERTOY)
#define u_res _ScreenParams
#define u_time iTime
#define u_mouse iMouse
#endif

#ifdef GLSLSANDBOX
	uniform float time;
	uniform float2 mouse;
	uniform float2 resolution;
#define u_res resolution
#define u_time time
#define u_mouse mouse
	void mainImage(_out(float4) fragColor, _in(float2) fragCoord);
	void main() { mainImage(gl_FragColor, gl_FragCoord.xy); }
#endif

#ifdef UE4
	_constant(float2) u_res = float2(0, 0);
	_constant(float2) u_mouse = float2(0, 0);
	_mutable(float) u_time = 0;
#endif

#define PI 3.14159265359

	struct ray_t {
		float3 origin;
		float3 direction;
	};
#define BIAS 1e-4 // small offset to avoid self-intersections

	struct sphere_t {
		float3 origin;
		float radius;
		int material;
	};

	struct plane_t {
		float3 direction;
		float distance;
		int material;
	};

	struct hit_t {
		float t;
		int material_id;
		float3 normal;
		float3 origin;
	};
#define max_dist 1e8
	_constant(hit_t) no_hit = _begin(hit_t)
		float(max_dist + 1e1), // 'infinite' distance
		-1, // material id
		float3(0., 0., 0.), // normal
		float3(0., 0., 0.) // origin
		_end;

	// ----------------------------------------------------------------------------
	// Various 3D utilities functions
	// ----------------------------------------------------------------------------

	ray_t get_primary_ray(
		_in(float3) cam_local_point,
		_inout(float3) cam_origin,
		_inout(float3) cam_look_at
	) {
		float3 fwd = normalize(cam_look_at - cam_origin);
		float3 up = float3(0, 1, 0);
		float3 right = cross(up, fwd);
		up = cross(fwd, right);

		ray_t r = _begin(ray_t)
			cam_origin,
			normalize(fwd + up * cam_local_point.y + right * cam_local_point.x)
			_end;
		return r;
	}

	_constant(mat3) mat3_ident = mat3(1, 0, 0, 0, 1, 0, 0, 0, 1);


	mat2 rotate_2d(
		_in(float) angle_degrees
	) {
		float angle = radians(angle_degrees);
		float _sin = sin(angle);
		float _cos = cos(angle);
		return mat2(_cos, -_sin, _sin, _cos);
	}

	mat3 rotate_around_z(
		_in(float) angle_degrees
	) {
		float angle = radians(angle_degrees);
		float _sin = sin(angle);
		float _cos = cos(angle);
		return mat3(_cos, -_sin, 0, _sin, _cos, 0, 0, 0, 1);
	}

	mat3 rotate_around_y(
		_in(float) angle_degrees
	) {
		float angle = radians(angle_degrees);
		float _sin = sin(angle);
		float _cos = cos(angle);
		return mat3(_cos, 0, _sin, 0, 1, 0, -_sin, 0, _cos);
	}

	mat3 rotate_around_x(
		_in(float) angle_degrees
	) {
		float angle = radians(angle_degrees);
		float _sin = sin(angle);
		float _cos = cos(angle);
		return mat3(1, 0, 0, 0, _cos, -_sin, 0, _sin, _cos);
	}

	// http://http.developer.nvidia.com/GPUGems3/gpugems3_ch24.html
	float3 linear_to_srgb(
		_in(float3) color
	) {
		const float p = 1. / 2.2;
		return float3(pow(color.r, p), pow(color.g, p), pow(color.b, p));
	}
	float3 srgb_to_linear(
		_in(float3) color
	) {
		const float p = 2.2;
		return float3(pow(color.r, p), pow(color.g, p), pow(color.b, p));
	}

#ifdef __cplusplus
	float3 faceforward(
		_in(float3) N,
		_in(float3) I,
		_in(float3) Nref
	) {
		return dot(Nref, I) < 0 ? N : -N;
	}
#endif

	float checkboard_pattern(
		_in(float2) pos,
		_in(float) scale
	) {
		float2 pattern = floor(pos * scale);
		return mod(pattern.x + pattern.y, 2.0);
	}

	float band(
		_in(float) start,
		_in(float) peak,
		_in(float) end,
		_in(float) t
	) {
		return
			smoothstep(start, peak, t) *
			(1. - smoothstep(peak, end, t));
	}

	// from https://www.shadertoy.com/view/4sSSW3
	// original http://orbit.dtu.dk/fedora/objects/orbit:113874/datastreams/file_75b66578-222e-4c7d-abdf-f7e255100209/content
	void fast_orthonormal_basis(
		_in(float3) n,
		_out(float3) f,
		_out(float3) r
	) {
		float a = 1. / (1. + n.z);
		float b = -n.x*n.y*a;
		f = float3(1. - n.x*n.x*a, b, -n.x);
		r = float3(b, 1. - n.y*n.y*a, -n.y);
	}

	// ----------------------------------------------------------------------------
	// Analytical surface-ray intersection routines
	// ----------------------------------------------------------------------------

	// geometrical solution
	// info: http://www.scratchapixel.com/old/lessons/3d-basic-lessons/lesson-7-intersecting-simple-shapes/ray-sphere-intersection/
	void intersect_sphere(
		_in(ray_t) ray,
		_in(sphere_t) sphere,
		_inout(hit_t) hit
	) {
		float3 rc = sphere.origin - ray.origin;
		float radius2 = sphere.radius * sphere.radius;
		float tca = dot(rc, ray.direction);
		if (tca < 0.) return;

		float d2 = dot(rc, rc) - tca * tca;
		if (d2 > radius2) return;

		float thc = sqrt(radius2 - d2);
		float t0 = tca - thc;
		float t1 = tca + thc;

		if (t0 < 0.) t0 = t1;
		if (t0 > hit.t) return;

		float3 impact = ray.origin + ray.direction * t0;

		hit.t = t0;
		hit.material_id = sphere.material;
		hit.origin = impact;
		hit.normal = (impact - sphere.origin) / sphere.radius;
	}


	// ----------------------------------------------------------------------------
	// Volumetric utilities
	// ----------------------------------------------------------------------------

	struct volume_sampler_t {
		float3 origin; // start of ray
		float3 pos; // current pos of acccumulation ray
		float height;

		float coeff_absorb;
		float T; // transmitance

		float3 C; // color
		float alpha;
	};

	volume_sampler_t begin_volume(
		_in(float3) origin,
		_in(float) coeff_absorb
	) {
		volume_sampler_t v = _begin(volume_sampler_t)
			origin, origin, 0.,
			coeff_absorb, 1.,
			float3(0., 0., 0.), 0.
			_end;
		return v;
	}

	float illuminate_volume(
		_inout(volume_sampler_t) vol,
		_in(float3) V,
		_in(float3) L
	);

	void integrate_volume(
		_inout(volume_sampler_t) vol,
		_in(float3) V,
		_in(float3) L,
		_in(float) density,
		_in(float) dt
	) {
		// change in transmittance (follows Beer-Lambert law)
		float T_i = exp(-vol.coeff_absorb * density * dt);
		// Update accumulated transmittance
		vol.T *= T_i;
		// integrate output radiance (here essentially color)
		vol.C += vol.T * illuminate_volume(vol, V, L) * density * dt;
		// accumulate opacity
		vol.alpha += (1. - T_i) * (1. - vol.alpha);
	}


	// ----------------------------------------------------------------------------
	// Noise function by iq from https://www.shadertoy.com/view/4sfGzS
	// ----------------------------------------------------------------------------

	float hash(
		_in(float) n
	) {
		return fract(sin(n)*753.5453123);
	}

	float noise_iq(
		_in(float3) x
	) {
		float3 p = floor(x);
		float3 f = fract(x);
		f = f*f*(3.0 - 2.0*f);

#if 1
		float n = p.x + p.y*157.0 + 113.0*p.z;
		return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
			lerp(hash(n + 157.0), hash(n + 158.0), f.x), f.y),
			lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
				lerp(hash(n + 270.0), hash(n + 271.0), f.x), f.y), f.z);
#else
		float2 uv = (p.xy + float2(37.0, 17.0)*p.z) + f.xy;
		float2 rg = textureLod(iChannel0, (uv + .5) / 256., 0.).yx;
		return lerp(rg.x, rg.y, f.z);
#endif
	}

#define noise(x) noise_iq(x)

	// ----------------------------------------------------------------------------
	// Fractional Brownian Motion
	// depends on custom basis function
	// ----------------------------------------------------------------------------

#define DECL_FBM_FUNC(_name, _octaves, _basis) float _name(_in(float3) pos, _in(float) lacunarity, _in(float) init_gain, _in(float) gain) { float3 p = pos; float H = init_gain; float t = 0.; for (int i = 0; i < _octaves; i++) { t += _basis * H; p *= lacunarity; H *= gain; } return t; }

	DECL_FBM_FUNC(fbm, 4, noise(p))

		// ----------------------------------------------------------------------------
		// Planet
		// ----------------------------------------------------------------------------
		_constant(sphere_t) planet = _begin(sphere_t)
		float3(0, 0, 0), 1., 0
		_end;

#define max_height .4
#define max_ray_dist (max_height * 4.)

	float3 background(
		_in(ray_t) eye
	) {
#if 0
		return float3(.15, .3, .4);
#else
		_constant(float3) sun_color = float3(1., .9, .55);
		float sun_amount = dot(eye.direction, float3(0, 0, 1));

		float3 sky = lerp(
			float3(.0, .05, .2),
			float3(.15, .3, .4),
			1.0 - eye.direction.y);
		sky += sun_color * min(pow(sun_amount, 30.0) * 5.0, 1.0);
		sky += sun_color * min(pow(sun_amount, 10.0) * .6, 1.0);

		return sky;
#endif
	}

	void setup_scene()
	{
	}

	void setup_camera(
		_inout(float3) eye,
		_inout(float3) look_at
	) {
#if 0
		eye = float3(.0, 0, -1.93);
		look_at = float3(-.1, .9, 2);
#else
		eye = float3(0, 0, -2.5);
		look_at = float3(0, 0, 2);
#endif
	}

	// ----------------------------------------------------------------------------
	// Clouds
	// ----------------------------------------------------------------------------
#define CLOUDS

#define anoise (abs(noise(p) * 2. - 1.))
	DECL_FBM_FUNC(fbm_clouds, 4, anoise)

#define vol_coeff_absorb 30.034
		_mutable(volume_sampler_t) cloud;

	float illuminate_volume(
		_inout(volume_sampler_t) cloud,
		_in(float3) V,
		_in(float3) L
	) {
		return exp(cloud.height) / .055;
	}

	void clouds_map(
		_inout(volume_sampler_t) cloud,
		_in(float) t_step
	) {
		float dens = fbm_clouds(
			cloud.pos * 3.2343 + float3(.35, 13.35, 2.67),
			2.0276, .5, .5);

#define cld_coverage .29475675 // higher=less clouds
#define cld_fuzzy .0335 // higher=fuzzy, lower=blockier
		dens *= smoothstep(cld_coverage, cld_coverage + cld_fuzzy, dens);

		dens *= band(.2, .35, .65, cloud.height);

		integrate_volume(cloud,
			cloud.pos, cloud.pos, // unused dummies 
			dens, t_step);
	}

	void clouds_march(
		_in(ray_t) eye,
		_inout(volume_sampler_t) cloud,
		_in(float) max_travel,
		_in(mat3) rot
	) {
		const int steps = 75;
		const float t_step = max_ray_dist / float(steps);
		float t = 0.;

		for (int i = 0; i < steps; i++) {
			if (t > max_travel || cloud.alpha >= 1.) return;

			float3 o = cloud.origin + t * eye.direction;
			cloud.pos = mul(rot, o - planet.origin);

			cloud.height = (length(cloud.pos) - planet.radius) / max_height;
			t += t_step;
			clouds_map(cloud, t_step);
		}
	}

	void clouds_shadow_march(
		_in(float3) dir,
		_inout(volume_sampler_t) cloud,
		_in(mat3) rot
	) {
		const int steps = 5;
		const float t_step = max_height / float(steps);
		float t = 0.;

		for (int i = 0; i < steps; i++) {
			float3 o = cloud.origin + t * dir;
			cloud.pos = mul(rot, o - planet.origin);

			cloud.height = (length(cloud.pos) - planet.radius) / max_height;
			t += t_step;
			clouds_map(cloud, t_step);
		}
	}

	// ----------------------------------------------------------------------------
	// Terrain
	// ----------------------------------------------------------------------------
#define TERR_STEPS 120
#define TERR_EPS .005
#define rnoise (1. - abs(noise(p) * 2. - 1.))

	DECL_FBM_FUNC(fbm_terr, 3, noise(p))
		DECL_FBM_FUNC(fbm_terr_r, 3, rnoise)

		DECL_FBM_FUNC(fbm_terr_normals, 7, noise(p))
		DECL_FBM_FUNC(fbm_terr_r_normals, 7, rnoise)

		float2 sdf_terrain_map(_in(float3) pos)
	{
		float h0 = fbm_terr(pos * 2.0987, 2.0244, .454, .454);
		float n0 = smoothstep(.35, 1., h0);

		float h1 = fbm_terr_r(pos * 1.50987 + float3(1.9489, 2.435, .5483), 2.0244, .454, .454);
		float n1 = smoothstep(.6, 1., h1);

		float n = n0 + n1;

		return float2(length(pos) - planet.radius - n * max_height, n / max_height);
	}

	float2 sdf_terrain_map_detail(_in(float3) pos)
	{
		float h0 = fbm_terr_normals(pos * 2.0987, 2.0244, .454, .454);
		float n0 = smoothstep(.35, 1., h0);

		float h1 = fbm_terr_r_normals(pos * 1.50987 + float3(1.9489, 2.435, .5483), 2.0244, .454, .454);
		float n1 = smoothstep(.6, 1., h1);

		float n = n0 + n1;

		return float2(length(pos) - planet.radius - n * max_height, n / max_height);
	}

	float3 sdf_terrain_normal(_in(float3) p)
	{
#define F(t) sdf_terrain_map_detail(t).x
		float3 dt = float3(0.001, 0, 0);

		return normalize(float3(
			F(p + dt.xzz) - F(p - dt.xzz),
			F(p + dt.zxz) - F(p - dt.zxz),
			F(p + dt.zzx) - F(p - dt.zzx)
			));
#undef F
	}

	// ----------------------------------------------------------------------------
	// Lighting
	// ----------------------------------------------------------------------------
	float3 setup_lights(
		_in(float3) L,
		_in(float3) normal
	) {
		float3 diffuse = float3(0, 0, 0);

		// key light
		float3 c_L = float3(7, 5, 3);
		diffuse += max(0., dot(L, normal)) * c_L;

		// fill light 1 - faked hemisphere
		float hemi = clamp(.25 + .5 * normal.y, .0, 1.);
		diffuse += hemi * float3(.4, .6, .8) * .2;

		// fill light 2 - ambient (reversed key)
		float amb = clamp(.12 + .8 * max(0., dot(-L, normal)), 0., 1.);
		diffuse += amb * float3(.4, .5, .6);

		return diffuse;
	}

	float3 illuminate(
		_in(float3) pos,
		_in(float3) eye,
		_in(mat3) local_xform,
		_in(float2) df
	) {
		// current terrain height at position
		float h = df.y;
		//return float3 (h);

		float3 w_normal = normalize(pos);
#define LIGHT
#ifdef LIGHT
		float3 normal = sdf_terrain_normal(pos);
		float N = dot(normal, w_normal);
#else
		float N = w_normal.y;
#endif

		// materials
#define c_water float3(.015, .110, .455)
#define c_grass float3(.086, .132, .018)
#define c_beach float3(.153, .172, .121)
#define c_rock  float3(.080, .050, .030)
#define c_snow  float3(.600, .600, .600)

	// limits
#define l_water .05
#define l_shore .17
#define l_grass .211
#define l_rock .351

		float s = smoothstep(.4, 1., h);
		float3 rock = lerp(
			c_rock, c_snow,
			smoothstep(1. - .3*s, 1. - .2*s, N));

		float3 grass = lerp(
			c_grass, rock,
			smoothstep(l_grass, l_rock, h));

		float3 shoreline = lerp(
			c_beach, grass,
			smoothstep(l_shore, l_grass, h));

		float3 water = lerp(
			c_water / 2., c_water,
			smoothstep(0., l_water, h));

#ifdef LIGHT
		float3 L = mul(local_xform, normalize(float3(1, 1, 0)));
		shoreline *= setup_lights(L, normal);
		float3 ocean = setup_lights(L, w_normal) * water;
#else
		float3 ocean = water;
#endif

		return lerp(
			ocean, shoreline,
			smoothstep(l_water, l_shore, h));
	}

	// ----------------------------------------------------------------------------
	// Rendering
	// ----------------------------------------------------------------------------
	float3 render(
		_in(ray_t) eye,
		_in(float3) point_cam
	) {
		mat3 rot_y = rotate_around_y(27.);
		mat3 rot = mul(rotate_around_x(u_time * -12.), rot_y);
		mat3 rot_cloud = mul(rotate_around_x(u_time * 8.), rot_y);
		if (u_mouse.z > 0.) {
			rot = rotate_around_y(-u_mouse.x);
			rot_cloud = rotate_around_y(-u_mouse.x);
			rot = mul(rot, rotate_around_x(u_mouse.y));
			rot_cloud = mul(rot_cloud, rotate_around_x(u_mouse.y));
		}

		sphere_t atmosphere = planet;
		atmosphere.radius += max_height;

		hit_t hit = no_hit;
		intersect_sphere(eye, atmosphere, hit);
		if (hit.material_id < 0) {
			return background(eye);
		}

		float t = 0.;
		float2 df = float2(1, max_height);
		float3 pos;
		float max_cld_ray_dist = max_ray_dist;

		for (int i = 0; i < TERR_STEPS; i++) {
			if (t > max_ray_dist) break;

			float3 o = hit.origin + t * eye.direction;
			pos = mul(rot, o - planet.origin);

			df = sdf_terrain_map(pos);

			if (df.x < TERR_EPS) {
				max_cld_ray_dist = t;
				break;
			}

			t += df.x * .4567;
		}

#ifdef CLOUDS
		cloud = begin_volume(hit.origin, vol_coeff_absorb);
		clouds_march(eye, cloud, max_cld_ray_dist, rot_cloud);
#endif

		if (df.x < TERR_EPS) {
			float3 c_terr = illuminate(pos, eye.direction, rot, df);
			float3 c_cld = cloud.C;
			float alpha = cloud.alpha;
			float shadow = 1.;

#ifdef CLOUDS // clouds ground shadows
			pos = mul(transpose(rot), pos);
			cloud = begin_volume(pos, vol_coeff_absorb);
			float3 local_up = normalize(pos);
			clouds_shadow_march(local_up, cloud, rot_cloud);
			shadow = lerp(.7, 1., step(cloud.alpha, 0.33));
#endif

			return lerp(c_terr * shadow, c_cld, alpha);
		}
		else {
			return lerp(background(eye), cloud.C, cloud.alpha);
		}
	}

#define FOV tan(radians(30.))
	// ----------------------------------------------------------------------------
	// Main Rendering function
	// depends on external defines: FOV
	// ----------------------------------------------------------------------------

	void mainImage(
		_out(float4) fragColor,
#ifdef SHADERTOY
		float2 fragCoord
#else
		_in(float2) fragCoord
#endif
	) {
		// assuming screen width is larger than height 
		float2 aspect_ratio = float2(u_res.x / u_res.y, 1);

		float3 color = float3(0, 0, 0);

		float3 eye, look_at;
		setup_camera(eye, look_at);

		setup_scene();

		float2 point_ndc = i.screenCoord.xy * _ScreenParams.xy / u_res.xy;
#ifdef HLSL
		point_ndc.y = 1. - point_ndc.y;
#endif
		float3 point_cam = float3(
			(2.0 * point_ndc - 1.0) * aspect_ratio * FOV,
			-1.0);

		ray_t ray = get_primary_ray(point_cam, eye, look_at);

		color += render(ray, point_cam);

		return float4(linear_to_srgb(color), 1);
	}

			ENDCG
		}
	}
}
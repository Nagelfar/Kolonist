﻿var Kolonist;
(function (Kolonist) {
    var Heightmap = (function () {
              

        function Heightmap(renderer) {
            this._renderer = renderer;
            this._material = null;
            this._geometry = null;
        }

        Heightmap.prototype.loadMap = function (data) {

            var width = data.Width,
                height = data.Height,
                scale = 10;

            var geometry = generateHeightMap(data.Heights);
            var material = generateMaterial(data.TerrainTypes, data.AvailiableTerrainTypes);

            geometry.computeCentroids();
            geometry.computeFaceNormals();
            geometry.computeVertexNormals();


            var subdivision = new THREE.SubdivisionModifier(2);
            subdivision.modify(geometry);

            this._geometry = geometry;
            this._material = material;

            return {
                geometry: geometry,
                material: material
            };

            function calculateIndex(x, y) {
                return x + y * width;
            }

            function generateHeightMap(heights) {

                var geometry = new THREE.PlaneGeometry(width, height, width - 1, height - 1);
                for (var i = 0; i < geometry.vertices.length; i++) {
                    var calculatedHeight = heights[i];
                    geometry.vertices[i].z = calculatedHeight;
                }

                return geometry;
            }
            function generateMaterial(terrainTypes, availiableTerrainTypes) {

                function nearestPow2(n) {
                    var l = Math.log(n) / Math.LN2;
                    return Math.pow(2, Math.ceil(l));
                }

                var tileTextureSize = 1024;
                var tileSize = 32;
                var textureSize = Math.min(nearestPow2(Math.max(width, height) * tileSize), 2048);
                var textureScale = Math.min(textureSize / tileTextureSize, 3.0);

                var tex_uniforms = {
                    tileTexture: {
                        type: 't',
                        value: THREE.ImageUtils.loadTexture(availiableTerrainTypes[0].Href)
                    },

                    texscale: {
                        type: 'f',
                        value: textureScale
                    },

                    scale: {
                        type: 'f',
                        value: scale
                    },
                    highlight_position: {
                        type: 'v3',
                        value: new THREE.Vector3()
                    }
                }

                tex_uniforms.tileTexture.value.wrapS = THREE.RepeatWrapping;
                tex_uniforms.tileTexture.value.wrapT = THREE.RepeatWrapping;

                var vertexShader = [
                    "varying vec2 vUv;",
                    "varying float height;",
                    "uniform float scale;",

                    "varying vec3 vPosition;",

                    "void main()",
                    "{",
                        "vUv = uv;",
                        "height = position.z;",

                        "vPosition = vec3(position.x, position.y, height * scale);",

                        "vec4 mvPosition = modelViewMatrix * vec4(vPosition, 1.0 );",
                        "gl_Position = projectionMatrix * mvPosition;",

                        THREE.ShaderChunk.defaultnormal_vertex,
                    "}"
                ].join('\n');

                var fragmentShader = [
                    "uniform sampler2D alpha;",
                    "uniform sampler2D tileTexture;",

                    "uniform vec3 highlight_position;",
                    "uniform float texscale;",

                    "varying vec2 vUv;",
                    "varying float height;",
                    "varying vec3 vPosition;",

                    "vec3 get_terrain_uv(float type, vec2 uv)",
                    "{",
                        "float tiles_per_row = 2.0;",
                        "float tile_size = 1.0 / tiles_per_row;",

                        // handles repeat of the tiles
                        "uv.x = mod(mod((uv.x / (1.0 / texscale)) , texscale),1.0);",
                        "uv.y = mod(mod((uv.y / (1.0 / texscale)) , texscale),1.0);",


                        // cutoff to prevent texutres from overlapping
                        "uv.x = (uv.x * .6) + .2;",
                        "uv.y = (uv.y * .6) + .2;",

                        // calculates the base coordinates for the tiles ("0/0" position of the tile)
                        "float tileX = mod(type, tiles_per_row) * tile_size;",
                        "float tileY = floor(type / tiles_per_row) * tile_size;",

                        // calculates the actual position within the base texture
                        "uv.x = tileX + clamp((uv.x * tile_size), 0.0, 1.0);",
                        "uv.y = tileY + clamp((uv.y * tile_size), 0.0, 1.0);",

                        "return texture2D(tileTexture, uv).rgb;",
                    "}",

                    "float blend(float coordinate, float offset) {",
                        "float fullBlendWidth = 1.5;",
                        "float blendingSpeed = 2.0;",

                        "return clamp(fullBlendWidth - abs(coordinate - offset) * blendingSpeed, 0.0, 1.0);",
                    "}",

                    "vec4 getBlendings(float z) {",
                        "vec4 result = vec4(0.0, 0.0, 0.0, 0.0);",
                        "result.r = blend(z, 0.8);",
                        "result.g = blend(z, 1.4);",
                        "result.b = blend(z, 1.8);",
                        "result.a = blend(z, 2.5);",
                        "return result;",
                   "}",

                   	"float dist_falloff(float distance, float falloff) {",
                   	    "float alpha = (falloff - distance) / falloff;",

                        "return clamp(alpha, 0.0, 1.0);",
                   	"}",

                    "void main() {",
                        "vec4 finalColor ;",

                        // Get the blend information                     
                        "vec4 mixmap    = getBlendings(height );",

                        "vec3 texRock  = get_terrain_uv( 1.0, vUv );",
                        "vec3 texGrass = get_terrain_uv(0.0 , vUv );",
                        "vec3 texSnow = get_terrain_uv(2.0 , vUv );",
                        "vec3 texSand  = get_terrain_uv(3.0 , vUv );",

                        // Mix the colors together
                        "texSand *= mixmap.r;",
                        "texGrass = mix(texSand,  texGrass, mixmap.g);",
                        "texRock = mix(texGrass, texRock, mixmap.b);  ",
                        "vec3 finalTexture  = mix(texRock,texSnow, mixmap.a);",

                        //"float distance = sqrt((vPosition.x - highlight_position.x) * (vPosition.x - highlight_position.x) + (vPosition.y - highlight_position.y) * (vPosition.y - highlight_position.y));",
		
                        //"bool show_highlight=false;",
                        //"float highlight_size=5.0;",
                        //"vec4 highlight_color = vec4(1.0, 0.0, 0.0, 1.0);",

                        //// Ring
                        //"if (show_highlight == true && distance < highlight_size / 2.0 ) {",
			
                        //    "gl_FragColor.r += highlight_color.r;",
                        //    "gl_FragColor.b += highlight_color.b;",
                        //    "gl_FragColor.g += highlight_color.g;",
                        //    "gl_FragColor.a += highlight_color.a;",
                        //    "gl_FragColor = normalize(gl_FragColor);",
			
                        //"}",

                        "finalColor = vec4(finalTexture, 1.0);",
                        //"if(vPosition.z/10.0 >= 1.0) finalColor = vec4(1.0,1.0,0.0,1.0);",
                        "gl_FragColor  = finalColor;",
                    "}"
                ].join('\n');

                var material = new THREE.ShaderMaterial({
                    fragmentShader: fragmentShader,
                    vertexShader: vertexShader,
                    uniforms: tex_uniforms
                });

                material.highlight = {
                    position: tex_uniforms.highlight_position
                };

                return material;
            }
        }

        return Heightmap;
    })();
    Kolonist.Heightmap = Heightmap;
})(Kolonist || (Kolonist = {}));
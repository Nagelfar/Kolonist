﻿var Kolonist;
(function (Kolonist) {
    var Heightmap = (function () {

        function Heightmap() {
            this._material = null;
            this._geometry = null;
        }

        Heightmap.prototype.highlightOnMap = function (point) {

            if (this._material && point) {
                this._material.highlight.position.x = point.x;
                this._material.highlight.position.y = -point.z;
            }
        };

        Heightmap.prototype.loadMap = function (data) {

            var width = data.Width,
                height = data.Height,
                scale = 10;

            this._geometry = generateHeightMap(data.Heights);
            this._material = generateMaterial(data.TerrainTypes, data.CompositeTerrainTile);

            this._geometry.computeCentroids();

            var subdivision = new THREE.SubdivisionModifier(1);
            subdivision.modify(this._geometry);

            var mesh = new THREE.Mesh(this._geometry, this._material);

            mesh.rotation.x = -Kolonist.Util.Degree2Rad(90);
            mesh.dynamic = true;

            this.mesh = mesh;

            return mesh;

            function generateHeightMap(heights) {

                var geometry = new THREE.PlaneGeometry(width, height, width - 1, height - 1);
                for (var i = 0; i < geometry.vertices.length; i++) {
                    var vertex = geometry.vertices[i];
                    var calculatedHeight = heights[i];
                    vertex.z = calculatedHeight * scale;
                    vertex.x = vertex.x * scale / 2;
                    vertex.y = vertex.y * scale / 2;
                }

                return geometry;
            }
            function generateMaterial(terrainTypes, tileTexture) {

                var tileTextureSize = 1024;
                var tileSize = 32;
                var textureSize = Math.min(Kolonist.Util.nearestPow2(Math.max(width, height) * tileSize), 2048);
                var textureScale = Math.min(textureSize / tileTextureSize, 3.0);

                var tex_uniforms = {
                    tileTexture: {
                        type: 't',
                        value: THREE.ImageUtils.loadTexture(tileTexture.Href)
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
                };

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
                        "height = position.z/scale;",

                        "vPosition = vec3(position.x, position.y, position.z);",

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
                        "result.a = blend(z, 2.5); ",
                        "return result;",
                   "}",

                   	"float dist_falloff(float distance, float falloff) {",
                        "if(distance == 0.0) return 1.0;",
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

                        "finalColor = vec4(finalTexture, 1.0);",

                        "float distance = sqrt((vPosition.x - highlight_position.x) * (vPosition.x - highlight_position.x) + (vPosition.y - highlight_position.y) * (vPosition.y - highlight_position.y));",

                        "bool show_highlight=true;",
                        "float highlight_size=15.0;",
                        "vec4 highlight_color = vec4(1.0, 0.0, 0.0, 1.0);",

                        // Ring
                        "if (show_highlight == true && distance < highlight_size / 2.0 ) {",

                            "float cutoff = dist_falloff(distance, highlight_size/ 2.0);",
                            "highlight_color = highlight_color  * cutoff;",
                            "finalColor.r += highlight_color.r;",
                            "finalColor.b += highlight_color.b;",
                            "finalColor.g += highlight_color.g;",
                            "finalColor.a += highlight_color.a;",
                            "finalColor = normalize(finalColor);",

                        "}",

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
                    position: tex_uniforms.highlight_position.value
                };

                return material;
            }
        };

        return Heightmap;
    })();
    Kolonist.Heightmap = Heightmap;
})(Kolonist || (Kolonist = {}));
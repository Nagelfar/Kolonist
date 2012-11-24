
var Renderer = (function () {
    defaultParameters = {
        width: 400,
        height: 400,
        viewAngle: 45,
        near: 0.1,
        far: 1000.0,
        cameraZPosition: 100
    };

    var camera, scene, renderer, controls;

    var _$parentContainer;

    function Renderer($parentContainer) {
        _$parentContainer = $parentContainer;
    }

    Renderer.prototype.init = function (parameters) {
        parameters = $.extend(true, {}, defaultParameters, parameters);

        camera = new THREE.PerspectiveCamera(parameters.viewAngle, parameters.width / parameters.height, parameters.near, parameters.far);
        camera.position.z = parameters.cameraZPosition;


        scene = new THREE.Scene();

        renderer = new THREE.WebGLRenderer();
        //renderer.setFaceCulling(false);
        renderer.setSize(parameters.width, parameters.height);

        controls = new THREE.TrackballControls(camera, renderer.domElement);

        _$parentContainer.append(renderer.domElement);
    }


    Renderer.prototype.addMesh = function (mesh) {
        scene.add(mesh);
    }

    Renderer.prototype.animate = function (callback) {

        var animation = function () {
            requestAnimationFrame(animation);

            callback();

            controls.update();

            renderer.render(scene, camera);
        }
        animation();
    }

    Renderer.prototype.loadMap = function (url) {
        $.getJSON(url, '', function (data) {

            var width = data.Width,
                height = data.Height;

            var geometry = generateHeightMap(data.Heights);
            var perlin = ImprovedNoise();
            for (var i = 0; i < geometry.vertices.length; i++) {
                var vertex = geometry.vertices[i];
                vertex.z = perlin.noise3(vertex.x, vertex.y, vertex.z) * 3;
            }

            var material = generateMaterial(data.TerrainTypes, data.AvailiableTerrainTypes);

            geometry.computeCentroids();
            geometry.computeFaceNormals();
            geometry.computeVertexNormals();


            var subdivision = new THREE.SubdivisionModifier(2);
            subdivision.modify(geometry);

            var mesh = new THREE.Mesh(geometry, material);
            scene.add(mesh);

            function calculateIndex(x, y) {
                return x * width + y;
            }

            function generateHeightMap(heights) {

                var geometry = new THREE.Geometry();

                for (var x = 0; x < width; x++) {
                    for (var y = 0; y < height; y++) {
                        geometry.vertices.push(new THREE.Vector3(x, y, heights[calculateIndex(x, y)]));
                    }
                }

                function pushUVs(x, y, uvs) {
                    uvs.push(new THREE.UV(x / width, y / height));
                    return calculateIndex(x, y);
                }

                var uvIndex = 0;
                geometry.faceVertexUvs = [];
                geometry.faceVertexUvs[uvIndex] = [];
                for (var x = 0; x < width - 1; x++) {
                    for (var y = 0; y < height - 1; y++) {

                        // generate the faces (2 per cell)
                        // and assign the uv-indices
                        var uvs = [];
                        var face = new THREE.Face3();

                        face.a = pushUVs(x, y, uvs);
                        face.b = pushUVs(x + 1, y, uvs);
                        face.c = pushUVs(x + 1, y + 1, uvs);


                        geometry.faceVertexUvs[uvIndex][geometry.faces.length] = uvs;
                        geometry.faces.push(face);

                        uvs = [];
                        face = new THREE.Face3();

                        face.a = pushUVs(x + 1, y + 1, uvs);
                        face.b = pushUVs(x, y + 1, uvs);
                        face.c = pushUVs(x, y, uvs);

                        geometry.faceVertexUvs[uvIndex][geometry.faces.length] = uvs;
                        geometry.faces.push(face);
                    }
                }
                return geometry;
            }
            function generateMaterial(terrainTypes, availiableTerrainTypes) {

                var texture = new THREE.Texture();

                var loadCount = 0;
                var imageLoader = new THREE.ImageLoader();

                var terrainImages = [];

                availiableTerrainTypes.forEach(function (terrain) {
                    var tile = new Image();
                    imageLoader.load(terrain.Href, tile);

                    tile.onload = function () {
                        loadCount++;

                        if (loadCount === availiableTerrainTypes.length) {
                            composeTexture();
                            texture.needsUpdate = true;
                        }
                    }
                    terrainImages[terrain.Rel] = tile;
                });
                function nearestPow2(n) {
                    var l = Math.log(n) / Math.LN2;
                    return Math.pow(2, Math.ceil(l));
                }

                var tileTextureSize = 1024;
                var tileSize = 32;
                var textureSize = nearestPow2(Math.max(width, height) * tileSize);
                var textureScale = textureSize / tileTextureSize;

                var tex_uniforms = {
                    alpha: {
                        type: 't',
                        value: texture,
                    },
                    tileTexture: {
                        type: 't',
                        value: THREE.ImageUtils.loadTexture(availiableTerrainTypes[0].Href)
                    },

                    texscale: {
                        type: 'f',
                        value: textureScale
                    }
                }

                tex_uniforms.alpha.value.wrapS = THREE.ClampToEdgeWrapping;
                tex_uniforms.alpha.value.wrapT = THREE.ClampToEdgeWrapping;

                tex_uniforms.tileTexture.value.wrapS = THREE.RepeatWrapping;
                tex_uniforms.tileTexture.value.wrapT = THREE.RepeatWrapping;

                var vertexShader = [
                    "varying vec2 vUv;",
                    "varying vec3 vNormal;",

                    "void main()",
                    "{",
                        "vUv = uv;",

                        THREE.ShaderChunk.default_vertex,

                        THREE.ShaderChunk.defaultnormal_vertex,
                    "}"
                ].join('\n');

                var fragmentShader = [
                    "uniform sampler2D alpha;",
                    "uniform sampler2D tileTexture;",

                    "uniform float texscale;",

                    "varying vec2 vUv;",

                    "vec3 get_terrain_uv(float type, vec2 uv)",
                    "{",
                        "float tiles_per_row = 2.0;",
                        "float tile_size = 1.0 / tiles_per_row;",

                        // handles repeat of the tiles
                        "uv.x = mod(mod((uv.x / (1.0 / texscale)) , texscale),1.0);",
                        "uv.y = mod(mod((uv.y / (1.0 / texscale)) , texscale),1.0);",

                        // calculates the base coordinates for the tiles ("0/0" position of the tile)
                        "float tileX = mod(type, tiles_per_row) * tile_size;",
                        "float tileY = floor(type / tiles_per_row) * tile_size;",

                        // calculates the actual position within the base texture
                        "uv.x = tileX + clamp((uv.x * tile_size), 0.0, 1.0);",
                        "uv.y = tileY + clamp((uv.y * tile_size), 0.0, 1.0);",

                        "return texture2D(tileTexture, uv).rgb;",
                    "}",

                    "float correctAlphaValue(float alpha)",
                    "{",
                        // correction for alpha value (alpha 0.0 does not work in texture)
                        "if(alpha<=0.01)",
                            "alpha=0.0;",

                        "return alpha;",
                    "}",

                    "void main()",
                    "{",
                        "vec4 finalColor ;",

                        // Get the blend information 
                        "vec4 mixmap    = texture2D( alpha, vUv ).rgba;",

                        "vec3 texRock  = get_terrain_uv( 1.0, vUv );",
                        "vec3 texGrass = get_terrain_uv(0.0 , vUv );",
                        "vec3 texSnow = get_terrain_uv(2.0 , vUv );",
                        "vec3 texSand  = get_terrain_uv(3.0 , vUv );",

                        "float a = correctAlphaValue(mixmap.a);",

                        // Mix the colors together
                        "texSand *= mixmap.r;",
                        "texGrass = mix(texSand,  texGrass, mixmap.g);",
                        "texSnow = mix(texGrass, texSnow, mixmap.b);  ",
                        "vec3 finalTexture  = mix(texSnow, texRock, a);",

                        "finalColor = vec4(finalTexture, 1.0);",
                        "gl_FragColor  = finalColor;",
                    "}"
                ].join('\n');

                var material = new THREE.ShaderMaterial({
                    fragmentShader: fragmentShader,
                    vertexShader: vertexShader,
                    uniforms: tex_uniforms
                });

                return material;

                function composeTexture() {

                    texture.image = document.createElement('canvas');
                    texture.image.width = textureSize;
                    texture.image.height = textureSize;
                    var imageContext = texture.image.getContext('2d');

                    var usedTileSize = textureSize / width;

                    for (var x = 0; x < width; x++) {
                        for (var y = 0; y < height; y++) {
                            //var terrainType = terrainTypes[calculateIndex(x, y)];
                            var terrainType = THREE.Math.randInt(0, 3);


                            imageContext.fillStyle =
                                'rgba('
                                    + (terrainType === 0 ? 255 : 0) + ','
                                    + (terrainType === 1 ? 255 : 0) + ','
                                    + (terrainType === 2 ? 255 : 0) + ','
                                    + (terrainType === 3 ? 1 : 0.01)
                                    + ')';

                            imageContext.fillRect(
                                Math.floor(x * usedTileSize), Math.floor(y * usedTileSize),
                                Math.ceil(usedTileSize), Math.ceil(usedTileSize)
                                );
                        }
                    }

                    material.needsUpdate = true;
                    $('#tmp').html(texture.image);
                }
            }
        });
    }
    return Renderer;
})();
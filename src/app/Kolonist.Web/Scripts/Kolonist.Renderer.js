
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
            var material = generateMaterial(data.TerrainTypes, data.AvailiableTerrainTypes);

            geometry.computeCentroids();
            geometry.computeFaceNormals();
            geometry.computeVertexNormals();


            var subdivision = new THREE.SubdivisionModifier(1);
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

                var count = 0;
                var update = function () {
                    count++;
                    if (count >= 2) {
                        material.needsUpdate = true;
                    }
                }
                var tex_uniforms = {
                    alpha: {
                        type: 't',
                        value: texture,
                    },
                    tex0: {
                        type: 't',
                        value: THREE.ImageUtils.loadTexture(availiableTerrainTypes[0].Href)
                    },
                    tex1: {
                        type: 't',
                        value: THREE.ImageUtils.loadTexture(availiableTerrainTypes[1].Href)
                    },
                    tex2: {
                        type: 't',
                        value: THREE.ImageUtils.loadTexture(availiableTerrainTypes[2].Href)
                    },
                    tex3: {
                        type: 't',
                        value: THREE.ImageUtils.loadTexture(availiableTerrainTypes[3].Href)
                    },
                    texscale: {
                        type: 'f',
                        value: textureScale
                    }
                }

                tex_uniforms.alpha.value.wrapS = THREE.ClampToEdgeWrapping;
                tex_uniforms.alpha.value.wrapT = THREE.ClampToEdgeWrapping;

                tex_uniforms.tex0.value.wrapS = THREE.RepeatWrapping;
                tex_uniforms.tex0.value.wrapT = THREE.RepeatWrapping;

                tex_uniforms.tex1.value.wrapS = THREE.RepeatWrapping;
                tex_uniforms.tex1.value.wrapT = THREE.RepeatWrapping;

                tex_uniforms.tex2.value.wrapS = THREE.RepeatWrapping;
                tex_uniforms.tex2.value.wrapT = THREE.RepeatWrapping;

                tex_uniforms.tex3.value.wrapS = THREE.RepeatWrapping;
                tex_uniforms.tex3.value.wrapT = THREE.RepeatWrapping;




                var material = new THREE.ShaderMaterial({
                    fragmentShader: $('#fragmentShader').text(),
                    vertexShader: $('#vertexShader').text(),
                    uniforms: tex_uniforms//THREE.UniformsUtils.merge([THREE.UniformsLib["common"], THREE.UniformsLib["lights"], tex_uniforms]),
                    //lights: true
                });

                //composeTexture();
                //var material = new THREE.MeshLambertMaterial({
                //    //color: 0x00ff00,
                //    wireframe: false,
                //    map: texture
                //});

                return material;

                function composeTexture() {


                    texture.image = document.createElement('canvas');
                    texture.image.width = textureSize;
                    texture.image.height = textureSize;
                    var imageContext = texture.image.getContext('2d');
                    //imageContext.globalAlpha=1.0
                    //imageContext.globalCompositeOperation = 'copy';
                    //var imageData = imageContext.createImageData(textureSize, textureSize);

                    //var channels = 4;
                    //function imageIndex(x, y) {
                    //    return ((y * imageData.width )+ x) * channels;
                    //}

                    var usedTileSize = textureSize / width;

                    for (var x = 0; x < width; x++) {
                        for (var y = 0; y < height; y++) {
                            //var terrainType = terrainTypes[calculateIndex(x, y)];
                            var terrainType = THREE.Math.randInt(0, 3);

                            //for (var tx = 0; tx < usedTileSize; tx++)
                            //    for (var ty = 0; ty < usedTileSize; ty++) {
                            //        var index = imageIndex((x * usedTileSize )+ tx, (y * usedTileSize )+ ty);
                            //        imageData.data[index + terrainType] = 255;
                            //        imageData.data[index + 3] = 255;
                            //    }
                            //var image = terrainImages[terrainType];(terrainType == 3 ? 1 : 0)
                            imageContext.fillStyle = 'rgba(' + (terrainType === 0 ? 255 : 0) + ',' + (terrainType === 1 ? 255 : 0) + ',' + (terrainType === 2 ? 255 : 0) + ',' + (terrainType === 3 ? 1 : 0.01) + ')';
                            //imageContext.fillStyle = 'rgba(255,0,0,0.01)'; 
                            //var style = "black";
                            //switch (terrainType) {
                            //    case 0:
                            //        style = "red";
                            //        break;
                            //    case 1:
                            //        style = "green";
                            //        break;
                            //    case 2:
                            //        style = "blue";
                            //        break;
                            //}
                            //imageContext.fillStyle = style;
                            var r = imageContext.fillRect(Math.floor(x * usedTileSize), Math.floor( y * usedTileSize),Math.ceil( usedTileSize), Math.ceil( usedTileSize));

                            //imageContext.drawImage(image, x * tileSize, y * tileSize, tileSize, tileSize);
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
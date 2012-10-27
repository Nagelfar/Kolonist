
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

                var material = new THREE.MeshLambertMaterial({
                    //color: 0x00ff00,
                    wireframe: false,
                    map: texture
                });

                return material;

                function composeTexture() {
                    function nearestPow2(n) {
                        var l = Math.log(n) / Math.LN2;
                        return Math.pow(2, Math.round(l));
                    }

                    var tileSize = 32;

                    texture.image = document.createElement('canvas');
                    texture.image.width = nearestPow2(width * tileSize);
                    texture.image.height = nearestPow2(height * tileSize);
                    var imageContext = texture.image.getContext('2d');

                    for (var x = 0; x < width; x++) {
                        for (var y = 0; y < height; y++) {
                            var terrainType = terrainTypes[calculateIndex(x, y)];

                            var image = terrainImages[terrainType];

                            imageContext.drawImage(image, x * tileSize, y * tileSize, tileSize, tileSize);
                        }
                    }


                    var improvedNoise = new ImprovedNoise();

                    var data = imageContext.getImageData(0, 0, texture.image.width, texture.image.height);
                    var newData = imageContext.createImageData(data);
                    for (var i = 0; i < data.data.length; i++)
                        newData.data[i] = data.data[i];

                    var maxShift = 5;
                    var channels = 4;

                    function imageIndex(x, y) {
                        return (x * data.width + y) * channels;
                    }
                  
                    function interpolate(first, second, alpha) {
                        return first * (1 - alpha) + second * alpha;
                    }
                    function interpolatePixels(x, y) {
                        x = x * tileSize;
                        y = y * tileSize;
                        for (var offsetX = 0; offsetX < maxShift; offsetX++) {
                            for (var offsetY = 0; offsetY < maxShift; offsetY++) {

                                for (var channel = 0; channel < channels; channel++) {

                                    var currentIndex = imageIndex(x - offsetX, y - offsetY) + channel;
                                    var nextIndex = imageIndex(x + offsetX, y + offsetY) + channel;

                                    newData.data[currentIndex] = interpolate(
                                        data.data[currentIndex],
                                        data.data[nextIndex],
                                        improvedNoise.noise2(x - offsetX, y - offsetY)
                                        );

                                    newData.data[nextIndex] = interpolate(
                                        data.data[currentIndex],
                                        data.data[nextIndex],
                                        improvedNoise.noise2(x + offsetX, y + offsetY)
                                        );
                                }
                            }
                        }
                    }

                    for (var x = 1 ; x < width - 2; x++) {
                        for (y = 1; y < height - 2; y++) {
                            interpolatePixels(x, y);
                        }
                    }
                    imageContext.putImageData(newData, 0, 0);
                    //$('#tmp').html(texture.image);
                }
            }
        });
    }
    return Renderer;
})();
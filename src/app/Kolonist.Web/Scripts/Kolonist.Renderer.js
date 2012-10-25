
var Renderer = (function () {
    defaultParameters = {
        width: 400,
        height: 400,
        viewAngle: 45,
        near: 0.1,
        far: 1000.0,
        cameraZPosition: 100
    };

    var camera, scene, renderer;

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

        _$parentContainer.append(renderer.domElement);
    }

    Renderer.prototype.lookAt = function (vector) {
        camera.lookAt(vector);
    }

    Renderer.prototype.camera = function (vector) {
        camera.position = camera.position.addSelf(vector);
    }

    Renderer.prototype.addMesh = function (mesh) {
        scene.add(mesh);
    }

    Renderer.prototype.animate = function (callback) {

        var animation = function () {
            requestAnimationFrame(animation);

            callback();

            renderer.render(scene, camera);
        }
        animation();
    }

    Renderer.prototype.loadMap = function (url) {
        $.getJSON(url, '', function (data) {

            var width = data.Width,
                height = data.Height;

            var geometry = generateHeightMap(data.Heights);
            data.AvailiableTerrainTypes = ['http://localhost:12930/Content/texture.jpg'];
            var material = generateMaterial(data.TerrainTypes, data.AvailiableTerrainTypes);

            geometry.computeVertexNormals();
            geometry.computeFaceNormals();
            geometry.computeCentroids();

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
                geometry.faceUvs = [];
                geometry.faceUvs[uvIndex] = [];

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
                        geometry.faceUvs[uvIndex][geometry.faces.length] = new THREE.UV(x / width, y / width);
                        geometry.faces.push(face);

                        uvs = [];
                        face = new THREE.Face3();

                        face.a = pushUVs(x + 1, y + 1, uvs);
                        face.c = pushUVs(x, y, uvs);
                        face.b = pushUVs(x, y + 1, uvs);

                        
                        geometry.faceVertexUvs[uvIndex][geometry.faces.length] = uvs;
                        geometry.faceUvs[uvIndex][geometry.faces.length] = new THREE.UV(x / width, y / width);
                        geometry.faces.push(face);
                    }
                }
                return geometry;
            }
            function generateMaterial(terrainTypes, availiableTerrainTypes) {

                var texture = new THREE.Texture();

                var loadCount = 0;
                var imageLoader = new THREE.ImageLoader();
                var terrainImages = availiableTerrainTypes.map(function (terrain) {
                    var tile = new Image();
                    imageLoader.load(terrain, tile);
                    
                    tile.onload = function () {
                        loadCount++;

                        if (loadCount === availiableTerrainTypes.length) {
                            composeTexture();
                            texture.needsUpdate = true;
                        }
                    }
                    return { terrainType: terrain, image: tile };
                });
                
                var material = new THREE.MeshLambertMaterial({
                    color: 0x00ff00,
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
                    // compose images
                    texture.image = document.createElement('canvas');
                    texture.image.width = nearestPow2(width * tileSize);
                    texture.image.height = nearestPow2(height * tileSize);
                    var imageContext = texture.image.getContext('2d');

                    for (var x = 0; x < width; x++) {
                        for (var y = 0; y < height; y++) {
                            var terrainType = terrainTypes[calculateIndex(x, y)];
                            var image = terrainImages[0].image;

                            imageContext.drawImage(image, x * tileSize, y * tileSize, tileSize, tileSize);

                        }
                    }

                    //$('#tmp').html(texture.image);
                }
            }
        });
    }
    return Renderer;
})();
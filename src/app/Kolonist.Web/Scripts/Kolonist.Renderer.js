
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
            var geometry = new THREE.Geometry();

            parseModel(data.vertices, data.faces);
            
            geometry.computeVertexNormals();
            geometry.computeFaceNormals();

            var material = new THREE.MeshLambertMaterial({
                color: 0xff0000,
                wireframe: true
                //map: texture
            });

            //    //var subdivision = new THREE.SubdivisionModifier(2);
            //    //subdivision.modify(data);

            var mesh = new THREE.Mesh(geometry, material);
            scene.add(mesh);

            function parseModel(vertices, faces) {

                var scale = 1;
                var offset = 0;
                var zLength = vertices.length;

                while (offset < zLength) {

                    vertex = new THREE.Vector3();

                    vertex.x = vertices[offset++] * scale;
                    vertex.y = vertices[offset++] * scale;
                    vertex.z = vertices[offset++] * scale;

                    geometry.vertices.push(vertex);
                }

                offset = 0;
                zLength = faces.length;

                while (offset < zLength) {

                    type = faces[offset++];

                    face = new THREE.Face3();

                    face.a = faces[offset++];
                    face.b = faces[offset++];
                    face.c = faces[offset++];

                    nVertices = 3;

                    geometry.faces.push(face);
                }
            }
           
        });
    }
    return Renderer;
})();
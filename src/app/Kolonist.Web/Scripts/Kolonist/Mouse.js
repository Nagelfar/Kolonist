/// <reference name="~/Scripts/Kolonist/Renderer.js"/>

var Kolonist;
(function (Kolonist) {

    var Mouse = (function () {

        var States = {
            UP: 0,
            DOWN: 1,
            DRAGGING: 3
        }

        var mouse_info = {
            x: 0,
            y: 0,
            button: 0,
            state: States.UP,
            point: null,
        };

        var _renderer, _sceneObject, _projector;

        function Mouse(renderer, sceneObject) {
            _renderer = renderer;
            _sceneObject = sceneObject;
            _projector = new THREE.Projector();

            var canvasRenderer = _renderer.getRenderer();
            canvasRenderer.domElement.onmousedown = function onmousedown(e) {
                mouse_info.state = States.UP;
                updateMouse(e);
            };
            canvasRenderer.domElement.onmouseup = function onmouseup(e) {
                mouse_info.state = States.DOWN;
                updateMouse(e);
            };
            canvasRenderer.domElement.onmousemove = function onmousemove(e) {
                if (mouse_info.state == States.UP) {
                    mouse_info.state = States.DRAGGING;
                }
                updateMouse(e);
                updateMouseCoordinates();
            };
            canvasRenderer.domElement.onmouseout = function onmouseout(e) {
                mouse_info.state = States.DOWN;
                updateMouse(e);
            };
        }

        var updateMouse = function (e) {
            e.preventDefault();
            e.cancelBubble = true;

            mouse_info.x = e.layerX;
            mouse_info.y = e.layerY;
            mouse_info.button = e.button;
        };

        var updateMouseCoordinates = function () {
            var camera = _renderer.getCamera();
            var vector = new THREE.Vector3(
                (mouse_info.x / _renderer.getParameters().width) * 2 - 1,
                -(mouse_info.y / _renderer.getParameters().height) * 2 + 1,
                0.5);
            _projector.unprojectVector(vector, camera);

            var ray = new THREE.Ray(camera.position, vector.subSelf(camera.position).normalize());

            var intersection = ray.intersectObject(_sceneObject);
            if (intersection.length === 0) {
                mouse_info.plot_coordinates.x = null;
                mouse_info.plot_coordinates.y = null;

                mouse_info.vertex_coordinates.x = null;
                mouse_info.vertex_coordinates.y = null;

                return null;
            } else {
                mouse_info.point = intersection[0].point;

            }

            //ground.materials[0].uniforms.ring_center.value.x = mouse_info.point.x;
            //ground.materials[0].uniforms.ring_center.value.y = -mouse_info.point.z;
        };

        Mouse.prototype.getInformation = function () {
            return mouse_info;
        }

        return Mouse;
    })();
    Kolonist.Mouse = Mouse;
})(Kolonist || (Kolonist = {}));
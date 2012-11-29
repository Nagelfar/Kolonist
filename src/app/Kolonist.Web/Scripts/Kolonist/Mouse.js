/// <reference name="~/Scripts/Kolonist/Renderer.js"/>

var Kolonist;
(function (Kolonist) {

    var Mouse = (function () {

        var States = {
            UP: 0,
            DOWN: 1,
            DRAGGING: 3
        }

        
        function Mouse(renderer, sceneObject) {
            if (!renderer)
                throw "Renderer needed";
            if (!sceneObject)
                throw "Scene object(s) needed";

            if (typeof (sceneObject) !== "Array")
                sceneObject = [sceneObject];

            var projector = new THREE.Projector();

            var mouse_info = {
                x: 0,
                y: 0,
                button: 0,
                state: States.UP,
                point: null,
            };

            var canvasRenderer = renderer.getRenderer();
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

            var updateMouse = function (e) {
                e.preventDefault();
                e.cancelBubble = true;
                
                mouse_info.x = e.offsetX;
                mouse_info.y = e.offsetY;
                mouse_info.button = e.button;
            };

            var updateMouseCoordinates = function () {
                var camera = renderer.getCamera();
                var vector = new THREE.Vector3(
                    (mouse_info.x / renderer.getParameters().width) * 2 - 1,
                    -(mouse_info.y / renderer.getParameters().height) * 2 + 1,
                    0.5);
                projector.unprojectVector(vector, camera);

                var ray = new THREE.Ray(camera.position, vector.subSelf(camera.position).normalize());

                var intersection = ray.intersectObjects(sceneObject);
                if (intersection.length === 0) {              
                    return null;
                } else {
                    mouse_info.point = intersection[0].point;
                }
            };

            this.getInformation = function () {
                return mouse_info;
            }
        }        

        return Mouse;
    })();
    Kolonist.Mouse = Mouse;
})(Kolonist || (Kolonist = {}));
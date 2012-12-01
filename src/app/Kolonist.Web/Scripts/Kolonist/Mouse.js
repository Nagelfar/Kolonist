/// <reference name="~/Scripts/Kolonist/Renderer.js"/>

var Kolonist;
(function (Kolonist) {

    var Mouse = (function () {

        var States = {
            UP: 0,
            DOWN: 1,
            DRAGGING: 3
        };


        function Mouse(renderer) {
            if (!renderer)
                throw "Renderer needed";

            var projector = new THREE.Projector();

            var clickHandlers = [];

            var registeredSceneObjects = [];

            var mouse_info = {
                x: 0,
                y: 0,
                button: 0,
                state: States.UP,
                point: null
            };

            var canvasRenderer = renderer.getRenderer();
            canvasRenderer.domElement.onmousedown = function onmousedown(e) {
                mouse_info.state = States.UP;
                updateMouse(e);
            };
            canvasRenderer.domElement.onmouseup = function onmouseup(e) {
                updateMouse(e);

                for (var i in clickHandlers) {
                    clickHandlers[i](mouse_info);
                }
                mouse_info.state = States.DOWN;
            };
            canvasRenderer.domElement.onmousemove = function onmousemove(e) {
                if (mouse_info.state === States.UP) {
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

                for (var i in registeredSceneObjects) {
                    var sceneObjects = registeredSceneObjects[i].objects;

                    var intersection;
                    if (typeof (sceneObjects) === "Array")
                        intersection = ray.intersectObjects(sceneObjects);
                    else
                        intersection = ray.intersectObject(sceneObjects);

                    if (intersection.length > 0) {
                        mouse_info.point = registeredSceneObjects[i].point = intersection[0].point;

                        var info = buildHitInformation(registeredSceneObjects[i].point);
                        for (var callback in registeredSceneObjects[i].callbacks) {
                            callback(info);
                        }
                    }
                }
            };

            this.getInformation = function (sceneObjects) {
                var hitInformation = findRegisteredObject(sceneObjects);
                if (hitInformation && hitInformation.length > 0)
                    hitInformation = hitInformation[0];
                return buildHitInformation(hitInformation);
            };

            this.registerClickHandler = function (handler) {
                clickHandlers.push(handler);
            }

            this.registerSceneObject = function (sceneObjects, callback) {
                if (!sceneObjects)
                    throw "Scene object(s) needed";

                var matches = findRegisteredObject(sceneObjects);
                if (!matches || matches.length === 0) {
                    matches = [{
                        objects: sceneObjects,
                        callbacks: []
                    }];

                    registeredSceneObjects.push(matches[0]);
                }

                if (callback)
                    matches.forEach(function (value) {
                        value.callbacks.push(callback);
                    });
            }

            function buildHitInformation(hitInformation) {
                var info = {
                    common: mouse_info,
                };
                if (hitInformation)
                    info.point = hitInformation.point;

                return info;
            }
            function findRegisteredObject(sceneObjects) {
                return registeredSceneObjects.filter(function (value) {
                    return value.objects == sceneObjects;
                });
            }

        }

        return Mouse;
    })();
    Kolonist.Mouse = Mouse;
})(Kolonist || (Kolonist = {}));
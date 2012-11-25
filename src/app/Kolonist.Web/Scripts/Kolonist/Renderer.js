var Kolonist;
(function (Kolonist) {
    var Renderer = (function () {
        defaultParameters = {
            width: 400,
            height: 400,
            viewAngle: 45,
            near: 0.1,
            far: 10000.0,
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

            controls = new THREE.OrbitControls(camera, renderer.domElement);
            controls.userRotate = false;
            controls.rotateUp(Kolonist.Util.Degree2Rad(45));
        
            function onKeyDown(event) {

                var vector = new THREE.Vector3();
                //event.preventDefault();
                var movement = 1;
                switch (event.keyCode) {

                    case 38: /*up*/
                    case 87: /*W*/ vector.z += -movement; break;

                    case 37: /*left*/
                    case 65: /*A*/ vector.x += -movement; break;

                    case 40: /*down*/
                    case 83: /*S*/ vector.z += movement; break;

                    case 39: /*right*/
                    case 68: /*D*/ vector.x += movement; break;

                }
                camera.position.addSelf(vector);
                controls.center.addSelf(vector);
            };
            controls.domElement.addEventListener('keydown', onKeyDown, false);
            controls.domElement.addEventListener('mousedown', function () {
                controls.domElement.focus();
            },false);
        
            controls.domElement.setAttribute('tabindex', -1);
        

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
            var hm = new Kolonist.Heightmap(this);

            $.getJSON(url, '', hm.loadMap);
        }
        return Renderer;
    })();

    Kolonist.Renderer = Renderer;
})(Kolonist || (Kolonist={}));
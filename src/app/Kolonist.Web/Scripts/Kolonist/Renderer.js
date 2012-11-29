var Kolonist;
(function (Kolonist) {
    var Renderer = (function () {
        var defaultParameters = {
            width: 400,
            height: 400,
            viewAngle: 45,
            near: 0.1,
            far: 10000.0,
            cameraZPosition: 100
        };

        //var camera, scene, renderer, controls, mouse;

        //var _$parentContainer;
        //var heightMap;

        function Renderer($parentContainer) {
            this._$parentContainer = $parentContainer;

            this.scene = new THREE.Scene();

            this.renderer = new THREE.WebGLRenderer();

            this.initializeCamara = function () {
                var controls = new THREE.OrbitControls(this.camera, this.renderer.domElement);
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
                    controls.object.position.addSelf(vector);
                    controls.center.addSelf(vector);
                };

                controls.domElement.addEventListener('keydown', onKeyDown, false);
                controls.domElement.addEventListener('mousedown', function () {
                    controls.domElement.focus();
                }, false);

                controls.domElement.setAttribute('tabindex', -1);

                this.controls = controls;
            }
        }

        Renderer.prototype.init = function (parameters) {
            this._parameters = $.extend(true, {}, defaultParameters, parameters);

            this.camera = new THREE.PerspectiveCamera(this._parameters.viewAngle, this._parameters.width / this._parameters.height, this._parameters.near, this._parameters.far);
            this.camera.position.z = this._parameters.cameraZPosition;
//renderer.setFaceCulling(false);
            this.renderer.setSize(parameters.width, parameters.height);

            this.initializeCamara();

            this._$parentContainer.append(this.renderer.domElement);
        }

        Renderer.prototype.addMesh = function (mesh) {
            this.scene.add(mesh);
        }

        Renderer.prototype.animate = function (callback) {

            var that = this;
            var animation = function () {
                requestAnimationFrame(animation);

                callback();

                that.controls.update();

                if (that.mouse && that.heightMap) {
                    var mousePosition = that.mouse.getInformation();
                    that.heightMap.highlightOnMap(mousePosition.point);
                }

                that.renderer.render(that.scene, that.camera);
            }
            animation();
        }

        Renderer.prototype.loadMap = function (url) {
            var heightMap = new Kolonist.Heightmap(this);
            var that = this;

            $.ajax(url)
                .success(function (data) {
                    var result = heightMap.loadMap(data);

                    that.mouse = new Kolonist.Mouse(that, result);

                    that.addMesh(result);
                });

            this.heightMap = heightMap;
        }

        Renderer.prototype.getCamera = function () {
            return this.camera;
        }
        Renderer.prototype.getRenderer = function () {
            return this.renderer;
        }
        Renderer.prototype.getParameters = function () {
            return this._parameters;
        }

        return Renderer;
    })();

    Kolonist.Renderer = Renderer;
})(Kolonist || (Kolonist = {}));

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
		parameters = $.extend(true, {}, parameters, defaultParameters);

		camera = new THREE.PerspectiveCamera(parameters.viewAngle, parameters.width / parameters.height, parameters.near, parameters.far);
		camera.position.z = parameters.cameraZPosition;

		scene = new THREE.Scene();

		renderer = new THREE.WebGLRenderer();
		renderer.setSize(parameters.width, parameters.height);

		_$parentContainer.append(renderer.domElement);
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

	return Renderer;
})();
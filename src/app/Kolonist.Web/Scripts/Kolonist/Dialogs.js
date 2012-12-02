
var Kolonist;
(function (Kolonist) {
    var Build = (function () {

        function Build($dialog) {

            this.openDialog = function (data) {
                if (data.point && data.isLeft()) {
                    var cube = new THREE.Mesh(
                        new THREE.CubeGeometry(5, 5, 5),
                        new THREE.MeshBasicMaterial({
                            color: 0x0000ff
                        })
                    );
                    cube.position.addSelf(new THREE.Vector3(data.point.x, data.point.y, data.point.z));

                    renderer.addMesh(cube);

                    
                    $dialog
                        .modal({
                            keyboard:true
                        })
                        .css({
                            "left": data.x,
                            "top": data.y,
                            "margin-left": 0,
                            "margin-top": 0,
                            "width": 'auto'
                        })

                    $dialog.find('.btn.build').tooltip({
                        //placement: 'bottom'
                    });
                    ;
                }
            }
        }
        return Build;
    })();

    Kolonist.Build = Build;
})(Kolonist || (Kolonist = {}));
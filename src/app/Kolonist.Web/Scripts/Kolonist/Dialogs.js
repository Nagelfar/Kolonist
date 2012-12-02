
var Kolonist;
(function (Kolonist) {
    var Build = (function () {

        function Build($dialog) {
            $dialog = $($dialog);

            var currentPoint = {
                position: null
            };

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

                    currentPoint.position = data.point;

                    $dialog
                        .modal({
                            keyboard: true
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

            $dialog.find('form').submit(function () {
                return false;
            });
            $dialog.find('.btn.build').click(function () {
                var data = {
                    Position: {
                        X: currentPoint.position.x,
                        Y: currentPoint.position.y
                    },
                    BuildingTypeId: $(this).data('building-type')
                };
                Kolonist.Util
                    .jsonPost($(this).parents('form').attr('action'), data)
                    .done(function (result) {
                      alert('success' + result);

                      $dialog.modal('hide');
                    }).error(function(data){
                        alert('error ' + data);
                    });
            });

        }
        return Build;
    })();

    Kolonist.Build = Build;
})(Kolonist || (Kolonist = {}));